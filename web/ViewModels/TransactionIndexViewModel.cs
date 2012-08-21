using System;
using System.Collections.Generic;

namespace HomeTrack.Web.ViewModels
{
	public class TransactionIndexViewModel
	{
		public Account Account { get; set; }
		public IEnumerable<Transaction> Transactions { get; set; }

		public class Transaction
		{
			public int Id { get; set; }
			public DateTime Date { get; set; }
			public string Description { get; set; }
			public decimal? Debit { get; set; }
			public decimal? Credit { get; set; }
			public int ReferenceId { get; set; }
		}
	}
}