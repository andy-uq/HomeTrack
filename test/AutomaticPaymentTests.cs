using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class AutomaticPaymentTests
	{
		private Account _income;
		private Account _pocketMoney;

		[SetUp]
		public void SetUp()
		{
			_income = AccountFactory.Income("Income");
			_pocketMoney = AccountFactory.Income("Pocket Money");
		}

		[Test]	 
		public void CreateAutomaticPayment()
		{
			var payment = new AutomaticPayment() {Debit = _income, Credit = _pocketMoney, Amount = 10M };
		}
	}
}