using System.Threading.Tasks;
using HomeTrack.Core;

namespace HomeTrack.SqlStore.Tests.AccountIdentifiers
{
	public class CreateAccountIdentifierAsyncTests
	{
		private readonly AccountIdentifierRepository _repository;

		public CreateAccountIdentifierAsyncTests(AccountIdentifierRepository repository)
		{
			_repository = repository;
		}

		public async Task CreateSimpleIdentifierAsync()
		{
			var identifier = new AccountIdentifier
			{
				Account = TestData.Bank,
				Pattern = new AmountPattern() {  Amount = 100, Direction = EntryType.Credit }
			};

			await _repository.AddOrUpdateAsync(identifier);
		}

		public async Task CreateCompositeIdentifierAsync()
		{
			var identifier = new AccountIdentifier
			{
				Account = TestData.Bank,
				Pattern = new CompositePattern
				{
					new AmountPattern() {  Amount = 100, Direction = EntryType.Credit },
					new DayOfMonthPattern() {  DaysOfMonth = new int[] { 1, 14 } }
				}
			};

			await _repository.AddOrUpdateAsync(identifier);
		}
	}
}