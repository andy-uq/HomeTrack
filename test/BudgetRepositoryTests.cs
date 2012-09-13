using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class BudgetRepositoryTests : RavenRepositoryTests
	{
		private Account _groceries;
		private Account _groceryBudget;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			_groceries = AccountFactory.Expense("Groceries");
			_groceryBudget = AccountFactory.Asset("Grocery Budget");
			
			GeneralLedger.Add(_groceries);
		}

		[Test]
		public void AddBudget()
		{
			GeneralLedger.AddBudget(new Budget { BudgetAccount = _groceryBudget, RealAccount = _groceries, Amount = 100M });
		}

		[Test]
		public void GetBudgetAccount()
		{
			GeneralLedger.AddBudget(new Budget { BudgetAccount = _groceryBudget, RealAccount = _groceries, Amount = 100M });
			Assert.That(GeneralLedger.GetBudgetAccount(_groceries.Id), Is.EquivalentTo(new[] { _groceryBudget }).Using(new AccountComparer()));
		}

		[Test]
		public void PostTransactionToAccountWithBudget()
		{
			GeneralLedger.AddBudget(new Budget { BudgetAccount = _groceryBudget, RealAccount = _groceries, Amount = 100M });
			Assert.That(GeneralLedger.GetBudgetAccount(_groceries.Id), Is.EquivalentTo(new[] { _groceryBudget }).Using(new AccountComparer()));
		}
	}
}