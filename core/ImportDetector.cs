using System.Linq;

namespace HomeTrack.Core
{
	public class ImportDetector
	{
		private readonly IImportDetector[] _importDetectors;

		public ImportDetector(IImportDetector[] importDetectors)
		{
			_importDetectors = importDetectors;
		}

		public IImportDetector GetImportDetector(string filename)
		{
			return _importDetectors.SingleOrDefault(x => x.Matches(filename));
		}
	}
}