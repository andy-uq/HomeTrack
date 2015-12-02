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
			foreach (var model in models.Patterns)
			{
				_database.Execute(
					"INSERT INTO [AccountIdentifierPattern] (AccountIdentifierId, Name, PropertiesJson) VALUES (@id, @name, @propertiesJson)",
						new { id, model.Name, model.PropertiesJson }
					);
			}
		}

		public IEnumerable<AccountIdentifier> GetAll()
		{
			var patterns = _database.Query<Models.AccountIdentifierPattern>("SELECT * FROM [AccountIdentifierPattern]").ToLookup(x => x.AccountIdentifierId);
			foreach (var model in _database.Query<Models.AccountIdentifier>("SELECT * FROM [AccountIdentifier]"))
			{
				model.Patterns = patterns[model.Id].ToArray();
				yield return model.Map<AccountIdentifier>();
			}
		}

		public void Remove(int id)
		{
			_database.Execute("DELETE FROM [AccountIdentifier] WHERE Id=@id", new { id });
		}

		public AccountIdentifier GetById(int id)
		{
			var model = _database.Query<Models.AccountIdentifier>("SELECT * FROM [AccountIdentifier] WHERE Id=@id", new {id}).Single();
			model.Patterns = _database.Query<Models.AccountIdentifierPattern>("SELECT * FROM [AccountIdentifierPattern] WHERE AccountIdentifierId=@id", new { id }).ToArray();

			return model.Map<AccountIdentifier>();
		}
	}
}