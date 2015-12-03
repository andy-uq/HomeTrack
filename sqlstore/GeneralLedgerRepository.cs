using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;
using HomeTrack.Mapping;

namespace HomeTrack.SqlStore
{
	public class GeneralLedgerRepository : IGeneralLedgerRepository, IGeneralLedgerAsyncRepository
	{
		private readonly SqlConnection _database;

		public GeneralLedgerRepository(SqlConnection database)
		{
			_database = database;
		}

		public IEnumerable<Account> Accounts
		{
			get
			{
				var accounts = _database.Query<Models.Account>("SELECT Account.* FROM Account");
				return accounts.MapAll<Account>();
			}
		}

		public IEnumerable<Account> DebitAccounts
		{
			get
			{
				var accounts = GetAccounts(EntryType.Debit);
				return accounts.MapAll<Account>();
			}
		}

		public IEnumerable<Account> CreditAccounts
		{
			get
			{
				var accounts = GetAccounts(EntryType.Credit);
				return accounts.MapAll<Account>();
			}
		}

		private IEnumerable<Models.Account> GetAccounts(EntryType entryType)
		{
			return _database.Query<Models.Account>(
				@"SELECT Account.* 
					FROM Account 
						INNER JOIN AccountType ON AccountType.Name = AccountTypeName 
					WHERE AccountType.EntryTypeName = @debitOrCredit",
				new {debitOrCredit = entryType.ToString()});
		}

		public Account GetAccount(string accountId)
		{
			var account = _database.Query<Models.Account>("SELECT Account.* FROM Account WHERE Id=@accountId", new {accountId}).Single();
			return account.Map<Account>();
		}

		public bool DeleteAccount(string accountId)
		{
			int rowCount = _database.Execute("DELETE FROM Account WHERE Id=@accountId AND NOT EXISTS (SELECT * FROM TransactionComponent WHERE AccountId = @accountId)", new { accountId });
			return rowCount == 1;
		}

		public IEnumerable<Budget> GetBudgetsForAccount(string accountId)
		{
			yield break;
		}

		public string Add(Account account)
		{
			var id = account.Id ?? Regex.Replace(account.Name.ToLower(), "[^a-z0-9-/]", "");

			_database.Execute("INSERT INTO [Account] (Id, Name, Description, AccountTypeName) VALUES (@id, @name, @description, @accountTypeName)",
				new { Id = id, account.Name, account.Description, accountTypeName = account.Type.ToString() });

			return id;
		}

		public void AddBudget(Budget budget)
		{
		}

		public bool Post(Transaction transaction)
		{
			var transactionComponents = new List<Models.TransactionComponent>();

			var model = transaction.Map<Models.Transaction>();
			transactionComponents.AddRange(transaction.Credit.Select(amount => amount.Map(new Models.TransactionComponent { TransactionId = model.Id, EntryTypeName = EntryType.Credit.ToString() })));
			transactionComponents.AddRange(transaction.Debit.Select(amount => amount.Map(new Models.TransactionComponent { TransactionId = model.Id, EntryTypeName = EntryType.Debit.ToString() })));

			_database.Execute(
				@"INSERT INTO [Transaction] (Id, Date, Amount, Reference, Description)
						VALUES (@id, @date, @amount, @reference, @description)",
				model);

			_database.Execute(
				@"INSERT INTO [TransactionComponent] (TransactionId, AccountId, EntryTypeName, Amount, Annotation, AppliedByRuleId)
						VALUES (@transactionId, @accountId, @entryTypeName, @amount, ISNULL(@annotation, ''), @appliedByRuleId)",
				transactionComponents);

			return true;
		}

