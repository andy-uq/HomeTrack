using System.Collections.Generic;
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

		public Import(ImportDetector importDetector)
		{
			_importDetector = importDetector;
		}

		public void Open(string filename, Stream stream = null)
		{
			_filename = filename;
			_import = _importDetector.GetImportDetector(_filename);
			_stream = stream ?? File.OpenRead(filename);
		}

		public string ImportType { get { return _import.Name; } }

		public string[] GetPropertyNames()
		{
			return _import.GetPropertyNames().ToArray();
		}

		public IEnumerable<IImportRow> GetData()
		{
			return _import.Import(_stream);
		}
	}

	public interface IImport
	{
		IEnumerable<IImportRow> GetData();
	}
}