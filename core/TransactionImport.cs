using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeTrack.Core
{
	public class TransactionImport
	{
		private readonly TransactionImportContext _context;
		private IDictionary<string, ImportRowOptions> _mappings;

		public TransactionImport(TransactionImportContext transactionImportContext, Account source)
		{
			_context = transactionImportContext;
			Credit = source;

			Result = new ImportResult();
		}

		public ImportResult Result { get; set; }
		public Account Credit { get; private set; }
		public Account UnclassifedDestination { get; set; }

		public async Task<IEnumerable<Transaction>> Process(IImport import, IDictionary<string, ImportRowOptions> mappings = null)
		{
			Result.Date = DateTimeServer.Now;
			Result.Name = import.Name;
			Result.ImportType = import.ImportType;

			_mappings = mappings;

			var transactions = new List<Transaction>();
			foreach (var transaction in await BuildTransactionAsync(import))
			{
				if (await _context.PostAsync(transaction))
					transactions.Add(transaction);
			}

			return transactions;
		}

		private async Task<IEnumerable<Transaction>> BuildTransactionAsync(IImport import)
		{
			var transactions = new List<Transaction>();

			foreach (var row in import.GetData())
			{
				if (row.Amount == 0M)
					continue;

				var account = await GetAccount(row) ?? row.IdentifyAccount(_context.Patterns) ?? UnclassifedDestination;
				if (account == null)
					continue;

				var description = GetDescription(row);
				var transaction = AsTransaction(row, account, description);
				transactions.Add(transaction);
			}

			return transactions;
		}

		private string GetDescription(IImportRow row)
		{
			if ( _mappings == null )
			{
				return null;
			}

			ImportRowOptions options;
			return _mappings.TryGetValue(row.Id, out options)
				? options.Description
				: null;
		}

		private async Task<Account> GetAccount(IImportRow row)
		{
			if ( _mappings == null )
			{
				return null;
			}

			ImportRowOptions options;
			return _mappings.TryGetValue(row.Id, out options) 
				? await _context.GetAccountAsync(options.Account) 
				: null;
		}

		private Transaction AsTransaction(IImportRow row, Account account, string description)
		{
			var transaction = row.Amount > 0 
				? new Transaction(Credit, account, row.Amount) 
				: new Transaction(account, Credit, -row.Amount);

			transaction.Date = row.Date;
			transaction.Description = description ?? row.Description;
			transaction.Reference = row.Id;

			Result.TransactionCount++;
			if ( account == UnclassifedDestination )
			{
				Result.UnclassifiedTransactions++;
			}

			return transaction;
		}
	}
}