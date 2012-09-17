using System;
using System.Collections.Generic;

namespace HomeTrack.Web.ViewModels
{
	public class TransactionDetails
	{
		public int Id { get; set; }
		public string AccountId { get; set; }

		public DateTime Date { get; set; }
		public string Description { get; set; }
		public string Reference { get; set; }
		public decimal Amount { get; set; }

		public IEnumerable<Amount> Debit { get; set; }
		public IEnumerable<Amount> Credit { get; set; }

		public IEnumerable<Amount> Amounts
		{
			get
			{
				foreach (var amount in Debit)
					yield return amount;

				foreach ( var amount in Credit )
					yield return amount;
			}
		}
	}
}