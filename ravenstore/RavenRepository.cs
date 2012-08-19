using System;
using System.Collections.Generic;
using Raven.Client;
using Raven.Client.Linq;

namespace HomeTrack.RavenStore
{
	public class RavenRepository : IRepository
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

		public IUnitOfWork CreateUnitOfWork()
		{
			return new RavenUnitOfWork(_documentStore.OpenSession());
		}
	}

	public class RavenUnitOfWork : IUnitOfWork
	{
		public IDocumentSession Session { get; set; }

		public RavenUnitOfWork(IDocumentSession session)
		{
			Session = session;
		}

		public void Add<T>(T value)
		{
			Attach(value);
		}

		public void Attach<T>(T value)
		{
			Session.Store(value);
		}

		public T GetById<T>(int id)
		{
			return Session.Load<T>(id);
		}

		public IEnumerable<T> GetAll<T>()
		{
			return Session.Query<T>();
		}

		public void SaveChanges()
		{
			Session.SaveChanges();
		}

		public IEnumerable<Transaction> GetTransactions(Account account)
		{
			return
				(
					from t in Session.Query<Transaction>()
					where
						System.Linq.Enumerable.Any(t.Credit, x => x.Account.Id == account.Id)
						|| System.Linq.Enumerable.Any(t.Debit, x => x.Account.Id == account.Id)
					select t
				);
		}

		public void Dispose()
		{
			Session.Dispose();
		}
	}
}