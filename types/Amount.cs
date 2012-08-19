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
			Direction = entryType;
			Value = value;
		}

		public Amount(Account account, decimal value)
		{
			if (value == 0M)
				throw new ArgumentException("Cannot express a value of zero");

			Account = account;

			if ( value >= 0M )
			{
				Direction = account.Direction;
				Value = value;
			}
			else
			{
				Direction = account.Direction.Invert();
				Value = -value;
			}
		}

		public Account Account { get; set; }
		public EntryType Direction { get; set; }
		public decimal Value { get; set; }

		public decimal DebitValue
		{
			get { return Direction == EntryType.Debit ? Value : -Value; }
		}

		public decimal CreditValue
		{
			get { return Direction == EntryType.Credit ? Value : -Value; }
		}

		public void Post()
		{
			Account.Post(Value, Direction);
		}
	}
}