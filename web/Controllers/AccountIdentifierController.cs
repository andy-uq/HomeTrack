using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HomeTrack.Core;
using HomeTrack.Web.ViewModels;

namespace HomeTrack.Web.Controllers
{
	public class AccountIdentifierController : Controller
	{
		private readonly IAccountIdentifierRepository _repository;
		private readonly GeneralLedger _generalLedger;
		private readonly IEnumerable<PatternBuilder> _patterns;

		public AccountIdentifierController(IAccountIdentifierRepository repository, GeneralLedger generalLedger, IEnumerable<PatternBuilder> patterns)
		{
			_repository = repository;
			_generalLedger = generalLedger;
			_patterns = patterns;
		}

		public ViewResult Index()
		{
			var model = _repository.GetAll();
			return View(model);
		}

		public ViewResult Create(string accountId)
		{
			var model = new AccountIdentifierViewModel { Accounts = _generalLedger, AvailablePatterns = _patterns, AccountId = accountId };
			return View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult Create(AccountIdentifierArgs args)
		{
			if ( !ModelState.IsValid )
				return ModelState.ToJson();
			
			var patterns =
				(
					from p in args.Patterns
					let patternBuilder = _patterns.Single(x => x.Name == p.Name)
					select patternBuilder.Build(p.Properties)
				).ToList();

			_repository.AddOrUpdate(new AccountIdentifier
			{
				Account = _generalLedger[args.AccountId],
				Pattern = patterns.Count == 1
				          	? patterns.First()
				          	: new CompositePattern(patterns)
			});

			return RedirectToAction("create", new { accountId = args.AccountId }).ToJson(ControllerContext);
		}

		public RedirectToRouteResult Remove(int id)
		{
			_repository.Remove(id);
			return RedirectToAction("index");
		}

		public ViewResult Edit(int id)
		{
			var identifier = _repository.GetById(id);
			var model = new AccountIdentifierViewModel
			{
				Accounts = _generalLedger,
				AvailablePatterns = _patterns,
				
				AccountId = identifier.Account.Id,
				Patterns = PatternBuilder.Parse(identifier.Pattern)
			};

			return View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult Edit(int id, AccountIdentifierArgs args)
		{
			if ( !ModelState.IsValid )
				return ModelState.ToJson();

			var patterns =
				(
					from p in args.Patterns
					let patternBuilder = _patterns.Single(x => x.Name == p.Name)
					select patternBuilder.Build(p.Properties)
				).ToList();

			_repository.AddOrUpdate(new AccountIdentifier
			{
				Id = id,
				Account = _generalLedger[args.AccountId],
				Pattern = patterns.Count == 1
							? patterns.First()
							: new CompositePattern(patterns)
			});

			return RedirectToAction("index").ToJson(ControllerContext);
		}
	}
}