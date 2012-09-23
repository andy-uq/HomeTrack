using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeTrack.Core
{
	public class TransactionImport
	{
		private readonly TransactionImportContext _context;
		private IDictionary<string, string> _mappings;

		public TransactionImport(TransactionImportContext transactionImportContext, Account source)
		{
			_context = transactionImportContext;
			Credit = source;

			Result = new ImportResult();
		}

		public ImportResult Result { get; set; }
		public Account Credit { get; private set; }
		public Account UnclassifedDestination { get; set; }

		public IEnumerable<Transaction> Process(IImport import, IDictionary<string, string> mappings = null)
		{
			Result.Date = DateTimeServer.Now;
			Result.Name = import.Name;
			Result.ImportType = import.ImportType;

			_mappings = mappings;

			return BuildTransaction(import).Where(transaction => _context.General.Post(transaction));
		}

		private IEnumerable<Transaction> BuildTransaction(IImport import)
		{
			return
				from row in import.GetData()
				where row.Amount != 0M
				let account = GetAccount(row) ?? row.IdentifyAccount(_context.Patterns) ?? UnclassifedDestination
				where account != null
				select AsTransaction(row, account);
		}

		private Account GetAccount(IImportRow row)
		{
			if ( _mappings == null )
			{
				return null;
			}

			string accountId;
			return _mappings.TryGetValue(row.Id, out accountId) 
				? _context.General[accountId] 
				: null;
		}

		private Transaction AsTransaction(IImportRow row, Account account)
		{
			var transaction = row.Amount > 0 
				? new Transaction(Credit, account, row.Amount) 
				: new Transaction(account, Credit, -row.Amount);

			transaction.Date = row.Date;
			transaction.Description = row.Description;
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