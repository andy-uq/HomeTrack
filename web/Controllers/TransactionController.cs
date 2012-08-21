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

			var model = new TransactionIndexViewModel
			{
				Account = account,
				Transactions =
					from x in _generalLedger.GetTransactions(id)
					orderby x.Date , x.Id
					select new TransactionIndexViewModel.Transaction
					{
						Id = x.Id,
						Date = x.Date,
						Description = x.Description,
						Debit = x.Debit.Where(a => a.Account.Id == id).Select<Amount, decimal?>(d => d.Value).SingleOrDefault(),
						Credit = x.Credit.Where(a => a.Account.Id == id).Select<Amount, decimal?>(d => d.Value).SingleOrDefault(),
					}
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
			var account = _generalLedger[id];
			var model = new ViewModels.Transaction()
			{
				Account = account,
				Accounts = accounts,
				Direction = account.Direction,
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

				bool isCredit = newTransaction.Direction == EntryType.Credit;
				var left = isCredit ? transaction.Credit : transaction.Debit;
				var right = isCredit ? transaction.Debit : transaction.Credit;
				
				left.Add(new Amount(account, newTransaction.Direction, newTransaction.Amount));
				foreach (var r in newTransaction.Related)
				{
					account = _generalLedger[r.AccountId];
					right.Add(new Amount(account, newTransaction.Direction.Invert(), r.Amount));
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
