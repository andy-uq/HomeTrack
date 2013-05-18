using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace HomeTrack.Core
{
	public class AmountRangePattern : IPattern
	{
		public decimal Min { get; set; }
		public decimal Max { get; set; }

		public bool IsMatch(IImportRow importRow)
		{
			var amount = Math.Abs(importRow.Amount);
			return amount >= Min && amount <= Max;
		}

		public override string ToString()
		{
			return string.Format("Amount >= {0:n2} and <= {1:n2}", Min, Max);
		}
	}

	public class AmountPattern : IPattern
	{
		public decimal Amount { get; set; }
		public EntryType Direction { get; set; }

		public bool IsMatch(IImportRow importRow)
		{
			switch (Direction)
			{
				case EntryType.NotSpecified:
					return Amount == Math.Abs(importRow.Amount);

				case EntryType.Debit:
					return Amount == importRow.Amount;

				case EntryType.Credit:
					return Amount == -importRow.Amount;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public override string ToString()
		{
			return string.Format("Amount={0:n2}{1}", Amount, Direction.ToDrCrString());
		}
	}

	public class DayOfMonthPattern : IPattern
	{
		public int[] DaysOfMonth { get; set; }

		public DayOfMonthPattern()
		{
		}

		public DayOfMonthPattern(params int[] daysOfMonth)
		{
			DaysOfMonth = daysOfMonth;
		}

		public bool IsMatch(IImportRow importRow)
		{
			return DaysOfMonth.Any(x => importRow.Date.Day == x);
		}

		public override string ToString()
		{
			return DaysOfMonth.Length == 1
			       	? string.Format("Day = {0}", DaysOfMonth[0])
			       	: string.Format("Day in [{0}]", string.Join(",", DaysOfMonth.Select(x => x.ToString(CultureInfo.InvariantCulture))));
		}
	}

	public class FieldPattern : IPattern
	{
		private Regex _regex;

		public string Name { get; set; }
		public string Pattern { get; set; }

		private Regex Regex
		{
			get { return _regex ?? (_regex = new Regex(Pattern, RegexOptions.IgnoreCase)); }
		}

		public bool IsMatch(IImportRow importRow)
		{
			var value = importRow.Properties.SingleOrDefault(x => x.Key == Name);
			return value.Value != null && Regex.IsMatch(value.Value.Trim());
		}

		public override string ToString()
		{
			return string.Format("{0} matches {1}", Name, Pattern);
		}
	}
}