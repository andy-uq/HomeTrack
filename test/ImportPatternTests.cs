using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HomeTrack.Core;
using Moq;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class ImportPatternTests
	{
		private Mock<IImportRow> _row1;
		private Mock<IImportRow> _row2;
		private Mock<IImportRow> _row3;

		[SetUp]
		public void SetUp()
		{
			_row1 = new Mock<IImportRow>(MockBehavior.Strict);
			_row1.SetupGet(x => x.Properties).Returns(new[] { new KeyValuePair<string, string>("Other Party", "Countdown Auckland") });
			_row1.SetupGet(x => x.Date).Returns(DateTime.Parse("2000-1-1"));
			_row1.SetupGet(x => x.Amount).Returns(10M);

			_row2 = new Mock<IImportRow>(MockBehavior.Strict);
			_row2.SetupGet(x => x.Properties).Returns(new[] { new KeyValuePair<string, string>("Other Party", "Blizzard WOW Subscription") });
			_row2.SetupGet(x => x.Date).Returns(DateTime.Parse("2000-1-5"));

			_row3 = new Mock<IImportRow>(MockBehavior.Strict);
			_row3.SetupGet(x => x.Properties).Returns(new[] { new KeyValuePair<string, string>("Other Party", "Payment Received - Thank you") });
			_row3.SetupGet(x => x.Date).Returns(DateTime.Parse("2000-1-21"));
		}
		
		[Test]
		public void CreateTransactionPattern()
		{
			new CompositePattern();
		}

		[Test]
		public void FieldPatternMatch()
		{
			var pattern = new FieldPattern() {Name = "Other Party", Pattern = "COUNTDOWN"};
			Assert.That(pattern.IsMatch(_row1.Object), Is.True);
		}

		[Test]
		public void DayOfMonthPatternMatch()
		{
			var pattern = new DayOfMonthPattern(1);
			Assert.That(pattern.IsMatch(_row1.Object), Is.True);
		}

		[Test]
		public void AmountPatternMatch()
		{
			var pattern = new AmountPattern() { Amount = 10M };
			Assert.That(pattern.IsMatch(_row1.Object), Is.True);
		}

		[Test]
		public void AmountPatternRangeMatch()
		{
			var pattern = new AmountRangePattern() { Min = 10M, Max = 10M };
			Assert.That(pattern.IsMatch(_row1.Object), Is.True);
		}

		[Test]
		public void MultipleDaysOfMonthPatternMatch()
		{
			var pattern = new DayOfMonthPattern(1, 5);
			Assert.That(pattern.IsMatch(_row1.Object), Is.True);
			Assert.That(pattern.IsMatch(_row2.Object), Is.True);
			Assert.That(pattern.IsMatch(_row3.Object), Is.False);
		}

		[Test]
		public void TransactionFieldPatternMatch()
		{
			var pattern = new CompositePattern
			{
				new FieldPattern() {Name = "Other Party", Pattern = "COUNTDOWN" },
				new DayOfMonthPattern() {DaysOfMonth = new[] { 1 } }
			};

			Assert.That(pattern.IsMatch(_row1.Object), Is.True);
			Assert.That(pattern.IsMatch(_row2.Object), Is.False);
			Assert.That(pattern.IsMatch(_row3.Object), Is.False);
		}
	}

}