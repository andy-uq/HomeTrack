using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Autofac;

namespace HomeTrack.Core
{
	public class PatternBuilder
	{
		private readonly Func<Dictionary<string, string>, IPattern> _build;
		private readonly Func<IPattern, Dictionary<string, string>> _parse;

		public string Name { get; set; }
		public Dictionary<string, string> Properties { get; set; }

		private PatternBuilder(string name, IEnumerable<string> properties, Func<Dictionary<string, string>, IPattern> build, Func<IPattern, Dictionary<string,string>> parse)
		{
			Name = name;
			Properties = properties.ToDictionary(k => k, v => string.Empty);
			_build = build;
			_parse = parse;
		}

		private void SetValues(IPattern pattern)
		{
			Properties = _parse(pattern);
		}

		public static void Register(ContainerBuilder builder)
		{
			foreach ( var b in GetPatterns() )
				builder.RegisterInstance(b);
		}

		public static IEnumerable<PatternBuilder> GetPatterns()
		{
			yield return new PatternBuilder("Amount", new[] {"Amount", "Direction"}, BuildAmount, ParseAmount);
			yield return new PatternBuilder("Amount Range", new[] { "Min", "Max" }, BuildAmountRange, ParseAmountRange);
			yield return new PatternBuilder("Day Of Month", new[] {"Days of Month"}, BuildDayOfMonth, ParseDayOfMonth);
			yield return new PatternBuilder("Field", new[] {"Name", "Pattern"}, BuildFieldPattern, ParseFieldPattern);
		}

		private static Dictionary<string, string> ParseFieldPattern(IPattern p)
		{
			var amount = (FieldPattern) p;
			return new Dictionary<string, string>
			{
				{"Name", amount.Name},
				{"Pattern", amount.Pattern}
			};
		}

		private static Dictionary<string, string> ParseAmountRange(IPattern p)
		{
			var amount = (AmountRangePattern)p;
			return new Dictionary<string, string>
			{
				{"Min", amount.Min.ToString("n2")},
				{"Max", amount.Max.ToString("n2")}
			};
		}

		private static Dictionary<string, string> ParseDayOfMonth(IPattern p)
		{
			var amount = (DayOfMonthPattern) p;
			return new Dictionary<string, string>
			{
				{"Days of Month", string.Join(", ", amount.DaysOfMonth.Select(x => x.ToString(CultureInfo.InvariantCulture)))}
			};
		}

		private static Dictionary<string, string> ParseAmount(IPattern p)
		{
			var amount = (AmountPattern) p;
			return new Dictionary<string, string>
			{
				{"Amount", amount.Amount.ToString("n2")},
				{"Direction", amount.Direction.ToString()}
			};
		}

		private static IPattern BuildFieldPattern(Dictionary<string, string> p)
		{
			return new FieldPattern
			{
				Name = GetValue<string>(p, "Name"),
				Pattern = GetValue<string>(p, "Pattern")
			};
		}

		private static IPattern BuildAmountRange(Dictionary<string, string> p)
		{
			return new AmountRangePattern
			{
				Min = GetValue<decimal>(p, "Min"), 
				Max = GetValue<decimal>(p, "Max")
			};
		}

		private static IPattern BuildDayOfMonth(Dictionary<string, string> p)
		{
			var value = GetValue<string>(p, "Days of Month") ?? string.Empty;
			return new DayOfMonthPattern
			{
				DaysOfMonth = Regex.Matches(value, @"\d+").Cast<Match>().Select(m => Convert.ToInt32(m.Value)).ToArray()
			};
		}

		private static IPattern BuildAmount(Dictionary<string, string> p)
		{
			return new AmountPattern
			{
				Amount = GetValue<decimal>(p, "Amount"),
				Direction = GetValue<EntryType>(p, "Direction")
			};
		}

		private static T GetValue<T>(IDictionary<string, string> p, string name)
		{
			string value;
			if ( p.TryGetValue(name, out value) )
			{
				return (T)Convert.ChangeType(value, typeof (T));
			}

			return default(T);
		}

		public IPattern Build(Dictionary<string, string> properties)
		{
			return _build(properties);
		}

		public static IEnumerable<PatternBuilder> Parse(IPattern pattern)
		{
			var compositePattern = pattern as CompositePattern;
			if (compositePattern == null)
			{
				var name = Regex.Match(pattern.GetType().Name, @"(\w+)Pattern").Groups[1].Value;
				var builder = GetPatterns().Single(x => name == x.Name.Replace(" ", ""));
				builder.SetValues(pattern);

				yield return builder;
			}
			else
			{
				foreach (var child in compositePattern.SelectMany(Parse))
				{
					yield return child;
				}
			}
		}
	}
}