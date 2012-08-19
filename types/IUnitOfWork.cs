using System;
using System.Collections.Generic;

namespace HomeTrack
{
	public interface IUnitOfWork : IDisposable
	{
		void Add<T>(T value);
		T GetById<T>(int id);
		IEnumerable<T> GetAll<T>();
		void Attach<T>(T value);
		void SaveChanges();

		IEnumerable<Transaction> GetTransactions(Account account);
	}

	public interface IRepository : IDisposable
	{
		IUnitOfWork CreateUnitOfWork();
	}
}