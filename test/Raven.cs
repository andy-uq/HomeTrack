using HomeTrack.RavenStore;
using Raven.Client.Document;
using Raven.Client.Embedded;

namespace HomeTrack.Tests
{
	public static class Raven
	{
		public static RavenRepository CreateRepository()
		{
			var store = new EmbeddableDocumentStore { RunInMemory = true };
			store.Initialize();

			return new RavenRepository(store);
		}
	}
}