using System;
using System.Collections.Generic;
using System.IO;
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
	public class ImportControllerTests
	{
		private ImportController _controller;
		private GeneralLedger _generalLedger;
		private Account _bank;
		private Account _groceries;
		private Account _wow;
		private Account _unclassifiedExpense;
		private DirectoryExplorer _directoryExplorer;
		private Mock<IImportDetector> _importDetector;

		private static readonly string _directory = TestSettings.GetFilename(@"~/Test Data");
		private Mock<IImportRepository> _repository;

		private IEnumerable<AccountIdentifier> GetAccountIdentifers()
		{
			yield return new AccountIdentifier { Pattern = new AmountPattern { Amount = 10M }, Account = _groceries };
			yield return new AccountIdentifier { Pattern = new FieldPattern { Name = "Other Party", Pattern = "Blizzard" }, Account = _wow };
		}

		public ImportControllerTests()
		{
			_bank = AccountFactory.Asset("bank", initialBalance: 100);
			_groceries = AccountFactory.Asset("groceries");
			_wow = AccountFactory.Asset("wow");
			_unclassifiedExpense = AccountFactory.Expense("unclassified");

			_directoryExplorer = new DirectoryExplorer(_directory);
			_repository = new Mock<IImportRepository>();

			_importDetector = new Mock<IImportDetector>(MockBehavior.Strict);
			_generalLedger = new GeneralLedger(new InMemoryRepository())
			{
				_bank,
				_groceries,
				_wow,
				_unclassifiedExpense
			};

			var transactionContext = new TransactionImportContext(_generalLedger, GetAccountIdentifers(), _repository);
			_controller = new ImportController(transactionContext, _directoryExplorer, new ImportDetector(new[] {_importDetector.Object}));
		}

		public void Directory()
		{
			var result = (ViewResult )_controller.Directory(null);
			Assert.That(result.Model, Is.InstanceOf<DirectoryExplorer>());

			var model = (DirectoryExplorer)result.Model;
			model.Name.Should().Be("/");
			Assert.That(model.GetDirectories(), Is.Not.Null.Or.Empty);
			Assert.That(model.GetDirectories().Select( x=>x.Name), Has.Member("Imports"));
		}

		public void ChangeDirectory()
		{
			var result = (ViewResult )_controller.Directory("imports@asb");
			Assert.That(result.Model, Is.InstanceOf<DirectoryExplorer>());

			var model = (DirectoryExplorer)result.Model;
			model.Name.Should().Be("/Imports/Asb");
			Assert.That(model.GetDirectories(), Is.Empty);
			Assert.That(model.GetFiles().Select( x=>x.Name), Has.Member("Export20120825200829.csv"));
		}

		public void Preview()
		{
			var filename = "imports@asb@export20120825200829.csv";

			var t = filename.Replace("@", "/");
			System.IO.Path.GetDirectoryName(t).Should().Be("imports\\asb");
			System.IO.Path.GetFileName(t).Should().Be("export20120825200829.csv");

			_importDetector.SetupGet(x => x.Name).Returns("Mock");
			_importDetector.Setup(x => x.Matches(It.IsRegex("export20120825200829.csv$")))
				.Returns(true);

			var result = (ViewResult)_controller.Preview(filename);
			Assert.That(result.Model, Is.InstanceOf<ImportPreview>());

			var model = (ImportPreview)result.Model;
			model.Import.Should().NotBeNull();
			model.Accounts.Should().NotBeNull();
			model.Import.ImportType.Should().Be("Mock");
			model.AccountIdentifiers.Should().NotBeEmpty();
		}

		[Test]
		public void Import()
		{
			var filename = "imports@asb@export20120825200829.csv";

			var t = filename.Replace("@", "/");
			System.IO.Path.GetDirectoryName(t).Should().Be("imports\\asb");
			System.IO.Path.GetFileName(t).Should().Be("export20120825200829.csv");

			_importDetector.SetupGet(x => x.Name).Returns("Mock");
			_importDetector.Setup(x => x.Matches(It.IsRegex("export20120825200829.csv$")))
				.Returns(true);

			var i1 = new WestpacVisaCsvImportRow { ProcessDate = DateTime.Now.Date, Amount = 10M, OtherParty = "COUNTDOWN" };
			var i2 = new WestpacVisaCsvImportRow { ProcessDate = DateTime.Now.Date, Amount = 20M, OtherParty = "MERCURY ENERGY" };

			_importDetector.Setup(x => x.Import(It.IsAny<Stream>()))
				.Returns<Stream>(_ => new[] { i1, i2 });

			var result = _controller.Import(_bank.Id, filename, _unclassifiedExpense.Id, new Dictionary<string, ImportRowOptions>());
			Assert.That(result, Is.InstanceOf<PartialViewResult>());

			var model = ((PartialViewResult)result).Model;
			Assert.That(model, Is.InstanceOf<IEnumerable<Transaction>>());

			var transactions = (IEnumerable<Transaction>) model;
			transactions.Count().Should().Be(2);
			transactions.First().Amount.Should().Be(10);
			transactions.Last().Amount.Should().Be(20);
		}

		[Test]
		public void History()
		{
			_repository.Setup(x => x.GetAll()).Returns(new[] {new ImportResult()});

			var result = _controller.History();
			Assert.That(result.Model, Is.InstanceOf<IEnumerable<ImportResult>>());

			var model = (IEnumerable<ImportResult>)result.Model;
			model.Should().NotBeEmpty();
		}
	}
}
