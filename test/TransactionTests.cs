using System;
using System.Linq;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class TransactionTests
	{
		private Account _bank;
		private Account _mortgage;
		private Account _cashOnHand;
		private Transaction _transaction;

		[SetUp]
		public void SetUp()
		{
			_bank = AccountFactory.Debit("Bank");
			_cashOnHand = AccountFactory.Debit("Bank");
			_mortgage = AccountFactory.Credit("Mortgage");

			_transaction = new Transaction
			{
				Amount = 10M,
				Debit = { new Amount(_bank, -10) },
				Credit = { new Amount(_cashOnHand, 10) }
			};
		}

		[Test, ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "Value can not be null.\r\nParameter name: debit")]
		public void CreateTransactionWithNoDebitAccount()
		{
			new Transaction(null, null, 0M);
		}

		[Test, ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "Value can not be null.\r\nParameter name: credit")]
		public void CreateTransactionWithNoCreditAccount()
		{
			new Transaction(_bank, null, 0M);
		}

		[Test]
		public void CreateTransaction()
		{
			new Transaction(_bank, _cashOnHand, -10M);
		}

		[Test]
		public void TransactionIs()
		{
			var t = new Transaction(_bank, _cashOnHand, -10M);
			Assert.That(t.Is(_bank));
			Assert.That(t.Is(_cashOnHand));
			Assert.That(t.Is(_mortgage), Is.False);
		}

		[Test]
		public void CreateTransactionWithObjectInitialiser()
		{
			new Transaction
			{
				Debit = { new Amount(_bank, -10) },
				Credit = { new Amount(_cashOnHand, -10) }
			};
		}

		[Test]
		public void DebitValue()
		{
			Assert.That(_transaction.Debit.Sum(x => x.DebitValue), Is.EqualTo(-10M));
			Assert.That(_transaction.Credit.Sum(x => x.CreditValue), Is.EqualTo(-10M));
		}

		[Test]
		public void CreditValue()
		{
			var a = new Amount(_bank, 10M);
			Assert.That(a.DebitValue, Is.EqualTo(10M));
			Assert.That(a.CreditValue, Is.EqualTo(-10M));
		}

		[Test]
		public void CheckTransaction()
		{
			Assert.That(_transaction.Check(), Is.True);
		}

		[Test]
		public void PostTransaction()
		{
			var generalLedger = new GeneralLedger(null);
			Assert.That(generalLedger.Post(_transaction), Is.True);
		}
	}
}