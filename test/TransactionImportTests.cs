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

		[SetUp]
		public void SetUpForImport()
		{
			DateTimeServer.SetLocal(new TestDateTimeServer(DateTime.Now));

			_visa = AccountFactory.Liability("visa");
			_electricity = AccountFactory.Expense("electricity");

			_general = new GeneralLedger(new InMemoryGeneralLedger()) {_visa, _electricity};

			var patterns = GetPatterns();

			_importContext = new TransactionImportContext(_general, patterns);
			_import = new Moq.Mock<IImport>(MockBehavior.Strict);
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
				new VisaCsvImportRow
				{
					Amount = 0M, OtherParty = "TXT Alert", ProcessDate = DateTimeServer.Now
				},
				new VisaCsvImportRow
				{
					Amount = -10M, OtherParty = "Mercury Energy", ProcessDate = DateTimeServer.Now
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

			Assert.That(_visa.Balance, Is.EqualTo(10M));
			Assert.That(_electricity.Balance, Is.EqualTo(10M));
		}

		[Test]
		public void CreateVisaImportWithUnclassifiedTransaction()
		{
			Account unclassified = AccountFactory.Expense("Unclassified expenses");
			var import = _importContext.CreateImport(_visa, unclassifiedDestination: unclassified);
			Assert.That(import.Credit, Is.EqualTo(_visa));

			var data = new[]
			{
				new VisaCsvImportRow
				{
					Amount = 0M, OtherParty = "TXT Alert", ProcessDate = DateTimeServer.Now
				},
				new VisaCsvImportRow
				{
					Amount = -10M, OtherParty = "Mercury Energy", ProcessDate = DateTimeServer.Now
				},
				new VisaCsvImportRow
				{
					Amount = -20M, OtherParty = "Countdown", ProcessDate = DateTimeServer.Now
				}
			};

			_import.Setup(x => x.GetData()).Returns(data);

			var transactions = import.Process(_import.Object).ToList();
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
			Assert.That(t2.RelatedAccounts(), Contains.Item(unclassified));
			Assert.That(t2.RelatedAccounts(), Contains.Item(_visa));
			Assert.That(t2.IsCreditAccount(_visa), Is.True);
			Assert.That(t2.IsDebitAccount(unclassified), Is.True);
			Assert.That(t2.Date, Is.EqualTo(DateTimeServer.Now));

			Assert.That(_visa.Balance, Is.EqualTo(30M));
			Assert.That(_electricity.Balance, Is.EqualTo(10M));
			Assert.That(unclassified.Balance, Is.EqualTo(20M));
		}
	}
}