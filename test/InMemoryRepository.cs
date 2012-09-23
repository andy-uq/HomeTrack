using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;

namespace HomeTrack.Tests
{
	public class InMemoryRepository : IGeneralLedgerRepository, IImportRepository
	{
		private readonly IMappingEngine _mappingEngine;
		private readonly ISet<Account> _accounts;
		private readonly ISet<Budget> _budgets;
		private readonly List<Transaction> _transactions;
		private readonly List<Tuple<ImportResult, ImportedTransaction[]>> _imports;
		private int nextId = 1;

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

		public InMemoryRepository(IMappingEngine mappingEngine = null)
		{
			_mappingEngine = mappingEngine;
			_accounts = new HashSet<Account>();
			_transactions = new List<Transaction>();
			_budgets = new HashSet<Budget>();
			_imports = new List<Tuple<ImportResult, ImportedTransaction[]>>();
		}

		public Account GetAccount(string accountId)
		{
			return _accounts.SingleOrDefault(x => x.Id.Equals(accountId, StringComparison.OrdinalIgnoreCase));
		}

		public bool DeleteAccount(string accountId)
		{
			if ( GetTransactions(accountId).Any() )
				throw new InvalidOperationException("Cannot delete an account that has transactions");

			var account = GetAccount(accountId);
			if ( account == null )
				return false;

			_accounts.Remove(account);
			return true;
		}

		public IEnumerable<Account> GetBudgetAccounts(string accountId)
		{
			return GetBudgetsForAccount(accountId).Select(budget => budget.BudgetAccount);
		}

		public IEnumerable<Budget> GetBudgetsForAccount(string accountId)
		{
			return
				(
					from budget in _budgets
					where budget.RealAccount.Id == accountId
					select budget
				);
		}

		public string Add(Account account)
		{
			account.Id = Regex.Replace(account.Name, @"\W+", string.Empty).ToLower();
			_accounts.Add(account);

			return account.Id;
		}

		public void AddBudget(Budget budget)
		{
			_budgets.Add(budget);
		}

		public bool Post(Transaction transaction)
		{
			if (transaction.Check())
			{
				foreach (var debitAmount in transaction.Debit)
					debitAmount.Post();

				foreach (var creditAmount in transaction.Credit)
					creditAmount.Post();

				transaction.Id = nextId++;
				_transactions.Add(transaction);
				return true;
			}

			return false;
		}

		public IEnumerable<ImportResult> GetAll()
		{
			return _imports.Select(x => x.Item1);
		}

		public IEnumerable<ImportedTransaction> GetTransactionIds(string importId)
		{
			return _imports.Single(x => x.Item1.Name == importId).Item2;
		}

		public IEnumerable<Transaction> GetTransactions(string accountId)
		{
			return
				(
					from t in _transactions
					where
						t.Credit.Any(x => x.Account.Id == accountId)
						|| t.Debit.Any(x => x.Account.Id == accountId)
					select t
				);
		}

		public Transaction GetTransaction(int id)
		{
			return _transactions.SingleOrDefault(x => x.Id == id);
		}

		public void Dispose()
		{			
		}

		public void Save(ImportResult result, IEnumerable<Transaction> transactions)
		{
			_imports.Add(new Tuple<ImportResult, ImportedTransaction[]>(result, transactions.Select(_mappingEngine.Map<ImportedTransaction>).ToArray()));
		}
	}
}