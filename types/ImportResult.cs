using System;

namespace HomeTrack
{
	public class ImportResult
	{
		public string Name { get; set; }
		public string ImportType { get; set; }

		public DateTime Date { get; set; }

		public int TransactionCount { get; set; }
		public int UnclassifiedTransactions { get; set; }
	}

	public class ImportedTransaction
	{
		public string Id { get; set; }
		public string TransactionId { get; set; }
		public bool Unclassified { get; set; }
		public decimal Amount { get; set; }
	}
}