using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HomeTrack.Core;

namespace HomeTrack.SqlStore.Tests.AccountIdentifiers
{
	public class DeleteAccountIdentifierAsyncTests
	{
		private readonly AccountIdentifierRepository _repository;

		public DeleteAccountIdentifierAsyncTests(AccountIdentifierRepository repository)
		{
			_repository = repository;
		}

		private async Task<IEnumerable<AccountIdentifier>> GetIdentifiersAsync()
		{
			var identifier = new AccountIdentifier
			{
				Account = TestData.Bank,
				Pattern = new AmountPattern() {Amount = 100, Direction = EntryType.Credit}
			};

			await _repository.AddOrUpdateAsync(identifier);
			return await _repository.GetAllAsync();
		}

		public async Task RemoveAsync()
		{
			var identifiers = await GetIdentifiersAsync();

			var target = identifiers.Single(i => i.Account == TestData.Bank);
			await _repository.RemoveAsync(target.Id);

			(await _repository.GetAllAsync()).Should().NotContain(i => i.Account == TestData.Bank);
		}
	}
}