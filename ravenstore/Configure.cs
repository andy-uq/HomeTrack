using AutoMapper;
using Autofac;
using Newtonsoft.Json;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Database.Server;

namespace HomeTrack.RavenStore
{
	public class Configure : IDemandBuilder
	{
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
			documentStore.UseEmbeddedHttpServer = true;
			documentStore.DataDirectory = DataDirectory;
			documentStore.DefaultDatabase = "HomeTrack";
			documentStore.Conventions.CustomizeJsonSerializer = ConfigureJsonSerialiser;

			NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8080);

			documentStore.Initialize();
		}


		private void ConfigureJsonSerialiser(JsonSerializer obj)
		{
			obj.TypeNameHandling = TypeNameHandling.None;
		}
	}
}