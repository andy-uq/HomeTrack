using System.Linq;
using HomeTrack.RavenStore;
using NUnit.Framework;
using Raven.Client.Embedded;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class TransactionRepositoryTests : RavenRepositoryTests
	{
		private Account _bank;
		private Account _mortgage;
		private Account _cashOnHand;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			_bank = AccountFactory.Asset("Bank", initialBalance: 100);
			_cashOnHand = AccountFactory.Asset("Cash on hand");
			_mortgage = AccountFactory.Liability("Mortgage", initialBalance:50);

			GeneralLedger.Add(_bank);
			GeneralLedger.Add(_cashOnHand);
			GeneralLedger.Add(_mortgage);
		}

		[Test]
		public void AddTransaction()
		{
			var t1 = new Transaction(_mortgage, _bank, 10M);
			GeneralLedger.Post(t1);
			
			Repository.UseOnceTo(s => Assert.That(s.Query<HomeTrack.RavenStore.Transaction>(), Is.Not.Empty));

			Assert.That(GeneralLedger["bank"].Balance, Is.EqualTo(90M));
			Assert.That(GeneralLedger["mortgage"].Balance, Is.EqualTo(40M));
		}

		[Test]
		public void SearchAccountTransactions()
		{
			var t1 = new Transaction(_mortgage, _bank, 10M) { Description = "Pay back mortgage" };
			GeneralLedger.Post(t1);

			var t2 = new Transaction(_cashOnHand, _bank, 10M) { Description = "Withdraw money" };
			GeneralLedger.Post(t2);

			var q = GeneralLedger.GetTransactions(_bank.Id);
			Assert.That(q, Is.EquivalentTo(new[] {t1, t2}).Using(new TransactionComparer()));

			q = GeneralLedger.GetTransactions(_mortgage.Id);
			Assert.That(q, Is.EquivalentTo(new[] {t1}).Using(new TransactionComparer()));

			Assert.That(GeneralLedger["bank"].Balance, Is.EqualTo(80M));
			Assert.That(GeneralLedger["mortgage"].Balance, Is.EqualTo(40M));
			Assert.That(GeneralLedger["cashOnHand"].Balance, Is.EqualTo(10M));
		}
	}
}