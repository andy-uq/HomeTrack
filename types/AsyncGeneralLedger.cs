using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeTrack
{
	public class AsyncGeneralLedger
	{
		private readonly IGeneralLedgerAsyncRepository _repository;

		public AsyncGeneralLedger(IGeneralLedgerAsyncRepository repository)
		{
			_repository = repository;
		}

		public Task<IEnumerable<Account>> GetDebitAccounts() { return _repository.GetDebitAccountsAsync(); }
		public Task<IEnumerable<Account>> GetCreditAccounts() { return _repository.GetCreditAccountsAsync(); }
		public Task<IEnumerable<Account>> GetAccountsAsync() { return _repository.GetAccountsAsync(); }

		public async Task AddAsync(Account account)
		{
			account.Id = await _repository.AddAsync(account);
		}

		public async Task AddRangeAsync(params Account[] accounts)
		{
			foreach (var account in accounts)
			{
				await AddAsync(account);
			}
		}

		public async Task<bool> TrialBalanceAsync()
		{
			var debit = await GetDebitAccounts();
			var credit = await GetCreditAccounts();

			return debit.Sum(x => x.Balance) == credit.Sum(x => x.Balance);
		}

		public Task<Transaction> GetTransactionAsync(string id)
		{
			return _repository.GetTransactionAsync(id);
		}

		public async Task<Account> GetAccountAsync(string accountId)
		{
			if (string.IsNullOrEmpty(accountId))
				throw new ArgumentNullException(nameof(accountId));

			Account account = await _repository.GetAccountAsync(accountId);
			if (account == null)
				throw new InvalidOperationException("Cannot find account: " + accountId);

			return account;
		}

		public async Task<bool> PostAsync(Transaction transaction)
		{
			var relatedAccounts = transaction
				.RelatedAccounts()
				.Distinct()
				.ToArray();

			foreach (var account in relatedAccounts)
			{
				var budgets = (await GetBudgetsForAccountAsync(account.Id)).ToArray();
				if (budgets.Length == 0)
					continue;

				await ProcessBudgetPayout(transaction, account, budgets);
			}

			if (transaction.Check())
			{
				transaction.Id = TransactionId.From(transaction);
				if (await _repository.PostAsync(transaction))
				{
					foreach (var debitAmount in transaction.Debit)
						debitAmount.Post();

					foreach (var creditAmount in transaction.Credit)
						creditAmount.Post();
				}

				return true;
			}

			return false;
		}

		private async Task ProcessBudgetPayout(Transaction transaction, Account account, IEnumerable<Budget> budgets)
		{
			Func<Task<Account>, Task> setBudgetAccount;
			Account debit = null, credit = null;

			if (transaction.IsDebitAccount(account))
			{
				setBudgetAccount = async a => { credit = await a; };
				debit = await GetAccountAsync("Budget");
			}
			else
			{
				setBudgetAccount = async a => { debit = await a; };
				credit = await GetAccountAsync("Budget");
			}

			foreach (var budget in budgets)
			{
				await setBudgetAccount(GetAccountAsync(budget.BudgetAccount.Id));
				transaction.Debit.Add(new Amount(debit, EntryType.Debit, transaction.Amount));
				transaction.Credit.Add(new Amount(credit, EntryType.Credit, transaction.Amount));
			}
		}

		public async Task<IEnumerable<Transaction>> GetTransactionsAsync(string accountId)
		{
			return await _repository.GetTransactionsAsync(accountId);
		}

		public void Dispose()
		{
		}

		public async Task AddBudget(Budget budget)
		{
			if (await _repository.GetAccountAsync(budget.BudgetAccount.Id) == null)
			{
				await AddAsync(budget.BudgetAccount);
			}

			await _repository.AddBudgetAsync(budget);
		}

		public Task<IEnumerable<Budget>> GetBudgetsForAccountAsync(string accountId)
		{
			return _repository.GetBudgetsForAccountAsync(accountId);
		}

		public async Task<IEnumerable<Account>> GetBudgetAccountAsync(string accountId)
		{
			var accounts = await _repository.GetBudgetsForAccountAsync(accountId);
			return accounts.Select(x => x.BudgetAccount);
		}

		public Task<bool> DeleteAccount(string id)
		{
			return _repository.DeleteAccountAsync(id);
		}
	}
}