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
			return importRow.Amount >= Min && importRow.Amount <= Max;
		}

		public override string ToString()
		{
			return string.Format("Amount >= {0:n2} and <= {1:n2}", Min, Max);
		}
	}

	public class AmountPattern : IPattern
	{
		public decimal Amount { get; set; }

		public bool IsMatch(IImportRow importRow)
		{
			return importRow.Amount == Amount;
		}

		public override string ToString()
		{
			return string.Format("Amount={0:n2}", Amount);
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
			return DaysOfMonth.Length == 0
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
			return Regex.IsMatch(value.Value);
		}
	}
}