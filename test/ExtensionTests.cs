using FluentAssertions;

namespace HomeTrack.Tests
{
	public class ExtensionTests
	{
		public void InvertEntryType()
		{
			EntryType.Debit.Invert().Should().Be(EntryType.Credit);
			EntryType.Credit.Invert().Should().Be(EntryType.Debit);
		}
	}
}