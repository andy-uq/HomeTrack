using System.Collections.Generic;
using System.Reflection;
using Fixie;
using Fixie.Execution;
using Fixie.Internal;

namespace FixieShim.Fixie
{
	public class ExecutionProxy : LongLivedMarshalByRefObject
	{
		public IReadOnlyList<MethodGroup> DiscoverTestMethodGroups(string assemblyFullPath, Options options)
		{
			var assembly = LoadAssembly(assemblyFullPath);
			return DiscoverTestMethodGroups(assembly, options);
		}

		public AssemblyResult RunMethods(string assemblyFullPath, Options options, Listener listener, MethodGroup[] methodGroups)
		{
			var assembly = LoadAssembly(assemblyFullPath);
			return RunMethods(assembly, options, listener, methodGroups);
		}

		public IReadOnlyList<MethodGroup> DiscoverTestMethodGroups(Assembly assembly, Options options)
		{
			return new Discoverer(options).DiscoverTestMethodGroups(assembly);
		}

		public AssemblyResult RunMethods(Assembly assembly, Options options, Listener listener, MethodGroup[] methodGroups)
		{
			return Runner(options, listener).RunMethods(assembly, methodGroups);
		}

		private static Assembly LoadAssembly(string assemblyFullPath)
		{
			return Assembly.Load(AssemblyName.GetAssemblyName(assemblyFullPath));
		}

		private static Runner Runner(Options options, Listener listener)
		{
			return new Runner(listener, options);
		}
	}
}