namespace HomeTrack.Tests
{
	public static class AccountFactory
	{
		public static Account Debit(string name)
		{
			return new Account(name, AccountType.Asset);
		}

		public static Account Credit(string name)
		{
			return new Account(name, AccountType.Liability);
		}
	}
}