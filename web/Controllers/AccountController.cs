using System.Collections.Generic;
using System.Transactions;
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
			_generalLedger.Add(account);
			if ( Request.IsAjaxRequest() )
			{
				return Json(account);
			}
			else
			{
				return RedirectToAction("Index");
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
		public ActionResult Edit(string id, string name, string description)
		{
			var account = _generalLedger[id];
			account.Name = name;
			account.Description = description;
			
			_generalLedger.Add(account);
			return RedirectToAction("Index");
		}

		public ViewResult Delete()
		{
			return View(_generalLedger);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult Delete(string[] accountIds)
		{
			if (accountIds == null || accountIds.Length == 0)
				return Json(null);

			using ( var transaction = new TransactionScope() )
			{
				foreach (var accountId in accountIds)
				{
					_generalLedger.DeleteAccount(accountId);
				}

				transaction.Complete();
			}

			return Json(new{ success = true });
		}
	}
}
