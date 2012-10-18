using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Database.Server;

namespace HomeTrack.RavenStore
{
	public class ConfigureEmbeddedDocumentStore : ConfigureDocumentStore
	{
		public bool UseEmbeddedHttpServer { get; set; }
		public string DataDirectory { get; set; }
		public bool RunInMemory { get; set; }

		protected override DocumentStore CreateDocumentStore()
		{
			return new EmbeddableDocumentStore();
		}

		protected override void InitialiseDocumentStore(DocumentStore documentStore)
		{
			var embeddedDocumentStore = (EmbeddableDocumentStore) documentStore;

			if ( RunInMemory )
			{
				embeddedDocumentStore.RunInMemory = true;
			}
			else
			{
				embeddedDocumentStore.UseEmbeddedHttpServer = UseEmbeddedHttpServer;
				embeddedDocumentStore.DataDirectory = DataDirectory;
			}

			if ( UseEmbeddedHttpServer )
			{
				NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8787);
			}

			base.InitialiseDocumentStore(documentStore);
		}
	}
}