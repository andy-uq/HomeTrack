using System;
using System.Collections.Generic;
using FluentAssertions;
using HomeTrack.Core;
using Moq;

namespace HomeTrack.Tests
{
	public class ImportPatternTests
	{
		private readonly Mock<IImportRow> _row1;
		private readonly Mock<IImportRow> _row2;
		private readonly Mock<IImportRow> _row3;

		public ImportPatternTests()
		{
			_row1 = new Mock<IImportRow>(MockBehavior.Strict);
			_row1.SetupGet(x => x.Properties)
				.Returns(new[] {new KeyValuePair<string, string>("Other Party", "Countdown Auckland")});
			_row1.SetupGet(x => x.Date).Returns(DateTime.Parse("2000-1-1"));
			_row1.SetupGet(x => x.Amount).Returns(-150M);

			_row2 = new Mock<IImportRow>(MockBehavior.Strict);
			_row2.SetupGet(x => x.Properties)
				.Returns(new[] {new KeyValuePair<string, string>("Other Party", "Blizzard WOW Subscription")});
			_row2.SetupGet(x => x.Date).Returns(DateTime.Parse("2000-1-5"));
			_row2.SetupGet(x => x.Amount).Returns(-12.5M);

			_row3 = new Mock<IImportRow>(MockBehavior.Strict);
			_row3.SetupGet(x => x.Properties)
				.Returns(new[] {new KeyValuePair<string, string>("Other Party", "Payment Received - Thank you")});
			_row3.SetupGet(x => x.Date).Returns(DateTime.Parse("2000-1-21"));
			_row3.SetupGet(x => x.Amount).Returns(1000M);
		}

		public void CreateTransactionPattern()
		{
			new CompositePattern();
		}

		public void FieldPatternMatch()
		{
			var pattern = new FieldPattern {Name = "Other Party", Pattern = "COUNTDOWN"};
			pattern.IsMatch(_row1.Object).Should().BeTrue();
			pattern.ToString().Should().Be("Other Party matches COUNTDOWN");
		}

		public void FieldPatternMatchWhereFieldDoesNotExist()
		{
			var pattern = new FieldPattern {Name = "This field does not exist", Pattern = "COUNTDOWN"};
			pattern.IsMatch(_row1.Object).Should().BeFalse();
		}

		public void DayOfMonthPatternMatch()
		{
			var pattern = new DayOfMonthPattern(1);
			pattern.IsMatch(_row1.Object).Should().BeTrue();
			pattern.ToString().Should().Be("Day = 1");
		}

		public void AmountPatternMatch()
		{
			var pattern = new AmountPattern {Amount = 150M, Direction = EntryType.NotSpecified};
			pattern.ToString().Should().Be("Amount=150.00");
			pattern.IsMatch(_row1.Object).Should().BeTrue();
		}

		public void CreditAmountPatternMatch()
		{
			var pattern = new AmountPattern {Amount = 150M, Direction = EntryType.Credit};
			pattern.ToString().Should().Be("Amount=150.00Cr");
			pattern.IsMatch(_row1.Object).Should().BeTrue();
		}

		public void DebitAmountPatternMatch()
		{
			var pattern = new AmountPattern {Amount = 1000M, Direction = EntryType.Debit};
			pattern.ToString().Should().Be("Amount=1,000.00Dr");
			pattern.IsMatch(_row3.Object).Should().BeTrue();
		}

		public void AmountPatternRangeMatch()
		{
			var pattern = new AmountRangePattern {Min = 100M, Max = 200M};
			pattern.IsMatch(_row1.Object).Should().BeTrue();
			pattern.ToString().Should().Be("Amount >= 100.00 and <= 200.00");
		}

		public void MultipleDaysOfMonthPatternMatch()
		{
			var pattern = new DayOfMonthPattern(1, 5);
			pattern.IsMatch(_row1.Object).Should().BeTrue();
			pattern.IsMatch(_row2.Object).Should().BeTrue();
			pattern.IsMatch(_row3.Object).Should().BeFalse();
			pattern.ToString().Should().Be("Day in [1,5]");
		}

		public void TransactionFieldPatternMatch()
		{
			var pattern = new CompositePattern
			{
				new FieldPattern {Name = "Other Party", Pattern = "COUNTDOWN"},
				new DayOfMonthPattern {DaysOfMonth = new[] {1, 7}}
			};

			pattern.IsMatch(_row1.Object).Should().BeTrue();
			pattern.IsMatch(_row2.Object).Should().BeFalse();
			pattern.IsMatch(_row3.Object).Should().BeFalse();
			pattern.ToString().Should().Be("(Other Party matches COUNTDOWN) AND (Day in [1,7])");
		}
	}
}