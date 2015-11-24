using System.Collections.Generic;
using Fixie;
using NUnit.Framework;

namespace FixieShim.Fixie
{
	public static class FixieTestFinder
	{
		public static IEnumerable<TestCaseData> GetTests()
		{
			var executionProxy = new ExecutionProxy();
			var methods = executionProxy.DiscoverTestMethodGroups(TestAssemblyFixture.Assembly, new Options());

			foreach (var method in methods)
				yield return new TestCaseData(method.FullName) {};
		}
	}
}