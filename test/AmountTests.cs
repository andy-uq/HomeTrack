using System;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class AmountTests
	{
		private Account _bank;
		private Account _mortgage;

		[SetUp]
		public void SetUp()
		{
			_bank = AccountFactory.Debit("Bank");
			_mortgage = AccountFactory.Credit("Mortgage");
		}

		[Test]
		public void CreateAmount()
		{
			var amt = new Amount(_bank, EntryType.Credit, 10M);
			Assert.That(amt.Account, Is.EqualTo(_bank));
			Assert.That(amt.Direction, Is.EqualTo(EntryType.Credit));
			Assert.That(amt.Value, Is.EqualTo(10M));
		}

		[Test]
		public void CreateAmountWithImplicitType()
		{
			var amt = new Amount(_bank, 10M);
			Assert.That(amt.Account, Is.EqualTo(_bank));
			Assert.That(amt.Direction, Is.EqualTo(EntryType.Debit));
			Assert.That(amt.Value, Is.EqualTo(10M));
		}

		[Test]
		public void CreateNegativeAmountWithImplicitType()
		{
			var amt = new Amount(_bank, -10M);
			Assert.That(amt.Account, Is.EqualTo(_bank));
			Assert.That(amt.Direction, Is.EqualTo(EntryType.Credit));
			Assert.That(amt.Value, Is.EqualTo(10M));
		}

		[Test,ExpectedException(typeof(ArgumentException))]
		public void CannotCreateZeroAmount()
		{
			new Amount(_bank, 0M);
		}

		[Test,ExpectedException(typeof(ArgumentException))]
		public void CannotCreateNegativeExplicitAmount()
		{
			new Amount(_bank, EntryType.Debit, -10M);
		}

		[Test]
		public void DebitValue()
		{
			var a = new Amount(_bank, -10M);
			Assert.That(a.DebitValue, Is.EqualTo(-10M));
			Assert.That(a.CreditValue, Is.EqualTo(10M));
		}

		[Test]
		public void CreditValue()
		{
			var a = new Amount(_bank, 10M);
			Assert.That(a.DebitValue, Is.EqualTo(10M));
			Assert.That(a.CreditValue, Is.EqualTo(-10M));
		}
	}
}