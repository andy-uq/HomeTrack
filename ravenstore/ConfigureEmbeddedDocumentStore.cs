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

		public void Build(ContainerBuilder containerBuilder)
		{
			var store = new EmbeddableDocumentStore();
			InitialiseDocumentStore(store);

			containerBuilder.RegisterInstance(store)
				.As<IDocumentStore>()
				.SingleInstance();

			var configuration = new RegisterMappings().GetMappings();
			var mappingEngine = new MappingEngine(configuration);
			containerBuilder.RegisterInstance<IMappingEngine>(mappingEngine);

			containerBuilder.RegisterType<GeneralLedgerRepository>()
				.As<IGeneralLedgerRepository>()
				.SingleInstance();
			
			containerBuilder.RegisterType<RavenRepository>()
				.SingleInstance();
		}

		public void InitialiseDocumentStore(EmbeddableDocumentStore documentStore)
		{
			documentStore.UseEmbeddedHttpServer = UseEmbeddedHttpServer;
			documentStore.DataDirectory = DataDirectory;
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