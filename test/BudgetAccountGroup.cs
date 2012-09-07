using System.Linq;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class BudgetAccountGroupTests
	{
		private BudgetAccountGroup _b;
		private Account _budgetAccount;
		private Account _expenseAccount;
		private GeneralLedger _general;

		[SetUp]
		public void SetUp()
		{
			_budgetAccount = AccountFactory.Expense("Grocery Budget");
			_expenseAccount = AccountFactory.Expense("Groceries");
			_general = new GeneralLedger(new InMemoryGeneralLedger()) { _budgetAccount, _expenseAccount };

			_b = new BudgetAccountGroup
			{
				BudgetAccount = _budgetAccount, 
				RealAccount = _expenseAccount,
				BudgetAmount = 100M
			};
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
		}

		[Test]
		public void PayBudgetedAmount()
		{
			var budgetAccount = AccountFactory.Expense("Expense Budget");
			var allocateTransaction = _b.Allocate(budgetAccount);
			_general.Post(allocateTransaction);

			var transaction = _b.Pay(50M);
			_general.Post(transaction);
	
			var debit = transaction.Debit.Single();
			Assert.That(debit.Account, Is.EqualTo(_expenseAccount));

			var credit = transaction.Credit.Single();
			Assert.That(credit.Account, Is.EqualTo(_budgetAccount));

			Assert.That(_budgetAccount.Balance, Is.EqualTo(50M));
			Assert.That(_expenseAccount.Balance, Is.EqualTo(50M));
		}

		[Test]
		public void GoOverBudgetedAmount()
		{
			var budgetAccount = AccountFactory.Expense("Expense Budget");
			var allocateTransaction = _b.Allocate(budgetAccount);
			_general.Post(allocateTransaction);

			var transaction = _b.Pay(150M);
			_general.Post(transaction);
	
			var debit = transaction.Debit.Single();
			Assert.That(debit.Account, Is.EqualTo(_expenseAccount));

			var credit = transaction.Credit.Single();
			Assert.That(credit.Account, Is.EqualTo(_budgetAccount));

			Assert.That(_budgetAccount.Balance, Is.EqualTo(-50M));
			Assert.That(_expenseAccount.Balance, Is.EqualTo(150M));
		}

		[Test]
		public void EndToEndAmount()
		{
			var budgetAccount = AccountFactory.Expense("Expense Budget");
			var bank = AccountFactory.Asset("Bank");

			var allocateTransaction = _b.Allocate(budgetAccount);
			_general.Post(allocateTransaction);



			var transaction = _b.Pay(150M);
			_general.Post(transaction);
	
			var debit = transaction.Debit.Single();
			Assert.That(debit.Account, Is.EqualTo(_expenseAccount));

			var credit = transaction.Credit.Single();
			Assert.That(credit.Account, Is.EqualTo(_budgetAccount));

			Assert.That(_budgetAccount.Balance, Is.EqualTo(-50M));
			Assert.That(_expenseAccount.Balance, Is.EqualTo(150M));
		}
	}
	
	public class BudgetAccountGroup
	{
		public Account BudgetAccount { get; set; }
		public Account RealAccount { get; set; }

		public decimal BudgetAmount { get; set; }

		public Transaction Allocate(Account budgetAccount)
		{
			return new Transaction(BudgetAccount, budgetAccount, BudgetAmount);
		}

		public Transaction Pay(decimal value)
		{
			return new Transaction(RealAccount, BudgetAccount, value);
		}
	}
}