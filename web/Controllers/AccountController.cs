using System.Web.Mvc;
using HomeTrack.RavenStore;

namespace HomeTrack.Web.Controllers
{
	public class AccountController : Controller
	{
		private readonly GeneralLedger _generalLedger;

		public AccountController(GeneralLedger generalLedger)
		{
			_generalLedger = generalLedger;
		}

		//
		// GET: /Account/

		public ActionResult Index()
		{
			return View(_generalLedger);
		}

		//
		// GET: /Account/Details/5

		public ActionResult Details(string id)
		{
			return View(_generalLedger[id]);
		}

		//
		// GET: /Account/Create

		public ActionResult Create()
		{
			return View();
		} 

		//
		// POST: /Account/Create

		[HttpPost]
		public ActionResult Create(Account account)
		{
			try
			{
				_generalLedger.Add(account);
				return RedirectToAction("Index");
			}
			catch
			{
				return View(account);
			}
		}
		
		//
		// GET: /Account/Edit/5

		public ActionResult Edit(string id)
		{
			return View(_generalLedger[id]);
		}

		//
		// POST: /Account/Edit/5

		[HttpPost]
		public ActionResult Edit(Account account)
		{
			try
			{
				_generalLedger.Add(account);
				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		//
		// GET: /Account/Delete/5
 
		public ActionResult Delete(int id)
		{
			return View();
		}

		//
		// POST: /Account/Delete/5

		[HttpPost]
		public ActionResult Delete(int id, FormCollection collection)
		{
			try
			{
				// TODO: Add delete logic here
 
				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}
	}
}
