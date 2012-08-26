using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HomeTrack.Core
{
	public interface IImportDetector
	{
		string Name { get; }
		bool Matches(string filename);
		IEnumerable<IImportRow> Import(Stream stream);

		IEnumerable<string> GetPropertyNames();
	}

	public class WestpacCsvImportDetector : IImportDetector
	{
		public static readonly string[] PropertyNames = new[] {"Analysis Code", "Description", "Other Party", "Particulars", "Reference"};

		#region IImportDetector Members

		public bool Matches(string asbFilename)
		{
			string filename = Path.GetFileName(asbFilename);
			Debug.Assert(filename != null, "filename != null");

			return Regex.IsMatch(filename, @"^A\d{2}_\d{4}_\d{7}_\d{3}-\d{2}[a-z]{3}\d{2}\.csv$", RegexOptions.IgnoreCase);
		}

		public string Name
		{
			get { return "Westpac"; }
		}

		#endregion

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
				reader.GetHeader();
				return reader.GetData<WestpacCsvImportRow>().ToArray();
			}
		}
	}

	public class AsbCsvImportDetector : IImportDetector
	{
		public static readonly string[] PropertyNames = new[] { "Cheque Number", "Memo", "Payee", "Transaction Type", "Unique Id" };

		#region IImportDetector Members

		public string Name
		{
			get { return "ASB"; }
		}

		public bool Matches(string asbFilename)
		{
			string filename = Path.GetFileName(asbFilename);
			Debug.Assert(filename != null, "filename != null");

			return Regex.IsMatch(filename, @"^Export\d{4}\d{2}\d{2}\d{2}\d{2}\d{2}\.csv$", RegexOptions.IgnoreCase);
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
			using ( reader )
			{
				reader.GetHeader(skip:6);
				return reader.GetData<AsbCsvImportRow>().ToArray();
			}
		}

		#endregion
	}
}