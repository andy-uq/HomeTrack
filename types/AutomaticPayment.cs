using System;

namespace HomeTrack
{
	public class AutomaticPayment
	{
		public Account Debit { get; set; }
		public Account Credit { get; set; }
		public decimal Amount { get; set; }

		public string Description { get; set; }

		public Transaction BuildTransaction()
		{
			return new Transaction(Debit, Credit, Amount)
			{
				Description = Description,
				Date = DateTimeServer.Now
			};
		}
	}
}