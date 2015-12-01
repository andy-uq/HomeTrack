using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using HomeTrack.Collections;
using HomeTrack.Core;
using HomeTrack.SqlStore.Mappings;

namespace HomeTrack.SqlStore.Tests
{
	public class AccountIdentifierFromModelTests
	{
		private readonly ITypeConverter<IEnumerable<Models.AccountIdentifier>, AccountIdentifier> _fromModel;

		public AccountIdentifierFromModelTests(AccountIdentifierMapping builder)
		{
			_fromModel = builder;
		}

		public void ToAmountPattern()
		{
			var model = new Models.AccountIdentifier
			{
				Id = 1,
				ParentId = null,
				AccountId = TestData.Bank.Id,
				Name = nameof(AmountPattern),
				PropertiesJson = "{\"amount\":100.0,\"direction\": \"Credit\"}"
			};

			var accountIdentifier = _fromModel.Convert(ResolutionContext.New(model.AsEnumerable()));
			accountIdentifier.Account.Should().Be(TestData.Bank);
			accountIdentifier.Id.Should().Be(1);
		}
	}
}