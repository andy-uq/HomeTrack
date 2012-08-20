using System.Collections.Generic;

namespace HomeTrack
{
	public interface IGeneralLedgerRepository
	{
		IEnumerable<Account> Accounts { get; }
		IEnumerable<Account> DebitAccounts { get; }
		IEnumerable<Account> CreditAccounts { get; }
		Account GetAccount(string accountId);
		void Add(Account account);

		bool Post(Transaction transaction);
		IEnumerable<Transaction> GetTransactions(string accountId);
	}
}