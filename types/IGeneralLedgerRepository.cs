using System;
using System.Collections.Generic;

namespace HomeTrack
{
	public interface IGeneralLedgerRepository : IDisposable
	{
		IEnumerable<Account> Accounts { get; }
		IEnumerable<Account> DebitAccounts { get; }
		IEnumerable<Account> CreditAccounts { get; }
		Account GetAccount(string accountId);
		IEnumerable<Budget> GetBudgetsForAccount(string accountId);

		string Add(Account account);
		void AddBudget(Budget budget);

		bool Post(Transaction transaction);
		IEnumerable<Transaction> GetTransactions(string accountId);
		Transaction GetTransaction(int id);
	}
}