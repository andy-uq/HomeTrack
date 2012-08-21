using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class GeneralLedgerTests
	{
		private GeneralLedger _ledger;
		private Account _bank;
		private Account _mortgage;

		[SetUp]
		public void SetUp()
		{
			_bank = AccountFactory.Debit("Bank", a => a.Balance = 100);
			_mortgage = AccountFactory.Credit("Mortgage", a => a.Balance = 100);

			_ledger = new GeneralLedger(new InMemoryGeneralLedger())
			{
				_bank,
				_mortgage,
			};
		}

		[Test]
		public void CreateGeneralLedger()
		{
			Assert.That(_ledger, Is.Not.Empty);
		}

		[Test]
		public void AccessAccountByName()
		{
			var account = _ledger["Bank"];
			Assert.That(account, Is.SameAs(_bank));
		}

		[Test]
		public void CreditAccountsAreCredit()
		{
			Assert.That(_ledger.CreditAccounts, Has.All.Matches<Account>(x => x.Direction == EntryType.Credit));
		}

		[Test]
		public void DebitAccountsAreDebit()
		{
			Assert.That(_ledger.DebitAccounts, Has.All.Matches<Account>(x => x.Direction == EntryType.Debit));
		}

		[Test]
		public void TrialBalanceWithZeroBalance()
		{
			Assert.That(_ledger.TrialBalance(), Is.True);
		}

		[Test]
		public void TrialBalanceFails()
		{
			_bank.Credit(10M);
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

			Assert.That(_mortgage.Balance, Is.EqualTo(90M));
			Assert.That(_bank.Balance, Is.EqualTo(90M));
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
}