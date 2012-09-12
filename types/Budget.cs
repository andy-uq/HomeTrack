using System;

namespace HomeTrack
{
	public class Budget
	{
		public Account BudgetAccount { get; set; }
		public Account RealAccount { get; set; }

		public decimal Amount { get; set; }
	}
}