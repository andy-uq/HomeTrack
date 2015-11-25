using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using Dapper;
using HomeTrack.Mapping;

namespace HomeTrack.SqlStore
{
	public class ImportRepository : IImportRepository
	{
		private readonly SqlConnection _database;

		public ImportRepository(SqlConnection database)
		{
			_database = database;
		}

		public void Save(ImportResult result, IEnumerable<Transaction> transactions)
		{
			using (var transaction = new TransactionScope())
			{
				var model = result.Map<Models.ImportResult>();

				var importId = _database.ExecuteScalar<int>(
					@"INSERT INTO ImportResult (Name, ImportTypeName, Date)
						OUTPUT inserted.id
						VALUES (@name, @importType, @date)", model);

				_database.Execute(
					@"INSERT INTO ImportedTransaction (Id, ImportId, Unclassified, Amount)
						VALUES (@id, @importId, @unclassified, @amount)",
					transactions
						.MapAll<Models.ImportedTransaction>()
						.Select(t => new {t.Id, importId, t.Unclassified, t.Amount}));

				transaction.Complete();
			}
		}

		public IEnumerable<ImportResult> GetAll()
		{
			var imports = _database.Query<Models.ImportResult>("SELECT ImportResult.* FROM ImportResult");
			return imports.MapAll<ImportResult>();
		}

		public IEnumerable<ImportedTransaction> GetTransactionIds(int importId)
		{
			var transactions = _database.Query<Models.ImportedTransaction>(
				@"SELECT ImportedTransaction.* 
					FROM ImportedTransaction 
					WHERE ImportId = @importId", new { importId });

			return transactions.MapAll<ImportedTransaction>();
		}

		public IEnumerable<Transaction> GetTransactions(int importId)
		{
			yield break;
		}
	}
}