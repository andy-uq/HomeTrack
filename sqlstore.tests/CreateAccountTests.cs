using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using NUnit.Framework;

namespace HomeTrack.SqlStore.Tests
{
	public class CreateAccountTests
	{
		private readonly GeneralLedgerRepository _generalLedger;
		private readonly string _id;

		public CreateAccountTests(GeneralLedgerRepository generalLedger)
		{
			_generalLedger = generalLedger;
			_id = _generalLedger.Add(new Account("Bank", AccountType.Asset));
		}

		public void CanGetAccount()
		{
			var account = _generalLedger.GetAccount(_id);
			account.Id.Should().Be("bank");
			account.Name.Should().Be("Bank");
			account.Description.Should().BeNull();
			account.Type.Should().Be(AccountType.Asset);
			account.Direction.Should().Be(EntryType.Debit);
		}

		public void CanListAccount()
		{
			_generalLedger.Accounts.Select(i => i.Id).Should().Contain(_id);
		}

		public void IsInDebitAccounts()
		{
			_generalLedger.DebitAccounts.Select(i => i.Id).Should().Contain(_id);
		}

		public void IsNotInCreditAccounts()
		{
			_generalLedger.CreditAccounts.Select(i => i.Id).Should().NotContain(_id);
		}
	}
}