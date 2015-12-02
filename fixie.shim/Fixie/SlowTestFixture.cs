using System;
using System.Linq.Expressions;
using System.Reflection;
using Autofac;
using FakeItEasy;

namespace FixieShim.Fixie
{
	public class SlowTestFixture
	{
		private static Lazy<Func<ContainerBuilder>> _iocCompositionRoot = new Lazy<Func<ContainerBuilder>>(FindIocCompositionRoot);

		private static Func<ContainerBuilder> FindIocCompositionRoot()
		{
			if (TestAssemblyFixture.Assembly != null)
			{
				foreach (var type in TestAssemblyFixture.Assembly.ExportedTypes)
				{
					var iocCompositionRoot = type.GetCustomAttribute<IocCompositionRootAttribute>();
					if (iocCompositionRoot == null)
						continue;

					return Expression.Lambda<Func<ContainerBuilder>>(Expression.Call(type.GetMethod(iocCompositionRoot.Method, Type.EmptyTypes))).Compile();
				}
			}

			return () => new ContainerBuilder();
		}

		public static Func<ContainerBuilder> IocCompositionRoot
		{
			get { return _iocCompositionRoot.Value; }
			set
			{
				if (_iocCompositionRoot.IsValueCreated)
					throw new InvalidOperationException($"${nameof(IocCompositionRoot)} can only be set once.  You cannot change roots, maybe you should have another test project?");

				_iocCompositionRoot = new Lazy<Func<ContainerBuilder>>(() => value);
			}
		}

		public SlowTestFixture()
		{
			if (_iocCompositionRoot == null)
				throw new InvalidOperationException($"You must set the global static ${nameof(IocCompositionRoot)}");

			Container = IocCompositionRoot().Build();
		}

		public IContainer Container { get; }
	}
}