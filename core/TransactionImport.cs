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

		public IEnumerable<Transaction> Process(IImport import)
		{
			return BuildTransaction(import).Where(transaction => _context.General.Post(transaction));
		}

		private IEnumerable<Transaction> BuildTransaction(IImport import)
		{
			return
				from row in import.GetData()
				where row.Amount != 0M
				let account = row.IdentifyAccount(_context.Patterns)
				where account != null
				select new Transaction(account, Credit, row.Amount)
				{
					Date = row.Date,
					Description = row.Description
				};
		}
	}
}