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

			_general = new GeneralLedger(new InMemoryGeneralLedger());
			_visa = AccountFactory.Liability("visa");
			_electricity = AccountFactory.Expense("electricity");

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
					Amount = 10M, OtherParty = "Mercury Energy", ProcessDate = DateTimeServer.Now
				}
			};

			_import.Setup(x => x.GetData()).Returns(data);

			var transactions = import.Process(_import.Object).ToList();
			Assert.That(transactions.Count, Is.EqualTo(1));

			var t1 = transactions.First();
			Assert.That(t1.Amount, Is.EqualTo(10M));
			Assert.That(t1.IsDebitAccount(_electricity), Is.True);
			Assert.That(t1.IsCreditAccount(_visa), Is.True);
			Assert.That(t1.Date, Is.EqualTo(DateTimeServer.Now));

			Assert.That(_visa.Balance, Is.EqualTo(10M));
			Assert.That(_electricity.Balance, Is.EqualTo(10M));
		}
	}
}