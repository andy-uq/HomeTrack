using System;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using HomeTrack.Core;

namespace HomeTrack.Tests
{
	public class CsvImportTests
	{
		private const string WP_FILENAME =
			@"~\Test Data\Imports\Westpac\A00_0000_0000000_000-12Aug12.csv";

		private const string ASB_FILENAME =
			@"~\Test Data\Imports\Asb\Export20120825200829.csv";

		private MemoryStream BuildCsv(params string[] lines)
		{
			var writer = new StringWriter();
			foreach (var line in lines)
				writer.WriteLine(line);

			return new MemoryStream(Encoding.ASCII.GetBytes(writer.ToString()));
		}

		public void AccumulatorSimple()
		{
			var accumulator = new Accumulator("A,B,C", delimiter: ',');
			accumulator.Next().Should().Be("A");
			accumulator.Next().Should().Be("B");
			accumulator.Next().Should().Be("C");

			accumulator.Next().Should().BeNull();
		}

		public void AccumulatorWithEscapedTextDelimiter()
		{
			var accumulator = new Accumulator("A,\"B,\"\"C\"\"\",D", delimiter: ',') {TextDelimiter = '"'};
			accumulator.Next().Should().Be("A");
			accumulator.Next().Should().Be("B,\"C\"");
			accumulator.Next().Should().Be("D");

			accumulator.Next().Should().BeNull();
		}

		public void AccumulatorWithRequiredTextDelimiter()
		{
			var accumulator = new Accumulator("A,\"B,C\",D", delimiter: ',') {TextDelimiter = '"'};
			accumulator.Next().Should().Be("A");
			accumulator.Next().Should().Be("B,C");
			accumulator.Next().Should().Be("D");

			accumulator.Next().Should().BeNull();
		}

		public void AccumulatorWithTextDelimiter()
		{
			var accumulator = new Accumulator("A,\"B\",C", delimiter: ',') {TextDelimiter = '"'};
			accumulator.Next().Should().Be("A");
			accumulator.Next().Should().Be("B");
			accumulator.Next().Should().Be("C");

			accumulator.Next().Should().BeNull();
		}

		public void DecodeAsbImport()
		{
			var filename = TestSettings.GetFilename(ASB_FILENAME);
			using (var reader = new CsvReader(filename))
			{
				reader.GetHeader(skip: l => !l.StartsWith("Date", StringComparison.OrdinalIgnoreCase));

				var actual = reader.GetData<AsbOrbitFastTrackCsvImportRow>().First();
				actual.Date.Should().Be(DateTime.Parse("2012/08/05"));
				actual.UniqueId.Should().Be("2012080501");
				actual.Amount.Should().Be(-3.80M);
				actual.TranType.Should().Be("TFR OUT");
			}
		}

		public void DecodeData()
		{
			var csv = BuildCsv("A,B,C", "1,2,3");
			using (var reader = new CsvReader(csv))
			{
				var header = reader.GetHeader();
				header.Should().Equal("A", "B", "C");

				var data = reader.GetData().ToArray();
				data[0].Should().Equal("1", "2", "3");
			}
		}

		public void DecodeDataAsObject()
		{
			var csv = BuildCsv("A,B,C", "1,2,3");
			using (var reader = new CsvReader(csv))
			{
				var header = reader.GetHeader();
				header.Should().Equal("A", "B", "C");

				var actual = reader.GetData<TestObj>().First();
				actual.A.Should().Be(1);
				actual.B.Should().Be(2);
				actual.C.Should().Be(3);
			}
		}

		public void DecodeHeader()
		{
			var csv = BuildCsv("A,B,C");
			using (var reader = new CsvReader(csv))
			{
				var header = reader.GetHeader();
				header.Should().Equal("A", "B", "C");
			}
		}

		public void DecodeHeaderWithSpacesAsObject()
		{
			var csv = BuildCsv("A Value,B*Value,C%Value", "A1,B2,C3");
			using (var reader = new CsvReader(csv))
			{
				var actual = reader.GetData<TestObj>().First();
				actual.AValue.Should().Be("A1");
				actual.BValue.Should().Be("B2");
				actual.CValue.Should().Be("C3");
			}
		}

		public void DecodeWestpacImport()
		{
			var filename = TestSettings.GetFilename(WP_FILENAME);
			using (var reader = new CsvReader(filename))
			{
				var actual = reader.GetData<WestpacCsvImportRow>().First();
				actual.Date.Should().Be(DateTime.Parse("13/08/2012"));
				actual.Amount.Should().Be(-140M);
				actual.OtherParty.Should().Be("Countdown Ri WBC ATM");
			}
		}

		public void GetLines()
		{
			var l1 = "LINE 1";
			var l2 = "LINE 2";

			var csv = BuildCsv(l1, l2);

			using (var reader = new CsvReader(csv))
			{
				var lines = reader.GetLines();
				lines.Should().Equal(l1, l2);
			}
		}

		private class TestObj
		{
			public int A { get; set; }
			public int B { get; set; }
			public int C { get; set; }
			public string AValue { get; set; }
			public string BValue { get; set; }
			public string CValue { get; set; }
		}
	}
}