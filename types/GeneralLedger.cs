using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HomeTrack
{
	public class GeneralLedger : IEnumerable<Account>
	{
		private readonly IGeneralLedgerRepository _repository;

		public GeneralLedger(IGeneralLedgerRepository repository)
		{
			_repository = repository;
		}

		public IEnumerable<Account> DebitAccounts { get { return _repository.DebitAccounts; } }
		public IEnumerable<Account> CreditAccounts { get { return _repository.CreditAccounts; } }

		public void Add(Account account)
		{
			_repository.Add(account);
		}

		public IEnumerator<Account> GetEnumerator()
		{
			return _repository.Accounts.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool TrialBalance()
		{
			var debit = DebitAccounts.Sum(x => x.Balance);
			var credit = CreditAccounts.Sum(x => x.Balance);

			return debit == credit;
		}

		public Account this[string accountId]
		{
			get { return _repository.GetAccount(accountId); }
		}

		public bool Post(Transaction transaction)
		{
			return _repository.Post(transaction);
		}

		public IEnumerable<Transaction> GetTransactions(string accountId)
		{
			return _repository.GetTransactions(accountId);
		}
	}
}