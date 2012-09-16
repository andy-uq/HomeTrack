using System.Linq;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class BudgetRepositoryTests : RavenRepositoryTests
	{
		private Account _groceries;
		private Account _groceryBudget;
		private Account _bank;
		private Account _budget;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			_bank = AccountFactory.Asset("Bank");
			_groceries = AccountFactory.Expense("Groceries");
			_groceryBudget = AccountFactory.Expense("Grocery Budget");
			_budget = AccountFactory.Expense("Budget");
			
			GeneralLedger.Add(_groceries);
			GeneralLedger.Add(_bank);
			GeneralLedger.Add(_groceryBudget);
			GeneralLedger.Add(_budget);
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
			GeneralLedger.AddBudget(new Budget {BudgetAccount = _groceryBudget, RealAccount = _groceries, Amount = 100M});
			GeneralLedger.Post(new Transaction(_groceries, _bank, 50M));

			Assert.That(Repository.UseOnceTo(s => s.Query<RavenStore.Documents.Transaction>().Count()), Is.EqualTo(1));

			var transaction = Repository.UseOnceTo(s => s.Query<RavenStore.Documents.Transaction>().Single());
			Assert.That(transaction.Amount, Is.EqualTo(50M));
			Assert.That(transaction.Debit.Sum(x => x.Value), Is.EqualTo(100M));
			Assert.That(transaction.Credit.Sum(x => x.Value), Is.EqualTo(100M));
		}
	}
}