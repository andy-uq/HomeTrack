using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using HomeTrack.Core;

namespace HomeTrack.Tests
{
	public class PatternBuilderTests
	{
		public void BuildAmountPattern()
		{
			var builder = PatternBuilder.GetPatterns().Single(x => x.Name == "Amount");
			var pattern = builder.Build(Dictionary(new[] {"Amount", "10"}, new[] {"Direction", "Credit"}));
			pattern.Should().BeOfType<AmountPattern>();

			var amount = (AmountPattern) pattern;
			amount.Amount.Should().Be(10M);
		}

		public void BuildAmountRangePattern()
		{
			var builder = PatternBuilder.GetPatterns().Single(x => x.Name == "Amount Range");
			var pattern = builder.Build(Dictionary(new[] {"Min", "10"}, new[] {"Max", "100"}));
			pattern.Should().BeOfType<AmountRangePattern>();

			var amountRangePattern = (AmountRangePattern) pattern;
			amountRangePattern.Min.Should().Be(10M);
			amountRangePattern.Max.Should().Be(100M);
		}

		public void BuildFieldPattern()
		{
			var builder = PatternBuilder.GetPatterns().Single(x => x.Name == "Field");
			var pattern = builder.Build(Dictionary(new[] {"Name", "n"}, new[] {"Pattern", "p"}));
			pattern.Should().BeOfType<FieldPattern>();

			var amountRangePattern = (FieldPattern) pattern;
			amountRangePattern.Name.Should().Be("n");
			amountRangePattern.Pattern.Should().Be("p");
		}

		public void BuildDayOfMonthPattern()
		{
			var builder = PatternBuilder.GetPatterns().Single(x => x.Name == "Day Of Month");
			var pattern = builder.Build(Dictionary(new[] {"Days of Month", "1, 15"}));
			pattern.Should().BeOfType<DayOfMonthPattern>();

			var amountRangePattern = (DayOfMonthPattern) pattern;
			amountRangePattern.DaysOfMonth.Should().Equal(1, 15);
		}

		public void BuildAmountPatternFromIPattern()
		{
			var builder = PatternBuilder.Parse(new AmountPattern {Amount = 10M, Direction = EntryType.Credit}).Single();
			builder.Name.Should().Be("Amount");
			builder.Properties.Keys.Should().Contain("Amount");
			builder.Properties.Keys.Should().Contain("Direction");
			builder.Properties["Amount"].Should().Be("10.00");
		}

		public void BuildFieldPatternFromIPattern()
		{
			var builder = PatternBuilder.Parse(new FieldPattern {Name = "n", Pattern = "p"}).Single();
			builder.Name.Should().Be("Field");
			builder.Properties.Keys.Should().Contain("Name");
			builder.Properties.Keys.Should().Contain("Pattern");
			builder.Properties["Name"].Should().Be("n");
			builder.Properties["Pattern"].Should().Be("p");
		}

		public void BuildAmountRangePatternFromIPattern()
		{
			var builder = PatternBuilder.Parse(new AmountRangePattern {Min = 10, Max = 100}).Single();
			builder.Name.Should().Be("Amount Range");
			builder.Properties.Keys.Should().Contain("Min");
			builder.Properties.Keys.Should().Contain("Max");
			builder.Properties["Min"].Should().Be("10.00");
			builder.Properties["Max"].Should().Be("100.00");
		}

		public void BuildDayOfMonthPatternFromIPattern()
		{
			var builder = PatternBuilder.Parse(new DayOfMonthPattern(1, 15)).Single();
			builder.Name.Should().Be("Day Of Month");
			builder.Properties.Keys.Should().Contain("Days of Month");
			builder.Properties["Days of Month"].Should().Be("1, 15");
		}

		private Dictionary<string, string> Dictionary(params string[][] values)
		{
			return values.ToDictionary(pair => pair[0], pair => pair[1]);
		}
	}
}