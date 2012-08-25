using NUnit.Framework;
using core;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class ImportDetectorTests
	{
		private const string WP_FILENAME = @"C:\Users\Andy\Documents\GitHub\HomeTrack\Test Data\Imports\Westpac\A00_0000_0000000_000-12Aug12.csv";
		private const string ASB_FILENAME = @"C:\Users\Andy\Documents\GitHub\HomeTrack\Test Data\Imports\Asb\Export20120825200829.csv";

		[Test]
		public void CanDetectWestpac()
		{
			var westpac = new WestpacCsvImportDetector();
			Assert.That(westpac.Matches(WP_FILENAME), Is.True);
			Assert.That(westpac.Matches(ASB_FILENAME), Is.False);
		}

		[Test]
		public void CanDetectAsb()
		{
			var asb = new AsbCsvImportDetector();
			Assert.That(asb.Matches(ASB_FILENAME), Is.True);
			Assert.That(asb.Matches(WP_FILENAME), Is.False);
		}
	}
}