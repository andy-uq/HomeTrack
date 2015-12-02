using System;
using System.Linq;
using FluentAssertions;

namespace HomeTrack.Tests
{
	public class AutomaticPaymentTests
	{
		private readonly AutomaticPayment _automaticPayment;
		private readonly GeneralLedger _generalLedger;
		private readonly Account _income;
		private readonly Account _pocketMoney;

		public AutomaticPaymentTests()
		{
			DateTimeServer.SetLocal(new TestDateTimeServer(DateTime.Parse("2012-1-1")));

			_income = AccountFactory.Income("Income", initialBalance: 100M);
			_pocketMoney = AccountFactory.Income("Pocket Money", initialBalance: 100M);
			_automaticPayment = new AutomaticPayment
			{
				Debit = _income,
				Credit = _pocketMoney,
				Amount = 10M,
				Description = "Weekly pocket money"
			};

			_generalLedger = new GeneralLedger(new InMemoryRepository())
			{
				_income,
				_pocketMoney
			};
		}

		public void BuildAutomaticPaymentTransaction()
		{
			var payment = _automaticPayment;
			var transaction = payment.BuildTransaction();
			transaction.Should().NotBeNull();
			transaction.Debit.Single().Account.Should().Be(_income);
			transaction.Credit.Single().Account.Should().Be(_pocketMoney);
			transaction.Amount.Should().Be(10M);
			transaction.Description.Should().Be("Weekly pocket money");
			transaction.Date.Should().Be(DateTime.Parse("2012-1-1"));
		}

		public void ApplyAutomaticPaymentTransaction()
		{
			var payment = _automaticPayment;
			var transaction = payment.BuildTransaction();
			_generalLedger.Post(transaction);

			_income.Balance.Should().Be(90M);
			_pocketMoney.Balance.Should().Be(110M);
		}
	}
}