using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using HomeTrack.Core;
using HomeTrack.Web.ViewModels;

namespace HomeTrack.Web.Controllers
{
	public class ImportController : Controller
	{
		private readonly TransactionImportContext _transactionImportContext;
		private readonly DirectoryExplorer _directoryExplorer;
		private readonly ImportDetector _importDetector;

		public ImportController(TransactionImportContext transactionImportContext, DirectoryExplorer directoryExplorer, ImportDetector importDetector)
		{
			_transactionImportContext = transactionImportContext;
			_directoryExplorer = directoryExplorer;
			_importDetector = importDetector;
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
			var actualFilename = filename.Replace("@", "/");

			string name = Path.GetFileName(actualFilename);

			var directoryName = Path.GetDirectoryName(actualFilename);
			if (directoryName != null)
			{
				string directory = directoryName.Replace('\\', '/');

				if ( !_directoryExplorer.NavigateTo(directory) )
					return new HttpNotFoundResult("A directory named " + directory + " could not be found");
			}

			var import = new Import(_importDetector);
			import.Open(_directoryExplorer.GetFilename(name));

			var model = new ImportPreview
			{
				FileName = filename,
				Accounts = _transactionImportContext.General.OrderBy(x => x.Type).ThenBy(x => x.Name),
				Import = import, 
				AccountIdentifiers = _transactionImportContext.Patterns
			};

			return View(model);
		}

		public ActionResult Import(string destinationAccountId, string filename, string unclassifiedAccountId, Dictionary<string, string> importRowMapping)
		{
			filename = filename.Replace("@", "/");

			string name = Path.GetFileName(filename);

			var directoryName = Path.GetDirectoryName(filename);
			if ( directoryName != null )
			{
				string directory = directoryName.Replace('\\', '/');

				if ( !_directoryExplorer.NavigateTo(directory) )
					return new HttpNotFoundResult("A directory named " + directory + " could not be found");
			}

			using ( var transaction = new TransactionScope() )
			{
				var import = new Import(_importDetector);
				import.Open(_directoryExplorer.GetFilename(name));

				var source = _transactionImportContext.General[destinationAccountId];
				var unclassifiedAccount = (unclassifiedAccountId == null)
				                          	? null
				                          	: _transactionImportContext.General[unclassifiedAccountId];

				var transactionImport = _transactionImportContext.CreateImport(source, unclassifiedDestination: unclassifiedAccount);
				var transactions = transactionImport.Process(import, importRowMapping).ToList();
				_transactionImportContext.Repository.Save(transactionImport.Result, transactions);

				transaction.Complete();
			
				return PartialView(transactions);
			}
		}

		public ViewResult History()
		{
			var model = _transactionImportContext.Repository.GetAll();
			return View(model);
		}
	}
}