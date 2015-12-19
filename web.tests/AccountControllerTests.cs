using System.Threading.Tasks;
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
		private Mock<IGeneralLedgerAsyncRepository> _repository;
		private AsyncGeneralLedger _generalLedger;
		private Account _bank;

		[SetUp]
		public void AccountController()
		{
			_repository = new Moq.Mock<IGeneralLedgerAsyncRepository>(MockBehavior.Strict);
			_bank = AccountFactory.Asset("bank", initialBalance:100);
			_repository.Setup(x => x.GetAccountAsync("bank"))
				.ReturnsAsync(_bank);
			
			_generalLedger = new AsyncGeneralLedger(_repository.Object);
			_controller = new AccountController(_generalLedger);
		}

		[Test]
		public async Task Index()
		{
			var accounts = new[] { _bank };
			_repository.Setup(x => x.GetAccountsAsync())
				.ReturnsAsync(accounts);

			var result = (ViewResult ) await _controller.Index();
			result.ViewName.Should().Be(string.Empty);
			result.Model.Should().Be(accounts);
		}

		[Test]
		public async Task Delete()
		{
			var accounts = new[] { _bank };
			_repository.Setup(x => x.GetAccountsAsync())
				.ReturnsAsync(accounts);

			var result = (ViewResult ) await _controller.Delete();
			result.ViewName.Should().Be(string.Empty);
			result.Model.Should().Be(accounts);
		}

		[Test]
		public async Task DeletePostEmpty()
		{
			var result = (JsonResult )await _controller.Delete(new string[0]);
			result.Data.Should().BeNull();
		}

		[Test]
		public async Task DeletePost()
		{
			_repository.Setup(x => x.DeleteAccountAsync(_bank.Id))
				.ReturnsAsync(true);

			var result = (JsonResult )await _controller.Delete(new[] { _bank.Id });
			Assert.That(result.Data, Has.Property("success").EqualTo(true));

			_repository.Verify(x => x.DeleteAccountAsync(_bank.Id));
		}

		[Test]
		public async Task Details()
		{
			var account = new Account();

			_repository.Setup(x => x.GetAccountAsync("bank"))
				.ReturnsAsync(account);

			var result = (ViewResult )await _controller.Details("bank");
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
		public async Task CreateAccount()
		{
			_controller.SetFakeControllerContext("~/account/create");

			var args = new Account() {Name = "Name", Description = "Description", Type = AccountType.Asset};

			_repository.Setup(x => x.AddAsync(args))
				.ReturnsAsync("name");

			var result = (RedirectToRouteResult)await _controller.Create(args);
			AssertRouteData(result.RouteValues, controller: "account", action: "Index");
		}

		[Test]
		public async Task CreateAccountAjax()
		{
			_controller.SetFakeControllerContext("~/account/create", isAjax:true);

			var args = new Account() {Name = "Name", Description = "Description", Type = AccountType.Asset};

			_repository.Setup(x => x.AddAsync(args))
				.ReturnsAsync("name");

			var result = (JsonResult)await _controller.Create(args);
			result.Data.Should().Be(args);
		}

		[Test]
		public async Task Edit()
		{
			var account = new Account();

			_repository.Setup(x => x.GetAccountAsync("bank"))
				.ReturnsAsync(account);

			var result = (ViewResult)await _controller.Edit("bank");
			result.Model.Should().Be(account);
			result.ViewName.Should().Be(string.Empty);
		}

		[Test]
		public async Task EditAccount()
		{
			var args = _bank;
			Account updatedAccount = null;
			_repository.Setup(x => x.AddAsync(args))
				.Callback<Account>(b => updatedAccount = b)
				.ReturnsAsync("name");

			var result = (RedirectToRouteResult)await _controller.Edit("bank", "Name", "Description");
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