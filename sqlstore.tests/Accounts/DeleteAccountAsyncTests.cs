using System;
using System.Linq;
using System.Threading.Tasks;
using FixieShim.Fixie;
using FluentAssertions;

namespace HomeTrack.SqlStore.Tests.Accounts
{
	public class DeleteAccountAsyncTests : IAsyncTest
	{
		private readonly GeneralLedgerRepository _generalLedger;
		private readonly string _id;

		public DeleteAccountAsyncTests(GeneralLedgerRepository generalLedger)
		{
			_id = TestData.Expenses.Id;
			_generalLedger = generalLedger;
		}

		public Task InitialiseAsync()
		{
			return _generalLedger.DeleteAccountAsync(_id);
		}

		public void GetAccountFailsAsync()
		{
			_generalLedger.Awaiting(_ => _.GetAccountAsync(TestData.Expenses.Id))
				.ShouldThrow<InvalidOperationException>();
		}

		public async Task AccountNotListedAsync()
		{
			var accounts = await _generalLedger.GetAccountsAsync();
			accounts.Select(i => i.Id).Should().NotContain(_id);
		}
	}
}