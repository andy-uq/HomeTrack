namespace HomeTrack
{
	public class AccountIdentifier
	{
		public IPattern Pattern { get; set; }
		public Account Account { get; set; }

		public bool IsMatch(IImportRow row)
		{
			return Pattern.IsMatch(row);
		}

		public override string ToString()
		{
			return string.Format("{0} <- {1}", Account, Pattern);
		}
	}
}