using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using NUnit.Framework;

namespace HomeTrack.SqlStore.Tests
{
	public class CreateAccountTests
	{
		[Test]
		public void CreateAccount()
		{
			using (var db = new TestDatabase().ApplyMigrations())
			{
				var repo = new GeneralLedgerRepository(db.Database);
				var id = repo.Add(new Account("Bank", AccountType.Asset));

				var account = repo.GetAccount(id);
				account.Id.Should().Be("bank");
				account.Name.Should().Be("Bank");
				account.Description.Should().BeNull();
				account.Type.Should().Be(AccountType.Asset);
				account.Direction.Should().Be(EntryType.Debit);
			}
		}
	}
}