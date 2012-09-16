using System;
using System.Collections.Generic;
using HomeTrack.Core;
using Raven.Client;
using Raven.Client.Linq;

namespace HomeTrack.RavenStore
{
	public class RavenRepository
	{
		private readonly IDocumentStore _documentStore;

		public RavenRepository(IDocumentStore documentStore)
		{
			_documentStore = documentStore;
		}

		public IDocumentStore DocumentStore
		{
			get { return _documentStore; }
		}

		public void Dispose()
		{
			_documentStore.Dispose();
		}

		public void UseOnceTo(Action<IDocumentSession> func, bool saveChanges = false)
		{
			_documentStore.UseOnceTo(func, saveChanges);
		}

		public T UseOnceTo<T>(Func<IDocumentSession, T> func, bool saveChanges = false)
		{
			return _documentStore.UseOnceTo(func, saveChanges);
		}
	}
}