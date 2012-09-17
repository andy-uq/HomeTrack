using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
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
		private Mock<IGeneralLedgerRepository> _repository;
		private IMappingEngine _mappingEngine;
		private GeneralLedger _generalLedger;
		
		private Account _bank;
		private Account _income;

		[SetUp]
		public void TransactionController()
		{
			_mappingEngine = new MappingProvider(new ViewModelTypeMapProvider()).Build();
			_repository = new Moq.Mock<IGeneralLedgerRepository>(MockBehavior.Strict);
			
			_bank = AccountFactory.Asset("bank");
			_income = AccountFactory.Income("income");

			_repository.Setup(x => x.GetAccount("bank"))
				.Returns(_bank);

			_repository.Setup(x => x.GetAccount("income"))
				.Returns(_income);

			_repository.Setup(x => x.GetBudgetsForAccount(It.IsAny<string>())).Returns(Enumerable.Empty<Budget>);

			_generalLedger = new GeneralLedger(_repository.Object);
			_controller = new TransactionController(_generalLedger, _mappingEngine);
		}

		[Test]
		public void Index()
		{
			var t1 = new Transaction(_bank, _income, 10M);
			var t2 = new Transaction(_bank, _income, 20M);

			_repository
				.Setup(x => x.GetTransactions("bank"))
				.Returns(new[] { t1, t2 });

			var result = (ViewResult)_controller.Index("bank");
			Assert.That(result.Model, Is.InstanceOf<TransactionIndexViewModel>());

			var model = (TransactionIndexViewModel) result.Model;
			Assert.That(model.Account, Is.EqualTo(_bank));
			
			Assert.That(model.Transactions, Is.EquivalentTo(new[] { t1, t2 }));
		}

		[Test]
		public void Details()
		{
			var t1 = new Transaction(_bank, _income, 10M);

			_repository
				.Setup(x => x.GetTransaction(1))
				.Returns(t1);

			var result = _controller.Details(1, _income.Id);
			Assert.That(result.Model, Is.InstanceOf<ViewModels.TransactionDetails>());

			var model = (ViewModels.TransactionDetails) result.Model;
			Assert.That(model.AccountId, Is.EqualTo(_income.Id));
		}

		[Test]
		public void Create()
		{
			_repository
				.Setup(x => x.Accounts)
				.Returns(new[] {_bank, _income});

			var result = (ViewResult)_controller.Create("bank");

			Assert.That(result.Model, Is.InstanceOf<ViewModels.NewTransaction>());
			var model = (ViewModels.NewTransaction)result.Model;
			Assert.That(model.Account, Is.EqualTo(_bank));
			Assert.That(model.Accounts, Is.EquivalentTo(new[] { _bank, _income }));
		}

		[Test]
		public void CreateTransaction()
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

			_repository.Setup(x => x.Post(It.IsAny<Transaction>()))
				.Returns(true)
				.Callback<Transaction>(t => {
					Assert.That(t.Amount, Is.EqualTo(10M));
					Assert.That(t.Debit, Is.EquivalentTo(new[] {new Amount(_bank, EntryType.Debit, 10M)}));
					Assert.That(t.Credit, Is.EquivalentTo(new[] {new Amount(_income, EntryType.Credit, 10M)}));
				});

			var result = (JsonResult) _controller.Create(args);
			Assert.That(result.Data, Has.Property("redirectUrl") /* .EqualTo("/transaction/index/bank") */);
		}

		[Test]
		public void UnbalancedTransactionIsError()
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

			_repository.Setup(x => x.Post(It.IsAny<Transaction>()))
				.Returns(false);

			var result = (JsonResult)_controller.Create(args);
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