using System.Collections.Generic;
using System.Linq;
using Fixie;

namespace FixieShim.Fixie
{
	public static class FixieTestFinder
	{
		public static IEnumerable<string> GetTests()
		{
			var executionProxy = new ExecutionProxy();
			var methods = executionProxy.DiscoverTestMethodGroups(TestAssemblyFixture.Assembly, new Options());

			return methods.Select(method => method.FullName);
		}
	}
}