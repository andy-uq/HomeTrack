using System.Collections.Generic;
using HomeTrack;

namespace sqlstore
{
	public class GeneralLedgerRepository : IGeneralLedgerRepository
	{
		public void Dispose()
		{
		}

		public IEnumerable<Account> Accounts { get; }
		public IEnumerable<Account> DebitAccounts { get; }
		public IEnumerable<Account> CreditAccounts { get; }

		public Account GetAccount(string accountId)
		{
			return null;
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
	}
}