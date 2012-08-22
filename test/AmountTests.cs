using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class AmountTests
	{
		[Test]
		public void DebitAmountToString()
		{
			var bank = AccountFactory.Asset("Bank");
			var amount = new Amount(bank, EntryType.Debit, 10M);

			Assert.That(amount.ToString(), Is.EqualTo("Bank: 10.00 Dr"));
		}

		[Test]
		public void CreditAmountToString()
		{
			var bank = AccountFactory.Asset("Bank");
			var amount = new Amount(bank, EntryType.Credit, 10M);

			Assert.That(amount.ToString(), Is.EqualTo("Bank: 10.00 Cr"));
		}
	}
}