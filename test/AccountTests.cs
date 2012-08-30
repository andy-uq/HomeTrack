using System;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class AccountTests
	{
		private Account _debitAccount;
		private Account _creditAccount;

		[SetUp]
		public void SetUp()
		{
			_debitAccount = AccountFactory.Asset("Bank");
			_creditAccount = AccountFactory.Liability("Mortgage");
		}

		[Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "The string can't be null or empty.\r\nParameter name: name")]
		public void CreateAccountWithNoName()
		{
			new Account(null, AccountType.Asset);
		}

		[Test]
		public void AccountTypeDr()
		{
			const AccountType type = AccountType.Asset;
			Assert.That(type.ToCrDrString(), Is.EqualTo("Dr"));
			Assert.That(type.ToDr(10), Is.EqualTo(10));
			Assert.That(type.ToCr(10), Is.EqualTo(null));
		}
		[Test]
		public void AccountToString()
		{
			Assert.That(_creditAccount.ToString(), Is.EqualTo("Mortgage"));
		}

		[Test]
		public void AccountTypeCr()
		{
			const AccountType type = AccountType.Liability;
			Assert.That(type.ToCrDrString(), Is.EqualTo("Cr"));
			Assert.That(type.ToDr(10), Is.EqualTo(null));
			Assert.That(type.ToCr(10), Is.EqualTo(10));
		}

		[Test]
		public void CreateDebitAccount()
		{
			var account = _debitAccount;
			Assert.That(account.Name, Is.EqualTo("Bank"));
			Assert.That(account.Type, Is.EqualTo(AccountType.Asset));
			Assert.That(account.Direction, Is.EqualTo(EntryType.Debit));
		}

		[Test]
		public void CreateCreditAccount()
		{
			var account = _creditAccount;
			Assert.That(account.Name, Is.EqualTo("Mortgage"));
			Assert.That(account.Type, Is.EqualTo(AccountType.Liability));
			Assert.That(account.Direction, Is.EqualTo(EntryType.Credit));
		}

		[Test]
		public void AccountHasBalance()
		{
			var account = _creditAccount;
			Assert.That(account.Balance, Is.EqualTo(0M));
		}

		[Test]
		public void PostAmountDirect()
		{
			var account = _debitAccount;
			account.Post(10M, EntryType.Debit);
			Assert.That(account.Balance, Is.EqualTo(10M));

			account.Post(10M, EntryType.Credit);
			Assert.That(account.Balance, Is.EqualTo(0M));
		}

		[Test]
		public void PostAmount()
		{
			var account = _debitAccount;
			var amount = new Amount(account, EntryType.Debit, 10M);
			amount.Post();

			Assert.That(account.Balance, Is.EqualTo(10M));

			amount = new Amount(account, EntryType.Credit, 10M);
			amount.Post();
			Assert.That(account.Balance, Is.EqualTo(0M));
		}

		[Test]
		public void PostDebitAmountToDebitAccount()
		{
			var account = _debitAccount;
			account.Debit(10M);
			Assert.That(account.Balance, Is.EqualTo(10M));
		}

		[Test]
		public void PostDebitAmountToCreditAccount()
		{
			var account = _creditAccount;
			account.Debit(10M);
			Assert.That(account.Balance, Is.EqualTo(-10M));
		}

		[Test]
		public void PostCreditAmountToCreditAccount()
		{
			var account = _creditAccount;
			account.Credit(10M);
			Assert.That(account.Balance, Is.EqualTo(10M));
		}

		[Test]
		public void PostCreditAmountToDebitAccount()
		{
			var account = _debitAccount;
			account.Credit(10M);
			Assert.That(account.Balance, Is.EqualTo(-10M));
		}

	}
}