using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using HomeTrack.Collections;
using HomeTrack.Core;
using HomeTrack.SqlStore.Mappings;
using HomeTrack.SqlStore.Models;

namespace HomeTrack.SqlStore.Tests
{
	public class AccountIdentifierFromModelTests
	{
		private readonly ITypeConverter<Models.AccountIdentifier, AccountIdentifier> _fromModel;

		public AccountIdentifierFromModelTests(AccountIdentifierMapping builder)
		{
			_fromModel = builder;
		}

		public void ToAmountPattern()
		{
			var model = new Models.AccountIdentifier
			{
				Id = 1,
				AccountId = TestData.Bank.Id,
				Primary = new AccountIdentifierRow()
				{
					Name = "Amount",
					PropertiesJson = "{\"Amount\":\"100.00\",\"Direction\": \"Credit\"}"
				}
			};

			var accountIdentifier = _fromModel.Convert(ResolutionContext.New(model));
			accountIdentifier.Account.Should().Be(TestData.Bank);
			accountIdentifier.Id.Should().Be(1);
			accountIdentifier.Pattern.Should().BeOfType<AmountPattern>();

			var amount = (AmountPattern )accountIdentifier.Pattern;
			amount.Amount.Should().Be(100m);
			amount.Direction.Should().Be(EntryType.Credit);
		}

		public void ToCompositePattern()
		{
			var model = new Models.AccountIdentifier
			{
				Id = 1,
				AccountId = TestData.Bank.Id,
				Primary =
				{
					Name = nameof(CompositePattern),
					PropertiesJson = ""
				},
				Secondaries = new[]
				{
					new AccountIdentifierRow
					{
						Name = "Amount",
						PropertiesJson = "{\"Amount\":\"100.00\",\"Direction\": \"Credit\"}"
					},
					new AccountIdentifierRow
					{
						Name = "Amount Range",
						PropertiesJson = "{\"Min\":\"10.00\",\"Max\":\"100.00\"}"
					},
				}
			};

			var accountIdentifier = _fromModel.Convert(ResolutionContext.New(model));
			accountIdentifier.Account.Should().Be(TestData.Bank);
			accountIdentifier.Id.Should().Be(1);
			accountIdentifier.Pattern.Should().BeOfType<CompositePattern>();

			var amount = (AmountPattern)((CompositePattern )accountIdentifier.Pattern).First();
			amount.Amount.Should().Be(100m);
			amount.Direction.Should().Be(EntryType.Credit);
		}
	}
}