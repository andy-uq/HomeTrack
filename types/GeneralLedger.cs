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

		public Transaction GetTransaction(string id)
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
			var relatedAccounts = transaction
				.RelatedAccounts()
				.Distinct()
				.ToArray();

			foreach (var account in relatedAccounts)
			{
				var budgets = GetBudgetsForAccount(account.Id).ToArray();
				if (budgets.Length == 0)
					continue;

				ProcessBudgetPayout(transaction, account, budgets);
			}

			return _repository.Post(transaction);
		}

		private void ProcessBudgetPayout(Transaction transaction, Account account, IEnumerable<Budget> budgets)
		{
			Action<Account> setBudgetAccount;
			Account debit = null, credit = null;

			if (transaction.IsDebitAccount(account))
			{
				setBudgetAccount = a => { credit = a; };
				debit = this["Budget"];
			}
			else
			{
				setBudgetAccount = a => { debit = a; };
				credit = this["Budget"];
			}

			foreach (var budget in budgets)
			{
				setBudgetAccount(this[budget.BudgetAccount.Id]);
				transaction.Debit.Add(new Amount(debit, EntryType.Debit, transaction.Amount));
				transaction.Credit.Add(new Amount(credit, EntryType.Credit, transaction.Amount));
			}
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

		public IEnumerable<Budget> GetBudgetsForAccount(string accountId)
		{
			return _repository.GetBudgetsForAccount(accountId);
		}

		public IEnumerable<Account> GetBudgetAccount(string accountId)
		{
			return _repository.GetBudgetsForAccount(accountId).Select(x => x.BudgetAccount);
		}

		public bool DeleteAccount(string id)
		{
			return _repository.DeleteAccount(id);
		}
	}
}