using System;
using System.Linq;
using FluentAssertions;

namespace HomeTrack.SqlStore.Tests.Accounts
{
	public class DeleteAccountTests
	{
		private readonly GeneralLedgerRepository _generalLedger;
		private readonly string _id;

		public DeleteAccountTests(GeneralLedgerRepository generalLedger)
		{
			_id = TestData.Expenses.Id;

			_generalLedger = generalLedger;
			_generalLedger.DeleteAccount(_id);
		}

		public void GetAccountFails()
		{
			_generalLedger.Invoking(_ => _.GetAccount(TestData.Expenses.Id)).ShouldThrow<InvalidOperationException>();
		}

		public void AccountNotListed()
		{
			_generalLedger.Accounts.Select(i => i.Id).Should().NotContain(_id);
		}
	}
}