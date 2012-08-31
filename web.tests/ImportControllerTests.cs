using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HomeTrack.Core;
using HomeTrack.RavenStore;
using HomeTrack.Tests;
using HomeTrack.Web.Controllers;
using HomeTrack.Web.ViewModels;
using Moq;
using NUnit.Framework;

namespace HomeTrack.Web.Tests
{
	[TestFixture]
	public class ImportControllerTests
	{
		private ImportController _controller;
		private Mock<IGeneralLedgerRepository> _repository;
		private GeneralLedger _generalLedger;
		private Account _bank;
		private Account _groceries;
		private Account _wow;
		private DirectoryExplorer _directoryExplorer;
		private Mock<IImportDetector> _importDetector;

		private static readonly string _directory = TestSettings.GetFilename(@"~/Test Data");

		private IEnumerable<AccountIdentifier> GetAccountIdentifers()
		{
			yield return new AccountIdentifier { Pattern = new AmountPattern { Amount = 10M }, Account = _groceries };
			yield return new AccountIdentifier { Pattern = new FieldPattern { Name = "Other Party", Pattern = "Blizzard" }, Account = _wow };
		}

		[SetUp]
		public void ImportController()
		{
			_repository = new Moq.Mock<IGeneralLedgerRepository>(MockBehavior.Strict);
			_bank = AccountFactory.Asset("bank", initialBalance: 100);
			_groceries = AccountFactory.Asset("groceries");
			_wow = AccountFactory.Asset("wow");
			
			_repository.Setup(x => x.GetAccount("bank"))
				.Returns(_bank);

			_directoryExplorer = new DirectoryExplorer(_directory);

			_importDetector = new Mock<IImportDetector>(MockBehavior.Strict);
			_generalLedger = new GeneralLedger(_repository.Object);

			_controller = new ImportController(_generalLedger, _directoryExplorer, new ImportDetector(new[] {_importDetector.Object}), GetAccountIdentifers());
		}

		[Test]
		public void Directory()
		{
			var result = (ViewResult )_controller.Directory(null);
			Assert.That(result.Model, Is.InstanceOf<DirectoryExplorer>());

			var model = (DirectoryExplorer)result.Model;
			Assert.That(model.Name, Is.EqualTo("/"));
			Assert.That(model.GetDirectories(), Is.Not.Null.Or.Empty);
			Assert.That(model.GetDirectories().Select( x=>x.Name), Has.Member("Imports"));
		}

		[Test]
		public void ChangeDirectory()
		{
			var result = (ViewResult )_controller.Directory("imports@asb");
			Assert.That(result.Model, Is.InstanceOf<DirectoryExplorer>());

			var model = (DirectoryExplorer)result.Model;
			Assert.That(model.Name, Is.EqualTo("/Imports/Asb"));
			Assert.That(model.GetDirectories(), Is.Empty);
			Assert.That(model.GetFiles().Select( x=>x.Name), Has.Member("Export20120825200829.csv"));
		}

		[Test]
		public void Preview()
		{
			var filename = "imports@asb@export20120825200829.csv";

			var t = filename.Replace("@", "/");
			Assert.That(System.IO.Path.GetDirectoryName(t), Is.EqualTo("imports\\asb"));
			Assert.That(System.IO.Path.GetFileName(t), Is.EqualTo("export20120825200829.csv"));

			_importDetector.SetupGet(x => x.Name).Returns("Mock");
			_importDetector.Setup(x => x.Matches(It.IsRegex("export20120825200829.csv$")))
				.Returns(true);

			var result = (ViewResult)_controller.Preview(filename);
			Assert.That(result.Model, Is.InstanceOf<ImportPreview>());

			var model = (ImportPreview)result.Model;
			Assert.That(model.Import, Is.Not.Null);
			Assert.That(model.Import.ImportType, Is.EqualTo("Mock"));
			Assert.That(model.AccountIdentifiers, Is.Not.Empty);
		}
	}

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
		public void Add()
		{
			var result = _controller.Create();
			Assert.That(result.Model, Is.InstanceOf<AccountIdentifierViewModel>());

			var model = (AccountIdentifierViewModel)result.Model;
			Assert.That(model.Accounts, Is.Not.Empty);			
			Assert.That(model.Patterns, Is.Not.Empty);			
		}

		[Test]
		public void PostAdd()
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
			Assert.That(result.RouteValues["action"], Is.EqualTo("index"));
		}
	}
}
