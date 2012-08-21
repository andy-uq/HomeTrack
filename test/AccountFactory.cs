using System;

namespace HomeTrack.Tests
{
	public static class AccountFactory
	{
		public static Account Debit(string name, Action<Account> initialise = null)
		{
			var account = new Account(name, AccountType.Asset);
			if (initialise != null)
				initialise(account);

			return account;
		}

		public static Account Credit(string name, Action<Account> initialise = null)
		{
			var account = new Account(name, AccountType.Liability);
			if ( initialise != null )
				initialise(account);

			return account;
		}
	}
}