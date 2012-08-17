﻿using System;
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
			_debitAccount = AccountFactory.Debit("Bank");
			_creditAccount = AccountFactory.Credit("Mortgage");
		}

		[Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "The string can't be null or empty.\r\nParameter name: name")]
		public void CreateAccountWithNoName()
		{
			new Account(null, EntryType.Debit);
		}

		[Test]
		public void CreateDebitAccount()
		{
			var account = _debitAccount;
			Assert.That(account.Name, Is.EqualTo("Bank"));
			Assert.That(account.Type, Is.EqualTo(EntryType.Debit));
		}

		[Test]
		public void CreateCreditAccount()
		{
			var account = _creditAccount;
			Assert.That(account.Name, Is.EqualTo("Mortgage"));
			Assert.That(account.Type, Is.EqualTo(EntryType.Credit));
		}

		[Test]
		public void AccountHasBalance()
		{
			var account = _creditAccount;
			Assert.That(account.Balance, Is.EqualTo(0M));
		}

		[Test]
		public void PostAmount()
		{
			var account = _debitAccount;
			account.Post(10M, EntryType.Debit);
			Assert.That(account.Balance, Is.EqualTo(10M));

			account.Post(10M, EntryType.Credit);
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