using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace core
{
	public class WestpacCsvImportDetector
	{
		public bool Matches(string wpFilename)
		{
			var filename = Path.GetFileName(wpFilename);
			Debug.Assert(filename != null, "filename != null");
			
			return Regex.IsMatch(filename, @"^A\d{2}_\d{4}_\d{7}_\d{3}-\d{2}[a-z]{3}\d{2}\.csv$", RegexOptions.IgnoreCase);
		}
	}
	public class AsbCsvImportDetector
	{
		public bool Matches(string wpFilename)
		{
			var filename = Path.GetFileName(wpFilename);
			Debug.Assert(filename != null, "filename != null");

			return Regex.IsMatch(filename, @"^Export\d{4}\d{2}\d{2}\d{2}\d{2}\d{2}\.csv$", RegexOptions.IgnoreCase);
		}
	}
}