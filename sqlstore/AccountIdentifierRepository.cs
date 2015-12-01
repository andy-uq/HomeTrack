using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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
			var models = identifier.Map<Models.AccountIdentifier>();

			int id = _database.Execute("INSERT INTO [AccountIdentifier] (AccountId) OUTPUT inserted.id VALUES (@accountId)", new { models.AccountId });

			_database.Execute(
					"INSERT INTO [AccountIdentifierPattern] (AccountIdentifierId, Name, PropertiesJson) VALUES (@id, @name, @propertiesJson)",
						new { id, models.Primary.Name, models.Primary.PropertiesJson }
				);

			foreach (var model in models.Secondaries)
			{
				_database.Execute(
					"INSERT INTO [AccountIdentifierPattern] (AccountIdentifierId, Name, PropertiesJson) VALUES (@id, @name, @propertiesJson)",
						new { id, model.Name, model.PropertiesJson }
					);
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