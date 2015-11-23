using System.Collections.Generic;
using System.Linq;
using Autofac;
using HomeTrack.Ioc;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Imports.Newtonsoft.Json;

namespace HomeTrack.RavenStore
{
	public class ConfigureDocumentStore : IFeatureRegistration
	{
		protected virtual DocumentStore CreateDocumentStore()
		{
			return new DocumentStore
			{
				ConnectionStringName = "raven"
			};
		}

		public void Register(ContainerBuilder builder)
		{
			var store = CreateDocumentStore();
			InitialiseDocumentStore(store);

			builder.RegisterInstance(store)
				.As<IDocumentStore>()
				.SingleInstance();

			builder.RegisterType<RavenEntityTypeMapProvider>()
				.As<ITypeMapProvider>();

			builder.RegisterType<GeneralLedgerRepository>()
				.As<IGeneralLedgerRepository>()
				.SingleInstance();

			builder.RegisterType<AccountIdentifierRepository>()
				.As<IAccountIdentifierRepository>()
				.SingleInstance();

			builder.RegisterType<ImportRepository>()
				.As<IImportRepository>()
				.SingleInstance();

			builder.Register(r => r.Resolve<IAccountIdentifierRepository>().GetAll());

			builder.RegisterType<RavenRepository>()
				.SingleInstance();
		}

		protected virtual void InitialiseDocumentStore(DocumentStore documentStore)
		{
			if (!(documentStore is EmbeddableDocumentStore))
			{
				documentStore.DefaultDatabase = "HomeTrack";
			}

			documentStore.Conventions.CustomizeJsonSerializer = ConfigureJsonSerialiser;
			documentStore.Initialize();

			OnInitialise(documentStore);
		}

		private void ConfigureJsonSerialiser(JsonSerializer obj)
		{
			obj.TypeNameHandling = TypeNameHandling.None;
		}

		private void OnInitialise(DocumentStore documentStore)
		{
			Raven.Client.Indexes.IndexCreation.CreateIndexes(typeof(ConfigureDocumentStore).Assembly, documentStore);

			using ( var session = documentStore.OpenSession() )
			{
				foreach ( var account in session.Query<Documents.Account>() )
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
			foreach ( var transaction in transactions )
			{
				var debit = transaction.Debit.Where(x => x.AccountId == account.Id).Sum(x => x.Value);
				var credit = transaction.Credit.Where(x => x.AccountId == account.Id).Sum(x => x.Value);

				switch ( accountType )
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