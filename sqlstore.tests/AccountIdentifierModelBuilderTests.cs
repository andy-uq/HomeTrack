using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using HomeTrack.Core;
using HomeTrack.SqlStore.Models;

namespace HomeTrack.SqlStore.Tests
{
	public class AccountIdentifierModelBuilderTests
	{
		private readonly AccountIdentifierModelBuilder _builder;

		public AccountIdentifierModelBuilderTests(AccountIdentifierModelBuilder builder)
		{
			_builder = builder;
		}

		public void FromAmountPattern()
		{
			var pattern = new AmountPattern { Amount = 100M, Direction = EntryType.Credit };
			var result = _builder.Convert(ToAccountIdentifier(pattern)).ToList();
			
			ValidateSimplePattern(result, nameof(AmountPattern), "{\"amount\":100.0,\"direction\":\"Credit\"}");
		}

		public void FromAmountRangePattern()
		{
			IPattern pattern = new AmountRangePattern { Max = 100, Min = 10 };
			var result = _builder.Convert(ToAccountIdentifier(pattern)).ToList();

			ValidateSimplePattern(result, nameof(AmountRangePattern), "{\"min\":10.0,\"max\":100.0}");
		}

		public void FromDayOfMonthPattern()
		{
			IPattern pattern = new DayOfMonthPattern(1, 2, 3);
			var result = _builder.Convert(ToAccountIdentifier(pattern)).ToList();

			ValidateSimplePattern(result, nameof(DayOfMonthPattern), "{\"daysOfMonth\":[1,2,3]}");
		}

		public void FromFieldPattern()
		{
			IPattern pattern = new FieldPattern { Name = "Description", Pattern = @"CNT(""[\d{4,9}]"")" };
			var result = _builder.Convert(ToAccountIdentifier(pattern)).ToList();

			ValidateSimplePattern(result, nameof(FieldPattern), @"{""name"":""Description"",""pattern"":""CNT(\""[\\d{4,9}]\"")""}");
		}

		public void FromCompositePattern()
		{
			IPattern pattern = new CompositePattern { new AmountPattern { Amount = 100M, Direction = EntryType.Credit }, new AmountRangePattern { Max = 100, Min = 10 } };
			var result = _builder.Convert(ToAccountIdentifier(pattern)).ToList();

			ValidateCompositePattern(result);
		}

		private static void ValidateCompositePattern(IReadOnlyCollection<Models.AccountIdentifier> result)
		{
			result.Should().HaveCount(3);
			var parent = result.First();

			parent.AccountId.Should().Be(TestData.Bank.Id);
			parent.Name.Should().Be(nameof(CompositePattern));
			parent.ParentId.Should().NotHaveValue();
			parent.Id.Should().Be(1);
			parent.PropertiesJson.Should().Be(@"");

			var left = result.Single(x => x.Name == nameof(AmountPattern));
			left.Id.Should().Be(2);
			left.ParentId.Should().Be(1);
			left.AccountId.Should().Be(TestData.Bank.Id);

			var right = result.Single(x => x.Name == nameof(AmountRangePattern));
			right.Id.Should().Be(3);
			right.ParentId.Should().Be(1);
			right.AccountId.Should().Be(TestData.Bank.Id);
		}

		private static void ValidateSimplePattern(IReadOnlyCollection<Models.AccountIdentifier> result, string expectedName, string expectedProperties)
		{
			result.Should().HaveCount(1);

			var identifier = result.Single();
			identifier.AccountId.Should().Be(TestData.Bank.Id);
			identifier.Name.Should().Be(expectedName);
			identifier.ParentId.Should().NotHaveValue();
			identifier.Id.Should().Be(1);
			identifier.PropertiesJson.Should().Be(expectedProperties);
		}

		private ResolutionContext ToAccountIdentifier(IPattern pattern)
		{
			return ResolutionContext.New(new HomeTrack.AccountIdentifier { Account = TestData.Bank, Pattern = pattern });
		}
	}
}