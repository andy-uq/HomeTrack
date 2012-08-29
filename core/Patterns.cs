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
			return importRow.Amount >= Min && importRow.Amount <= Max;
		}
	}

	public class AmountPattern : IPattern
	{
		public decimal Amount { get; set; }

		public bool IsMatch(IImportRow importRow)
		{
			return importRow.Amount == Amount;
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
			return Regex.IsMatch(value.Value);
		}
	}
}