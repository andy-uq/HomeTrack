using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HomeTrack.Core;
using HomeTrack.RavenStore;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	class AccountIdentifierRepositoryTests : RavenRepositoryTests
	{
		protected IAccountIdentifierRepository AccountIdentifierRepository { get; set; }

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			AccountIdentifierRepository = new AccountIdentifierRepository(Repository, MappingEngine);
		}

		[Test]
		public void Add()
		{
			var i = new AccountIdentifier
			{
				Account = AccountFactory.Expense("Groceries")
			};

			AccountIdentifierRepository.Add(i);

			Assert.That(Repository.UseOnceTo(s => s.Query<HomeTrack.RavenStore.Documents.AccountIdentifier>()), Is.Not.Empty);
			Assert.That(i.Id, Is.Not.Null.Or.Empty);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void RemoveThrowsExceptionWhenIdIsNull()
		{
			AccountIdentifierRepository.Remove(null);
		}

		[Test]
		public void Remove()
		{
			var i = new AccountIdentifier
			{
				Account = AccountFactory.Expense("Groceries")
			};

			AccountIdentifierRepository.Add(i);
			AccountIdentifierRepository.Remove(i.Id);

			Assert.That(Repository.UseOnceTo(s => s.Query<HomeTrack.RavenStore.Documents.AccountIdentifier>()), Is.Empty);
		}

		[Test]
		public void GetAll()
		{
			var i1 = new AccountIdentifier {Account = AccountFactory.Expense("Groceries"), Pattern = new AmountPattern {Amount = 10M}};
			var i2 = new AccountIdentifier {Account = AccountFactory.Expense("Fuel"), Pattern = new DayOfMonthPattern(1, 15)};
			var i3 = new AccountIdentifier
			{
				Account = AccountFactory.Expense("Electricity"),
				Pattern = new CompositePattern
				{
					new DayOfMonthPattern(1),
					new AmountRangePattern {Min = 10M, Max = 100M}
				}
			};

			AccountIdentifierRepository.Add(i1);
			AccountIdentifierRepository.Add(i2);
			AccountIdentifierRepository.Add(i3);

			var collection = AccountIdentifierRepository.GetAll().ToArray();

			Assert.That(collection, Is.Not.Empty);
			Assert.That(collection, Is.EquivalentTo(new[] {i1, i2, i3}).Using(new AccountIdentifierComparer()));
		}
	}

	internal class AccountIdentifierComparer : IEqualityComparer<AccountIdentifier>
	{
		public bool Equals(AccountIdentifier x, AccountIdentifier y)
		{
			var patternComparer = new PatternComparer();

			return x.Account.Id == y.Account.Id
					&& patternComparer.Equals(x.Pattern, y.Pattern);
		}
			
		public int GetHashCode(AccountIdentifier obj)
		{
			throw new System.NotImplementedException();
		}
	}

	internal class PatternComparer : IEqualityComparer<IPattern>
	{
		public bool Equals(IPattern x, IPattern y)
		{
			if ( x == null && y == null )
				return true;

			if ( x == null || y == null )
				return false;

			return x.GetType() == y.GetType();
		}

		public int GetHashCode(IPattern obj)
		{
			throw new System.NotImplementedException();
		}
	}
}