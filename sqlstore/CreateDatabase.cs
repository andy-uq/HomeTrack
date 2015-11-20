using System;
using FluentMigrator;

namespace HomeTrack.SqlStore
{
	public class TableNames
	{
		public const string Account = nameof(Account);
		public const string AccountType = nameof(AccountType);
	}

	public class CreateDatabase : Migration
	{
		public override void Up()
		{
			Create.Table(TableNames.AccountType)
				.WithColumn("Name").AsString(20).PrimaryKey();

			foreach (var name in Enum.GetNames(typeof (AccountType)))
				Insert.IntoTable(TableNames.AccountType)
					.Row(new {name});

			Create.Table(TableNames.Account)
				.WithColumn("Id").AsString(50).PrimaryKey()
				.WithColumn("Name").AsString()
				.WithColumn("Description").AsString()
				.WithColumn("AccountTypeName").AsString(20).ForeignKey(TableNames.AccountType, "Name")
				;
		}

		public override void Down()
		{
			Delete.Table(TableNames.Account);
			Delete.Table(TableNames.AccountType);
		}
	}
}