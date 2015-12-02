using System.Threading.Tasks;
using FluentAssertions;
using HomeTrack.Core;

namespace HomeTrack.SqlStore.Tests.AccountIdentifiers
{
	public class GetAccountIdentifierAsyncTests
	{
		private readonly AccountIdentifierRepository _repository;

		public GetAccountIdentifierAsyncTests(AccountIdentifierRepository repository)
		{
			_repository = repository;
		}

		private Task CreateIdentifierAsync()
		{
			var identifier = new AccountIdentifier
			{
				Account = TestData.Bank,
				Pattern = new AmountPattern() {Amount = 100, Direction = EntryType.Credit}
			};

			return _repository.AddOrUpdateAsync(identifier);
		}

		public async Task GetAllAsync()
		{
			await CreateIdentifierAsync();

			var identifiers = await _repository.GetAllAsync();
			identifiers.Should().Contain(i => i.Account == TestData.Bank);
		}

		public async Task GetByIdAsync()
		{
			await CreateIdentifierAsync();

			var identifier = await _repository.GetByIdAsync(1);
			identifier.Account.Should().Be(TestData.Bank);
			identifier.Pattern.Should().BeOfType<AmountPattern>();
		}
	}
}