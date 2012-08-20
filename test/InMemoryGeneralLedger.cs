using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HomeTrack.Tests
{
	public class InMemoryGeneralLedger : IGeneralLedgerRepository
	{
		private readonly ISet<Account> _accounts;
		private readonly List<Transaction> _transactions;

		public IEnumerable<Account> Accounts
		{
			get { return _accounts; }
		}

		public IEnumerable<Account> DebitAccounts
		{
			get { return _accounts.Where(x => x.Direction == EntryType.Debit); }
		}

		public IEnumerable<Account> CreditAccounts
		{
			get { return _accounts.Where(x => x.Direction == EntryType.Credit); }
		}

		public InMemoryGeneralLedger()
		{
			_accounts = new HashSet<Account>();
			_transactions = new List<Transaction>();
		}

		public Account GetAccount(string accountId)
		{
			return _accounts.Single(x => x.Id.Equals(accountId, StringComparison.OrdinalIgnoreCase));
		}

		public void Add(Account account)
		{
			account.Id = Regex.Replace(account.Name, @"\W+", string.Empty).ToLower();
			_accounts.Add(account);
		}

		public bool TrialBalance()
		{
			var debit = DebitAccounts.Sum(x => x.Balance);
			var credit = CreditAccounts.Sum(x => x.Balance);

			return debit == credit;
		}

		public Account this[string accountId]
		{
			get { return GetAccount(accountId); }
		}

		public bool Post(Transaction transaction)
		{
			if (transaction.Check())
			{
				foreach (var debitAmount in transaction.Debit)
					debitAmount.Post();

				foreach (var creditAmount in transaction.Credit)
					creditAmount.Post();

				_transactions.Add(transaction);
				return true;
			}

			return false;
		}

		public IEnumerable<Transaction> GetTransactions(string accountId)
		{
			return
				(
					from t in _transactions
					where
						System.Linq.Enumerable.Any(t.Credit, x => x.Account.Id == accountId)
						|| System.Linq.Enumerable.Any(t.Debit, x => x.Account.Id == accountId)
					select t
				);
		}
	}
}