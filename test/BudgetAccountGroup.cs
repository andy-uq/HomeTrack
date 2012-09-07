using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class BudgetAccountGroupTests
	{
		private BudgetAccountGroup _b;

		[SetUp]
		public void SetUp()
		{
			_b = new BudgetAccountGroup
			{
				BudgetAccount = AccountFactory.Expense("Grocery Budget"), 
				RealAccount = AccountFactory.Expense("Groceries"),

			};
		}

		[Test]
		public void AllocateBudget()
		{
			var budgetAccount = AccountFactory.Expense("Expense Budget");
			_b.Allocate(budgetAccount);
		}
	}



	public class BudgetAccountGroup
	{
		public Account BudgetAccount { get; set; }
		public Account RealAccount { get; set; }

		public decimal BudgetAmount { get; set; }

		public Transaction Allocate(Account budgetAccount)
		{
			return new Transaction();
		}
	}
}