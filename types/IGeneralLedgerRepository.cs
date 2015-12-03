using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeTrack
{
	public interface IGeneralLedgerRepository : IDisposable
	{
		IEnumerable<Account> Accounts { get; }
		IEnumerable<Account> DebitAccounts { get; }
		IEnumerable<Account> CreditAccounts { get; }
		Account GetAccount(string accountId);
		bool DeleteAccount(string accountId);
		IEnumerable<Budget> GetBudgetsForAccount(string accountId);

		string Add(Account account);
		void AddBudget(Budget budget);

		bool Post(Transaction transaction);
		IEnumerable<Transaction> GetTransactions(string accountId);
		Transaction GetTransaction(string id);
	}

	public interface IGeneralLedgerAsyncRepository
	{
		Task<Account> GetAccountAsync(string accountId);
		Task<IEnumerable<Account>> GetAccountsAsync();
		Task<IEnumerable<Account>> GetDebitAccountsAsync();
		Task<IEnumerable<Account>> GetCreditAccountsAsync();
		Task<bool> DeleteAccountAsync(string accountId);
		Task<IEnumerable<Budget>> GetBudgetsForAccountAsync(string accountId);
		Task<string> AddAsync(Account account);
		Task AddBudgetAsync(Budget budget);
		Task<bool> PostAsync(Transaction transaction);
		Task<IEnumerable<Transaction>> GetTransactionsAsync(string accountId);
		Task<Transaction> GetTransactionAsync(string id);
	}
}