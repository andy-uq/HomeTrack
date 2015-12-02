using System;
using FluentAssertions;

namespace HomeTrack.Tests
{
	public class AccountTests
	{
		private readonly Account _creditAccount;
		private readonly Account _debitAccount;

		public AccountTests()
		{
			_debitAccount = AccountFactory.Asset("Bank");
			_creditAccount = AccountFactory.Liability("Mortgage");
		}

		public void CreateAccountWithNoName()
		{
			this.Invoking(_ => new Account(null, AccountType.Asset))
				.ShouldThrow<ArgumentException>()
				.WithMessage("The string can't be null or empty.\r\nParameter name: name");
		}

		public void AccountTypeDr()
		{
			const AccountType type = AccountType.Asset;
			type.ToCrDrString().Should().Be("Dr");
			type.ToDr(10).Should().Be(10);
			type.ToCr(10).Should().Be(null);
		}

		public void UnknownAccountTypeThrowsException()
		{
			var account = new Account {Name = "XXX"};

			EntryType entryType;
			account.Invoking(_ => entryType = _.Direction)
				.ShouldThrow<InvalidOperationException>()
				.WithMessage("The account \"XXX\" does not have an account type set.");
		}

		public void AccountToString()
		{
			_creditAccount.ToString().Should().Be("Mortgage");
		}

		public void AccountTypeCr()
		{
			const AccountType type = AccountType.Liability;
			type.ToCrDrString().Should().Be("Cr");
			type.ToDr(10).Should().Be(null);
			type.ToCr(10).Should().Be(10);
		}

		public void CreateDebitAccount()
		{
			var account = _debitAccount;
			account.Name.Should().Be("Bank");
			account.Type.Should().Be(AccountType.Asset);
			account.Direction.Should().Be(EntryType.Debit);
		}

		public void CreateCreditAccount()
		{
			var account = _creditAccount;
			account.Name.Should().Be("Mortgage");
			account.Type.Should().Be(AccountType.Liability);
			account.Direction.Should().Be(EntryType.Credit);
		}

		public void AccountHasBalance()
		{
			var account = _creditAccount;
			account.Balance.Should().Be(0M);
		}

		public void PostAmountDirect()
		{
			var account = _debitAccount;
			account.Post(10M, EntryType.Debit);
			account.Balance.Should().Be(10M);

			account.Post(10M, EntryType.Credit);
			account.Balance.Should().Be(0M);
		}

		public void PostAmount()
		{
			var account = _debitAccount;
			var amount = new Amount(account, EntryType.Debit, 10M);
			amount.Post();

			account.Balance.Should().Be(10M);

			amount = new Amount(account, EntryType.Credit, 10M);
			amount.Post();
			account.Balance.Should().Be(0M);
		}

		public void PostDebitAmountToDebitAccount()
		{
			var account = _debitAccount;
			account.Debit(10M);
			account.Balance.Should().Be(10M);
		}

		public void PostDebitAmountToCreditAccount()
		{
			var account = _creditAccount;
			account.Debit(10M);
			account.Balance.Should().Be(-10M);
		}

		public void PostCreditAmountToCreditAccount()
		{
			var account = _creditAccount;
			account.Credit(10M);
			account.Balance.Should().Be(10M);
		}

		public void PostCreditAmountToDebitAccount()
		{
			var account = _debitAccount;
			account.Credit(10M);
			account.Balance.Should().Be(-10M);
		}
	}
}