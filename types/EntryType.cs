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
			switch (entryType)
			{
				case EntryType.Debit:
					return EntryType.Credit;
				
				case EntryType.Credit:
					return EntryType.Debit;

				default:
					return EntryType.NotSpecified;
			}
		}

		public static string ToDrCrString(this EntryType entryType)
		{
			switch ( entryType )
			{
				case EntryType.Debit:
					return "Dr";

				case EntryType.Credit:
					return "Cr";

				default:
					return string.Empty;
			}
		}
	}
}