		public IEnumerable<Transaction> GetTransactions(string accountId)
		{
			var transactions = _database.Query<Models.Transaction>(
				@"SELECT [Transaction].* 
					FROM [Transaction] 
					WHERE EXISTS (SELECT * FROM TransactionComponent WHERE AccountId=@accountId AND [Transaction].Id = TransactionId) 
					ORDER BY [Date]", new { accountId });

			return transactions.MapAll<Transaction>();
		}

		public Transaction GetTransaction(string id)
		{
			var transaction = _database.Query<Models.Transaction>(
				@"SELECT [Transaction].* 
					FROM [Transaction]
					WHERE Id = @id", new { id });

			return transaction.Map<Transaction>();
		}

		public void Dispose()
		{
			_database.Dispose();
		}

		public async Task<Account> GetAccountAsync(string accountId)
		{
			var account = await _database.QueryAsync<Models.Account>("SELECT Account.* FROM Account WHERE Id=@accountId", new { accountId });
			return account.Single().Map<Account>();
		}

		private Task<IEnumerable<Models.Account>> GetAccountsAsync(EntryType entryType)
		{
			return _database.QueryAsync<Models.Account>(
				@"SELECT Account.* 
					FROM Account 
						INNER JOIN AccountType ON AccountType.Name = AccountTypeName 
					WHERE AccountType.EntryTypeName = @debitOrCredit",
				new { debitOrCredit = entryType.ToString() });
		}

		public async Task<IEnumerable<Account>> GetAccountsAsync()
		{
			var accounts = await _database.QueryAsync<Models.Account>(@"SELECT Account.* FROM Account");
			return accounts.MapAll<HomeTrack.Account>().ToList();
		}

		public async Task<IEnumerable<Account>> GetDebitAccountsAsync()
		{
			var accounts = await GetAccountsAsync(EntryType.Debit);
			return accounts.MapAll<HomeTrack.Account>().ToList();
		}

		public async Task<IEnumerable<Account>> GetCreditAccountsAsync()
		{
			var accounts = await GetAccountsAsync(EntryType.Credit);
			return accounts.MapAll<HomeTrack.Account>().ToList();
		}

		public async Task<bool> DeleteAccountAsync(string accountId)
		{
			int rowCount = await _database.ExecuteAsync("DELETE FROM Account WHERE Id=@accountId AND NOT EXISTS (SELECT * FROM TransactionComponent WHERE AccountId = @accountId)", new { accountId });
			return rowCount == 1;
		}

		public Task<IEnumerable<Budget>> GetBudgetsForAccountAsync(string accountId)
		{
			return Task.FromResult(Enumerable.Empty<Budget>());
		}

		public async Task<string> AddAsync(Account account)
		{
			var id = account.Id ?? Regex.Replace(account.Name.ToLower(), "[^a-z0-9-/]", "");

			await _database.ExecuteAsync("INSERT INTO [Account] (Id, Name, Description, AccountTypeName) VALUES (@id, @name, @description, @accountTypeName)",
				new { Id = id, account.Name, account.Description, accountTypeName = account.Type.ToString() });

			return id;
		}

		public Task AddBudgetAsync(Budget budget)
		{
			return Task.CompletedTask;
		}

		public async Task<bool> PostAsync(Transaction transaction)
		{
			var transactionComponents = new List<Models.TransactionComponent>();

			var model = transaction.Map<Models.Transaction>();
			transactionComponents.AddRange(transaction.Credit.Select(amount => amount.Map(new Models.TransactionComponent { TransactionId = model.Id, EntryTypeName = EntryType.Credit.ToString() })));
			transactionComponents.AddRange(transaction.Debit.Select(amount => amount.Map(new Models.TransactionComponent { TransactionId = model.Id, EntryTypeName = EntryType.Debit.ToString() })));

			await _database.ExecuteAsync(
				@"INSERT INTO [Transaction] (Id, Date, Amount, Reference, Description)
						VALUES (@id, @date, @amount, @reference, @description)",
				model);

			await _database.ExecuteAsync(
				@"INSERT INTO [TransactionComponent] (TransactionId, AccountId, EntryTypeName, Amount, Annotation, AppliedByRuleId)
						VALUES (@transactionId, @accountId, @entryTypeName, @amount, ISNULL(@annotation, ''), @appliedByRuleId)",
				transactionComponents);

			return true;
		}

		public async Task<IEnumerable<Transaction>> GetTransactionsAsync(string accountId)
		{
			var transactions = await _database.QueryAsync<Models.Transaction>(
				@"SELECT [Transaction].* 
					FROM [Transaction] 
					WHERE EXISTS (SELECT * FROM TransactionComponent WHERE AccountId=@accountId AND [Transaction].Id = TransactionId) 
					ORDER BY [Date]", new { accountId });

			return transactions.MapAll<Transaction>();
		}

		public async Task<Transaction> GetTransactionAsync(string id)
		{
			var transaction = await _database.QueryAsync<Models.Transaction>(
				@"SELECT [Transaction].* 
					FROM [Transaction]
					WHERE Id = @id", new { id });

			return transaction.Single().Map<Transaction>();
		}
	}
}