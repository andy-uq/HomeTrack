using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using FluentAssertions;
using HomeTrack.Tests;
using HomeTrack.Web.Controllers;
using HomeTrack.Web.ViewModels;
using Moq;
using NUnit.Framework;
using web.tests;

namespace HomeTrack.Web.Tests
{
	[TestFixture]
	public class TransactionControllerTests
	{
		private TransactionController _controller;
		private Mock<IGeneralLedgerAsyncRepository> _repository;
		private IMappingEngine _mappingEngine;
		private AsyncGeneralLedger _generalLedger;
		
		private Account _bank;
		private Account _income;

		[SetUp]
		public void TransactionController()
		{
			_mappingEngine = new MappingProvider(new ViewModelTypeMapProvider()).Build();
			_repository = new Moq.Mock<IGeneralLedgerAsyncRepository>(MockBehavior.Strict);
			
			_bank = AccountFactory.Asset("bank");
			_income = AccountFactory.Income("income");

			_repository.Setup(x => x.GetAccountAsync("bank"))
				.ReturnsAsync(_bank);

			_repository.Setup(x => x.GetAccountAsync("income"))
				.ReturnsAsync(_income);

			_repository.Setup(x => x.GetBudgetsForAccountAsync(It.IsAny<string>()))
				.ReturnsAsync(Enumerable.Empty<Budget>());

			_generalLedger = new AsyncGeneralLedger(_repository.Object);
			_controller = new TransactionController(_generalLedger, _mappingEngine);
		}

		[Test]
		public async Task Index()
		{
			var t1 = new Transaction(_bank, _income, 10M);
			var t2 = new Transaction(_bank, _income, 20M);

			_repository
				.Setup(x => x.GetTransactionsAsync("bank"))
				.ReturnsAsync(new[] { t1, t2 });

			var result = (ViewResult)await _controller.Index("bank");
			Assert.That(result.Model, Is.InstanceOf<TransactionIndexViewModel>());

			var model = (TransactionIndexViewModel) result.Model;
			model.Account.Should().Be(_bank);
			
			Assert.That(model.Transactions, Is.EquivalentTo(new[] { t1, t2 }));
		}

		[Test]
		public async Task Details()
		{
			var t1 = new Transaction(_bank, _income, 10M);

			_repository
				.Setup(x => x.GetTransactionAsync("1"))
				.ReturnsAsync(t1);

			var result = (ViewResult)await _controller.Details("1", _income.Id);
			Assert.That(result.Model, Is.InstanceOf<ViewModels.TransactionDetails>());

			var model = (ViewModels.TransactionDetails) result.Model;
			model.AccountId.Should().Be(_income.Id);
		}

		[Test]
		public async Task Create()
		{
			_repository
				.Setup(x => x.GetAccountsAsync())
				.ReturnsAsync(new[] {_bank, _income});

			var result = (ViewResult)await _controller.Create("bank");

			Assert.That(result.Model, Is.InstanceOf<ViewModels.NewTransaction>());
			var model = (ViewModels.NewTransaction)result.Model;
			model.Account.Should().Be(_bank);
			Assert.That(model.Accounts, Is.EquivalentTo(new[] { _bank, _income }));
		}

		[Test]
		public async Task CreateTransaction()
		{
			_controller.SetFakeControllerContext("~/transaction/create/bank");

			var args = new NewTransactionArgs()
			{
				AccountId = _bank.Id,
				Amount = 10M,
				Direction = EntryType.Debit,
				Related = new[]
				{
					new RelatedAccount {AccountId = _income.Id, Amount = 10M},
				}
			};

			_repository.Setup(x => x.PostAsync(It.IsAny<Transaction>()))
				.ReturnsAsync(true)
				.Callback<Transaction>(t => {
					t.Amount.Should().Be(10M);
					Assert.That(t.Debit, Is.EquivalentTo(new[] {new Amount(_bank, EntryType.Debit, 10M)}));
					Assert.That(t.Credit, Is.EquivalentTo(new[] {new Amount(_income, EntryType.Credit, 10M)}));
				});

			var result = (JsonResult) await _controller.Create(args);
			Assert.That(result.Data, Has.Property("redirectUrl") /* .EqualTo("/transaction/index/bank") */);
		}

		[Test]
		public async Task UnbalancedTransactionIsError()
		{
			_controller.SetFakeControllerContext("~/transaction/create/bank");

			var args = new NewTransactionArgs()
			{
				AccountId = _bank.Id,
				Amount = 10M,
				Related = new[]
				{
					new RelatedAccount {AccountId = _income.Id, Amount = 100M},
				}
			};

			_repository.Setup(x => x.PostAsync(It.IsAny<Transaction>()))
				.ReturnsAsync(false);

			var result = (JsonResult)await _controller.Create(args);
			Assert.That(result.Data, Has.Property("Tag"));
			Assert.That(result.Data, Has.Property("State"));
		}

		public class ViewModelComparer : 
			IEqualityComparer<HomeTrack.Web.ViewModels.NewTransaction>
		{
			public bool Equals(ViewModels.NewTransaction x, ViewModels.NewTransaction y)
			{
				return 
					x.Date == y.Date 
					&& x.Account.Id == y.Account.Id
					&& x.Description == y.Description;
			}

			public int GetHashCode(ViewModels.NewTransaction obj)
			{
				throw new System.NotImplementedException();
			}
		}
	}
}