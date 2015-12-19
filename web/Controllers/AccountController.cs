using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;

namespace HomeTrack.Web.Controllers
{
	public class AccountController : Controller
	{
		private readonly AsyncGeneralLedger _generalLedger;

		public AccountController(AsyncGeneralLedger generalLedger)
		{
			_generalLedger = generalLedger;
		}

		//
		// GET: /Account/

		public async Task<ActionResult> Index()
		{
			var accounts = await _generalLedger.GetAccountsAsync();
			return View(accounts);
		}

		//
		// GET: /Account/Details/5

		public async Task<ActionResult> Details(string id)
		{
			var account = await _generalLedger.GetAccountAsync(id);
			return View(account);
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
		public async Task<ActionResult> Create(Account account)
		{
			await _generalLedger.AddAsync(account);
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

		public async Task<ActionResult> Edit(string id)
		{
			var account = await _generalLedger.GetAccountAsync(id);
			return View(account);
		}

		//
		// POST: /Account/Edit/5

		[HttpPost]
		public async Task<ActionResult> Edit(string id, string name, string description)
		{
			var account = await _generalLedger.GetAccountAsync(id);
			account.Name = name;
			account.Description = description;
			
			await _generalLedger.AddAsync(account);
			return RedirectToAction("Index");
		}

		public async Task<ActionResult> Delete()
		{
			var accounts = await _generalLedger.GetAccountsAsync();
			return View(accounts);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public async Task<ActionResult> Delete(string[] accountIds)
		{
			if (accountIds == null || accountIds.Length == 0)
				return Json(null);

			using ( var transaction = new TransactionScope() )
			{
				foreach (var accountId in accountIds)
				{
					await _generalLedger.DeleteAccount(accountId);
				}

				transaction.Complete();
			}

			return Json(new{ success = true });
		}
	}
}
