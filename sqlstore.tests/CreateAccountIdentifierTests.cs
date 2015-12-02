using HomeTrack.Core;

namespace HomeTrack.SqlStore.Tests
{
	public class CreateAccountIdentifierTests
	{
		private readonly AccountIdentifierRepository _repository;

		public CreateAccountIdentifierTests(AccountIdentifierRepository repository)
		{
			_repository = repository;
		}

		public void CreateSimpleIdentifier()
		{
			var identifier = new AccountIdentifier
			{
				Account = TestData.Bank,
				Pattern = new AmountPattern() {  Amount = 100, Direction = EntryType.Credit }
			};

			_repository.AddOrUpdate(identifier);
		}

		public void CreateCompositeIdentifier()
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

			_repository.AddOrUpdate(identifier);
		}
	}
}