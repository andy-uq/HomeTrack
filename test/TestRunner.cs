using System.Collections.Generic;
using Fixie;
using FixieShim;
using FixieShim.Fixie;
using FixieShim.NUnit;
using NUnit.Framework;

namespace HomeTrack.SqlStore.Tests
{
	[TestFixture]
	public class TestRunner : TestAssembly
	{
		public TestRunner()
		{
			Apply<TestcaseClassPerClassConvention>();
			Apply<TestcaseClassPerFixtureConvention>();

			TestAssemblyFixture.SetAssembly(typeof (TestRunner).Assembly);
		}

		private IEnumerable<TestCaseData> GetTests()
		{
			return FixieTestFinder.GetTests();
		}

		[TestCaseSource(typeof (TestRunner), nameof(GetTests))]
		public void Fixie(string fullName)
		{
			NUnitWrapper.Fixie(fullName);
		}
	}
}