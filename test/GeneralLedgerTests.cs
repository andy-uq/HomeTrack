using System;
using System.Collections.Generic;
using FluentAssertions;

namespace HomeTrack.Tests
{
	public abstract class GeneralLedgerTests : IDisposable
	{
		private readonly Account _bank;
		private readonly GeneralLedger _ledger;
		private readonly Account _mortgage;

		public GeneralLedgerTests()
		{
			_bank = AccountFactory.Asset("Bank", initialBalance: 100);
			_mortgage = AccountFactory.Liability("Mortgage", initialBalance: 100);

			_ledger = new GeneralLedger(LedgerRepository)
			{
				_bank,
				_mortgage
			};
		}

		protected abstract IGeneralLedgerRepository LedgerRepository { get; }

		public void Dispose()
		{
			_ledger.Dispose();
		}

		public void CreateGeneralLedger()
		{
			_ledger.Should().NotBeEmpty();
		}

		public void AccessAccountByName()
		{
			var account = _ledger["Bank"];
			account.Should().Be(_bank);
		}

		public void NullNameThrowsException()
		{
			Account account;
			_ledger.Invoking(_ => account = _[null])
				.ShouldThrow<ArgumentNullException>()
				.WithMessage("Value cannot be null.\r\nParameter name: accountId");
		}

		public void CreditAccountsAreCredit()
		{
			_ledger.CreditAccounts.Should().NotBeEmpty();
			_ledger.CreditAccounts.Should().OnlyContain(x => x.Direction == EntryType.Credit);
		}

		public void DebitAccountsAreDebit()
		{
			_ledger.DebitAccounts.Should().NotBeEmpty();
			_ledger.DebitAccounts.Should().OnlyContain(x => x.Direction == EntryType.Debit);
		}

		public void TrialBalanceWithZeroBalance()
		{
			_ledger.TrialBalance().Should().BeTrue();
		}

		public void ChangeBalanceSucceeds()
		{
			_bank.Balance.Should().Be(100M);
			_bank.Credit(10M);
			_bank.Balance.Should().Be(90M);
			_ledger.Add(_bank);
			_ledger[_bank.Id].Balance.Should().Be(90M);
		}

		public void TrialBalanceFails()
		{
			_bank.Balance.Should().Be(100M);
			_bank.Credit(10M);
			_bank.Balance.Should().Be(90M);

			_ledger.Add(_bank);

			_ledger[_bank.Id].Balance.Should().Be(90M);

			_ledger.TrialBalance().Should().BeFalse();
		}

		public void TrialBalanceSucceeds()
		{
			_bank.Credit(10M);
			_mortgage.Debit(10M);

			_ledger.TrialBalance().Should().BeTrue();
		}

		public void PostCredit()
		{
			var t = new Transaction(_mortgage, _bank, 10M);
			_ledger.Post(t);

			_ledger[_mortgage.Id].Balance.Should().Be(90M);
			_ledger[_bank.Id].Balance.Should().Be(90M);
		}

		public void GetTransaction()
		{
			var t1 = new Transaction(_mortgage, _bank, 10M);
			_ledger.Post(t1);

			var t2 = _ledger.GetTransaction(t1.Id);
			t2.Should().Be(t1);
		}

		public void PostDebit()
		{
			var t = new Transaction(_bank, _mortgage, 10M);
			_ledger.Post(t);

			_mortgage.Balance.Should().Be(110M);
			_bank.Balance.Should().Be(110M);
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

	public class InMemoryLedgerTests : GeneralLedgerTests
	{
		protected override IGeneralLedgerRepository LedgerRepository
		{
			get { return new InMemoryRepository(); }
		}
	}
}