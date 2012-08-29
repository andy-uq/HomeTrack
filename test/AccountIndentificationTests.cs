using System;
using System.Collections.Generic;
using HomeTrack.Core;
using Moq;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class AccountIndentificationTests
	{
		private Account _groceries;
		private Account _wow;

		private Mock<IImportRow> _row1;
		private Mock<IImportRow> _row2;
		private Mock<IImportRow> _row3;

		[SetUp]
		public void SetUp()
		{
			_groceries = AccountFactory.Expense("Groceries");
			_wow = AccountFactory.Expense("WOW subscription");

			_row1 = new Mock<IImportRow>(MockBehavior.Strict);
			_row1.SetupGet(x => x.Properties).Returns(new[] { new KeyValuePair<string, string>("Other Party", "Countdown Auckland") });
			_row1.SetupGet(x => x.Date).Returns(DateTime.Parse("2000-1-1"));
			_row1.SetupGet(x => x.Amount).Returns(10M);

			_row2 = new Mock<IImportRow>(MockBehavior.Strict);
			_row2.SetupGet(x => x.Properties).Returns(new[] { new KeyValuePair<string, string>("Other Party", "Blizzard WOW Subscription") });
			_row2.SetupGet(x => x.Date).Returns(DateTime.Parse("2000-1-5"));
			_row2.SetupGet(x => x.Amount).Returns(15M);

			_row3 = new Mock<IImportRow>(MockBehavior.Strict);
			_row3.SetupGet(x => x.Properties).Returns(new[] { new KeyValuePair<string, string>("Other Party", "Payment Received - Thank you") });
			_row3.SetupGet(x => x.Date).Returns(DateTime.Parse("2000-1-21"));
			_row3.SetupGet(x => x.Amount).Returns(-1768M);
		}

		[Test]
		public void CreateAccountIdentifier()
		{
			var a1 = new AccountIdentifier { Pattern = new AmountPattern { Amount = 10M }, Account = _groceries };
			var a2 = new AccountIdentifier { Pattern = new FieldPattern { Name = "Other Party", Pattern = "Blizzard" }, Account = _wow };

			Assert.That(_row1.Object.IdentifyAccount(new[] { a1, a2 }), Is.EqualTo(_groceries));
			Assert.That(_row2.Object.IdentifyAccount(new[] { a1, a2 }), Is.EqualTo(_wow));
			Assert.That(_row3.Object.IdentifyAccount(new[] { a1, a2 }), Is.Null);
		}

	}

	
}