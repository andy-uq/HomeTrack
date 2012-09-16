using System;

namespace HomeTrack
{
	public class ImportResult
	{
		public string Name { get; set; }
		public DateTime Date { get; set; }

		public int TransactionCount { get; set; }
		public int UnclassifiedTransactions { get; set; }
	}
}