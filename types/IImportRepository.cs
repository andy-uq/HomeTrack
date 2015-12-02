using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeTrack
{
	public interface IImportRepository
	{
		int Save(ImportResult result, IEnumerable<Transaction> transactions);
		IEnumerable<ImportResult> GetAll();
		IEnumerable<ImportedTransaction> GetTransactionIds(int importId);
		IEnumerable<Transaction> GetTransactions(int importId);
	}

	public interface IImportAsyncRepository
	{
		Task<int> SaveAsync(ImportResult result, IEnumerable<Transaction> transactions);
		Task<IEnumerable<ImportResult>> GetAllAsync();
		Task<IEnumerable<ImportedTransaction>> GetTransactionIdsAsync(int importId);
		Task<IEnumerable<Transaction>> GetTransactionsAsync(int importId);
	}
}