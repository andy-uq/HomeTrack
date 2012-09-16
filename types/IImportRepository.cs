using System.Collections.Generic;

namespace HomeTrack
{
	public interface IImportRepository
	{
		void Save(ImportResult result, IEnumerable<Transaction> transactions);
	}
}