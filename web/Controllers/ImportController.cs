using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using HomeTrack.Core;
using HomeTrack.Web.ViewModels;

namespace HomeTrack.Web.Controllers
{
	public class ImportController : Controller
	{
		private readonly GeneralLedger _generalLedger;
		private readonly DirectoryExplorer _directoryExplorer;
		private readonly ImportDetector _importDetector;
		private readonly IEnumerable<AccountIdentifier> _accountIdentifiers;

		public ImportController(GeneralLedger generalLedger, DirectoryExplorer directoryExplorer, ImportDetector importDetector, IEnumerable<AccountIdentifier> accountIdentifiers)
		{
			_generalLedger = generalLedger;
			_directoryExplorer = directoryExplorer;
			_importDetector = importDetector;
			_accountIdentifiers = accountIdentifiers;
		}

		public ActionResult Directory(string path)
		{
			if (!string.IsNullOrEmpty(path))
			{
				path = path.Replace("@", "/");

				if (!_directoryExplorer.NavigateTo(path))
					return new HttpNotFoundResult("A directory named " + path + " could not be found");
			}

			return View(_directoryExplorer);
		}

		public ActionResult Preview(string filename)
		{
			filename = filename.Replace("@", "/");

			string name = Path.GetFileName(filename);

			var directoryName = Path.GetDirectoryName(filename);
			if (directoryName != null)
			{
				string directory = directoryName.Replace('\\', '/');

				if ( !_directoryExplorer.NavigateTo(directory) )
					return new HttpNotFoundResult("A directory named " + directory + " could not be found");
			}

			var import = new Import(_importDetector);
			import.Open(_directoryExplorer.GetFilename(name));

			var model = new ImportPreview {Import = import, AccountIdentifiers = _accountIdentifiers };

			return View(model);
		}
	}
}