using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HomeTrack.Core
{
	public class WestpacVisaCsvImportDetector : IImportDetector
	{
		public static readonly string[] PropertyNames = new[] { "Other Party", "Transaction Date", "City", "Country Code", "Foreign Details", "Credit Plan Name" };

		public string Name
		{
			get { return "Visa"; }
		}

		public bool Matches(string visaFilename)
		{
			string filename = Path.GetFileName(visaFilename);
			Debug.Assert(filename != null, "filename != null");

			return Regex.IsMatch(filename, @"^AX{4}_X{4}_X{4}_\d{4}-\d{2}[a-z]{3}\d{2}\.csv$", RegexOptions.IgnoreCase);
		}

		public IEnumerable<IImportRow> Import(string wpFilename)
		{
			var reader = new CsvReader(wpFilename);
			return ImportRows(reader);
		}

		public IEnumerable<IImportRow> Import(Stream stream)
		{
			var reader = new CsvReader(stream);
			return ImportRows(reader);
		}

		public IEnumerable<string> GetPropertyNames()
		{
			return PropertyNames;
		}

		private IEnumerable<IImportRow> ImportRows(CsvReader reader)
		{
			using ( reader )
			{
				reader.GetHeader();
				return reader.GetData<WestpacVisaCsvImportRow>().ToArray();
			}
		}
	}
}