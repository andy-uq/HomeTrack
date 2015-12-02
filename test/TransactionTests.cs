using System;
using System.Linq;
using FluentAssertions;

namespace HomeTrack.Tests
{
	public class TransactionTests
	{
		private readonly Account _bank;
		private readonly Account _cashOnHand;
		private readonly Account _mortgage;
		private readonly Transaction _transaction;

		public TransactionTests()
		{
			_bank = AccountFactory.Asset("Bank");
			_cashOnHand = AccountFactory.Asset("Cash on hand");
			_mortgage = AccountFactory.Liability("Mortgage");

			_transaction = new Transaction
			{
				Amount = 10M,
				Debit = {new Amount(_bank, EntryType.Debit, 10)},
				Credit = {new Amount(_cashOnHand, EntryType.Credit, 10)}
			};
		}

		public void CreateTransactionWithNoDebitAccount()
		{
			this.Invoking(_ => new Transaction(null, null, 0M))
				.ShouldThrow<ArgumentNullException>()
				.WithMessage("Value can not be null.\r\nParameter name: debit");
		}

		public void CreateTransactionWithNoCreditAccount()
		{
			this.Invoking(_ => new Transaction(_bank, null, 0M))
				.ShouldThrow<ArgumentNullException>()
				.WithMessage("Value can not be null.\r\nParameter name: credit");
		}

		public void CreateTransaction()
		{
			var t = new Transaction(_cashOnHand, _bank, 10M) {Date = new DateTime(2012, 1, 1), Description = "Withdrawal"};
			t.Debit.First().Direction.Should().Be(EntryType.Debit);
			t.Credit.First().Direction.Should().Be(EntryType.Credit);
			t.ToString().Should().Be("2012-01-01 00:00 - Withdrawal $10.00");
		}

		public void RelatedAccounts()
		{
			var t = new Transaction(_cashOnHand, _bank, 10M);
			t.RelatedAccounts().OrderBy(x => x.Name).Should().Equal(_bank, _cashOnHand);
		}

		public void TransactionIs()
		{
			var t = new Transaction(_cashOnHand, _bank, 10M);
			t.Is(_bank).Should().BeTrue();
			t.Is(_cashOnHand).Should().BeTrue();
			t.Is(_mortgage).Should().BeFalse();
		}

		public void IsDebitOrIsCredit()
		{
			var t = new Transaction(debit: _cashOnHand, credit: _bank, amount: 10M);

			t.IsDebitAccount(_cashOnHand).Should().BeTrue();
			t.IsDebitAccount(_bank).Should().BeFalse();
			t.IsDebitAccount(_mortgage).Should().BeFalse();

			t.IsCreditAccount(_cashOnHand).Should().BeFalse();
			t.IsCreditAccount(_bank).Should().BeTrue();
			t.IsCreditAccount(_mortgage).Should().BeFalse();
		}

		public void CreateTransactionWithObjectInitialiser()
		{
			new Transaction
			{
				Debit = {new Amount(_bank, EntryType.Debit, 10)},
				Credit = {new Amount(_cashOnHand, EntryType.Credit, 10)}
			};
		}

		public void DebitValue()
		{
			_transaction.Debit.Sum(x => x.DebitValue).Should().Be(10M);
			_transaction.Credit.Sum(x => x.CreditValue).Should().Be(10M);
		}

		public void CreditValue()
		{
			var a = new Amount(_bank, EntryType.Debit, 10M);
			a.DebitValue.Should().Be(10M);
			a.CreditValue.Should().Be(-10M);
		}

		public void CheckTransaction()
		{
			_transaction.Check().Should().BeTrue();
		}

		public void PostTransaction()
		{
			var generalLedger = new GeneralLedger(new InMemoryRepository());
			generalLedger.Post(_transaction).Should().BeTrue();
		}
	}
}