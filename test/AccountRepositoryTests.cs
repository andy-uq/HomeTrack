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
			Repository.UseOnceTo(s => Assert.That(s.Query<HomeTrack.RavenStore.Account>(), Is.Not.Empty));
		}
	}
}