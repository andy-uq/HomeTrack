using System.Linq;
using FluentAssertions;
using HomeTrack.Core;

namespace HomeTrack.Tests.Budgets
{
	public class PostTransactionToAccountWithBudgetTests : BudgetAccountGroup
	{
		protected readonly Budget _b;
		protected readonly Account _expenseAccount;
		protected readonly Account _expenseBudgetAccount;
		protected readonly GeneralLedger _general;

		public PostTransactionToAccountWithBudgetTests()
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

		public void PostTransactionToAccountWithBudget()
		{
			var budgetAccount = AccountFactory.Expense("Budget");
			var bank = AccountFactory.Asset("Bank", 1000M);
			_general.Add(budgetAccount);
			_general.Add(bank);
			_general.AddBudget(_b);

			_general.Post(_b.Allocate(budgetAccount));
			_b.BudgetAccount.Balance.Should().Be(100M);

			var transaction = new Transaction(_expenseAccount, bank, 25M);
			_general.Post(transaction).Should().BeTrue();

			transaction.Amount.Should().Be(25M);
			transaction.Debit.Sum(x => x.Value).Should().Be(50M);
			transaction.Credit.Sum(x => x.Value).Should().Be(50M);

			bank.Balance.Should().Be(975M);
			_b.BudgetAccount.Balance.Should().Be(75M);
		}
	}
}