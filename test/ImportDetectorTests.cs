using System;
using System.Linq;
using HomeTrack.Core;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class ImportTests
	{
		private static readonly string _wpFilename = TestSettings.GetFilename(@"~\Test Data\Imports\Westpac\A00_0000_0000000_000-12Aug12.csv");
		private static readonly string _asbFilename = TestSettings.GetFilename(@"~\Test Data\Imports\Asb\Export20120825200829.csv");
		private static readonly string _visaFilename = TestSettings.GetFilename(@"~\Test Data\Imports\Visa\AXXXX_XXXX_XXXX_9623-01Apr12.csv");

		[Test]
		public void CanDetectWestpac()
		{
			var westpac = new WestpacCsvImportDetector();
			Assert.That(westpac.Name, Is.EqualTo("Westpac"));

			Assert.That(westpac.GetPropertyNames(), Is.EquivalentTo(new[] { "Analysis Code", "Description", "Other Party", "Particulars", "Reference" }));

			Assert.That(westpac.Matches(_wpFilename), Is.True);
			Assert.That(westpac.Matches(_asbFilename), Is.False);
			Assert.That(westpac.Matches(_visaFilename), Is.False);
		}

		[Test]
		public void CanDetectAsb()
		{
			var asb = new AsbCsvImportDetector();
			Assert.That(asb.Name, Is.EqualTo("ASB"));

			Assert.That(asb.Matches(_asbFilename), Is.True);
			Assert.That(asb.Matches(_wpFilename), Is.False);
			Assert.That(asb.Matches(_visaFilename), Is.False);
		}

		[Test]
		public void CanDetectVisa()
		{
			var visa = new VisaCsvImportDetector();
			Assert.That(visa.Name, Is.EqualTo("Visa"));

			Assert.That(visa.Matches(_visaFilename), Is.True);
			Assert.That(visa.Matches(_asbFilename), Is.False);
			Assert.That(visa.Matches(_wpFilename), Is.False);
		}

		[Test]
		public void CanDetectArbitrary()
		{
			var wp = new WestpacCsvImportDetector();
			var asb = new AsbCsvImportDetector();
			var visa = new VisaCsvImportDetector();

			var importDetector = new ImportDetector(new IImportDetector[] { wp, asb, visa });
			Assert.That(importDetector.GetImportDetector(_wpFilename), Is.EqualTo(wp));
			Assert.That(importDetector.GetImportDetector(_asbFilename), Is.EqualTo(asb));
			Assert.That(importDetector.GetImportDetector(_visaFilename), Is.EqualTo(visa));
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
		public void VisaImportRow()
		{
			IImportRow visa = new VisaCsvImportRow
			{
				ForeignDetails = "Foreign Details",
				CreditPlanName = "Credit Plan Name",
				OtherParty = "Other Party",
				City = "City",
				CountryCode = "NZ",
				TransactionDate = DateTime.Parse("2000-1-1")
			};

			Assert.That(visa.Properties.Select(x => x.Key), Is.EquivalentTo(VisaCsvImportDetector.PropertyNames));
			Assert.That(visa.Properties.Select(x => x.Value), Is.EquivalentTo(new[] { "Foreign Details", "2000-01-01", "Credit Plan Name", "Other Party", "City", "NZ" }));
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
			var import = wp.Import(_wpFilename);

			Assert.That(import.Count(), Is.EqualTo(18));
			
			var row = import.Last();
			Assert.That(row.Date, Is.EqualTo(DateTime.Parse("2012-8-25")));
			Assert.That(row.Amount, Is.EqualTo(400M));
		}

		[Test]
		public void CanImportAsb()
		{
			var asb = new AsbCsvImportDetector();
			var import = asb.Import(_asbFilename);

			Assert.That(import.Count(), Is.EqualTo(45));

			var row = import.Last();
			Assert.That(row.Date, Is.EqualTo(DateTime.Parse("2012-8-25")));
			Assert.That(row.Amount, Is.EqualTo(-160M));
		}

		[Test]
		public void CanImportVisa()
		{
			var visa = new VisaCsvImportDetector();
			var import = visa.Import(_visaFilename);

			Assert.That(import.Count(), Is.EqualTo(64));

			var row = import.Last();
			Assert.That(row.Date, Is.EqualTo(DateTime.Parse("2012-4-27")));
			Assert.That(row.Amount, Is.EqualTo(-1768.3M));
		}

		[Test]
		public void CanImportArbitrary()
		{
			var wp = new WestpacCsvImportDetector();
			var asb = new AsbCsvImportDetector();
			var visa = new VisaCsvImportDetector();

			var importDetector = new ImportDetector(new IImportDetector[] { wp, asb, visa });
			var import = new Import(importDetector);
			import.Open(_wpFilename);
			
			Assert.That(import.ImportType, Is.EqualTo(wp.Name));
			Assert.That(import.GetPropertyNames(), Is.EqualTo(wp.GetPropertyNames()));
			var data = import.GetData().ToArray();
			Assert.That(data, Is.Not.Empty);
			Assert.That(data.Last().Id, Is.EqualTo("A00_0000_0000000_000-12Aug12/18"));

			import = new Import(importDetector);
			import.Open(_asbFilename);
			Assert.That(import.ImportType, Is.EqualTo(asb.Name));
			Assert.That(import.GetPropertyNames(), Is.EqualTo(asb.GetPropertyNames()));
			data = import.GetData().ToArray();
			Assert.That(data, Is.Not.Empty);
			Assert.That(data.Last().Id, Is.EqualTo("Export20120825200829/45"));

			import = new Import(importDetector);
			import.Open(_visaFilename);
			Assert.That(import.ImportType, Is.EqualTo(visa.Name));
			Assert.That(import.GetPropertyNames(), Is.EqualTo(visa.GetPropertyNames()));
			data = import.GetData().ToArray();
			Assert.That(data, Is.Not.Empty);
			Assert.That(data.Last().Id, Is.EqualTo("AXXXX_XXXX_XXXX_9623-01Apr12/64"));
		}
	}
}