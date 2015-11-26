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

		public int Save(ImportResult result, IEnumerable<Transaction> transactions)
		{
			using (var transaction = new TransactionScope())
			{
				var model = result.Map<Models.ImportResult>();
				var transactionRecords = new List<Models.Transaction>();
				var transactionComponents = new List<Models.TransactionComponent>();

				foreach (var item in transactions)
				{
					var record = item.Map<Models.Transaction>();
					transactionRecords.Add(record);
					transactionComponents.AddRange(item.Credit.Select(amount => amount.Map(new Models.TransactionComponent {TransactionId = record.Id, EntryTypeName = EntryType.Credit.ToString() })));
					transactionComponents.AddRange(item.Debit.Select(amount => amount.Map(new Models.TransactionComponent {TransactionId = record.Id, EntryTypeName = EntryType.Debit.ToString() })));
				}

				var importId = _database.ExecuteScalar<int>(
					@"INSERT INTO ImportResult (Name, ImportTypeName, Date)
						OUTPUT inserted.id
						VALUES (@name, @importType, @date)", model);

				_database.Execute(
					@"INSERT INTO ImportedTransaction (Id, ImportId, Unclassified, Amount)
						VALUES (@id, @importId, @unclassified, @amount)",
					transactionRecords
						.MapAll<Models.ImportedTransaction>()
						.Select(t => new {t.Id, importId, t.Unclassified, t.Amount}));

				_database.Execute(
					@"INSERT INTO [Transaction] (Id, Date, Amount, Reference, Description)
						VALUES (@id, @date, @amount, @reference, @description)",
					transactionRecords);

				_database.Execute(
					@"INSERT INTO [TransactionComponent] (TransactionId, AccountId, EntryTypeName, Amount, Annotation, AppliedByRuleId)
						VALUES (@transactionId, @accountId, @entryTypeName, @amount, ISNULL(@annotation, ''), @appliedByRuleId)",
					transactionComponents);

				transaction.Complete();
				return importId;
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
			var transactions = _database.Query<Models.Transaction>(
				@"SELECT [Transaction].* 
					FROM ImportedTransaction 
						INNER JOIN [Transaction] ON [Transaction].Id = ImportedTransaction.Id
					WHERE ImportId = @importId", new { importId });

			return transactions.MapAll<Transaction>();
		}
	}
}