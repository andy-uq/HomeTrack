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
		public void Create()
		{
			var result = _controller.Create();
			Assert.That(result.Model, Is.InstanceOf<AccountIdentifierViewModel>());

			var model = (AccountIdentifierViewModel)result.Model;
			Assert.That(model.Accounts, Is.Not.Empty);			
			Assert.That(model.Patterns, Is.Not.Empty);			
		}

		[Test]
		public void PostCreateWithNoArgsFails()
		{
			_controller.ModelState.Add("AccoundId", new ModelState());
			_controller.ModelState.AddModelError("AccountId", "AccountId is a required field");

			var result = (JsonResult )_controller.Create(null);
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

			_repository.Setup(x => x.Add(It.IsAny<AccountIdentifier>()))
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