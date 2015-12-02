using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using HomeTrack.Core;

namespace HomeTrack.SqlStore.Tests.AccountIdentifiers
{
	public class DeleteAccountIdentifierTests
	{
		private readonly AccountIdentifierRepository _repository;
		private readonly IEnumerable<AccountIdentifier> _identifiers;

		public DeleteAccountIdentifierTests(AccountIdentifierRepository repository)
		{
			var identifier = new AccountIdentifier
			{
				Account = TestData.Bank,
				Pattern = new AmountPattern() { Amount = 100, Direction = EntryType.Credit }
			};

			_repository = repository;
			_repository.AddOrUpdate(identifier);

			_identifiers = _repository.GetAll();
		}

		public void Remove()
		{
			var target = _identifiers.Single(i => i.Account == TestData.Bank);
			_repository.Remove(target.Id);

			_repository.GetAll().Should().NotContain(i => i.Account == TestData.Bank);
		}
	}
}