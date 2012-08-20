using System;
using System.Linq;
using System.Web.Mvc;
using HomeTrack.RavenStore;
using HomeTrack.Web.ViewModels;

namespace HomeTrack.Web.Controllers
{
	public class TransactionController : Controller
	{
		private readonly GeneralLedger _generalLedger;

		public TransactionController(GeneralLedger generalLedger)
		{
			_generalLedger = generalLedger;
		}

		//
		// GET: /Transaction/2

		public ActionResult Index(string id)
		{
			var account = _generalLedger[id];
			if (account == null)
				return new HttpNotFoundResult();

			var model = new AccountViewModel
			{
				Account = account,
				Transactions = _generalLedger.GetTransactions(id)
			};

			return View(model);
		}

		//
		// GET: /Transaction/Details/5

		public ActionResult Details(int id)
		{
			return View();
		}

		//
		// GET: /Transaction/Create

		public ActionResult Create(string id)
		{
			var accounts = _generalLedger.ToArray();
			var model = new ViewModels.Transaction()
			{
				Account = _generalLedger[id],
				Accounts = accounts,
				Date = DateTime.Now,
				Related = new[] {new EditRelatedAccount() {Accounts = accounts}}
			};

			return
				View(model);
		}

		//
		// POST: /Transaction/Create

		[HttpPost]
		public ActionResult Create(NewTransaction newTransaction)
		{
			if ( ModelState.IsValid )
			{
				var account = _generalLedger[newTransaction.AccountId];
				if (account == null)
					throw new InvalidOperationException("Cannot find an account named " + newTransaction.AccountId);

				var transaction = new Transaction()
				{
					Date = newTransaction.Date,
					Description = newTransaction.Description,
					Amount = newTransaction.Amount,
				};

				bool isCredit = account.Direction == EntryType.Credit;
				var left = isCredit ? transaction.Credit : transaction.Debit;
				var right = isCredit ? transaction.Debit : transaction.Credit;
				
				left.Add(new Amount(account, newTransaction.Amount));
				foreach (var r in newTransaction.Related)
				{
					account = _generalLedger[r.AccountId];
					right.Add(new Amount(account, r.Amount));
				}

				if ( _generalLedger.Post(transaction) )
				{
					return Json(new {redirectUrl = Url.Action("Index", new {account.Id})});
				}

				ModelState.AddModelError("Amount", string.Format("Right hand amount must equal {0:c}", newTransaction.Amount));
			}

			return ModelState.JsonValidation();
		}
		
		//
		// GET: /Transaction/Edit/5
 
		public ActionResult Edit(int id)
		{
			return View();
		}

		//
		// POST: /Transaction/Edit/5

		[HttpPost]
		public ActionResult Edit(int id, FormCollection collection)
		{
			try
			{
				// TODO: Add update logic here
 
				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		//
		// GET: /Transaction/Delete/5
 
		public ActionResult Delete(int id)
		{
			return View();
		}

		//
		// POST: /Transaction/Delete/5

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
