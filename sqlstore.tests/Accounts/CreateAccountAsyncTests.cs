using System.Linq;
using System.Threading.Tasks;
using FixieShim.Fixie;
using FluentAssertions;

namespace HomeTrack.SqlStore.Tests.Accounts
{
	public class CreateAccountAsyncTests : IAsyncTest
	{
		private readonly GeneralLedgerRepository _generalLedger;
		private string _id;

		public CreateAccountAsyncTests(GeneralLedgerRepository generalLedger)
		{
			_generalLedger = generalLedger;
		}

		public async Task InitialiseAsync()
		{
			_id = await _generalLedger.AddAsync(new Account("New Account", AccountType.Asset));
		}

		public async Task CanGetAccountAsync()
		{
			var account = await _generalLedger.GetAccountAsync(_id);
			account.Id.Should().Be("newaccount");
			account.Name.Should().Be("New Account");
			account.Description.Should().BeNull();
			account.Type.Should().Be(AccountType.Asset);
			account.Direction.Should().Be(EntryType.Debit);
		}

		public async Task CanListAccountAsync()
		{
			var accounts = await _generalLedger.GetAccountsAsync();
			accounts.Select(i => i.Id).Should().Contain(_id);
		}

		public async Task IsInDebitAccountsAsync()
		{
			var accounts = await _generalLedger.GetDebitAccountsAsync();
			accounts.Select(i => i.Id).Should().Contain(_id);
		}

		public async Task IsNotInCreditAccountsAsync()
		{
			var accounts = await _generalLedger.GetCreditAccountsAsync();
			accounts.Select(i => i.Id).Should().NotContain(_id);
		}
	}
}