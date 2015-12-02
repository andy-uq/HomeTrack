using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using HomeTrack.Core;
using Moq;

namespace HomeTrack.Tests
{
	public class TransactionImportTests
	{
		private readonly Account _electricity;
		private readonly GeneralLedger _general;
		private readonly Account _groceries;
		private readonly Mock<IImport> _import;
		private readonly TransactionImportContext _importContext;
		private readonly Mock<IImportRepositoryAsync> _repository;
		private readonly Account _visa;

		public TransactionImportTests()
		{
			DateTimeServer.SetLocal(new TestDateTimeServer(DateTime.Parse("2012-1-1")));

			_visa = AccountFactory.Liability("visa");
			_electricity = AccountFactory.Expense("electricity");
			_groceries = AccountFactory.Expense("groceries");

			_general = new GeneralLedger(new InMemoryRepository()) {_visa, _electricity, _groceries};
			_repository = new Mock<IImportRepositoryAsync>(MockBehavior.Strict);

			var patterns = GetPatterns();

			_importContext = new TransactionImportContext(_general, patterns, _repository.Object);
			_import = new Mock<IImport>(MockBehavior.Strict);
			_import.SetupGet(x => x.Name).Returns("Mock Import");
			_import.SetupGet(x => x.ImportType).Returns("Mock");
		}

		private IEnumerable<AccountIdentifier> GetPatterns()
		{
			yield return new AccountIdentifier
			{
				Account = _electricity,
				Pattern = new FieldPattern {Name = "Other Party", Pattern = "Mercury"}
			};
		}

		public void CreateVisaImport()
		{
			var import = _importContext.CreateImport(_visa);
			import.Credit.Should().Be(_visa);

			var data = new[]
			{
				new WestpacVisaCsvImportRow
				{
					Id = "I/1",
					Amount = 0M,
					OtherParty = "TXT Alert",
					ProcessDate = DateTimeServer.Now
				},
				new WestpacVisaCsvImportRow
				{
					Id = "I/2",
					Amount = -10M,
					OtherParty = "Mercury Energy",
					ProcessDate = DateTimeServer.Now
				}
			};

			_import.Setup(x => x.GetData()).Returns(data);

			var transactions = import.Process(_import.Object).ToList();
			transactions.Count.Should().Be(1);

			var t1 = transactions.First();
			t1.Amount.Should().Be(10M);
			t1.RelatedAccounts().Should().Contain(_electricity);
			t1.RelatedAccounts().Should().Contain(_visa);
			t1.IsCreditAccount(_visa).Should().BeTrue();
			t1.IsDebitAccount(_electricity).Should().BeTrue();
			t1.Date.Should().Be(DateTimeServer.Now);
			t1.Reference.Should().Be("I/2");

			_visa.Balance.Should().Be(10M);
			_electricity.Balance.Should().Be(10M);

			var result = import.Result;
			result.Date.Should().Be(DateTime.Parse("2012-1-1"));
			result.Name.Should().Be("Mock Import");
			result.ImportType.Should().Be("Mock");
			result.TransactionCount.Should().Be(1);
			result.UnclassifiedTransactions.Should().Be(0);
		}

		public void CreateVisaImportWithUnclassifiedTransaction()
		{
			var unclassified = AccountFactory.Expense("Unclassified expenses");
			var import = _importContext.CreateImport(_visa, unclassifiedDestination: unclassified);
			import.Credit.Should().Be(_visa);

			var data = new[]
			{
				new WestpacVisaCsvImportRow
				{
					Amount = 0M,
					OtherParty = "TXT Alert",
					ProcessDate = DateTimeServer.Now
				},
				new WestpacVisaCsvImportRow
				{
					Amount = -10M,
					OtherParty = "Mercury Energy",
					ProcessDate = DateTimeServer.Now
				},
				new WestpacVisaCsvImportRow
				{
					Amount = -20M,
					OtherParty = "Countdown",
					ProcessDate = DateTimeServer.Now
				}
			};

			_import.Setup(x => x.GetData()).Returns(data);

			var transactions = import.Process(_import.Object).ToList();

			AssertResult(import, transactions, unclassified);

			var result = import.Result;
			result.UnclassifiedTransactions.Should().Be(1);
		}

