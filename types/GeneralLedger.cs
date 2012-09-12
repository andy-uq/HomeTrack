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
			account.Id = _repository.Add(account);
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

		public Transaction GetTransaction(int id)
		{
			return _repository.GetTransaction(id);
		}

		public Account this[string accountId]
		{
			get
			{
				if ( string.IsNullOrEmpty(accountId) )
					throw new ArgumentNullException("accountId");
				
				Account account = _repository.GetAccount(accountId);
				if (account == null)
					throw new InvalidOperationException("Cannot find account: " + accountId);

				return account;
			}
		}

		public bool Post(Transaction transaction)
		{
			return _repository.Post(transaction);
		}

		public IEnumerable<Transaction> GetTransactions(string accountId)
		{
			return _repository.GetTransactions(accountId);
		}

		public void Dispose()
		{
			_repository.Dispose();
		}

		public void AddBudget(Budget budget)
		{
			if (_repository.GetAccount(budget.BudgetAccount.Id) == null)
			{
				Add(budget.BudgetAccount);
			}

			_repository.AddBudget(budget);
		}

		public IEnumerable<Account> GetBudgetAccount(string accountId)
		{
			return _repository.GetBudgetAccounts(accountId);
		}
	}
}