using System;
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
				NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8080);
			}

			documentStore.Initialize();
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
	}
}