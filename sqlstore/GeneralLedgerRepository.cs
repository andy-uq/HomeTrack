using System.Collections.Generic;
using System.Data.SqlClient;
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
			var account = _database.Query<Models.Account>("SELECT * FROM Account WHERE Id=@accountId", new {accountId});
			return account.MapTo<Account>();
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
			return null;
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
		}
	}
}