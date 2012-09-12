using System;

namespace HomeTrack
{
	public class BudgetAccountGroup
	{
		public Account BudgetAccount { get; set; }
		public Account RealAccount { get; set; }

		public decimal BudgetAmount { get; set; }
	}
}