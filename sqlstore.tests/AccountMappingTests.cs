using FluentAssertions;
using HomeTrack.Mapping;
using NUnit.Framework;

namespace HomeTrack.SqlStore.Tests
{
	public class AccountMappingTests
	{
		public void ToModel()
		{
			var account = new HomeTrack.Account("Bank", AccountType.Asset);
			var model = account.Map<Models.Account>();

			model.AccountTypeName.Should().Be("Asset");
		}

		public void FromModel()
		{
			var model = new Models.Account { AccountTypeName = "Asset" };
			var account = model.Map<HomeTrack.Account>();
			
			account.Type.Should().Be(AccountType.Asset);
		}
	}
}