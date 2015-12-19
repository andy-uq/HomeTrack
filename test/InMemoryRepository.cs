using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HomeTrack.Collections;
using HomeTrack.Mapping;

namespace HomeTrack.Tests
{
	public class InMemoryRepository : IGeneralLedgerRepository, IGeneralLedgerAsyncRepository, IImportRepository, IImportAsyncRepository, IEnumerable<Account>
	{
		private readonly ISet<Account> _accounts;
		private readonly ISet<Budget> _budgets;
		private readonly List<Tuple<ImportResult, ImportedTransaction[]>> _imports;
		private readonly List<Transaction> _transactions;

		public InMemoryRepository()
		{
			_accounts = new HashSet<Account>();
			_transactions = new List<Transaction>();
			_budgets = new HashSet<Budget>();
			_imports = new List<Tuple<ImportResult, ImportedTransaction[]>>();
		}

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

		public Account GetAccount(string accountId)
		{
			return _accounts.SingleOrDefault(x => x.Id.Equals(accountId, StringComparison.OrdinalIgnoreCase));
		}

		public bool DeleteAccount(string accountId)
		{
			if (GetTransactions(accountId).Any())
				throw new InvalidOperationException("Cannot delete an account that has transactions");

			var account = GetAccount(accountId);
			if (account == null)
				return false;

			_accounts.Remove(account);
			return true;
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
			if (_transactions.Any(x => x.Id == transaction.Id))
				return false;

			_transactions.Add(transaction);
			return true;
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

		public Transaction GetTransaction(string id)
		{
			return _transactions.SingleOrDefault(x => x.Id == id);
		}

		public void Dispose()
		{
		}

		public IEnumerable<ImportResult> GetAll()
		{
			return _imports.Select(x => x.Item1);
		}

		public IEnumerable<ImportedTransaction> GetTransactionIds(int importId)
		{
			return _imports.Single(x => x.Item1.Id == importId).Item2;
		}

		public IEnumerable<Transaction> GetTransactions(int importId)
		{
			var target = _imports.Where(i => i.Item1.Id == importId).Select(i => i.Item2).Single().Select(t => t.Id).AsSet();
			return
				(
					from t in _transactions
					where target.Contains(t.Id)
					select t
					);
		}

		public int Save(ImportResult result, IEnumerable<Transaction> transactions)
		{
			var import = Tuple.Create(result, transactions.Select(t => t.Map<ImportedTransaction>()).ToArray());

			_imports.Add(import);
			result.Id = _imports.Count;

			return result.Id;
		}

		public Task<int> SaveAsync(ImportResult result, IEnumerable<Transaction> transactions)
		{
			return Task.FromResult(Save(result, transactions));
		}

		public Task<IEnumerable<ImportResult>> GetAllAsync()
		{
			return Task.FromResult(GetAll());
		}

		public Task<IEnumerable<ImportedTransaction>> GetTransactionIdsAsync(int importId)
		{
			return Task.FromResult(GetTransactionIds(importId));
		}

		public Task<IEnumerable<Transaction>> GetTransactionsAsync(int importId)
		{
			return Task.FromResult(GetTransactions(importId));
		}

		public IEnumerable<Account> GetBudgetAccounts(string accountId)
		{
			return GetBudgetsForAccount(accountId).Select(budget => budget.BudgetAccount);
		}

		public Task<Account> GetAccountAsync(string accountId)
		{
			return Task.FromResult(GetAccount(accountId));
		}

		public Task<IEnumerable<Account>> GetAccountsAsync()
		{
			return Task.FromResult(Accounts);
		}

		public Task<IEnumerable<Account>> GetDebitAccountsAsync()
		{
			return Task.FromResult(DebitAccounts);
		}

		public Task<IEnumerable<Account>> GetCreditAccountsAsync()
		{
			return Task.FromResult(CreditAccounts);
		}

		public Task<bool> DeleteAccountAsync(string accountId)
		{
			return Task.FromResult(DeleteAccount(accountId));
		}

		public Task<IEnumerable<Budget>> GetBudgetsForAccountAsync(string accountId)
		{
			return Task.FromResult(GetBudgetsForAccount(accountId));
		}

		public Task<string> AddAsync(Account account)
		{
			return Task.FromResult(Add(account));
		}

		public Task AddBudgetAsync(Budget budget)
		{
			AddBudget(budget);
			return Task.CompletedTask;
		}

		public Task<bool> PostAsync(Transaction transaction)
		{
			return Task.FromResult(Post(transaction));
		}

		public Task<IEnumerable<Transaction>> GetTransactionsAsync(string accountId)
		{
			return Task.FromResult(GetTransactions(accountId));
		}

		public Task<Transaction> GetTransactionAsync(string id)
		{
			return Task.FromResult(GetTransaction(id));
		}

		public IEnumerator<Account> GetEnumerator()
		{
			return Accounts.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}