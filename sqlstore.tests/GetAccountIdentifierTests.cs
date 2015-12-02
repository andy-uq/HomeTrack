using FluentAssertions;
using HomeTrack.Core;

namespace HomeTrack.SqlStore.Tests
{
	public class GetAccountIdentifierTests
	{
		private readonly AccountIdentifierRepository _repository;

		public GetAccountIdentifierTests(AccountIdentifierRepository repository)
		{
			var identifier = new AccountIdentifier
			{
				Account = TestData.Bank,
				Pattern = new AmountPattern() { Amount = 100, Direction = EntryType.Credit }
			};

			_repository = repository;
			_repository.AddOrUpdate(identifier);
		}

		public void GetAll()
		{
			var identifiers = _repository.GetAll();
			identifiers.Should().Contain(i => i.Account == TestData.Bank);
		}

		public void GetById()
		{
			var identifier = _repository.GetById(1);
			identifier.Account.Should().Be(TestData.Bank);
			identifier.Pattern.Should().BeOfType<AmountPattern>();
		}
	}
}