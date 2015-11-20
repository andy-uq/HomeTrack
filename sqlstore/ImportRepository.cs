using System.Collections.Generic;
using HomeTrack;

namespace sqlstore
{
	public class ImportRepository : IImportRepository
	{
		public void Save(ImportResult result, IEnumerable<Transaction> transactions)
		{
		}

		public IEnumerable<ImportResult> GetAll()
		{
			yield break;
		}

		public IEnumerable<ImportedTransaction> GetTransactionIds(string importId)
		{
			yield break;
		}

		public IEnumerable<Transaction> GetTransactions(string importId)
		{
			yield break;
		}
	}
}