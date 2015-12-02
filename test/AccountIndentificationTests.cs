using System;
using System.Collections.Generic;
using FluentAssertions;
using HomeTrack.Core;
using Moq;

namespace HomeTrack.Tests
{
	public class AccountIndentificationTests
	{
		private readonly Account _groceries;
		private readonly Mock<IImportRow> _row1;
		private readonly Mock<IImportRow> _row2;
		private readonly Mock<IImportRow> _row3;
		private readonly Account _wow;

		public AccountIndentificationTests()
		{
			_groceries = AccountFactory.Expense("Groceries");
			_wow = AccountFactory.Expense("WOW subscription");

			_row1 = new Mock<IImportRow>(MockBehavior.Strict);
			_row1.SetupGet(x => x.Properties)
				.Returns(new[] {new KeyValuePair<string, string>("Other Party", "Countdown Auckland")});
			_row1.SetupGet(x => x.Date).Returns(DateTime.Parse("2000-1-1"));
			_row1.SetupGet(x => x.Amount).Returns(10M);

			_row2 = new Mock<IImportRow>(MockBehavior.Strict);
			_row2.SetupGet(x => x.Properties)
				.Returns(new[] {new KeyValuePair<string, string>("Other Party", "Blizzard WOW Subscription")});
			_row2.SetupGet(x => x.Date).Returns(DateTime.Parse("2000-1-5"));
			_row2.SetupGet(x => x.Amount).Returns(15M);

			_row3 = new Mock<IImportRow>(MockBehavior.Strict);
			_row3.SetupGet(x => x.Properties)
				.Returns(new[] {new KeyValuePair<string, string>("Other Party", "Payment Received - Thank you")});
			_row3.SetupGet(x => x.Date).Returns(DateTime.Parse("2000-1-21"));
			_row3.SetupGet(x => x.Amount).Returns(-1768M);
		}

		public void AccountIdentifierName()
		{
			var a1 = new AccountIdentifier {Pattern = new AmountPattern {Amount = 10M}, Account = _groceries};
			a1.ToString().Should().Be("Groceries <- Amount=10.00");
		}

		public void CreateAccountIdentifier()
		{
			var a1 = new AccountIdentifier {Pattern = new AmountPattern {Amount = 10M}, Account = _groceries};
			var a2 = new AccountIdentifier
			{
				Pattern = new FieldPattern {Name = "Other Party", Pattern = "Blizzard"},
				Account = _wow
			};

			_row1.Object.IdentifyAccount(new[] {a1, a2}).Should().Be(_groceries);
			_row2.Object.IdentifyAccount(new[] {a1, a2}).Should().Be(_wow);
			_row3.Object.IdentifyAccount(new[] {a1, a2}).Should().BeNull();
		}
	}
}