using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using HomeTrack.Core;

namespace HomeTrack.Tests
{
	public class ImportTests
	{
		private static readonly string _wpFilename =
			TestSettings.GetFilename(@"~\Test Data\Imports\Westpac\A00_0000_0000000_000-12Aug12.csv");

		private static readonly string _asbFilename =
			TestSettings.GetFilename(@"~\Test Data\Imports\Asb\Export20120825200829.csv");

		private static readonly string _asbVisaFilename =
			TestSettings.GetFilename(@"~\Test Data\Imports\Asb\Export_CreditCard_20121018.csv");

		private static readonly string _wpVisaFilename =
			TestSettings.GetFilename(@"~\Test Data\Imports\Visa\AXXXX_XXXX_XXXX_9623-01Apr12.csv");

		public void CanDetectWestpac()
		{
			var westpac = new WestpacCsvImportDetector();
			westpac.Name.Should().Be("Westpac");

			westpac.GetPropertyNames()
				.Should()
				.BeEquivalentTo("Analysis Code", "Description", "Other Party", "Particulars", "Reference");

			westpac.Matches(_wpFilename).Should().BeTrue();
			westpac.Matches(_asbFilename).Should().BeFalse();
			westpac.Matches(_wpVisaFilename).Should().BeFalse();
		}

		public void CanDetectAsb()
		{
			var asb = new AsbOrbitFastTrackCsvImportDetector();
			asb.Name.Should().Be("ASB Orbit FastTrack");

			asb.Matches(_asbFilename).Should().BeTrue();
			asb.Matches(_wpFilename).Should().BeFalse();
			asb.Matches(_wpVisaFilename).Should().BeFalse();
		}

		public void CanDetectAsbCreditCard()
		{
			var asb = new AsbVisaCsvImportDetector();
			asb.Name.Should().Be("ASB Visa");

			asb.Matches(_asbVisaFilename).Should().BeTrue();
			asb.Matches(_asbFilename).Should().BeFalse();
			asb.Matches(_wpFilename).Should().BeFalse();
			asb.Matches(_wpVisaFilename).Should().BeFalse();
		}

		public void CanDetectWestpacVisa()
		{
			var visa = new WestpacVisaCsvImportDetector();
			visa.Name.Should().Be("Visa");

			visa.Matches(_wpVisaFilename).Should().BeTrue();
			visa.Matches(_asbFilename).Should().BeFalse();
			visa.Matches(_wpFilename).Should().BeFalse();
		}

		public void CanDetectArbitrary()
		{
			var wp = new WestpacCsvImportDetector();
			var asbOrbit = new AsbOrbitFastTrackCsvImportDetector();
			var asbVisa = new AsbVisaCsvImportDetector();
			var wpVisa = new WestpacVisaCsvImportDetector();

			var importDetector = new ImportDetector(new IImportDetector[] {wp, wpVisa, asbOrbit, asbVisa});
			importDetector.GetImportDetector(_wpFilename).Should().Be(wp);
			importDetector.GetImportDetector(_asbFilename).Should().Be(asbOrbit);
			importDetector.GetImportDetector(_wpVisaFilename).Should().Be(wpVisa);
			importDetector.GetImportDetector(_asbVisaFilename).Should().Be(asbVisa);
		}

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

			wp.Properties.Select(x => x.Key).Should().BeEquivalentTo(WestpacCsvImportDetector.PropertyNames);
			wp.Properties.Select(x => x.Value)
				.Should()
				.BeEquivalentTo("AnalysisCode", "Description", "OtherParty", "Particulars", "Reference");
		}

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

