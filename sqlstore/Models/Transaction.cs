using System;

namespace HomeTrack.SqlStore.Models
{
	public class Transaction
	{
		public Amount[] Debit { get; set; }
		public Amount[] Credit { get; set; }

		public int Id { get; set; }

		public DateTime Date { get; set; }
		public string Description { get; set; }
		public string Reference { get; set; }

		public decimal Amount { get; set; }
	}

	public class Amount
	{
		public string AccountId { get; set; }
		public string AccountName { get; set; }
		public EntryType Direction { get; set; }
		public decimal Value { get; set; }
	}
}