using System.IO;
using System.Linq;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class DirectoryExplorerTests
	{
		private const string DIRECTORY = @"C:\Users\Andy\Documents\GitHub\HomeTrack\Test Data";

		[Test]
		public void GetDirectory()
		{
			var directories = Directory.GetDirectories(DIRECTORY);

			var explorer = new DirectoryExplorer(DIRECTORY);
			Assert.That(explorer.GetDirectories().Select(x => x.FullName), Is.EqualTo(directories));
			Assert.That(explorer.Name, Is.EqualTo("/"));
		}

		[Test]
		public void NavigateTo()
		{
			var explorer = new DirectoryExplorer(DIRECTORY);
			Assert.That(explorer.NavigateTo("imports/westpac"), Is.True);
			Assert.That(explorer.Name, Is.EqualTo("/Imports/Westpac"));
		}

		[Test]
		public void GetFiles()
		{
			var files = Directory.GetFiles(DIRECTORY);

			var explorer = new DirectoryExplorer(DIRECTORY);
			Assert.That(explorer.GetFiles().Select(x => x.FullName), Is.EqualTo(files));
		}
	}
}