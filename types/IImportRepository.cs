using System.Collections.Generic;

namespace HomeTrack
{
	public interface IImportRepository
	{
		void Save(ImportResult result, IEnumerable<Transaction> transactions);
		IEnumerable<ImportResult> GetAll();
		IEnumerable<ImportedTransaction> GetTransactionIds(int importId);
		IEnumerable<Transaction> GetTransactions(int importId);
	}
}