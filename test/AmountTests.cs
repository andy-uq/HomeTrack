using FluentAssertions;

namespace HomeTrack.Tests
{
	public class AmountTests
	{
		public void DebitAmountToString()
		{
			var amount = new Amount(TestData.Bank, EntryType.Debit, 10M);
			amount.ToString().Should().Be("Bank: 10.00 Dr");
		}

		public void DebitAmountAsDr()
		{
			var amount = new Amount(TestData.Bank, EntryType.Debit, 10M);
			amount.AsDr().Should().Be(10M);
		}

		public void DebitAmountAsCr()
		{
			var amount = new Amount(TestData.Bank, EntryType.Debit, 10M);
			amount.AsCr().Should().NotHaveValue();
		}

		public void CreditAmountToString()
		{
			var amount = new Amount(TestData.Bank, EntryType.Credit, 10M);

			amount.ToString().Should().Be("Bank: 10.00 Cr");
		}
	}
}