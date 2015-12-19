using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using HomeTrack.Web.ViewModels;

namespace HomeTrack.Web.Controllers
{
	public class TransactionController : Controller
	{
		private readonly AsyncGeneralLedger _generalLedger;
		private readonly IMappingEngine _mappingEngine;

		public TransactionController(AsyncGeneralLedger generalLedger, IMappingEngine mappingEngine)
		{
			_generalLedger = generalLedger;
			_mappingEngine = mappingEngine;
		}

		//
		// GET: /Transaction/2

		public async Task<ActionResult> Index(string id)
		{
			var account = await _generalLedger.GetAccountAsync(id);

			var model = new TransactionIndexViewModel
			{
				Account = account,
				Transactions =
					from x in await _generalLedger.GetTransactionsAsync(id)
					orderby x.Date , x.Id
					select x
			};

			return View(model);
		}

		//
		// GET: /Transaction/Create

		public async Task<ActionResult> Create(string id)
		{
			var accounts = (await _generalLedger.GetAccountsAsync()).ToArray();
			var account = await _generalLedger.GetAccountAsync(id);
			var model = new ViewModels.NewTransaction()
			{
				Account = account,
				Accounts = accounts,
				Direction = account.Direction,
				Date = DateTime.Now,
				Related = new[] {new EditRelatedAccount() {Accounts = accounts}}
			};

			return View(model);
		}

		//
		// POST: /Transaction/Create

		[HttpPost]
		public async Task<ActionResult> Create(NewTransactionArgs newTransaction)
		{
			if ( ModelState.IsValid )
			{
				var account = await _generalLedger.GetAccountAsync(newTransaction.AccountId);
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
					account = await _generalLedger.GetAccountAsync(r.AccountId);
					right.Add(new Amount(account, newTransaction.Direction.Invert(), r.Amount));
				}

				if ( await _generalLedger.PostAsync(transaction) )
				{
					return this.JsonRedirect(redirectUrl: Url.Action("Index", new { account.Id }));
				}

				ModelState.AddModelError("Amount", string.Format("Right hand amount must equal {0:c}", newTransaction.Amount));
			}

			return ModelState.ToJson();
		}

		public async Task<ActionResult> Details(string id, string accountId)
		{
			var transaction = await _generalLedger.GetTransactionAsync(id);
			var model = _mappingEngine.Map<TransactionDetails>(transaction);
			model.AccountId = accountId;

			return View(model);
		}

		public ActionResult Edit(int id)
		{
			throw new NotImplementedException();
		}
	}
}
