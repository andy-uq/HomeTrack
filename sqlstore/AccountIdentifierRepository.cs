using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using HomeTrack.Mapping;

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
			var models = identifier.Map<Models.AccountIdentifier[]>();
			foreach (var model in models)
			{
				
			}
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