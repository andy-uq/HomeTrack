using FluentMigrator;
using HomeTrack.Mapping;

namespace HomeTrack.SqlStore.Tests
{
	[Profile("Integration")]
	public class CreateTestData : Migration
	{
		public override void Up()
		{
			foreach (var account in new[] { TestData.Bank, TestData.Expenses })
			{
				Insert.IntoTable(TableNames.Account)
					.Row(account.Map<Models.Account>());
			}
		}

		public override void Down()
		{
			
		}
	}
}