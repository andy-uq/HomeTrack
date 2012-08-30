using System.IO;
using System.Linq;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class DirectoryExplorerTests
	{
		private static readonly string _directory = TestSettings.GetFilename(@"~/Test Data");

		[Test]
		public void GetDirectory()
		{
			var directories = Directory.GetDirectories(_directory);

			var explorer = new DirectoryExplorer(_directory);
			Assert.That(explorer.GetDirectories().Select(x => x.FullName), Is.EqualTo(directories));
			Assert.That(explorer.Name, Is.EqualTo("/"));
		}

		[Test]
		public void NavigateTo()
		{
			var explorer = new DirectoryExplorer(_directory);
			Assert.That(explorer.NavigateTo("imports/westpac"), Is.True);
			Assert.That(explorer.Name, Is.EqualTo("/Imports/Westpac"));
		}

		[Test]
		public void GetFiles()
		{
			var files = Directory.GetFiles(_directory);

			var explorer = new DirectoryExplorer(_directory);
			Assert.That(explorer.GetFiles().Select(x => x.FullName), Is.EqualTo(files));
		}

		[Test]
		public void GetFilename()
		{
			var explorer = new DirectoryExplorer(_directory);
			Assert.That(explorer.NavigateTo("imports/westpac"), Is.True);
			Assert.That(explorer.GetFilename("abcd.csv"), Is.EqualTo(Path.Combine(_directory, "Imports\\Westpac", "abcd.csv")));
		}
	}
}