			visa.Properties.Select(x => x.Key).Should().BeEquivalentTo(WestpacVisaCsvImportDetector.PropertyNames);
			visa.Properties.Select(x => x.Value)
				.Should()
				.BeEquivalentTo("Foreign Details", "2000-01-01", "Credit Plan Name", "Other Party", "City", "NZ");
		}

		public void AsbOrbitFastTrackImportRow()
		{
			IImportRow asb = new AsbOrbitFastTrackCsvImportRow
			{
				ChequeNumber = "ChequeNumber",
				Memo = "Memo",
				Payee = "Payee",
				TranType = "TranType",
				UniqueId = "UniqueId"
			};

			asb.Properties.Select(x => x.Key).Should().BeEquivalentTo(AsbOrbitFastTrackCsvImportDetector.PropertyNames);
			asb.Properties.Select(x => x.Value).Should().BeEquivalentTo("ChequeNumber", "Memo", "Payee", "TranType", "UniqueId");
		}

		public void AsbVisaImportRow()
		{
			IImportRow asb = new AsbVisaCsvImportRow
			{
				DateOfTransaction = DateTime.Parse("2000-1-1"),
				TranType = "TranType",
				UniqueId = "UniqueId",
				Reference = "Reference"
			};

			asb.Properties.Select(x => x.Key).Should().BeEquivalentTo(AsbVisaCsvImportDetector.PropertyNames);
			asb.Properties.Select(x => x.Value).Should().BeEquivalentTo("2000-01-01", "TranType", "UniqueId", "Reference", null);
		}

		public void CanImportWestpac()
		{
			var wp = new WestpacCsvImportDetector();
			var import = wp.Import(_wpFilename);

			import.Count().Should().Be(18);

			var row = import.Last();
			row.Date.Should().Be(DateTime.Parse("2012-8-25"));
			row.Amount.Should().Be(400M);
		}

		public void CanImportAsbOrbitFastTrack()
		{
			var asb = new AsbOrbitFastTrackCsvImportDetector();
			var import = asb.Import(_asbFilename);

			import.Count().Should().Be(45);

			var row = import.Last();
			row.Date.Should().Be(DateTime.Parse("2012-8-25"));
			row.Amount.Should().Be(-160M);
		}

		public void CanImportAsbVisa()
		{
			var asb = new AsbVisaCsvImportDetector();
			var import = asb.Import(_asbVisaFilename);

			import.Count().Should().Be(43);

			var row = import.Last();
			row.Date.Should().Be(DateTime.Parse("2012-10-15"));
			row.Amount.Should().Be(17.98M);
		}

		public void CanImportVisa()
		{
			var visa = new WestpacVisaCsvImportDetector();
			var import = visa.Import(_wpVisaFilename);

			import.Count().Should().Be(64);

			var row = import.Last();
			row.Date.Should().Be(DateTime.Parse("2012-4-27"));
			row.Amount.Should().Be(-1768.3M);
		}

		public void CanNotImportBadFile()
		{
			var wp = new WestpacCsvImportDetector();
			var asb = new AsbOrbitFastTrackCsvImportDetector();
			var wpVisa = new WestpacVisaCsvImportDetector();
			var asbVisa = new AsbVisaCsvImportDetector();

			var importDetector = new ImportDetector(new IImportDetector[] {wp, asb, wpVisa, asbVisa});
			var import = new Import(importDetector);
			import.Open("bad.txt", Stream.Null).Should().BeFalse();
		}

		public void CanImportArbitrary()
		{
			var wp = new WestpacCsvImportDetector();
			var asb = new AsbOrbitFastTrackCsvImportDetector();
			var wpVisa = new WestpacVisaCsvImportDetector();
			var asbVisa = new AsbVisaCsvImportDetector();

			var importDetector = new ImportDetector(new IImportDetector[] {wp, asb, wpVisa, asbVisa});
			var import = new Import(importDetector);

			import.Open(_wpFilename).Should().BeTrue();
			import.ImportType.Should().Be(wp.Name);
			import.GetPropertyNames().Should().Equal(wp.GetPropertyNames());

			var data = import.GetData().ToArray();
			data.Should().NotBeEmpty();
			data.Last().Id.Should().Be("A00_0000_0000000_000-12Aug12/18");

			import = new Import(importDetector);
			import.Open(_asbFilename).Should().BeTrue();
			import.ImportType.Should().Be(asb.Name);
			import.GetPropertyNames().Should().Equal(asb.GetPropertyNames());
			data = import.GetData().ToArray();
			data.Should().NotBeEmpty();
			data.Last().Id.Should().Be("asb/2012082501");

			import = new Import(importDetector);
			import.Open(_asbVisaFilename).Should().BeTrue();
			import.ImportType.Should().Be(asbVisa.Name);
			import.GetPropertyNames().Should().Equal(asbVisa.GetPropertyNames());
			data = import.GetData().ToArray();
			data.Should().NotBeEmpty();
			data.Last().Id.Should().Be("visa/2012101505");

			import = new Import(importDetector);
			import.Open(_wpVisaFilename).Should().BeTrue();
			import.ImportType.Should().Be(wpVisa.Name);
			import.GetPropertyNames().Should().Equal(wpVisa.GetPropertyNames());
			data = import.GetData().ToArray();
			data.Should().NotBeEmpty();
			data.Last().Id.Should().Be("AXXXX_XXXX_XXXX_9623-01Apr12/64");
		}
	}
}