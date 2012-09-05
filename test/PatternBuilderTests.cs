using System.Collections.Generic;
using System.Linq;
using HomeTrack.Core;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class PatternBuilderTests
	{
		[Test]
		public void BuildAmountPattern()
		{
			var builder = PatternBuilder.GetPatterns().Single(x => x.Name == "Amount");
			var pattern = builder.Build(Dictionary(new[] {"Amount", "10"}));
			Assert.That(pattern, Is.InstanceOf<AmountPattern>());

			var amount = (AmountPattern) pattern;
			Assert.That(amount.Amount, Is.EqualTo(10M));
		}

		[Test]
		public void BuildAmountRangePattern()
		{
			var builder = PatternBuilder.GetPatterns().Single(x => x.Name == "Amount Range");
			var pattern = builder.Build(Dictionary(new[] {"Min", "10"}, new[] { "Max", "100" }));
			Assert.That(pattern, Is.InstanceOf<AmountRangePattern>());

			var amountRangePattern = (AmountRangePattern) pattern;
			Assert.That(amountRangePattern.Min, Is.EqualTo(10M));
			Assert.That(amountRangePattern.Max, Is.EqualTo(100M));
		}

		[Test]
		public void BuildFieldPattern()
		{
			var builder = PatternBuilder.GetPatterns().Single(x => x.Name == "Field");
			var pattern = builder.Build(Dictionary(new[] {"Name", "n"}, new[] { "Pattern", "p" }));
			Assert.That(pattern, Is.InstanceOf<FieldPattern>());

			var amountRangePattern = (FieldPattern)pattern;
			Assert.That(amountRangePattern.Name, Is.EqualTo("n"));
			Assert.That(amountRangePattern.Pattern, Is.EqualTo("p"));
		}

		[Test]
		public void BuildDayOfMonthPattern()
		{
			var builder = PatternBuilder.GetPatterns().Single(x => x.Name == "Day Of Month");
			var pattern = builder.Build(Dictionary(new[] {"Days of Month", "1, 15"}));
			Assert.That(pattern, Is.InstanceOf<DayOfMonthPattern>());

			var amountRangePattern = (DayOfMonthPattern)pattern;
			Assert.That(amountRangePattern.DaysOfMonth, Is.EqualTo(new[] { 1, 15 }));
		}

		[Test]
		public void BuildAmountPatternFromIPattern()
		{
			var builder = PatternBuilder.Parse(new AmountPattern { Amount = 10M, Direction = EntryType.Credit }).Single();
			Assert.That(builder.Name, Is.EqualTo("Amount"));
			Assert.That(builder.Properties.Keys, Has.Member("Amount"));
			Assert.That(builder.Properties.Keys, Has.Member("Direction"));
			Assert.That(builder.Properties["Amount"], Is.EqualTo("10.00"));
		}

		[Test]
		public void BuildFieldPatternFromIPattern()
		{
			var builder = PatternBuilder.Parse(new FieldPattern { Name = "n", Pattern = "p" }).Single();
			Assert.That(builder.Name, Is.EqualTo("Field"));
			Assert.That(builder.Properties.Keys, Has.Member("Name"));
			Assert.That(builder.Properties.Keys, Has.Member("Pattern"));
			Assert.That(builder.Properties["Name"], Is.EqualTo("n"));
			Assert.That(builder.Properties["Pattern"], Is.EqualTo("p"));
		}

		[Test]
		public void BuildAmountRangePatternFromIPattern()
		{
			var builder = PatternBuilder.Parse(new AmountRangePattern { Min = 10, Max = 100 }).Single();
			Assert.That(builder.Name, Is.EqualTo("Amount Range"));
			Assert.That(builder.Properties.Keys, Has.Member("Min"));
			Assert.That(builder.Properties.Keys, Has.Member("Max"));
			Assert.That(builder.Properties["Min"], Is.EqualTo("10.00"));
			Assert.That(builder.Properties["Max"], Is.EqualTo("100.00"));
		}

		[Test]
		public void BuildDayOfMonthPatternFromIPattern()
		{
			var builder = PatternBuilder.Parse(new DayOfMonthPattern(1, 15)).Single();
			Assert.That(builder.Name, Is.EqualTo("Day Of Month"));
			Assert.That(builder.Properties.Keys, Has.Member("Days of Month"));
			Assert.That(builder.Properties["Days of Month"], Is.EqualTo("1, 15"));
		}

		private Dictionary<string, string> Dictionary(params string[][] values)
		{
			return values.ToDictionary(pair => pair[0], pair => pair[1]);
		}
	}
}