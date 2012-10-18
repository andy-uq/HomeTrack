using System;
using System.Collections.Generic;
using System.Linq;
using HomeTrack.Core;
using Moq;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class TransactionImportTests
	{
		private GeneralLedger _general;
		private Account _visa;

		private TransactionImportContext _importContext;

		private Mock<IImport> _import;
		private Account _electricity;
		private Account _groceries;
		private Mock<IImportRepository> _repository;

		[SetUp]
		public void SetUpForImport()
		{
			DateTimeServer.SetLocal(new TestDateTimeServer(DateTime.Parse("2012-1-1")));

			_visa = AccountFactory.Liability("visa");
			_electricity = AccountFactory.Expense("electricity");
			_groceries = AccountFactory.Expense("groceries");

			_general = new GeneralLedger(new InMemoryRepository()) {_visa, _electricity, _groceries };
			_repository = new Mock<IImportRepository>(MockBehavior.Strict);

			var patterns = GetPatterns();

			_importContext = new TransactionImportContext(_general, patterns, _repository.Object);
			_import = new Moq.Mock<IImport>(MockBehavior.Strict);
			_import.SetupGet(x => x.Name).Returns("Mock Import");
			_import.SetupGet(x => x.ImportType).Returns("Mock");
		}

		private IEnumerable<AccountIdentifier> GetPatterns()
		{
			yield return new AccountIdentifier
			{
				Account = _electricity, 
				Pattern = new FieldPattern { Name = "Other Party", Pattern = "Mercury" }
			};
		}

		[Test]
		public void CreateVisaImport()
		{
			var import = _importContext.CreateImport(_visa);
			Assert.That(import.Credit, Is.EqualTo(_visa));

			var data = new[]
			{
				new WestpacVisaCsvImportRow
				{
					Id = "I/1", Amount = 0M, OtherParty = "TXT Alert", ProcessDate = DateTimeServer.Now
				},
				new WestpacVisaCsvImportRow
				{
					Id = "I/2", Amount = -10M, OtherParty = "Mercury Energy", ProcessDate = DateTimeServer.Now
				}
			};

			_import.Setup(x => x.GetData()).Returns(data);

			var transactions = import.Process(_import.Object).ToList();
			Assert.That(transactions.Count, Is.EqualTo(1));

			var t1 = transactions.First();
			Assert.That(t1.Amount, Is.EqualTo(10M));
			Assert.That(t1.RelatedAccounts(), Contains.Item(_electricity));
			Assert.That(t1.RelatedAccounts(), Contains.Item(_visa));
			Assert.That(t1.IsCreditAccount(_visa), Is.True);
			Assert.That(t1.IsDebitAccount(_electricity), Is.True);
			Assert.That(t1.Date, Is.EqualTo(DateTimeServer.Now));
			Assert.That(t1.Reference, Is.EqualTo("I/2"));

			Assert.That(_visa.Balance, Is.EqualTo(10M));
			Assert.That(_electricity.Balance, Is.EqualTo(10M));

			var result = import.Result;
			Assert.That(result.Date, Is.EqualTo(DateTime.Parse("2012-1-1")));
			Assert.That(result.Name, Is.EqualTo("Mock Import"));
			Assert.That(result.ImportType, Is.EqualTo("Mock"));
			Assert.That(result.TransactionCount, Is.EqualTo(1));
			Assert.That(result.UnclassifiedTransactions, Is.EqualTo(0));
		}

		[Test]
		public void CreateVisaImportWithUnclassifiedTransaction()
		{
			Account unclassified = AccountFactory.Expense("Unclassified expenses");
			var import = _importContext.CreateImport(_visa, unclassifiedDestination: unclassified);
			Assert.That(import.Credit, Is.EqualTo(_visa));

			var data = new[]
			{
				new WestpacVisaCsvImportRow
				{
					Amount = 0M, OtherParty = "TXT Alert", ProcessDate = DateTimeServer.Now
				},
				new WestpacVisaCsvImportRow
				{
					Amount = -10M, OtherParty = "Mercury Energy", ProcessDate = DateTimeServer.Now
				},
				new WestpacVisaCsvImportRow
				{
					Amount = -20M, OtherParty = "Countdown", ProcessDate = DateTimeServer.Now
				}
			};

			_import.Setup(x => x.GetData()).Returns(data);

			var transactions = import.Process(_import.Object).ToList();
			
			AssertResult(import, transactions, unclassified);

			var result = import.Result;
			Assert.That(result.UnclassifiedTransactions, Is.EqualTo(1));
		}

		[Test]
		public void CreateVisaImportWithManualAccounts()
		{
			var import = _importContext.CreateImport(_visa);
			Assert.That(import.Credit, Is.EqualTo(_visa));

			var data = new[]
			{
				new WestpacVisaCsvImportRow
				{
					Id = "i/1", Amount = 0M, OtherParty = "TXT Alert", ProcessDate = DateTimeServer.Now
				},
				new WestpacVisaCsvImportRow
				{
					Id = "i/2", Amount = -10M, OtherParty = "Mercury Energy", ProcessDate = DateTimeServer.Now
				},
				new WestpacVisaCsvImportRow
				{
					Id = "i/3", Amount = -20M, OtherParty = "Countdown", ProcessDate = DateTimeServer.Now
				}
			};

			_import.Setup(x => x.GetData()).Returns(data);

			var mappings = new Dictionary<string, string>
			{
				{data[1].Id, _electricity.Id},
				{data[2].Id, _groceries.Id},
			};

			var transactions = import.Process(_import.Object, mappings).ToList();
			AssertResult(import, transactions, _groceries);
		}

		private void AssertResult(TransactionImport import, ICollection<Transaction> transactions, Account expectedSecondAccount)
		{
			Assert.That(transactions.Count, Is.EqualTo(2));

			var t1 = transactions.First();
			Assert.That(t1.Amount, Is.EqualTo(10M));
			Assert.That(t1.RelatedAccounts(), Contains.Item(_electricity));
			Assert.That(t1.RelatedAccounts(), Contains.Item(_visa));
			Assert.That(t1.IsCreditAccount(_visa), Is.True);
			Assert.That(t1.IsDebitAccount(_electricity), Is.True);
			Assert.That(t1.Date, Is.EqualTo(DateTimeServer.Now));

			var t2 = transactions.Last();
			Assert.That(t2.Amount, Is.EqualTo(20M));
			Assert.That(t2.RelatedAccounts(), Contains.Item(expectedSecondAccount));
			Assert.That(t2.RelatedAccounts(), Contains.Item(_visa));
			Assert.That(t2.IsCreditAccount(_visa), Is.True);
			Assert.That(t2.IsDebitAccount(expectedSecondAccount), Is.True);
			Assert.That(t2.Date, Is.EqualTo(DateTimeServer.Now));

			Assert.That(_visa.Balance, Is.EqualTo(30M));
			Assert.That(_electricity.Balance, Is.EqualTo(10M));
			Assert.That(expectedSecondAccount.Balance, Is.EqualTo(20M));

			var result = import.Result;
			Assert.That(result.Date, Is.EqualTo(DateTime.Parse("2012-1-1")));
			Assert.That(result.Name, Is.EqualTo("Mock Import"));
			Assert.That(result.TransactionCount, Is.EqualTo(2));
		}

		[Test]
		public void PersistImportResult()
		{
			Account unclassified = AccountFactory.Expense("Unclassified expenses");
			var import = _importContext.CreateImport(_visa, unclassifiedDestination: unclassified);
			Assert.That(import.Credit, Is.EqualTo(_visa));

			var data = new[]
			{
				new WestpacVisaCsvImportRow
				{
					Amount = 0M, OtherParty = "TXT Alert", ProcessDate = DateTimeServer.Now
				},
				new WestpacVisaCsvImportRow
				{
					Amount = -10M, OtherParty = "Mercury Energy", ProcessDate = DateTimeServer.Now
				},
				new WestpacVisaCsvImportRow
				{
					Amount = -20M, OtherParty = "Countdown", ProcessDate = DateTimeServer.Now
				}
			};

			_import.Setup(x => x.GetData()).Returns(data);

			var transactions = import.Process(_import.Object).ToList();
			Assert.That(transactions.Count, Is.EqualTo(2));

			Assert.That(_visa.Balance, Is.EqualTo(30M));
		}
	}
}