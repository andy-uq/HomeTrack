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

		private string IdPrefix { get; set; }

		public Import(ImportDetector importDetector)
		{
			_importDetector = importDetector;
		}

		public void Open(string filename, Stream stream = null)
		{
			_filename = filename;
			_import = _importDetector.GetImportDetector(_filename);
			_stream = stream ?? File.OpenRead(filename);

			IdPrefix = Path.GetFileNameWithoutExtension(filename);
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
			row.Id = IdPrefix == null 
				? rowId.ToString(CultureInfo.InvariantCulture) 
				: string.Concat(IdPrefix, '/', rowId);


			return row;
		}
	}

	public interface IImport
	{
		IEnumerable<IImportRow> GetData();
	}
}