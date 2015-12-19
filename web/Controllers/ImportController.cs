using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using HomeTrack.Collections;
using HomeTrack.Core;
using HomeTrack.Web.ViewModels;

namespace HomeTrack.Web.Controllers
{
	public class ImportController : Controller
	{
		private readonly AsyncGeneralLedger _generalLedger;
		private readonly TransactionImportContext _transactionImportContext;
		private readonly DirectoryExplorer _directoryExplorer;
		private readonly ImportDetector _importDetector;

		public ImportController(AsyncGeneralLedger generalLedger, TransactionImportContext transactionImportContext, DirectoryExplorer directoryExplorer, ImportDetector importDetector)
		{
			_generalLedger = generalLedger;
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

		public async Task<ActionResult> Preview(string filename)
		{
			var actualFilename = filename.Replace("@", "/");

			string name = Path.GetFileName(actualFilename);

			var directoryName = Path.GetDirectoryName(actualFilename);
			if (string.IsNullOrEmpty(directoryName))
			{
				_directoryExplorer.NavigateToRoot();
			}
			else
			{
				string directory = directoryName.Replace('\\', '/');

				if ( !_directoryExplorer.NavigateTo(directory) )
					return new HttpNotFoundResult("A directory named " + directory + " could not be found");
			}

			var import = new Import(_importDetector);
			var importDetected = import.Open(_directoryExplorer.GetFilename(name));
			if (!importDetected)
			{
				return View("CouldNotDetectFileType");
			}

			var accounts = await _generalLedger.GetAccountsAsync();
			var model = new ImportPreview
			{
				FileName = filename,
				Accounts = accounts.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList(),
				Import = import, 
				AccountIdentifiers = _transactionImportContext.Patterns
			};

			return View(model);
		}

		public async Task<ActionResult> Import(string destinationAccountId, string filename, string unclassifiedAccountId, Dictionary<string, ImportRowOptions> importRowMapping)
		{
			filename = filename.Replace("@", "/");

			string name = Path.GetFileName(filename);

			var directoryName = Path.GetDirectoryName(filename);
			if ( string.IsNullOrEmpty(directoryName) )
			{
				_directoryExplorer.NavigateToRoot();
			}
			else
			{
				string directory = directoryName.Replace('\\', '/');

				if ( !_directoryExplorer.NavigateTo(directory) )
					return new HttpNotFoundResult("A directory named " + directory + " could not be found");
			}

			using ( var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				var import = new Import(_importDetector);
				import.Open(_directoryExplorer.GetFilename(name));

				var source = await _generalLedger.GetAccountAsync(destinationAccountId);
				var unclassifiedAccount = unclassifiedAccountId == null
				                          	? null
				                          	: await _generalLedger.GetAccountAsync(unclassifiedAccountId);

				var transactionImport = _transactionImportContext.CreateImport(source, unclassifiedDestination: unclassifiedAccount);
				var transactions = (await transactionImport.Process(import, importRowMapping)).AsList();

				await _transactionImportContext.Repository.SaveAsync(transactionImport.Result, transactions);

				transaction.Complete();
			
				return PartialView(transactions);
			}
		}

		public async Task<ViewResult> History()
		{
			var model = await _transactionImportContext.Repository.GetAllAsync();
			return View(model);
		}
	}
}