using HomeTrack.RavenStore;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class AccountRepositoryTests : RavenRepositoryTests
	{
		[Test]
		public void AddAccount()
		{
			var bank = AccountFactory.Debit("Bank");
			GeneralLedger.Add(bank);
			Assert.That(Repository.UseOnceTo(s => s.Query<HomeTrack.RavenStore.Account>()), Is.Not.Empty);
		}
	}
}