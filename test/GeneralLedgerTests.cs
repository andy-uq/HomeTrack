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
			_bank = AccountFactory.Debit("Bank");
			_mortgage = AccountFactory.Credit("Mortgage");

			_ledger = new GeneralLedger
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
	}
}