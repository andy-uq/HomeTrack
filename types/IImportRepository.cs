using System.Collections.Generic;

namespace HomeTrack
{
	public interface IImportRepository
	{
		void Save(ImportResult result, IEnumerable<Transaction> transactions);
		IEnumerable<ImportResult> GetAll();
		IEnumerable<ImportedTransaction> GetTransactionIds(string importId);
		IEnumerable<Transaction> GetTransactions(string importId);
	}
}