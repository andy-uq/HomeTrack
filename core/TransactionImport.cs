using System.Collections.Generic;
using System.Linq;

namespace HomeTrack.Core
{
	public class TransactionImport
	{
		private readonly TransactionImportContext _context;

		public TransactionImport(TransactionImportContext transactionImportContext, Account source)
		{
			_context = transactionImportContext;
			Credit = source;
		}

		public Account Credit { get; private set; }
		public Account UnclassifedDestination { get; set; }

		public IEnumerable<Transaction> Process(IImport import)
		{
			return BuildTransaction(import).Where(transaction => _context.General.Post(transaction));
		}

		private IEnumerable<Transaction> BuildTransaction(IImport import)
		{
			return
				from row in import.GetData()
				where row.Amount != 0M
				let account = row.IdentifyAccount(_context.Patterns) ?? UnclassifedDestination
				where account != null
				select AsTransaction(row, account);
		}

		private Transaction AsTransaction(IImportRow row, Account account)
		{
			var transaction = row.Amount > 0 
				? new Transaction(Credit, account, row.Amount) 
				: new Transaction(account, Credit, -row.Amount);

			transaction.Date = row.Date;
			transaction.Description = row.Description;
			transaction.Reference = row.Id;

			return transaction;
		}
	}
}