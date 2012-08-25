using System.Linq;
using System.Web.Mvc;
using HomeTrack.Web.ViewModels;

namespace HomeTrack.Web.Controllers
{
	public class ImportController : Controller
	{
		private readonly GeneralLedger _generalLedger;
		private readonly DirectoryExplorer _directoryExplorer;

		public ImportController(GeneralLedger generalLedger, DirectoryExplorer directoryExplorer)
		{
			_generalLedger = generalLedger;
			_directoryExplorer = directoryExplorer;
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
	}
}