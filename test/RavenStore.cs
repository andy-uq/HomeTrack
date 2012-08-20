using System;
using HomeTrack.RavenStore;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;

namespace HomeTrack.Tests
{
	public static class RavenStore
	{
		public static RavenRepository CreateRepository()
		{
			var store = new EmbeddableDocumentStore { RunInMemory = true };
			store.Initialize();

			return new RavenRepository(store);
		}
	}

	public static class RavenExtensions
	{
		public static T UseOnceTo<T>(this RavenRepository respository, Func<IDocumentSession, T> func)
		{
			using (var unitOfWork = respository.DocumentStore.OpenSession())
			{
				return func(unitOfWork);
			}
		}

		public static void UseOnceTo(this RavenRepository respository, Action<IDocumentSession> func)
		{
			using (var unitOfWork = respository.DocumentStore.OpenSession())
			{
				func(unitOfWork);
			}
		}
	}
}