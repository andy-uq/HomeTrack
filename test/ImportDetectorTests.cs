using System;
using System.IO;
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
		private static readonly string _asbVisaFilename = TestSettings.GetFilename(@"~\Test Data\Imports\Asb\Export_CreditCard_20121018.csv");
		private static readonly string _wpVisaFilename = TestSettings.GetFilename(@"~\Test Data\Imports\Visa\AXXXX_XXXX_XXXX_9623-01Apr12.csv");

		[Test]
		public void CanDetectWestpac()
		{
			var westpac = new WestpacCsvImportDetector();
			Assert.That(westpac.Name, Is.EqualTo("Westpac"));

			Assert.That(westpac.GetPropertyNames(), Is.EquivalentTo(new[] { "Analysis Code", "Description", "Other Party", "Particulars", "Reference" }));

			Assert.That(westpac.Matches(_wpFilename), Is.True);
			Assert.That(westpac.Matches(_asbFilename), Is.False);
			Assert.That(westpac.Matches(_wpVisaFilename), Is.False);
		}

		[Test]
		public void CanDetectAsb()
		{
			var asb = new AsbOrbitFastTrackCsvImportDetector();
			Assert.That(asb.Name, Is.EqualTo("ASB Orbit FastTrack"));

			Assert.That(asb.Matches(_asbFilename), Is.True);
			Assert.That(asb.Matches(_wpFilename), Is.False);
			Assert.That(asb.Matches(_wpVisaFilename), Is.False);
		}

		[Test]
		public void CanDetectAsbCreditCard()
		{
			var asb = new AsbVisaCsvImportDetector();
			Assert.That(asb.Name, Is.EqualTo("ASB Visa"));

			Assert.That(asb.Matches(_asbVisaFilename), Is.True);
			Assert.That(asb.Matches(_asbFilename), Is.False);
			Assert.That(asb.Matches(_wpFilename), Is.False);
			Assert.That(asb.Matches(_wpVisaFilename), Is.False);
		}

		[Test]
		public void CanDetectWestpacVisa()
		{
			var visa = new WestpacVisaCsvImportDetector();
			Assert.That(visa.Name, Is.EqualTo("Visa"));

			Assert.That(visa.Matches(_wpVisaFilename), Is.True);
			Assert.That(visa.Matches(_asbFilename), Is.False);
			Assert.That(visa.Matches(_wpFilename), Is.False);
		}

		[Test]
		public void CanDetectArbitrary()
		{
			var wp = new WestpacCsvImportDetector();
			var asbOrbit = new AsbOrbitFastTrackCsvImportDetector();
			var asbVisa = new AsbVisaCsvImportDetector();
			var wpVisa = new WestpacVisaCsvImportDetector();

			var importDetector = new ImportDetector(new IImportDetector[] { wp, wpVisa, asbOrbit, asbVisa });
			Assert.That(importDetector.GetImportDetector(_wpFilename), Is.EqualTo(wp));
			Assert.That(importDetector.GetImportDetector(_asbFilename), Is.EqualTo(asbOrbit));
			Assert.That(importDetector.GetImportDetector(_wpVisaFilename), Is.EqualTo(wpVisa));
			Assert.That(importDetector.GetImportDetector(_asbVisaFilename), Is.EqualTo(asbVisa));
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
			IImportRow visa = new WestpacVisaCsvImportRow
			{
				ForeignDetails = "Foreign Details",
				CreditPlanName = "Credit Plan Name",
				OtherParty = "Other Party",
				City = "City",
				CountryCode = "NZ",
				TransactionDate = DateTime.Parse("2000-1-1")
			};

			Assert.That(visa.Properties.Select(x => x.Key), Is.EquivalentTo(WestpacVisaCsvImportDetector.PropertyNames));
			Assert.That(visa.Properties.Select(x => x.Value), Is.EquivalentTo(new[] { "Foreign Details", "2000-01-01", "Credit Plan Name", "Other Party", "City", "NZ" }));
		}

		[Test]
		public void AsbOrbitFastTrackImportRow()
		{
			IImportRow asb = new AsbOrbitFastTrackCsvImportRow
			{
				ChequeNumber = "ChequeNumber",
				Memo = "Memo",
				Payee = "Payee",
				TranType = "TranType",
				UniqueId = "UniqueId",
			};

			Assert.That(asb.Properties.Select(x => x.Key), Is.EquivalentTo(AsbOrbitFastTrackCsvImportDetector.PropertyNames));
			Assert.That(asb.Properties.Select(x => x.Value), Is.EquivalentTo(new[] { "ChequeNumber", "Memo", "Payee", "TranType", "UniqueId" }));
		}

		[Test]
		public void AsbVisaImportRow()
		{
			IImportRow asb = new AsbVisaCsvImportRow
			{
				DateOfTransaction = DateTime.Parse("2000-1-1"),
				TranType = "TranType",
				UniqueId = "UniqueId",
				Reference = "Reference"
			};

			Assert.That(asb.Properties.Select(x => x.Key), Is.EquivalentTo(AsbVisaCsvImportDetector.PropertyNames));
			Assert.That(asb.Properties.Select(x => x.Value), Is.EquivalentTo(new[] { "2000-01-01", "TranType", "UniqueId", "Reference" }));
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
		public void CanImportAsbOrbitFastTrack()
		{
			var asb = new AsbOrbitFastTrackCsvImportDetector();
			var import = asb.Import(_asbFilename);

			Assert.That(import.Count(), Is.EqualTo(45));

			var row = import.Last();
			Assert.That(row.Date, Is.EqualTo(DateTime.Parse("2012-8-25")));
			Assert.That(row.Amount, Is.EqualTo(-160M));
		}

		[Test]
		public void CanImportAsbVisa()
		{
			var asb = new AsbVisaCsvImportDetector();
			var import = asb.Import(_asbVisaFilename);

			Assert.That(import.Count(), Is.EqualTo(43));

			var row = import.Last();
			Assert.That(row.Date, Is.EqualTo(DateTime.Parse("2012-10-15")));
			Assert.That(row.Amount, Is.EqualTo(17.98M));
		}

		[Test]
		public void CanImportVisa()
		{
			var visa = new WestpacVisaCsvImportDetector();
			var import = visa.Import(_wpVisaFilename);

			Assert.That(import.Count(), Is.EqualTo(64));

			var row = import.Last();
			Assert.That(row.Date, Is.EqualTo(DateTime.Parse("2012-4-27")));
			Assert.That(row.Amount, Is.EqualTo(-1768.3M));
		}

		[Test]
		public void CanNotImportBadFile()
		{
			var wp = new WestpacCsvImportDetector();
			var asb = new AsbOrbitFastTrackCsvImportDetector();
			var wpVisa = new WestpacVisaCsvImportDetector();
			var asbVisa = new AsbVisaCsvImportDetector();

			var importDetector = new ImportDetector(new IImportDetector[] { wp, asb, wpVisa, asbVisa });
			var import = new Import(importDetector);
			Assert.That(import.Open("bad.txt", Stream.Null), Is.False);
		}

		[Test]
		public void CanImportArbitrary()
		{
			var wp = new WestpacCsvImportDetector();
			var asb = new AsbOrbitFastTrackCsvImportDetector();
			var wpVisa = new WestpacVisaCsvImportDetector();
			var asbVisa = new AsbVisaCsvImportDetector();

			var importDetector = new ImportDetector(new IImportDetector[] { wp, asb, wpVisa, asbVisa});
			var import = new Import(importDetector);

			Assert.That(import.Open(_wpFilename), Is.True);			
			Assert.That(import.ImportType, Is.EqualTo(wp.Name));
			Assert.That(import.GetPropertyNames(), Is.EqualTo(wp.GetPropertyNames()));

			var data = import.GetData().ToArray();
			Assert.That(data, Is.Not.Empty);
			Assert.That(data.Last().Id, Is.EqualTo("A00_0000_0000000_000-12Aug12/18"));

			import = new Import(importDetector);
			Assert.That(import.Open(_asbFilename), Is.True);
			Assert.That(import.ImportType, Is.EqualTo(asb.Name));
			Assert.That(import.GetPropertyNames(), Is.EqualTo(asb.GetPropertyNames()));
			data = import.GetData().ToArray();
			Assert.That(data, Is.Not.Empty);
			Assert.That(data.Last().Id, Is.EqualTo("asb/2012082501"));

			import = new Import(importDetector);
			Assert.That(import.Open(_asbVisaFilename), Is.True);
			Assert.That(import.ImportType, Is.EqualTo(asbVisa.Name));
			Assert.That(import.GetPropertyNames(), Is.EqualTo(asbVisa.GetPropertyNames()));
			data = import.GetData().ToArray();
			Assert.That(data, Is.Not.Empty);
			Assert.That(data.Last().Id, Is.EqualTo("visa/2012101505"));

			import = new Import(importDetector);
			Assert.That(import.Open(_wpVisaFilename), Is.True);
			Assert.That(import.ImportType, Is.EqualTo(wpVisa.Name));
			Assert.That(import.GetPropertyNames(), Is.EqualTo(wpVisa.GetPropertyNames()));
			data = import.GetData().ToArray();
			Assert.That(data, Is.Not.Empty);
			Assert.That(data.Last().Id, Is.EqualTo("AXXXX_XXXX_XXXX_9623-01Apr12/64"));
		}
	}
}