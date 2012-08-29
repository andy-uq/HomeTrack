namespace HomeTrack.Core
{
	public class AccountIdentifier
	{
		public IPattern Pattern { get; set; }
		public Account Account { get; set; }

		public bool IsMatch(IImportRow row)
		{
			return Pattern.IsMatch(row);
		}
	}
}