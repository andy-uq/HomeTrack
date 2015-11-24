using System;
using Fixie.Execution;

namespace FixieShim.NUnit
{
	public class NUnitListener : LongLivedMarshalByRefObject, Listener
	{
		public void AssemblyStarted(AssemblyInfo assembly)
		{
		}

		public void CaseSkipped(SkipResult result)
		{
			Console.WriteLine("Test '{0}' skipped{1}", result.Name, result.SkipReason == null ? null : ": " + result.SkipReason);
		}

		public void CasePassed(PassResult result)
		{
		}

		public void CaseFailed(FailResult result)
		{
			FailResult = result;

			Console.WriteLine("Test '{0}' failed: {1}", result.Name, result.Exceptions.PrimaryException.DisplayName);
			Console.WriteLine(result.Exceptions.CompoundStackTrace);
			Console.WriteLine();
		}

		public bool HasError => FailResult != null;
		public FailResult FailResult { get; set; }

		public void AssemblyCompleted(AssemblyInfo assembly, AssemblyResult result)
		{
		}
	}
}