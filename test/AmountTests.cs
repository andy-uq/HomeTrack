using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class AmountTests
	{
		private Account _account;

		public AmountTests()
		{
			_account = AccountFactory.Asset("Bank");
		}

		[Test]
		public void DebitAmountToString()
		{
			var amount = new Amount(_account, EntryType.Debit, 10M);
			Assert.That(amount.ToString(), Is.EqualTo("Bank: 10.00 Dr"));
		}

		[Test]
		public void DebitAmountAsDr()
		{
			var amount = new Amount(_account, EntryType.Debit, 10M);
			Assert.That(amount.AsDr(), Is.EqualTo(10M));
		}
		
		[Test]
		public void DebitAmountAsCr()
		{
			var amount = new Amount(_account, EntryType.Debit, 10M);
			Assert.That(amount.AsCr(), Is.Null);
		}

		[Test]
		public void CreditAmountToString()
		{
			var amount = new Amount(_account, EntryType.Credit, 10M);

			Assert.That(amount.ToString(), Is.EqualTo("Bank: 10.00 Cr"));
		}
	}
}