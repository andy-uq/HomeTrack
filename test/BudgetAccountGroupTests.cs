using System;
using System.Linq;
using HomeTrack.Core;
using NUnit.Framework;

namespace HomeTrack.Tests
{
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
			_general = new GeneralLedger(new InMemoryGeneralLedger()) { _expenseBudgetAccount, _expenseAccount };

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
			
			var allocateTransaction = _b.Allocate(budgetAccount);
			_general.Post(allocateTransaction);

			var tuple = _b.Pay(budgetAccount, bank, 50M);
			_general.Post(tuple.Item1);
			_general.Post(tuple.Item2);

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

			var tuple = _b.Pay(budgetAccount, bank, 150M);
			_general.Post(tuple.Item1);
			_general.Post(tuple.Item2);
	
			Assert.That(_expenseBudgetAccount.Balance, Is.EqualTo(-50M));
			Assert.That(_expenseAccount.Balance, Is.EqualTo(150M));
			Assert.That(bank.Balance, Is.EqualTo(850M));
			Assert.That(budgetAccount.Balance, Is.EqualTo(50M));
		}

		[Test]
		public void EndToEndAmount()
		{
			var budgetAccount = AccountFactory.Expense("Expense Budget");
			var bank = AccountFactory.Asset("Bank");

			var allocateTransaction = _b.Allocate(budgetAccount);
			_general.Post(allocateTransaction);

			var tuple = _b.Pay(budgetAccount, bank, 50M);
			_general.Post(tuple.Item1);
			_general.Post(tuple.Item2);

			Assert.That(_expenseBudgetAccount.Balance, Is.EqualTo(50M));
			Assert.That(_expenseAccount.Balance, Is.EqualTo(50M));
		}
	}
}