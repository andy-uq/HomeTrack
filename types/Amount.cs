using System;
using EnsureThat;

namespace HomeTrack
{
	public class Amount : IEquatable<Amount>
	{
		public Amount()
		{
		}

		public Amount(Account account, EntryType entryType, decimal value)
		{
			if (value <= 0M)
				throw new ArgumentException("Cannot express a value of zero or less");

			Account = account;
			Direction = entryType;
			Value = value;
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

		public bool Equals(Amount other)
		{
			return Value == other.Value
			       && Account.Equals(other.Account)
			       && Direction == other.Direction;
		}

		public override string ToString()
		{
			return string.Format("{0}: {1:n2} {2}", Account.Name, Value, Direction.ToDrCrString());
		}
	}
}