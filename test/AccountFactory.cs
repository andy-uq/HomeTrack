namespace HomeTrack.Tests
{
	public static class AccountFactory
	{
		public static Account Debit(string name)
		{
			return new Account(name, EntryType.Debit);
		}

		public static Account Credit(string name)
		{
			return new Account(name, EntryType.Credit);
		}
	}
}