using System.Reflection;

namespace FixieShim
{
	public class TestAssemblyFixture
	{
		public static Assembly Assembly { get; private set; }

		public static void SetAssembly(Assembly assembly)
		{
			Assembly = assembly;
		}
	}
}