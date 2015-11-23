using System;
using System.Linq;
using FluentAssertions;
using HomeTrack.Core;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	// TODO: Change to fixie to stop breaking on parallel execution
	[TestFixture]
	public class BudgetAccountGroupTests
	{
		private Budget _b;
		private Account _expenseBudgetAccount;
		private Account _expenseAccount;
		private GeneralLedger _general;

		[SetUp]
		public void SetUp()
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

		[Test]
		public void CreateBudget()
		{
			_general.AddBudget(_b);
		}

		[Test]
		public void GetLinkedAccount()
		{
			_general.AddBudget(_b);
			var account = _general.GetBudgetAccount(_b.RealAccount.Id);
			Assert.That(account, Is.EquivalentTo(new[] { _b.BudgetAccount }));
		}
		
		[Test]
		public void PostTransactionToAccountWithBudget()
		{
			var budgetAccount = AccountFactory.Expense("Budget");
			var bank = AccountFactory.Asset("Bank", 1000M);
			_general.Add(budgetAccount);
			_general.Add(bank);
			_general.AddBudget(_b);
			
			_general.Post(_b.Allocate(budgetAccount));
			Assert.That(_b.BudgetAccount.Balance, Is.EqualTo(100M));

			var transaction = new Transaction(_expenseAccount, bank, 25M);
			Assert.That(_general.Post(transaction), Is.True);

			Assert.That(transaction.Amount, Is.EqualTo(25M));
			Assert.That(transaction.Debit.Sum(x => x.Value), Is.EqualTo(50M));
			Assert.That(transaction.Credit.Sum(x => x.Value), Is.EqualTo(50M));

			Assert.That(bank.Balance, Is.EqualTo(975M));
			Assert.That(_b.BudgetAccount.Balance, Is.EqualTo(75M));
		}

		[Test]
		public void AllocateBudget()
		{
			var budgetAccount = AccountFactory.Expense("Expense Budget");
			var transaction = _b.Allocate(budgetAccount);
			
			var credit = transaction.Credit.Single();
			Assert.That(credit.Account, Is.EqualTo(budgetAccount));
			Assert.That(credit.Value, Is.EqualTo(100M));
			
			var debit = transaction.Debit.Single();
			Assert.That(debit.Account, Is.EqualTo(_b.BudgetAccount));
			Assert.That(credit.Value, Is.EqualTo(100M));

			_general.Post(transaction);
			Assert.That(budgetAccount.Balance, Is.EqualTo(-100M));
			Assert.That(_expenseBudgetAccount.Balance, Is.EqualTo(100M));
		}

		[Test]
		public void PayBudgetedAmount()
		{
			var budgetAccount = AccountFactory.Expense("Expense Budget");
			var bank = AccountFactory.Asset("Bank", 1000M);

			_general.Add(budgetAccount);
			_general.Add(bank);

			var allocateTransaction = _b.Allocate(budgetAccount);
			_general.Post(allocateTransaction);

			Assert.That(_expenseBudgetAccount.Balance, Is.EqualTo(100M));
			Assert.That(_expenseAccount.Balance, Is.EqualTo(0M));

			var transaction = _b.Pay(budgetAccount, bank, 50M);
			_general.Post(transaction);

			Assert.That(_expenseBudgetAccount.Balance, Is.EqualTo(50M));
			Assert.That(_expenseAccount.Balance, Is.EqualTo(50M));
		}

		[Test]
		public void GoOverBudgetedAmount()
		{
			var budgetAccount = AccountFactory.Expense("Expense Budget");
			var bank = AccountFactory.Asset("Bank", 1000M);
			var allocateTransaction = _b.Allocate(budgetAccount);
			_general.Post(allocateTransaction);

			var transaction = _b.Pay(budgetAccount, bank, 150M);
			_general.Post(transaction).Should().BeTrue();
	
			Assert.That(_expenseAccount.Balance, Is.EqualTo(150M));
			Assert.That(bank.Balance, Is.EqualTo(850M));
			Assert.That(budgetAccount.Balance, Is.EqualTo(50M));

			Assert.That(_expenseBudgetAccount.Balance, Is.EqualTo(-50M));
		}

		[Test]
		public void EndToEndAmount()
		{
			var budgetAccount = AccountFactory.Expense("Expense Budget");
			var bank = AccountFactory.Asset("Bank");

			var allocateTransaction = _b.Allocate(budgetAccount);
			_general.Post(allocateTransaction);

			var transaction = _b.Pay(budgetAccount, bank, 50M);
			_general.Post(transaction);

			Assert.That(_expenseBudgetAccount.Balance, Is.EqualTo(50M));
			Assert.That(_expenseAccount.Balance, Is.EqualTo(50M));
		}
	}
}