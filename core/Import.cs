using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace HomeTrack.Core
{
	public class Import : IImport
	{
		private readonly ImportDetector _importDetector;
		private string _filename;
		private Stream _stream;
		private IImportDetector _import;

		public string Name { get; private set; }

		public Import(ImportDetector importDetector)
		{
			_importDetector = importDetector;
		}

		public bool Open(string filename, Stream stream = null)
		{
			_filename = filename;
			_stream = stream ?? File.OpenRead(filename);

			Name = Path.GetFileNameWithoutExtension(filename);

			_import = _importDetector.GetImportDetector(_filename);
			return _import != null;
		}

		public string ImportType { get { return _import.Name; } }

		public string[] GetPropertyNames()
		{
			return _import.GetPropertyNames().ToArray();
		}

		public IEnumerable<IImportRow> GetData()
		{
			var rowId = 1;
			foreach ( var row in _import.Import(_stream) )
			{
				yield return BuildId(row, rowId);
				rowId++;
			}
		}

		private IImportRow BuildId(IImportRow row, int rowId)
		{
			if ( row.Id == null )
			{
				row.Id = Name == null
				         	? rowId.ToString(CultureInfo.InvariantCulture)
				         	: string.Concat(Name, '/', rowId);
			}

			return row;
		}
	}

	public interface IImport
	{
		string Name { get; }
		string ImportType { get; }
		IEnumerable<IImportRow> GetData();
	}
}