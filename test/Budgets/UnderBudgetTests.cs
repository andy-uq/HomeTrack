using FluentAssertions;
using HomeTrack.Core;

namespace HomeTrack.Tests.Budgets
{
	public class UnderBudgetTests : BudgetAccountGroup
	{
		protected readonly Budget _b;
		protected readonly Account _expenseAccount;
		protected readonly Account _expenseBudgetAccount;
		protected readonly GeneralLedger _general;

		public UnderBudgetTests()
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

		public void PayBudgetedAmount()
		{
			var budgetAccount = AccountFactory.Expense("Expense Budget");
			var bank = AccountFactory.Asset("Bank", 1000M);

			_general.Add(budgetAccount);
			_general.Add(bank);

			var allocateTransaction = _b.Allocate(budgetAccount);
			_general.Post(allocateTransaction);

			_expenseBudgetAccount.Balance.Should().Be(100M);
			_expenseAccount.Balance.Should().Be(0M);

			var transaction = _b.Pay(budgetAccount, bank, 50M);
			_general.Post(transaction);

			_expenseBudgetAccount.Balance.Should().Be(50M);
			_expenseAccount.Balance.Should().Be(50M);
		}
	}
}