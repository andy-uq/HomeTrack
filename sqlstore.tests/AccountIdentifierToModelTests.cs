using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using FluentAssertions;
using HomeTrack.Core;
using HomeTrack.SqlStore.Models.Mapping;

namespace HomeTrack.SqlStore.Tests
{
	public class AccountIdentifierToModelTests
	{
		private readonly ITypeConverter<AccountIdentifier, Models.AccountIdentifier> _toModel;

		public AccountIdentifierToModelTests(AccountIdentifierMapping builder)
		{
			_toModel = builder;
		}

		public void FromAmountPattern()
		{
			var pattern = new AmountPattern { Amount = 100M, Direction = EntryType.Credit };
			var result = _toModel.Convert(ToAccountIdentifier(pattern));
			
			ValidateSimplePattern(result, "Amount", "{\"Amount\":\"100.00\",\"Direction\":\"Credit\"}");
		}

		public void FromAmountRangePattern()
		{
			IPattern pattern = new AmountRangePattern { Max = 100, Min = 10 };
			var result = _toModel.Convert(ToAccountIdentifier(pattern));

			ValidateSimplePattern(result, "Amount Range", "{\"Min\":\"10.00\",\"Max\":\"100.00\"}");
		}

		public void FromDayOfMonthPattern()
		{
			IPattern pattern = new DayOfMonthPattern(1, 2, 3);
			var result = _toModel.Convert(ToAccountIdentifier(pattern));

			ValidateSimplePattern(result, "Day Of Month", "{\"Days of Month\":\"1, 2, 3\"}");
		}

		public void FromFieldPattern()
		{
			IPattern pattern = new FieldPattern { Name = "Description", Pattern = @"CNT(""[\d{4,9}]"")" };
			var result = _toModel.Convert(ToAccountIdentifier(pattern));

			ValidateSimplePattern(result, "Field", @"{""Name"":""Description"",""Pattern"":""CNT(\""[\\d{4,9}]\"")""}");
		}

		public void FromCompositePattern()
		{
			IPattern pattern = new CompositePattern { new AmountPattern { Amount = 100M, Direction = EntryType.Credit }, new AmountRangePattern { Max = 100, Min = 10 } };
			var result = _toModel.Convert(ToAccountIdentifier(pattern));

			ValidateCompositePattern(result);
		}

		private static void ValidateCompositePattern(Models.AccountIdentifier result)
		{
			result.Patterns.Should().HaveCount(2);

			result.AccountId.Should().Be(TestData.Bank.Id);

			result.Patterns.Select(x => x.Name).Should().Equal("Amount", "Amount Range");
		}

		private static void ValidateSimplePattern(Models.AccountIdentifier result, string expectedName, string expectedProperties)
		{
			result.Patterns.Length.Should().Be(1);

			result.AccountId.Should().Be(TestData.Bank.Id);
			result.Patterns[0].Name.Should().Be(expectedName);
			result.Patterns[0].PropertiesJson.Should().Be(expectedProperties);
		}

		private ResolutionContext ToAccountIdentifier(IPattern pattern)
		{
			return ResolutionContext.New(new HomeTrack.AccountIdentifier { Account = TestData.Bank, Pattern = pattern });
		}
	}
}