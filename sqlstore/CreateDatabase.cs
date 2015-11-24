using System;
using FluentMigrator;

namespace HomeTrack.SqlStore
{
	public class TableNames
	{
		public const string Account = nameof(Account);
		public const string AccountType = nameof(AccountType);
	}

	[Migration(1)]
	public class CreateDatabase : Migration
	{
		public override void Up()
		{
			Create.Table(TableNames.AccountType)
				.WithColumn("Name").AsString(20).PrimaryKey()
				.WithColumn("IsDebitOrCredit").AsString(20);

			foreach (AccountType value in Enum.GetValues(typeof (AccountType)))
			{
				if (value == AccountType.NotSet)
					continue;

				Insert.IntoTable(TableNames.AccountType)
					.Row(new {name = value.ToString(), isDebitOrCredit = value.IsDebitOrCredit().ToString() });
			}

			Create.Table(TableNames.Account)
				.WithColumn("Id").AsString(50).PrimaryKey()
				.WithColumn("Name").AsString()
				.WithColumn("Description").AsString().Nullable()
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