using System;

namespace HomeTrack.RavenStore.Documents
{
	public class ImportResult
	{
		public int Id { get; set; }

		public string Name { get; set; }
		public DateTime Date { get; set; }
		public ImportedTransaction[] Transactions { get; set; }

		public int TransactionCount { get; set; }
		public int UnclassifiedTransactions { get; set; }
	}

	public class ImportedTransaction
	{
		public string Id { get; set; }
		public int TransactionId { get; set; }
	}
}