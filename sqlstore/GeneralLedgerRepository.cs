using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using Dapper;
using HomeTrack.Mapping;

namespace HomeTrack.SqlStore
{
	public class GeneralLedgerRepository : IGeneralLedgerRepository
	{
		private readonly SqlConnection _database;

		public GeneralLedgerRepository(SqlConnection database)
		{
			_database = database;
		}

		public IEnumerable<Account> Accounts
		{
			get
			{
				var accounts = _database.Query<Models.Account>("SELECT Account.* FROM Account");
				return accounts.MapAll<Account>();
			}
		}

		public IEnumerable<Account> DebitAccounts
		{
			get
			{
				var accounts = GetAccounts(EntryType.Debit);
				return accounts.MapAll<Account>();
			}
		}

		public IEnumerable<Account> CreditAccounts
		{
			get
			{
				var accounts = GetAccounts(EntryType.Credit);
				return accounts.MapAll<Account>();
			}
		}

		private IEnumerable<Models.Account> GetAccounts(EntryType entryType)
		{
			return _database.Query<Models.Account>(
				@"SELECT Account.* 
					FROM Account 
						INNER JOIN AccountType ON AccountType.Name = AccountTypeName 
					WHERE AccountType.EntryTypeName = @debitOrCredit",
				new {debitOrCredit = entryType.ToString()});
		}

		public Account GetAccount(string accountId)
		{
			var account = _database.Query<Models.Account>("SELECT Account.* FROM Account WHERE Id=@accountId", new {accountId}).Single();
			return account.Map<Account>();
		}

		public bool DeleteAccount(string accountId)
		{
			int rowCount = _database.Execute("DELETE FROM Account WHERE Id=@accountId AND NOT EXISTS (SELECT * FROM TransactionComponent WHERE AccountId = @accountId)", new { accountId });
			return rowCount == 1;
		}

		public IEnumerable<Budget> GetBudgetsForAccount(string accountId)
		{
			yield break;
		}

		public string Add(Account account)
		{
			var id = account.Id ?? Regex.Replace(account.Name.ToLower(), "[^a-z0-9-/]", "");

			_database.Execute("INSERT INTO [Account] (Id, Name, Description, AccountTypeName) VALUES (@id, @name, @description, @accountTypeName)",
				new { Id = id, account.Name, account.Description, accountTypeName = account.Type.ToString() });

			return id;
		}

		public void AddBudget(Budget budget)
		{
		}

		public bool Post(Transaction transaction)
		{
			return false;
		}

		public IEnumerable<Transaction> GetTransactions(string accountId)
		{
			var transactions = _database.Query<Models.Transaction>(
				@"SELECT [Transaction].* 
					FROM [Transaction] 
					WHERE EXISTS (SELECT * FROM TransactionComponent WHERE AccountId=@accountId AND [Transaction].Id = TransactionId)", new { accountId });

			return transactions.MapAll<Transaction>();
		}

		public Transaction GetTransaction(string id)
		{
			var transaction = _database.Query<Models.Transaction>(
				@"SELECT [Transaction].* 
					FROM [Transaction]
					WHERE Id = @id", new { id });

			return transaction.Map<Transaction>();
		}

		public void Dispose()
		{
			_database.Dispose();
		}
	}
}