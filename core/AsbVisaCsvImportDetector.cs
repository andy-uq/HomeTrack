using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HomeTrack.Core
{
	public class AsbVisaCsvImportDetector : IImportDetector
	{
		public static readonly string[] PropertyNames = new[] { "Transaction Date", "Transaction Type", "Unique Id", "Reference" };

		#region IImportDetector Members							

		public string Name
		{
			get { return "ASB Visa"; }
		}

		public bool Matches(string filename)
		{
			string name = Path.GetFileName(filename);
			Debug.Assert(name != null, "filename != null");

			if (!Regex.IsMatch(name, @"^Export(_(\w+_)+)?\d{4}\d{2}\d{2}(\d{2}\d{2}\d{2})?\.csv$", RegexOptions.IgnoreCase))
				return false;

			using ( var fs = File.OpenText(filename) )
			{
				string tagLine;
				if (fs.ReadLine() == null || (tagLine = fs.ReadLine()) == null)
					return false;

				return Regex.IsMatch(tagLine, "VISA Platinum");
			}
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

		private static IEnumerable<IImportRow> ImportRows(CsvReader reader)
		{
			using (reader)
			{
				reader.GetHeader(skip: l => !l.StartsWith("Date", StringComparison.OrdinalIgnoreCase));
				return reader.GetData<AsbVisaCsvImportRow>().ToArray();
			}
		}

		#endregion
	}
}