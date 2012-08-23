using System;
using System.Linq;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class AutomaticPaymentTests
	{
		private Account _income;
		private Account _pocketMoney;
		private GeneralLedger _generalLedger;
		private AutomaticPayment _automaticPayment;

		[SetUp]
		public void SetUp()
		{
			DateTimeServer.SetLocal(new TestDateTimeServer(DateTime.Parse("2012-1-1")));

			_income = AccountFactory.Income("Income", initialBalance:100M);
			_pocketMoney = AccountFactory.Income("Pocket Money", initialBalance: 100M);
			_automaticPayment = new AutomaticPayment() { Debit = _income, Credit = _pocketMoney, Amount = 10M, Description = "Weekly pocket money" };

			_generalLedger = new GeneralLedger(new InMemoryGeneralLedger())
			{
				_income,
				_pocketMoney
			};
		}

		[Test]	 
		public void BuildAutomaticPaymentTransaction()
		{
			var payment = _automaticPayment;
			var transaction = payment.BuildTransaction();
			Assert.That(transaction, Is.Not.Null);
			Assert.That(transaction.Debit.Single().Account, Is.EqualTo(_income));
			Assert.That(transaction.Credit.Single().Account, Is.EqualTo(_pocketMoney));
			Assert.That(transaction.Amount, Is.EqualTo(10M));
			Assert.That(transaction.Description, Is.EqualTo("Weekly pocket money"));
			Assert.That(transaction.Date, Is.EqualTo(DateTime.Parse("2012-1-1")));
		}

		[Test]	 
		public void ApplyAutomaticPaymentTransaction()
		{
			var payment = _automaticPayment;
			var transaction = payment.BuildTransaction();
			_generalLedger.Post(transaction);

			Assert.That(_income.Balance, Is.EqualTo(90M));
			Assert.That(_pocketMoney.Balance, Is.EqualTo(110M));
		}
	}
}