using System;
using System.Linq;
using HomeTrack.Core;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class ImportTests
	{
		private const string WP_FILENAME = @"C:\Users\Andy\Documents\GitHub\HomeTrack\Test Data\Imports\Westpac\A00_0000_0000000_000-12Aug12.csv";
		private const string ASB_FILENAME = @"C:\Users\Andy\Documents\GitHub\HomeTrack\Test Data\Imports\Asb\Export20120825200829.csv";

		[Test]
		public void CanDetectWestpac()
		{
			var westpac = new WestpacCsvImportDetector();
			Assert.That(westpac.Name, Is.EqualTo("Westpac"));

			Assert.That(westpac.GetPropertyNames(), Is.EquivalentTo(new[] { "Analysis Code", "Description", "Other Party", "Particulars", "Reference" }));

			Assert.That(westpac.Matches(WP_FILENAME), Is.True);
			Assert.That(westpac.Matches(ASB_FILENAME), Is.False);
		}

		[Test]
		public void CanDetectAsb()
		{
			var asb = new AsbCsvImportDetector();
			Assert.That(asb.Name, Is.EqualTo("ASB"));

			Assert.That(asb.Matches(ASB_FILENAME), Is.True);
			Assert.That(asb.Matches(WP_FILENAME), Is.False);
		}

		[Test]
		public void CanDetectArbitrary()
		{
			var wp = new WestpacCsvImportDetector();
			var asb = new AsbCsvImportDetector();

			var importDetector = new ImportDetector(new IImportDetector[] { wp, asb });
			Assert.That(importDetector.GetImportDetector(WP_FILENAME), Is.EqualTo(wp));
			Assert.That(importDetector.GetImportDetector(ASB_FILENAME), Is.EqualTo(asb));
		}

		[Test]
		public void WestpacImportRow()
		{
			IImportRow wp = new WestpacCsvImportRow
			{
				AnalysisCode = "AnalysisCode",
				Description = "Description",
				OtherParty = "OtherParty",
				Particulars = "Particulars",
				Reference = "Reference"
			};

			Assert.That(wp.Properties.Select(x => x.Key), Is.EquivalentTo(WestpacCsvImportDetector.PropertyNames));
			Assert.That(wp.Properties.Select(x => x.Value), Is.EquivalentTo(new[] { "AnalysisCode", "Description", "OtherParty", "Particulars", "Reference" }));
		}

		[Test]
		public void AsbImportRow()
		{
			IImportRow asb = new AsbCsvImportRow
			{
				ChequeNumber = "ChequeNumber",
				Memo = "Memo",
				Payee = "Payee",
				TranType = "TranType",
				UniqueId = "UniqueId",
			};

			Assert.That(asb.Properties.Select(x => x.Key), Is.EquivalentTo(AsbCsvImportDetector.PropertyNames));
			Assert.That(asb.Properties.Select(x => x.Value), Is.EquivalentTo(new[] { "ChequeNumber", "Memo", "Payee", "TranType", "UniqueId" }));
		}

		[Test]
		public void CanImportWestpac()
		{
			var wp = new WestpacCsvImportDetector();
			var import = wp.Import(WP_FILENAME);

			Assert.That(import.Count(), Is.EqualTo(18));
			
			var row = import.Last();
			Assert.That(row.Date, Is.EqualTo(DateTime.Parse("2012-8-25")));
			Assert.That(row.Amount, Is.EqualTo(400M));
		}

		[Test]
		public void CanImportAsb()
		{
			var asb = new AsbCsvImportDetector();
			var import = asb.Import(ASB_FILENAME);

			Assert.That(import.Count(), Is.EqualTo(45));

			var row = import.Last();
			Assert.That(row.Date, Is.EqualTo(DateTime.Parse("2012-8-25")));
			Assert.That(row.Amount, Is.EqualTo(-160M));
		}

		[Test]
		public void CanImportArbitrary()
		{
			var wp = new WestpacCsvImportDetector();
			var asb = new AsbCsvImportDetector();

			var importDetector = new ImportDetector(new IImportDetector[] { wp, asb });
			var import = new Import(importDetector);
			import.Open(WP_FILENAME);
			
			Assert.That(import.ImportType, Is.EqualTo(wp.Name));
			Assert.That(import.GetPropertyNames(), Is.EqualTo(wp.GetPropertyNames()));
			import.GetData();

			import = new Import(importDetector);
			import.Open(ASB_FILENAME);
			Assert.That(import.ImportType, Is.EqualTo(asb.Name));
			Assert.That(import.GetPropertyNames(), Is.EqualTo(asb.GetPropertyNames()));
			import.GetData();
		}
	}
}