		public void CreateVisaImportWithManualAccounts()
		{
			var import = _importContext.CreateImport(_visa);
			import.Credit.Should().Be(_visa);

			var data = new[]
			{
				new WestpacVisaCsvImportRow
				{
					Id = "i/1",
					Amount = 0M,
					OtherParty = "TXT Alert",
					ProcessDate = DateTimeServer.Now
				},
				new WestpacVisaCsvImportRow
				{
					Id = "i/2",
					Amount = -10M,
					OtherParty = "Mercury Energy",
					ProcessDate = DateTimeServer.Now
				},
				new WestpacVisaCsvImportRow
				{
					Id = "i/3",
					Amount = -20M,
					OtherParty = "Countdown",
					ProcessDate = DateTimeServer.Now
				}
			};

			_import.Setup(x => x.GetData()).Returns(data);

			var mappings = new Dictionary<string, ImportRowOptions>
			{
				{data[1].Id, new ImportRowOptions {Account = _electricity.Id}},
				{data[2].Id, new ImportRowOptions {Account = _groceries.Id, Description = "Special event"}}
			};

			var transactions = import.Process(_import.Object, mappings).ToList();
			AssertResult(import, transactions, _groceries);

			transactions[1].Description.Should().Be("Special event");
		}

		private void AssertResult(TransactionImport import, ICollection<Transaction> transactions,
			Account expectedSecondAccount)
		{
			transactions.Count.Should().Be(2);

			var t1 = transactions.First();
			t1.Amount.Should().Be(10M);
			t1.RelatedAccounts().Should().Contain(_electricity);
			t1.RelatedAccounts().Should().Contain(_visa);
			t1.IsCreditAccount(_visa).Should().BeTrue();
			t1.IsDebitAccount(_electricity).Should().BeTrue();
			t1.Date.Should().Be(DateTimeServer.Now);

			var t2 = transactions.Last();
			t2.Amount.Should().Be(20M);
			t2.RelatedAccounts().Should().Contain(expectedSecondAccount);
			t2.RelatedAccounts().Should().Contain(_visa);
			t2.IsCreditAccount(_visa).Should().BeTrue();
			t2.IsDebitAccount(expectedSecondAccount).Should().BeTrue();
			t2.Date.Should().Be(DateTimeServer.Now);

			_visa.Balance.Should().Be(30M);
			_electricity.Balance.Should().Be(10M);
			expectedSecondAccount.Balance.Should().Be(20M);

			var result = import.Result;
			result.Date.Should().Be(DateTime.Parse("2012-1-1"));
			result.Name.Should().Be("Mock Import");
			result.TransactionCount.Should().Be(2);
		}

		public void PersistImportResult()
		{
			var unclassified = AccountFactory.Expense("Unclassified expenses");
			var import = _importContext.CreateImport(_visa, unclassifiedDestination: unclassified);
			import.Credit.Should().Be(_visa);

			var data = new[]
			{
				new WestpacVisaCsvImportRow
				{
					Amount = 0M,
					OtherParty = "TXT Alert",
					ProcessDate = DateTimeServer.Now
				},
				new WestpacVisaCsvImportRow
				{
					Amount = -10M,
					OtherParty = "Mercury Energy",
					ProcessDate = DateTimeServer.Now
				},
				new WestpacVisaCsvImportRow
				{
					Amount = -20M,
					OtherParty = "Countdown",
					ProcessDate = DateTimeServer.Now
				}
			};

			_import.Setup(x => x.GetData()).Returns(data);

			var transactions = import.Process(_import.Object).ToList();
			transactions.Count.Should().Be(2);

			_visa.Balance.Should().Be(30M);
		}
	}
}