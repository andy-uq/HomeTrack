using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

namespace HomeTrack.SqlStore
{
	public class AccountIdentifierRepository : IAccountIdentifierRepository
	{
		private readonly SqlConnection _database;

		public AccountIdentifierRepository(SqlConnection database)
		{
			_database = database;
		}

		public void AddOrUpdate(AccountIdentifier identifier)
		{
		}

		public IEnumerable<AccountIdentifier> GetAll()
		{
			yield break;
		}

		public void Remove(int id)
		{
		}

		public AccountIdentifier GetById(int id)
		{
			return null;
		}
	}
}