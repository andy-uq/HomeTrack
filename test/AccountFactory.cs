using System;

namespace HomeTrack.Tests
{
	public static class AccountFactory
	{
		public static Account Asset(string name, Action<Account> initialise = null)
		{
			return NewAccount(name, AccountType.Asset, initialise);
		}

		public static Account Liability(string name, Action<Account> initialise = null)
		{
			return NewAccount(name, AccountType.Liability, initialise);
		}

		public static Account Income(string name, Action<Account> initialise = null)
		{
			return NewAccount(name, AccountType.Income, initialise);
		}

		private static Account NewAccount(string name, AccountType accountType, Action<Account> initialise)
		{
			var account = new Account(name, accountType);
			if (initialise != null)
				initialise(account);

			return account;
		}
	}
}