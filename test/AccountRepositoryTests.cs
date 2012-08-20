using NUnit.Framework;
using Raven.Client.Embedded;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class AccountRepositoryTests
	{
		private Account _bank;
		private EmbeddableDocumentStore _store;

		[SetUp]
		public void SetUp()
		{
			_store = new EmbeddableDocumentStore { RunInMemory = true };
			_store.Initialize();

			_bank = AccountFactory.Debit("Bank");
		}

		[Test]
		public void AddAccount()
		{
			using ( var repository = RavenStore.CreateRepository() )
			{
				using (var u = repository.CreateUnitOfWork())
				{
					u.Add(_bank);
					u.SaveChanges();
				}

				repository.UseOnceTo(s => Assert.That(s.Query<Account>(), Is.Not.Empty));
			}
		}
	}
}