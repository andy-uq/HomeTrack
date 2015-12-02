using System.IO;
using System.Linq;
using FluentAssertions;

namespace HomeTrack.Tests
{
	public class DirectoryExplorerTests
	{
		private static readonly string _directory = TestSettings.GetFilename(@"~/Test Data");

		public void GetDirectory()
		{
			var directories = Directory.GetDirectories(_directory);

			var explorer = new DirectoryExplorer(_directory);
			explorer.GetDirectories().Select(x => x.FullName).Should().Equal(directories);
			explorer.Name.Should().Be("/");
		}

		public void NavigateTo()
		{
			var explorer = new DirectoryExplorer(_directory);
			explorer.NavigateTo("imports/westpac").Should().BeTrue();
			explorer.Name.Should().Be("/Imports/Westpac");
		}

		public void NavigateToRoot()
		{
			var explorer = new DirectoryExplorer(_directory);
			explorer.NavigateToRoot().Should().BeTrue();
			explorer.Name.Should().Be("/");
		}

		public void GetFiles()
		{
			var files = Directory.GetFiles(_directory);

			var explorer = new DirectoryExplorer(_directory);
			explorer.GetFiles().Select(x => x.FullName).Should().Equal(files);
		}

		public void GetFilename()
		{
			var explorer = new DirectoryExplorer(_directory);
			explorer.NavigateTo("imports/westpac").Should().BeTrue();
			explorer.GetFilename("abcd.csv").Should().Be(Path.Combine(_directory, "Imports\\Westpac", "abcd.csv"));
		}
	}
}