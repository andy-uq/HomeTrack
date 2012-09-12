using AutoMapper;
using HomeTrack.RavenStore;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class BudgetMappingTests
	{
		private IMappingEngine _mappingEngine;

		[SetUp]
		public void SetUp()
		{
			var typeMapProvider = new RavenEntityTypeMapProvider();
			_mappingEngine = new MappingProvider(typeMapProvider).Build();
		}

		[Test]
		public void BudgetToRavenDocument()
		{
			var expense = AccountFactory.Expense("Groceries");
			var expenseBudget = AccountFactory.Expense("Grocery Budget");
			var budget = new Budget {RealAccount = expense, BudgetAccount = expenseBudget, Amount = 100M};

			var document = _mappingEngine.Map<HomeTrack.RavenStore.Documents.Budget>(budget);

			Assert.That(document.AccountId, Is.EqualTo(expense.Id));
			Assert.That(document.BudgetAccountId, Is.EqualTo(expenseBudget.Id));
			Assert.That(document.Amount, Is.EqualTo(100M));
		}

		[Test]
		public void RavenDocumentToBudget()
		{
			var expense = AccountFactory.Expense("Groceries");
			var expenseBudget = AccountFactory.Expense("Grocery Budget");
			var document = new RavenStore.Documents.Budget { AccountId = "groceries", BudgetAccountId = "grocerybudget", Amount = 100M };

			var budget = _mappingEngine.Map<Budget>(document);

			Assert.That(budget.RealAccount.Id, Is.EqualTo(expense.Id));
			Assert.That(budget.BudgetAccount.Id, Is.EqualTo(expenseBudget.Id));
			Assert.That(budget.Amount, Is.EqualTo(100M));
		}
	}
}