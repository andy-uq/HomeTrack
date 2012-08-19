using System.Web.Mvc;

namespace HomeTrack.Web.Controllers
{
	public class AccountController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public AccountController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		
		//
		// GET: /Account/

		public ActionResult Index()
		{
			return View(_unitOfWork.GetAll<Account>());
		}

		//
		// GET: /Account/Details/5

		public ActionResult Details(int id)
		{
			return View(_unitOfWork.GetById<Account>(id));
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
				_unitOfWork.Add(account);
				_unitOfWork.SaveChanges();

				return RedirectToAction("Index");
			}
			catch
			{
				return View(account);
			}
		}
		
		//
		// GET: /Account/Edit/5
 
		public ActionResult Edit(int id)
		{
			return View(_unitOfWork.GetById<Account>(id));
		}

		//
		// POST: /Account/Edit/5

		[HttpPost]
		public ActionResult Edit(int id, Account account)
		{
			try
			{
				account.Id = id;
				_unitOfWork.Attach(account);
				_unitOfWork.SaveChanges();
 
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
