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
			_bank = AccountFactory.Asset("Bank");
			_cashOnHand = AccountFactory.Asset("Bank");
			_mortgage = AccountFactory.Liability("Mortgage");

			_transaction = new Transaction
			{
				Amount = 10M,
				Debit = { new Amount(_bank, EntryType.Debit, 10) },
				Credit = { new Amount(_cashOnHand, EntryType.Credit, 10) }
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
			var t = new Transaction(_cashOnHand, _bank, 10M) { Date = new DateTime(2012, 1, 1), Description = "Withdrawal" };
			Assert.That(t.Debit.First().Direction, Is.EqualTo(EntryType.Debit));
			Assert.That(t.Credit.First().Direction, Is.EqualTo(EntryType.Credit));
			Assert.That(t.ToString(), Is.EqualTo("2012-01-01 00:00 - Withdrawal $10.00"));
		}

		[Test]
		public void RelatedAccounts()
		{
			var t = new Transaction(_cashOnHand, _bank, 10M);
			Assert.That(t.RelatedAccounts(), Is.EqualTo(new[] {_bank, _cashOnHand}).Using(new AccountComparer()));
		}

		[Test]
		public void TransactionIs()
		{
			var t = new Transaction(_cashOnHand, _bank, 10M);
			Assert.That(t.Is(_bank));
			Assert.That(t.Is(_cashOnHand));
			Assert.That(t.Is(_mortgage), Is.False);
		}

		[Test]
		public void IsDebitOrIsCredit()
		{
			var t = new Transaction(_cashOnHand, _bank, 10M);

			Assert.That(t.IsDebitAccount(_cashOnHand), Is.True);
			Assert.That(t.IsDebitAccount(_bank), Is.False);
			Assert.That(t.IsDebitAccount(_mortgage), Is.False);

			Assert.That(t.IsCreditAccount(_cashOnHand), Is.False);
			Assert.That(t.IsCreditAccount(_bank), Is.True);
			Assert.That(t.IsCreditAccount(_mortgage), Is.False);
		}

		[Test]
		public void CreateTransactionWithObjectInitialiser()
		{
			new Transaction
			{
				Debit = { new Amount(_bank, EntryType.Debit, 10) },
				Credit = { new Amount(_cashOnHand, EntryType.Credit, 10) }
			};
		}

		[Test]
		public void DebitValue()
		{
			Assert.That(_transaction.Debit.Sum(x => x.DebitValue), Is.EqualTo(10M));
			Assert.That(_transaction.Credit.Sum(x => x.CreditValue), Is.EqualTo(10M));
		}

		[Test]
		public void CreditValue()
		{
			var a = new Amount(_bank, EntryType.Debit, 10M);
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
			var generalLedger = new GeneralLedger(new InMemoryGeneralLedger());
			Assert.That(generalLedger.Post(_transaction), Is.True);
		}
	}
}