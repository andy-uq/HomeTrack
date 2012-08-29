namespace HomeTrack
{
	public interface IPattern
	{
		bool IsMatch(IImportRow importRow);
	}
}