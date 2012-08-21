namespace HomeTrack
{
	public enum EntryType
	{
		NotSpecified,
		Debit,
		Credit
	}

	public static class EntryTypeExtensions
	{
		public static EntryType Invert(this EntryType entryType)
		{
			return entryType == EntryType.Debit ? EntryType.Credit : EntryType.Debit;
		}
	}
}