using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HomeTrack.Tests
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

		public static IEnumerable<Account> Accounts
		{
			get
			{
				return typeof (TestData)
					.GetFields()
					.Cast<FieldInfo>()
					.Where(f => f.FieldType == typeof (Account))
					.Select(f => f.GetValue(null))
					.Cast<Account>();
			}
		}

		private static Account CreateAccount(string name, AccountType accountType)
		{
			var account = new Account(name, accountType);
			account.Id = Regex.Replace(account.Name.ToLower(), "[^a-z0-9-/]", "");

			return account;
		}

		public class AccountLookup : IAccountLookup
		{
			public IEnumerator<Account> GetEnumerator()
			{
				return Accounts.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public Account this[string accountId] => Accounts.Single(a => a.Id == accountId);
		}
	}
}