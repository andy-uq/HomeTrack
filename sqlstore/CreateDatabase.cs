using System;
using System.Data;
using FluentMigrator;

namespace HomeTrack.SqlStore
{
	public class TableNames
	{
		public const string EntryType = nameof(EntryType);

		public const string Account = nameof(Account);
		public const string AccountType = nameof(AccountType);

		public const string ImportResult = nameof(ImportResult);
		public const string ImportedTransaction = nameof(ImportedTransaction);

		public const string Transaction = nameof(Transaction);
		public const string TransactionComponent = nameof(TransactionComponent);

		public const string AccountIdentifier = nameof(AccountIdentifier);
		public const string AccountIdentifierPattern = nameof(AccountIdentifierPattern);
	}

	[Migration(1)]
	public class CreateDatabase : Migration
	{
		public override void Up()
		{
			CreateLookupTables();

			CreateAccountTables();
			CreateTransactionTables();
			CreateImportTables();
		}

		private void CreateAccountTables()
		{
			Create.Table(TableNames.Account)
				.WithColumn("Id").AsString(50)
					.PrimaryKey()
				.WithColumn("Name").AsString(250)
				.WithColumn("Description").AsString().Nullable()
				.WithColumn("AccountTypeName").AsString(20)
					.ForeignKey(TableNames.AccountType, "Name")
				;

			Create.Table(TableNames.AccountIdentifier)
				.WithColumn("Id").AsInt32().Identity()
					.PrimaryKey()
				.WithColumn("AccountId").AsString(50)
					.ForeignKey(TableNames.Account, "Id")
					.Indexed()
				;

			Create.Table(TableNames.AccountIdentifierPattern)
				.WithColumn("AccountIdentifierId").AsInt32()
					.ForeignKey(TableNames.AccountIdentifier, "Id").OnDelete(Rule.Cascade)
					.Indexed()
				.WithColumn("Name").AsString(250)
				.WithColumn("PropertiesJson").AsString()
				;
		}

		private void CreateLookupTables()
		{
			Create.Table(TableNames.EntryType)
				.WithColumn("Name").AsString(20).PrimaryKey();

			foreach (EntryType value in Enum.GetValues(typeof (EntryType)))
			{
				if (value == EntryType.NotSpecified)
					continue;

				Insert.IntoTable(TableNames.EntryType)
					.Row(new {name = value.ToString()});
			}

			Create.Table(TableNames.AccountType)
				.WithColumn("Name").AsString(20)
					.PrimaryKey()
				.WithColumn("EntryTypeName").AsString(20)
					.ForeignKey(TableNames.EntryType, "Name");

			foreach (AccountType value in Enum.GetValues(typeof (AccountType)))
			{
				if (value == AccountType.NotSet)
					continue;

				Insert.IntoTable(TableNames.AccountType)
					.Row(new {name = value.ToString(), entryTypeName = value.IsDebitOrCredit().ToString()});
			}
		}

		private void CreateImportTables()
		{
			Create.Table(TableNames.ImportResult)
				.WithColumn("Id").AsInt32().Identity()
					.PrimaryKey()
				.WithColumn("Name").AsString(250)
				.WithColumn("ImportTypeName").AsString(50)
				.WithColumn("Date").AsDateTime()
				;

			Create.Table(TableNames.ImportedTransaction)
				.WithColumn("Id").AsAnsiString(32)
					.PrimaryKey()
					.ForeignKey(TableNames.Transaction, "Id")
				.WithColumn("ImportId").AsInt32()
					.ForeignKey(TableNames.ImportResult, "Id").OnDelete(Rule.Cascade)
					.Indexed()
				.WithColumn("Unclassified").AsBoolean()
				.WithColumn("Amount").AsDecimal(19, 4)
				;
		}

		private void CreateTransactionTables()
		{
			Create.Table(TableNames.Transaction)
				.WithColumn("Id").AsAnsiString(32)
					.PrimaryKey()
				.WithColumn("Date").AsDateTime()
				.WithColumn("Amount").AsDecimal(19, 4)
				.WithColumn("Reference").AsAnsiString(32)
				.WithColumn("Description").AsString()
				;

			Create.Table(TableNames.TransactionComponent)
				.WithColumn("TransactionId").AsAnsiString(32)
					.ForeignKey(TableNames.Transaction, "Id").OnDelete(Rule.Cascade)
				.WithColumn("AccountId").AsString(50).ForeignKey(TableNames.Account, "Id")
				.WithColumn("EntryTypeName").AsString(20).ForeignKey(TableNames.EntryType, "Name")
				.WithColumn("Amount").AsDecimal(19, 4)
				.WithColumn("Annotation").AsString()
				.WithColumn("AppliedByRuleId").AsInt32().Nullable()
				;

			Create.Index()
				.OnTable(TableNames.TransactionComponent)
					.OnColumn("TransactionId").Ascending()
					.OnColumn("AccountId").Ascending()
					.OnColumn("EntryTypeName").Ascending()
				.WithOptions().Unique();
		}

		public override void Down()
		{
			Delete.Table(TableNames.Account);
			Delete.Table(TableNames.AccountType);
		}
	}
}