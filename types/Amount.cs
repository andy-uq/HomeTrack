using System;
using EnsureThat;

namespace HomeTrack
{
	public class Amount
	{
		public Amount(Account account, EntryType entryType, decimal value)
		{
			Ensure.That(() => value).IsGt(0M);

			Account = account;
			Type = entryType;
			Value = value;
		}

		public Amount(Account account, decimal value)
		{
			if (value == 0M)
				throw new ArgumentException("Cannot express a value of zero");

			Account = account;

			if ( value >= 0M )
			{
				Type = account.Type;
				Value = value;
			}
			else
			{
				Type = account.Type.Invert();
				Value = -value;
			}
		}

		public Account Account { get; set; }
		public EntryType Type { get; set; }
		public decimal Value { get; set; }

		public decimal DebitValue
		{
			get { return Type == EntryType.Debit ? Value : -Value; }
		}

		public decimal CreditValue
		{
			get { return Type == EntryType.Credit ? Value : -Value; }
		}
	}
}