using System;
using System.Text.RegularExpressions;

namespace HomeTrack.Tests
{
	public static class AccountFactory
	{
		public static Account Asset(string name, decimal initialBalance = 0, Action<Account> initialise = null)
		{
			return NewAccount(name, AccountType.Asset, initialBalance, initialise);
		}

		public static Account Liability(string name, decimal initialBalance = 0, Action<Account> initialise = null)
		{
			return NewAccount(name, AccountType.Liability, initialBalance, initialise);
		}

		public static Account Income(string name, decimal initialBalance = 0, Action<Account> initialise = null)
		{
			return NewAccount(name, AccountType.Income, initialBalance, initialise);
		}

		public static Account Expense(string name, decimal initialBalance = 0, Action<Account> initialise = null)
		{
			return NewAccount(name, AccountType.Expense, initialBalance, initialise);
		}

		private static Account NewAccount(string name, AccountType accountType, decimal initialBalance,
			Action<Account> initialise)
		{
			var account = new Account(name, accountType)
			{
				Id = Regex.Replace(name, @"\W+", string.Empty).ToLowerInvariant(),
				Balance = initialBalance
			};

			initialise?.Invoke(account);

			return account;
		}
	}
}