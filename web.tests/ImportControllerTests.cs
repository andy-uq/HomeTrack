using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
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
		private DirectoryExplorer _directoryExplorer;

		private const string DIRECTORY = @"C:\Users\Andy\Documents\GitHub\HomeTrack\Test Data";

		[SetUp]
		public void ImportController()
		{
			_repository = new Moq.Mock<IGeneralLedgerRepository>(MockBehavior.Strict);
			_bank = AccountFactory.Asset("bank", initialBalance: 100);
			_repository.Setup(x => x.GetAccount("bank"))
				.Returns(_bank);

			_directoryExplorer = new DirectoryExplorer(DIRECTORY);

			_generalLedger = new GeneralLedger(_repository.Object);
			_controller = new ImportController(_generalLedger, _directoryExplorer);
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
			var result = (ViewResult )_controller.Directory("imports/asb");
			Assert.That(result.Model, Is.InstanceOf<DirectoryExplorer>());

			var model = (DirectoryExplorer)result.Model;
			Assert.That(model.Name, Is.EqualTo("/Imports/Asb"));
			Assert.That(model.GetDirectories(), Is.Empty);
			Assert.That(model.GetFiles().Select( x=>x.Name), Has.Member("Export20120825200829.csv"));
		}
	}
}
