namespace HomeTrack.Tests
{
	public static class AccountFactory
	{
		private static int _nextId = 1;

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