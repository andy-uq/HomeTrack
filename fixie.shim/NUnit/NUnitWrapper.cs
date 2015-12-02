using System;
using Fixie;
using Fixie.Execution;
using NUnit.Framework;

namespace FixieShim.NUnit
{
	public class NUnitWrapper
	{
		public static void Fixie(string fullName)
		{
			var methodGroup = new MethodGroup(fullName);
			var executionProxy = new Fixie.ExecutionProxy();
			var errorListener = new NUnitListener();
			var result = executionProxy.RunMethods(TestAssemblyFixture.Assembly, new Options(), errorListener, new[] { methodGroup });

			Report(result, errorListener);
		}

		public static void Report(AssemblyResult result, NUnitListener errorListener)
		{
			Console.WriteLine(result.Summary);

			if (errorListener.HasError)
			{
				Assert.Fail(errorListener.FailResult.Exceptions.CompoundStackTrace);
			}
		}
	}
}