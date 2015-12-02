using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using HomeTrack.Core;
using HomeTrack.Web.ViewModels;

namespace HomeTrack.Web.Controllers
{
	public class AccountIdentifierController : Controller
	{
		private readonly IAccountIdentifierAsyncRepository _repository;
		private readonly GeneralLedger _generalLedger;
		private readonly IEnumerable<PatternBuilder> _patterns;

		public AccountIdentifierController(IAccountIdentifierAsyncRepository repository, GeneralLedger generalLedger, IEnumerable<PatternBuilder> patterns)
		{
			_repository = repository;
			_generalLedger = generalLedger;
			_patterns = patterns;
		}

		public async Task<ViewResult> Index()
		{
			var model = await _repository.GetAllAsync();
			return View(model);
		}

		public ViewResult Create(string accountId)
		{
			var model = new AccountIdentifierViewModel { Accounts = _generalLedger, AvailablePatterns = _patterns, AccountId = accountId };
			return View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public async Task<JsonResult> Create(AccountIdentifierArgs args)
		{
			if ( !ModelState.IsValid )
				return ModelState.ToJson();
			
			var patterns =
				(
					from p in args.Patterns
					let patternBuilder = _patterns.Single(x => x.Name == p.Name)
					select patternBuilder.Build(p.Properties)
				).ToList();

			await  _repository.AddOrUpdateAsync(new AccountIdentifier
			{
				Account = _generalLedger[args.AccountId],
				Pattern = patterns.Count == 1
				          	? patterns.First()
				          	: new CompositePattern(patterns)
			});

			return RedirectToAction("create", new { accountId = args.AccountId }).ToJson(ControllerContext);
		}

		public async Task<RedirectToRouteResult> Remove(int id)
		{
			await _repository.RemoveAsync(id);
			return RedirectToAction("index");
		}

		public async Task<ViewResult> Edit(int id)
		{
			var identifier = await _repository.GetByIdAsync(id);
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
		public async Task<JsonResult> Edit(int id, AccountIdentifierArgs args)
		{
			if ( !ModelState.IsValid )
				return ModelState.ToJson();

			var patterns =
				(
					from p in args.Patterns
					let patternBuilder = _patterns.Single(x => x.Name == p.Name)
					select patternBuilder.Build(p.Properties)
				).ToList();

			await _repository.AddOrUpdateAsync(new AccountIdentifier
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