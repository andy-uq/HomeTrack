using FluentAssertions;
using HomeTrack.Core;

namespace HomeTrack.Tests.Budgets
{
	public class EndToEndBudgetTests 
	{
		private readonly Budget _b;
		private readonly Account _expenseAccount;
		private readonly Account _expenseBudgetAccount;
		private readonly GeneralLedger _general;

		public EndToEndBudgetTests()
		{
			_expenseBudgetAccount = AccountFactory.Expense("Grocery Budget");
			_expenseAccount = AccountFactory.Expense("Groceries");
			_general = new GeneralLedger(new InMemoryRepository()) { _expenseBudgetAccount, _expenseAccount };

			_b = new Budget
			{
				BudgetAccount = _expenseBudgetAccount,
				RealAccount = _expenseAccount,
				Amount = 100M
			};
		}

		public void EndToEndAmount()
		{
			var budgetAccount = AccountFactory.Expense("Expense Budget");
			var bank = AccountFactory.Asset("Bank");

			var allocateTransaction = _b.Allocate(budgetAccount);
			_general.Post(allocateTransaction);

			var transaction = _b.Pay(budgetAccount, bank, 50M);
			_general.Post(transaction);

			_expenseBudgetAccount.Balance.Should().Be(50M);
			_expenseAccount.Balance.Should().Be(50M);
		}
	}
}