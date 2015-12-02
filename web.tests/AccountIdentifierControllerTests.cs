using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FluentAssertions;
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
			model.Should().NotBeEmpty();			
		}

		[Test]
		public void Remove()
		{
			_repository.Setup(x => x.Remove(It.IsAny<int>()));

			var result = _controller.Remove(1);
			Assert.That(result, Is.InstanceOf<RedirectToRouteResult>());
			result.RouteValues["action"].Should().Be("index");
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
			model.Accounts.Should().NotBeEmpty();
			model.AvailablePatterns.Should().NotBeEmpty();
			model.AccountId.Should().Be(account.Id);
			model.Patterns.Should().NotBeEmpty();
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
					a.Id.Should().Be(id);
					a.Account.Id.Should().Be("groceries");
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
			model.Accounts.Should().NotBeEmpty();			
			model.AvailablePatterns.Should().NotBeEmpty();			
		}

		[Test]
		public void CreateWithAccountId()
		{
			var result = _controller.Create("bank");
			Assert.That(result.Model, Is.InstanceOf<AccountIdentifierViewModel>());

			var model = (AccountIdentifierViewModel)result.Model;
			model.AccountId.Should().Be("bank");			
			model.Accounts.Should().NotBeEmpty();			
			model.AvailablePatterns.Should().NotBeEmpty();			
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
					new PatternArgs {Name = "Amount", Properties = {{"Amount", "10"},{"Direction","Credit"}}},
					new PatternArgs {Name = "Amount Range", Properties = {{"Min", "10"},{"Max", "100"},}},
				}
			};

			_repository.Setup(x => x.AddOrUpdate(It.IsAny<AccountIdentifier>()))
				.Callback<AccountIdentifier>(a => {
					a.Account.Id.Should().Be("groceries");
					Assert.That(a.Pattern, Is.InstanceOf<CompositePattern>());

					var p = (CompositePattern) a.Pattern;
					Assert.That(p.ElementAt(0), Is.InstanceOf<AmountPattern>());
					Assert.That(p.ElementAt(1), Is.InstanceOf<AmountRangePattern>());
				})
				;

			var result = _controller.Create(args);
			Assert.That(result.Data, Has.Property("redirectUrl"));
		}
	}
}