using System.Collections.Generic;
using System.Web.Mvc;
using HomeTrack.Core;
using HomeTrack.Tests;
using HomeTrack.Web.Controllers;
using HomeTrack.Web.ViewModels;
using Moq;
using NUnit.Framework;

namespace HomeTrack.Web.Tests
{
	[TestFixture]
	public class AccountIdentifierControllerTests
	{
		private Mock<IAccountIdentifierRepository> _repository;
		private AccountIdentifierController _controller;
		private Mock<IGeneralLedgerRepository> _generalLedgerRepository;

		[SetUp]
		public void SetUp()
		{
			_generalLedgerRepository = new Moq.Mock<IGeneralLedgerRepository>(MockBehavior.Strict);
			
			var expense = AccountFactory.Expense("Groceries");
			_generalLedgerRepository.SetupGet(x => x.Accounts).Returns(new[] { expense });
			_generalLedgerRepository.Setup(x => x.GetAccount(expense.Id)).Returns(expense);

			_repository = new Mock<IAccountIdentifierRepository>(MockBehavior.Strict);
			_controller = new AccountIdentifierController(_repository.Object, new GeneralLedger(_generalLedgerRepository.Object), PatternBuilder.GetPatterns());
		}

		[Test]
		public void Index()
		{
			_repository.Setup(x => x.GetAll()).Returns(new[] {new AccountIdentifier()});

			var result = _controller.Index();
			Assert.That(result.Model, Is.InstanceOf<IEnumerable<AccountIdentifier>>());

			var model = (IEnumerable<AccountIdentifier>)result.Model;
			Assert.That(model, Is.Not.Empty);			
		}

		[Test]
		public void Remove()
		{
			_repository.Setup(x => x.Remove(It.IsAny<int>()));

			var result = _controller.Remove(1);
			Assert.That(result, Is.InstanceOf<RedirectToRouteResult>());
			Assert.That(result.RouteValues["action"], Is.EqualTo("index"));
		}

		[Test]
		public void Edit()
		{
			const int id = 1;
			var account = AccountFactory.Expense("groceries");
			IPattern p1 = new AmountPattern {Amount = 10M};
			IPattern p2 = new DayOfMonthPattern(1,15);
			_repository.Setup(x => x.GetById(It.IsAny<int>())).Returns(new AccountIdentifier() { Id = 1, Account = account, Pattern = new CompositePattern(new[] { p1, p2 })});

			var result = _controller.Edit(id);
			Assert.That(result.Model, Is.InstanceOf<AccountIdentifierViewModel>());

			var model = (AccountIdentifierViewModel)result.Model;
			Assert.That(model.Accounts, Is.Not.Empty);
			Assert.That(model.AvailablePatterns, Is.Not.Empty);
			Assert.That(model.AccountId, Is.EqualTo(account.Id));
			Assert.That(model.Patterns, Is.Not.Empty);
		}

		[Test]
		public void EditPost()
		{
			const int id = 1;

			var args = new AccountIdentifierArgs
			{
				AccountId = "groceries",
				Patterns = new[]
				{
					new PatternArgs {Name = "Amount", Properties = {{"Amount", "10"}}}
				}
			};

			_repository.Setup(x => x.AddOrUpdate(It.IsAny<AccountIdentifier>()))
				.Callback<AccountIdentifier>(a =>
				{
					Assert.That(a.Id, Is.EqualTo(id));
					Assert.That(a.Account.Id, Is.EqualTo("groceries"));
					Assert.That(a.Pattern, Is.InstanceOf<AmountPattern>());
				})
				;

			var result = _controller.Edit(id, args);
			Assert.That(result.Data, Has.Property("redirectUrl"));
		}

		[Test]
		public void Create()
		{
			var result = _controller.Create((string) null);
			Assert.That(result.Model, Is.InstanceOf<AccountIdentifierViewModel>());

			var model = (AccountIdentifierViewModel)result.Model;
			Assert.That(model.Accounts, Is.Not.Empty);			
			Assert.That(model.AvailablePatterns, Is.Not.Empty);			
		}

		[Test]
		public void CreateWithAccountId()
		{
			var result = _controller.Create("bank");
			Assert.That(result.Model, Is.InstanceOf<AccountIdentifierViewModel>());

			var model = (AccountIdentifierViewModel)result.Model;
			Assert.That(model.AccountId, Is.EqualTo("bank"));			
			Assert.That(model.Accounts, Is.Not.Empty);			
			Assert.That(model.AvailablePatterns, Is.Not.Empty);			
		}

		[Test]
		public void PostCreateWithNoArgsFails()
		{
			_controller.ModelState.Add("AccoundId", new ModelState());
			_controller.ModelState.AddModelError("AccountId", "AccountId is a required field");

			var result = _controller.Create(new AccountIdentifierArgs());
			Assert.That(result.Data, Has.Property("State").Not.Empty);
			Assert.That(result.Data, Has.Property("State").With.Some.Property("Name").EqualTo("AccountId"));
			Assert.That(result.Data, Has.Property("State").With.Some.Property("Errors").With.Some.EqualTo("AccountId is a required field"));
		}

		[Test]
		public void PostCreate()
		{
			var args = new AccountIdentifierArgs
			{
				AccountId = "groceries",
				Patterns = new[]
				{
					new PatternArgs {Name = "Amount", Properties = {{"Amount", "10"}}}
				}
			};

			_repository.Setup(x => x.AddOrUpdate(It.IsAny<AccountIdentifier>()))
				.Callback<AccountIdentifier>(a => {
					Assert.That(a.Account.Id, Is.EqualTo("groceries"));
					Assert.That(a.Pattern, Is.InstanceOf<AmountPattern>());
				})
				;

			var result = _controller.Create(args);
			Assert.That(result.Data, Has.Property("redirectUrl"));
		}
	}
}