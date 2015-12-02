using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HomeTrack.Mapping;
using HomeTrack.SqlStore.Models;

namespace HomeTrack.SqlStore
{
	public class AccountIdentifierRepository : IAccountIdentifierRepository, IAccountIdentifierAsyncRepository
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

		public async Task AddOrUpdateAsync(AccountIdentifier identifier)
		{
			var models = identifier.Map<Models.AccountIdentifier>();

			int id = await _database.ExecuteAsync("INSERT INTO [AccountIdentifier] (AccountId) OUTPUT inserted.id VALUES (@accountId)", new { models.AccountId });
			foreach (var model in models.Patterns)
			{
				await _database.ExecuteAsync(
					"INSERT INTO [AccountIdentifierPattern] (AccountIdentifierId, Name, PropertiesJson) VALUES (@id, @name, @propertiesJson)",
						new { id, model.Name, model.PropertiesJson }
					);
			}
		}

		public async Task<IEnumerable<AccountIdentifier>> GetAllAsync()
		{
			var results = new List<AccountIdentifier>();

			var patterns = (await _database.QueryAsync<Models.AccountIdentifierPattern>("SELECT * FROM [AccountIdentifierPattern]"))
				.ToLookup(x => x.AccountIdentifierId);

			foreach (var model in await _database.QueryAsync<Models.AccountIdentifier>("SELECT * FROM [AccountIdentifier]"))
			{
				model.Patterns = patterns[model.Id].ToArray();
				results.Add(model.Map<AccountIdentifier>());
			}

			return results;
		}

		public Task RemoveAsync(int id)
		{
			return _database.ExecuteAsync("DELETE FROM [AccountIdentifier] WHERE Id=@id", new { id });
		}

		public async Task<AccountIdentifier> GetByIdAsync(int id)
		{
			var model = (await _database.QueryAsync<Models.AccountIdentifier>("SELECT * FROM [AccountIdentifier] WHERE Id=@id", new { id })).Single();
			var patterns = await _database.QueryAsync<Models.AccountIdentifierPattern>("SELECT * FROM [AccountIdentifierPattern] WHERE AccountIdentifierId=@id", new { id });

			model.Patterns = patterns.ToArray();

			return model.Map<AccountIdentifier>();
		}
	}
}