using System;
using FluentMigrator;

namespace HomeTrack.SqlStore
{
	public class TableNames
	{
		public const string Account = nameof(Account);
		public const string AccountType = nameof(AccountType);

		public const string ImportResult = nameof(ImportResult);
		public const string ImportedTransaction = nameof(ImportedTransaction);
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
				.WithColumn("Name").AsString(250)
				.WithColumn("Description").AsString().Nullable()
				.WithColumn("AccountTypeName").AsString(20).ForeignKey(TableNames.AccountType, "Name")
				;

			Create.Table(TableNames.ImportResult)
				.WithColumn("Id").AsInt32().Identity().PrimaryKey()
				.WithColumn("Name").AsString(250)
				.WithColumn("ImportTypeName").AsString(50)
				.WithColumn("Date").AsDateTime()
				;

			Create.Table(TableNames.ImportedTransaction)
				.WithColumn("Id").AsAnsiString(32).PrimaryKey()
				.WithColumn("ImportId").AsInt32().ForeignKey(TableNames.ImportResult, "Id").Indexed()
				.WithColumn("Unclassified").AsBoolean()
				.WithColumn("Amount").AsDecimal(19, 4)
				;
		}

		public override void Down()
		{
			Delete.Table(TableNames.Account);
			Delete.Table(TableNames.AccountType);
		}
	}
}