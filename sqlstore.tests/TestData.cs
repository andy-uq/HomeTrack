using System.Text.RegularExpressions;

namespace HomeTrack.SqlStore.Tests
{
	public static class TestData
	{
		public static readonly Account Bank;
		public static readonly Account Expenses;
		
		static TestData()
		{
			Bank = CreateAccount("Bank", AccountType.Asset);
			Expenses = CreateAccount("Expenses", AccountType.Expense);
		}

		private static Account CreateAccount(string name, AccountType accountType)
		{
			var account = new Account(name, accountType);
			account.Id = Regex.Replace(account.Name.ToLower(), "[^a-z0-9-/]", "");

			return account;
		}
	}
}