using System.Linq;
using FluentAssertions;
using HomeTrack.Core;

namespace HomeTrack.Tests.Budgets
{
	public class CreateBudgetTests
	{
		private readonly Budget _b;
		private readonly Account _expenseAccount;
		private readonly Account _expenseBudgetAccount;
		private readonly GeneralLedger _general;

		public CreateBudgetTests()
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

			_general.AddBudget(_b);
		}

		public void AllocateBudget()
		{
			var budgetAccount = AccountFactory.Expense("Expense Budget");
			var transaction = _b.Allocate(budgetAccount);

			var credit = transaction.Credit.Single();
			credit.Account.Should().Be(budgetAccount);
			credit.Value.Should().Be(100M);

			var debit = transaction.Debit.Single();
			debit.Account.Should().Be(_b.BudgetAccount);
			credit.Value.Should().Be(100M);

			_general.Post(transaction);
			budgetAccount.Balance.Should().Be(-100M);
			_expenseBudgetAccount.Balance.Should().Be(100M);
		}

		public void GetLinkedAccount()
		{
			var account = _general.GetBudgetAccount(_b.RealAccount.Id);
			account.Should().BeEquivalentTo(_b.BudgetAccount);
		}
	}
}