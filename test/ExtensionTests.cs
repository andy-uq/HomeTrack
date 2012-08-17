using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class ExtensionTests
	{
		[Test]
		public void InvertEntryType()
		{
			Assert.That(EntryType.Debit.Invert(), Is.EqualTo(EntryType.Credit));
			Assert.That(EntryType.Credit.Invert(), Is.EqualTo(EntryType.Debit));
		}
	}
}