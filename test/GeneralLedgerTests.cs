﻿using System;
using System.Collections;
using System.Collections.Generic;
using AutoMapper;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	public abstract class GeneralLedgerTests
	{
		private GeneralLedger _ledger;
		private Account _bank;
		private Account _mortgage;

		[SetUp]
		public void SetUp()
		{
			_bank = AccountFactory.Asset("Bank", initialBalance:100);
			_mortgage = AccountFactory.Liability("Mortgage", initialBalance:100);

			_ledger = new GeneralLedger(LedgerRepository)
			{
				_bank,
				_mortgage,
			};
		}

		[TearDown]
		public void CleanUp()
		{
			_ledger.Dispose();
		}

		protected abstract IGeneralLedgerRepository LedgerRepository { get; }

		[Test]
		public void CreateGeneralLedger()
		{
			Assert.That(_ledger, Is.Not.Empty);
		}

		[Test]
		public void AccessAccountByName()
		{
			var account = _ledger["Bank"];
			Assert.That(account, Is.EqualTo(_bank).Using(new AccountComparer()));
		}

		[Test,ExpectedException(typeof(ArgumentNullException))]
		public void NullNameThrowsException()
		{
			var account = _ledger[null];
		}

		[Test]
		public void CreditAccountsAreCredit()
		{
			Assert.That(_ledger.CreditAccounts, Is.Not.Empty);
			Assert.That(_ledger.CreditAccounts, Has.All.Matches<Account>(x => x.Direction == EntryType.Credit));
		}

		[Test]
		public void DebitAccountsAreDebit()
		{
			Assert.That(_ledger.DebitAccounts, Is.Not.Empty);
			Assert.That(_ledger.DebitAccounts, Has.All.Matches<Account>(x => x.Direction == EntryType.Debit));
		}

		[Test]
		public void TrialBalanceWithZeroBalance()
		{
			Assert.That(_ledger.TrialBalance(), Is.True);
		}

		[Test]
		public void ChangeBalanceSucceeds()
		{
			Assert.That(_bank.Balance, Is.EqualTo(100M));
			_bank.Credit(10M);
			Assert.That(_bank.Balance, Is.EqualTo(90M));
			_ledger.Add(_bank);
			Assert.That(_ledger[_bank.Id].Balance, Is.EqualTo(90M));
		}

		[Test]
		public void TrialBalanceFails()
		{
			Assert.That(_bank.Balance, Is.EqualTo(100M));
			_bank.Credit(10M);
			Assert.That(_bank.Balance, Is.EqualTo(90M));
	
			_ledger.Add(_bank);

			Assert.That(_ledger[_bank.Id].Balance, Is.EqualTo(90M));

			Assert.That(_ledger.TrialBalance(), Is.False);
		}

		[Test]
		public void TrialBalanceSucceeds()
		{
			_bank.Credit(10M);
			_mortgage.Debit(10M);

			Assert.That(_ledger.TrialBalance(), Is.True);
		}

		[Test]
		public void PostCredit()
		{
			var t = new Transaction(_mortgage, _bank, 10M);
			_ledger.Post(t);

			Assert.That(_ledger[_mortgage.Id].Balance, Is.EqualTo(90M));
			Assert.That(_ledger[_bank.Id].Balance, Is.EqualTo(90M));
		}

		[Test]
		public void GetTransaction()
		{
			var t1 = new Transaction(_mortgage, _bank, 10M);
			_ledger.Post(t1);

			var t2 = _ledger.GetTransaction(t1.Id);
			Assert.That(t2, Is.EqualTo(t1).Using(new TransactionComparer()));
		}

		[Test]
		public void PostDebit()
		{
			var t = new Transaction(_bank, _mortgage, 10M);
			_ledger.Post(t);

			Assert.That(_mortgage.Balance, Is.EqualTo(110M));
			Assert.That(_bank.Balance, Is.EqualTo(110M));
		}
	}

	public class AccountComparer : IEqualityComparer<Account>
	{
		public bool Equals(Account x, Account y)
		{
			return
				x.Equals(y)
				&& x.Balance == y.Balance;
		}

		public int GetHashCode(Account obj)
		{
			throw new NotImplementedException();
		}
	}

	[TestFixture]
	public class InMemoryLedgerTests : GeneralLedgerTests
	{
		protected override IGeneralLedgerRepository LedgerRepository
		{
			get { return new InMemoryRepository(); }
		}
	}
}