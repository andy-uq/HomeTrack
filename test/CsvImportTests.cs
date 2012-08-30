using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HomeTrack.Core;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class CsvImportTests
	{
		private const string WP_FILENAME =
			@"~\Test Data\Imports\Westpac\A00_0000_0000000_000-12Aug12.csv";

		private const string ASB_FILENAME =
			@"~\Test Data\Imports\Asb\Export20120825200829.csv";

		private class TestObj
		{
			public int A { get; set; }
			public int B { get; set; }
			public int C { get; set; }

			public string AValue { get; set; }
			public string BValue { get; set; }
			public string CValue { get; set; }
		}

		private MemoryStream BuildCsv(params string[] lines)
		{
			var writer = new StringWriter();
			foreach (string line in lines)
				writer.WriteLine(line);

			return new MemoryStream(Encoding.ASCII.GetBytes(writer.ToString()));
		}

		[Test]
		public void AccumulatorSimple()
		{
			var accumulator = new Accumulator("A,B,C", delimiter: ',');
			Assert.That(accumulator.Next(), Is.EqualTo("A"));
			Assert.That(accumulator.Next(), Is.EqualTo("B"));
			Assert.That(accumulator.Next(), Is.EqualTo("C"));

			Assert.That(accumulator.Next(), Is.Null);
		}

		[Test]
		public void AccumulatorWithEscapedTextDelimiter()
		{
			var accumulator = new Accumulator("A,\"B,\"\"C\"\"\",D", delimiter: ',') {TextDelimiter = '"'};
			Assert.That(accumulator.Next(), Is.EqualTo("A"));
			Assert.That(accumulator.Next(), Is.EqualTo("B,\"C\""));
			Assert.That(accumulator.Next(), Is.EqualTo("D"));

			Assert.That(accumulator.Next(), Is.Null);
		}

		[Test]
		public void AccumulatorWithRequiredTextDelimiter()
		{
			var accumulator = new Accumulator("A,\"B,C\",D", delimiter: ',') {TextDelimiter = '"'};
			Assert.That(accumulator.Next(), Is.EqualTo("A"));
			Assert.That(accumulator.Next(), Is.EqualTo("B,C"));
			Assert.That(accumulator.Next(), Is.EqualTo("D"));

			Assert.That(accumulator.Next(), Is.Null);
		}

		[Test]
		public void AccumulatorWithTextDelimiter()
		{
			var accumulator = new Accumulator("A,\"B\",C", delimiter: ',') {TextDelimiter = '"'};
			Assert.That(accumulator.Next(), Is.EqualTo("A"));
			Assert.That(accumulator.Next(), Is.EqualTo("B"));
			Assert.That(accumulator.Next(), Is.EqualTo("C"));

			Assert.That(accumulator.Next(), Is.Null);
		}

		[Test]
		public void DecodeAsbImport()
		{
			var filename = TestSettings.GetFilename(ASB_FILENAME);
			using (var reader = new CsvReader(filename))
			{
				reader.GetHeader(skip: 6);
				AsbCsvImportRow actual = reader.GetData<AsbCsvImportRow>().First();
				Assert.That(actual.Date, Is.EqualTo(DateTime.Parse("2012/08/05")));
				Assert.That(actual.UniqueId, Is.EqualTo("2012080501"));
				Assert.That(actual.Amount, Is.EqualTo(-3.80M));
				Assert.That(actual.TranType, Is.EqualTo("TFR OUT"));
			}
		}

		[Test]
		public void DecodeData()
		{
			MemoryStream csv = BuildCsv("A,B,C", "1,2,3");
			using (var reader = new CsvReader(csv))
			{
				IEnumerable<string> header = reader.GetHeader();
				Assert.That(header, Is.EqualTo(new[] {"A", "B", "C"}));

				IEnumerable<string[]> data = reader.GetData();
				Assert.That(data, Is.EquivalentTo(new[] {new[] {"1", "2", "3"}}));
			}
		}

		[Test]
		public void DecodeDataAsObject()
		{
			MemoryStream csv = BuildCsv("A,B,C", "1,2,3");
			using (var reader = new CsvReader(csv))
			{
				IEnumerable<string> header = reader.GetHeader();
				Assert.That(header, Is.EqualTo(new[] {"A", "B", "C"}));

				TestObj actual = reader.GetData<TestObj>().First();
				Assert.That(actual.A, Is.EqualTo(1));
				Assert.That(actual.B, Is.EqualTo(2));
				Assert.That(actual.C, Is.EqualTo(3));
			}
		}

		[Test]
		public void DecodeHeader()
		{
			MemoryStream csv = BuildCsv("A,B,C");
			using (var reader = new CsvReader(csv))
			{
				IEnumerable<string> header = reader.GetHeader();
				Assert.That(header, Is.EqualTo(new[] {"A", "B", "C"}));
			}
		}

		[Test]
		public void DecodeHeaderWithSpacesAsObject()
		{
			MemoryStream csv = BuildCsv("A Value,B*Value,C%Value", "A1,B2,C3");
			using (var reader = new CsvReader(csv))
			{
				TestObj actual = reader.GetData<TestObj>().First();
				Assert.That(actual.AValue, Is.EqualTo("A1"));
				Assert.That(actual.BValue, Is.EqualTo("B2"));
				Assert.That(actual.CValue, Is.EqualTo("C3"));
			}
		}

		[Test]
		public void DecodeWestpacImport()
		{
			var filename = TestSettings.GetFilename(WP_FILENAME);
			using (var reader = new CsvReader(filename))
			{
				WestpacCsvImportRow actual = reader.GetData<WestpacCsvImportRow>().First();
				Assert.That(actual.Date, Is.EqualTo(DateTime.Parse("13/08/2012")));
				Assert.That(actual.Amount, Is.EqualTo(-140M));
				Assert.That(actual.OtherParty, Is.EqualTo("Countdown Ri WBC ATM"));
			}
		}

		[Test]
		public void GetLines()
		{
			string l1 = "LINE 1";
			string l2 = "LINE 2";

			MemoryStream csv = BuildCsv(l1, l2);

			using (var reader = new CsvReader(csv))
			{
				IEnumerable<string> lines = reader.GetLines();
				Assert.That(lines, Is.EqualTo(new[] {l1, l2}));
			}
		}
	}
}