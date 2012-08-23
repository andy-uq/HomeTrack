using HomeTrack.RavenStore;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	public class AccountRepositoryTests : RavenRepositoryTests
	{
		[Test]
		public void AddAccount()
		{
			var bank = AccountFactory.Asset("Bank");
			GeneralLedger.Add(bank);
			Assert.That(Repository.UseOnceTo(s => s.Query<HomeTrack.RavenStore.Account>()), Is.Not.Empty);
		}		
		
		[Test]
		public void EditAccountDescription()
		{
			var bank = AccountFactory.Asset("Bank", initialBalance: 10M);
			GeneralLedger.Add(bank);

			bank.Description = "Updated description";
			GeneralLedger.Add(bank);

			var ravenId = string.Concat("accounts/", bank.Id);
			Assert.That(Repository.UseOnceTo(s => s.Load<HomeTrack.RavenStore.Account>(ravenId)), Is.Not.Null);
			Assert.That(Repository.UseOnceTo(s => s.Load<HomeTrack.RavenStore.Account>(ravenId)), Has.Property("Description").EqualTo("Updated description"));
			Assert.That(Repository.UseOnceTo(s => s.Load<HomeTrack.RavenStore.Account>(ravenId)), Has.Property("Balance").EqualTo(10M));
		}
	}
}