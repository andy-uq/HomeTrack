using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Autofac;
using Newtonsoft.Json;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Database.Server;

namespace HomeTrack.RavenStore
{
	public class ConfigureEmbeddedDocumentStore : IDemandBuilder
	{
		private Action<DocumentStore> _afterInitialise;
		
		public bool UseEmbeddedHttpServer { get; set; }
		public string DataDirectory { get; set; }
		public bool RunInMemory { get; set; }

		public void Build(ContainerBuilder containerBuilder)
		{
			var store = new EmbeddableDocumentStore();
			InitialiseDocumentStore(store);

			containerBuilder.RegisterInstance(store)
				.As<IDocumentStore>()
				.SingleInstance();

			containerBuilder.RegisterType<RavenEntityTypeMapProvider>()
				.As<ITypeMapProvider>();
			
			containerBuilder.RegisterType<GeneralLedgerRepository>()
				.As<IGeneralLedgerRepository>()
				.SingleInstance();

			containerBuilder.RegisterType<AccountIdentifierRepository>()
				.As<IAccountIdentifierRepository>()
				.SingleInstance();

			containerBuilder.RegisterType<ImportRepository>()
				.As<IImportRepository>()
				.SingleInstance();

			containerBuilder.Register(r => r.Resolve<IAccountIdentifierRepository>().GetAll());

			containerBuilder.RegisterType<RavenRepository>()
				.SingleInstance();
		}

		private void InitialiseDocumentStore(EmbeddableDocumentStore documentStore)
		{
			if ( RunInMemory )
			{
				documentStore.RunInMemory = true;
			}
			else
			{
				documentStore.UseEmbeddedHttpServer = UseEmbeddedHttpServer;
				documentStore.DataDirectory = DataDirectory;
			}

			documentStore.DefaultDatabase = "HomeTrack";
			documentStore.Conventions.CustomizeJsonSerializer = ConfigureJsonSerialiser;

			if (UseEmbeddedHttpServer)
			{
				NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8787);
			}

			documentStore.Initialize();
			OnInitialise(documentStore);

			if ( _afterInitialise != null )
			{
				_afterInitialise(documentStore);
			}
		}
		
		private void ConfigureJsonSerialiser(JsonSerializer obj)
		{
			obj.TypeNameHandling = TypeNameHandling.None;
		}

		public void AfterInitialise(Action<DocumentStore> afterInitialise)
		{
			_afterInitialise = afterInitialise;
		}

		private void OnInitialise(DocumentStore documentStore)
		{
			using (var session = documentStore.OpenSession())
			{
				foreach (var account in session.Query<Documents.Account>())
				{
					var id = account.Id;

					var query =
					(
						from t in session.Query<Documents.Transaction>()
						where
							t.Credit.Any(x => x.AccountId == id)
							|| t.Debit.Any(x => x.AccountId == id)
						select t
					);

					var balance = CalculateBalance(account, query);

					account.Balance = balance;
				}

				session.SaveChanges();
			}
		}

		private static decimal CalculateBalance(Documents.Account account, IEnumerable<Documents.Transaction> transactions)
		{
			var accountType = account.Type.IsDebitOrCredit();

			var balance = 0M;
			foreach (var transaction in transactions)
			{
				var debit = transaction.Debit.Where(x => x.AccountId == account.Id).Sum(x => x.Value);
				var credit = transaction.Credit.Where(x => x.AccountId == account.Id).Sum(x => x.Value);

				switch (accountType)
				{
					case EntryType.Debit:
						balance += debit;
						balance -= credit;
						break;

					case EntryType.Credit:
						balance += credit;
						balance -= debit;
						break;
				}
			}

			return balance;
		}
	}
}