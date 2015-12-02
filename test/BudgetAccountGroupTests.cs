using System.Linq;
using FluentAssertions;
using HomeTrack.Core;

namespace HomeTrack.Tests
{
	public class BudgetAccountGroupTests
	{
		private readonly Budget _b;
		private readonly Account _expenseAccount;
		private readonly Account _expenseBudgetAccount;
		private readonly GeneralLedger _general;

		public BudgetAccountGroupTests()
		{
			_expenseBudgetAccount = AccountFactory.Expense("Grocery Budget");
			_expenseAccount = AccountFactory.Expense("Groceries");
			_general = new GeneralLedger(new InMemoryRepository()) {_expenseBudgetAccount, _expenseAccount};

			_b = new Budget
			{
				BudgetAccount = _expenseBudgetAccount,
				RealAccount = _expenseAccount,
				Amount = 100M
			};
		}

		public void CreateBudget()
		{
			_general.AddBudget(_b);
		}

		public void GetLinkedAccount()
		{
			_general.AddBudget(_b);
			var account = _general.GetBudgetAccount(_b.RealAccount.Id);
			account.Should().BeEquivalentTo(_b.BudgetAccount);
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

		public void GoOverBudgetedAmount()
		{
			var budgetAccount = AccountFactory.Expense("Expense Budget");
			var bank = AccountFactory.Asset("Bank", 1000M);
			var allocateTransaction = _b.Allocate(budgetAccount);
			_general.Post(allocateTransaction);

			var transaction = _b.Pay(budgetAccount, bank, 150M);
			_general.Post(transaction).Should().BeTrue();

			_expenseAccount.Balance.Should().Be(150M);
			bank.Balance.Should().Be(850M);
			budgetAccount.Balance.Should().Be(50M);

			_expenseBudgetAccount.Balance.Should().Be(-50M);
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