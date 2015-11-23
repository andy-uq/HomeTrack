using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using Dapper;

namespace HomeTrack.SqlStore
{
	public class GeneralLedgerRepository : IGeneralLedgerRepository
	{
		private readonly SqlConnection _database;

		public GeneralLedgerRepository(SqlConnection database)
		{
			_database = database;
		}

		public IEnumerable<Account> Accounts { get; }
		public IEnumerable<Account> DebitAccounts { get; }
		public IEnumerable<Account> CreditAccounts { get; }

		public Account GetAccount(string accountId)
		{
			var account = _database.Query<Models.Account>("SELECT Account.* FROM Account WHERE Id=@accountId", new {accountId}).Single();
			return new Account(account.Name, (AccountType )Enum.Parse(typeof(AccountType), account.AccountTypeName, ignoreCase: false))
			{
				Description = account.Description,
				Id = account.Id,
			};
		}

		public bool DeleteAccount(string accountId)
		{
			return false;
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
			yield break;
		}

		public Transaction GetTransaction(int id)
		{
			return null;
		}

		public void Dispose()
		{
			_database.Dispose();
		}
	}
}