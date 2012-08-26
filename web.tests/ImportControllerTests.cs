using System.Linq;
using System.Web.Mvc;
using HomeTrack.Core;
using HomeTrack.Tests;
using HomeTrack.Web.Controllers;
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
		private DirectoryExplorer _directoryExplorer;
		private Mock<IImportDetector> _importDetector;

		private const string DIRECTORY = @"C:\Users\Andy\Documents\GitHub\HomeTrack\Test Data";

		[SetUp]
		public void ImportController()
		{
			_repository = new Moq.Mock<IGeneralLedgerRepository>(MockBehavior.Strict);
			_bank = AccountFactory.Asset("bank", initialBalance: 100);
			_repository.Setup(x => x.GetAccount("bank"))
				.Returns(_bank);

			_directoryExplorer = new DirectoryExplorer(DIRECTORY);

			_importDetector = new Mock<IImportDetector>(MockBehavior.Strict);
			_generalLedger = new GeneralLedger(_repository.Object);

			_controller = new ImportController(_generalLedger, _directoryExplorer, new ImportDetector(new[] {_importDetector.Object}));
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
			Assert.That(result.Model, Is.InstanceOf<Import>());

			var model = (Import )result.Model;
			Assert.That(model.ImportType, Is.EqualTo("Mock"));
		}
	}
}
