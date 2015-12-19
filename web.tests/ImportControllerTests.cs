using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
		private Account _bank;
		private Account _groceries;
		private Account _wow;
		private Account _unclassifiedExpense;
		private Mock<IImportDetector> _importDetector;
		private DirectoryExplorer _directoryExplorer;

		private static readonly string _directory = TestSettings.GetFilename(@"~/Test Data");
		private IImportAsyncRepository _repository;

		private IEnumerable<AccountIdentifier> GetAccountIdentifers()
		{
			yield return new AccountIdentifier { Pattern = new AmountPattern { Amount = 10M }, Account = _groceries };
			yield return new AccountIdentifier { Pattern = new FieldPattern { Name = "Other Party", Pattern = "Blizzard" }, Account = _wow };
		}

		[SetUp]
		public void SetUp()
		{
			_bank = AccountFactory.Asset("bank", initialBalance: 100);
			_groceries = AccountFactory.Asset("groceries");
			_wow = AccountFactory.Asset("wow");
			_unclassifiedExpense = AccountFactory.Expense("unclassified");

			_directoryExplorer = new DirectoryExplorer(_directory);
			_repository = new InMemoryRepository();

			_importDetector = new Mock<IImportDetector>(MockBehavior.Strict);

			var asyncGeneralLedger = new AsyncGeneralLedger(new InMemoryRepository() { _bank,
				_groceries,
				_wow,
				_unclassifiedExpense});

			var transactionContext = new TransactionImportContext(asyncGeneralLedger, GetAccountIdentifers(), _repository);
			_controller = new ImportController(asyncGeneralLedger, transactionContext, _directoryExplorer, new ImportDetector(new[] {_importDetector.Object}));
		}

		[Test]
		public void Directory()
		{
			var result = (ViewResult )_controller.Directory(null);
			Assert.That(result.Model, Is.InstanceOf<DirectoryExplorer>());

			var model = (DirectoryExplorer)result.Model;
			model.Name.Should().Be("/");
			Assert.That(model.GetDirectories(), Is.Not.Null.Or.Empty);
			Assert.That(model.GetDirectories().Select( x=>x.Name), Has.Member("Imports"));
		}

		[Test]
		public void ChangeDirectory()
		{
			var result = (ViewResult )_controller.Directory("imports@asb");
			Assert.That(result.Model, Is.InstanceOf<DirectoryExplorer>());

			var model = (DirectoryExplorer)result.Model;
			model.Name.Should().Be("/Imports/Asb");
			Assert.That(model.GetDirectories(), Is.Empty);
			Assert.That(model.GetFiles().Select( x=>x.Name), Has.Member("Export20120825200829.csv"));
		}

		[Test]
		public async Task Preview()
		{
			var filename = "imports@asb@export20120825200829.csv";

			var t = filename.Replace("@", "/");
			System.IO.Path.GetDirectoryName(t).Should().Be("imports\\asb");
			System.IO.Path.GetFileName(t).Should().Be("export20120825200829.csv");

			_importDetector.SetupGet(x => x.Name).Returns("Mock");
			_importDetector.Setup(x => x.Matches(It.IsRegex("export20120825200829.csv$")))
				.Returns(true);

			var result = (ViewResult)await _controller.Preview(filename);
			Assert.That(result.Model, Is.InstanceOf<ImportPreview>());

			var model = (ImportPreview)result.Model;
			model.Import.Should().NotBeNull();
			model.Accounts.Should().NotBeNull();
			model.Import.ImportType.Should().Be("Mock");
			model.AccountIdentifiers.Should().NotBeEmpty();
		}

		[Test]
		public async Task Import()
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

			var result = await _controller.Import(_bank.Id, filename, _unclassifiedExpense.Id, new Dictionary<string, ImportRowOptions>());
			Assert.That(result, Is.InstanceOf<PartialViewResult>());

			var model = ((PartialViewResult)result).Model;
			Assert.That(model, Is.InstanceOf<IEnumerable<Transaction>>());

			var transactions = ((IEnumerable<Transaction>) model).ToArray();
			transactions.Count().Should().Be(2);
			transactions.First().Amount.Should().Be(10);
			transactions.Last().Amount.Should().Be(20);
		}

		[Test]
		public async Task History()
		{
			var result = await _controller.History();
			Assert.That(result.Model, Is.InstanceOf<IEnumerable<ImportResult>>());

			var model = (IEnumerable<ImportResult>)result.Model;
			model.Should().NotBeEmpty();
		}
	}
}
