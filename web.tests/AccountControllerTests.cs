using System.Web.Mvc;
using System.Web.Routing;
using FluentAssertions;
using HomeTrack;
using HomeTrack.Tests;
using HomeTrack.Web.Controllers;
using Moq;
using NUnit.Framework;

namespace web.tests
{
	[TestFixture]
	public class AccountControllerTests
	{
		private AccountController _controller;
		private Mock<IGeneralLedgerRepository> _repository;
		private GeneralLedger _generalLedger;
		private Account _bank;

		[SetUp]
		public void AccountController()
		{
			_repository = new Moq.Mock<IGeneralLedgerRepository>(MockBehavior.Strict);
			_bank = AccountFactory.Asset("bank", initialBalance:100);
			_repository.Setup(x => x.GetAccount("bank"))
				.Returns(_bank);
			
			_generalLedger = new GeneralLedger(_repository.Object);
			_controller = new AccountController(_generalLedger);
		}

		[Test]
		public void Index()
		{
			var result = (ViewResult )_controller.Index();
			result.ViewName.Should().Be(string.Empty);
			result.Model.Should().Be(_generalLedger);
		}

		[Test]
		public void Delete()
		{
			var result = (ViewResult )_controller.Delete();
			result.ViewName.Should().Be(string.Empty);
			result.Model.Should().Be(_generalLedger);
		}

		[Test]
		public void DeletePostEmpty()
		{
			var result = _controller.Delete(new string[0]);
			result.Data.Should().BeNull();
		}

		[Test]
		public void DeletePost()
		{
			_repository.Setup(x => x.DeleteAccount(_bank.Id)).Returns(true).Verifiable();

			var result = _controller.Delete(new[] { _bank.Id });
			Assert.That(result.Data, Has.Property("success").EqualTo(true));

			_repository.Verify(x => x.DeleteAccount(_bank.Id));
		}

		[Test]
		public void Details()
		{
			var account = new Account();

			_repository.Setup(x => x.GetAccount("bank"))
				.Returns(account);

			var result = (ViewResult )_controller.Details("bank");
			result.Model.Should().Be(account);
			result.ViewName.Should().Be(string.Empty);
		}

		[Test]
		public void Create()
		{
			var result = (ViewResult )_controller.Create();
			result.Model.Should().Be(null);
			result.ViewName.Should().Be(string.Empty);
		}

		[Test]
		public void CreateAccount()
		{
			_controller.SetFakeControllerContext("~/account/create");

			var args = new Account() {Name = "Name", Description = "Description", Type = AccountType.Asset};

			_repository.Setup(x => x.Add(args))
				.Returns("name");

			var result = (RedirectToRouteResult)_controller.Create(args);
			AssertRouteData(result.RouteValues, controller: "account", action: "Index");
		}

		[Test]
		public void CreateAccountAjax()
		{
			_controller.SetFakeControllerContext("~/account/create", isAjax:true);

			var args = new Account() {Name = "Name", Description = "Description", Type = AccountType.Asset};

			_repository.Setup(x => x.Add(args))
				.Returns("name");

			var result = (JsonResult)_controller.Create(args);
			result.Data.Should().Be(args);
		}

		[Test]
		public void Edit()
		{
			var account = new Account();

			_repository.Setup(x => x.GetAccount("bank"))
				.Returns(account);

			var result = (ViewResult)_controller.Edit("bank");
			result.Model.Should().Be(account);
			result.ViewName.Should().Be(string.Empty);
		}

		[Test]
		public void EditAccount()
		{
			var args = _bank;
			Account updatedAccount = null;
			_repository.Setup(x => x.Add(args))
				.Callback<Account>(b => updatedAccount = b)
				.Returns("name");

			var result = (RedirectToRouteResult)_controller.Edit("bank", "Name", "Description");
			AssertRouteData(result.RouteValues, null, action: "Index");

			updatedAccount.Name.Should().Be("Name");
			updatedAccount.Description.Should().Be("Description");
			updatedAccount.Balance.Should().Be(100M);
		}
		
		private void AssertRouteData(RouteValueDictionary routeData, string controller = null, string action = null, string id = null)
		{
			routeData.Should().NotBeNull();
			Assert.That(routeData["controller"], Is.EqualTo(controller), "Bad controller");
			Assert.That(routeData["action"], Is.EqualTo(action), "Bad action");
			Assert.That(routeData["id"], Is.Null.Or.EqualTo((object)id ?? UrlParameter.Optional), "Bad ID parameter");
		}
	}
}