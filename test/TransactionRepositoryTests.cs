using System.Collections.Generic;
using System.Linq;
using HomeTrack.RavenStore;
using NUnit.Framework;
using Raven.Client.Embedded;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class TransactionRepositoryTests
	{
		private Account _bank;
		private Account _mortgage;
		private Account _cashOnHand;

		[SetUp]
		public void SetUp()
		{
			_bank = AccountFactory.Debit("Bank");
			_cashOnHand = AccountFactory.Debit("Bank");
			_mortgage = AccountFactory.Credit("Mortgage");
		}

		[Test]
		public void AddTransaction()
		{
			using ( var repository = RavenStore.CreateRepository() )
			{
				using (var u = repository.CreateUnitOfWork())
				{
					u.Add(new Transaction());
					u.SaveChanges();
				}

				repository.UseOnceTo(s => Assert.That(s.Query<Transaction>(), Is.Not.Empty));
			}
		}

		[Test]
		public void SearchAccountTransactions()
		{
			var t1 = new Transaction(_bank, _mortgage, 10M);
			var t2 = new Transaction(_bank, _cashOnHand, 10M);
			
			using ( var repository = RavenStore.CreateRepository() )
			{
				using (var u = repository.CreateUnitOfWork())
				{
					u.Add(t1);
					u.Add(t2);
					u.SaveChanges();
				}

				using (var u = repository.CreateUnitOfWork())
				{
					var q = u.GetTransactions(_bank);
					Assert.That(q, Is.EquivalentTo(new[] {t1, t2}).Using(new TransactionComparer()));

					q = u.GetTransactions(_mortgage);
					Assert.That(q, Is.EquivalentTo(new[] {t1}).Using(new TransactionComparer()));
				}
			}
		}

		public class TransactionComparer : IEqualityComparer<Transaction>
		{
			public bool Equals(Transaction x, Transaction y)
			{
				return x.Id == y.Id;
			}

			public int GetHashCode(Transaction obj)
			{
				return obj.Id.GetHashCode();
			}
		}
	}
}