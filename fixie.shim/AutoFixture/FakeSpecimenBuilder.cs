using System;
using System.Reflection;
using Autofac;
using FakeItEasy;
using Ploeh.AutoFixture.Kernel;

namespace FixieShim.AutoFixture
{
	public class FakeSpecimenBuilder : ISpecimenBuilder
	{
		private readonly IContainer _container;

		public FakeSpecimenBuilder(IContainer container)
		{
			_container = container;
		}

		public object Create(object request, ISpecimenContext context)
		{
			var paramInfo = request as ParameterInfo;

			if (paramInfo == null)
				return new NoSpecimen();

			var fake = paramInfo.GetCustomAttribute<SlackFakeAttribute>();
			if (fake != null)
			{
				return CreateFake(paramInfo);
			}

			var strict = paramInfo.GetCustomAttribute<StrictFakeAttribute>();
			if (strict != null)
			{
				return CreateStrict(paramInfo);
			}

			return new NoSpecimen();
		}

		private object CreateStrict(ParameterInfo paramInfo)
		{
			var method = typeof (FakeSpecimenBuilder)
				.GetMethod(nameof(CreateStrictFunc), Type.EmptyTypes)
				.MakeGenericMethod(paramInfo.ParameterType);

			var fake = method.Invoke(null, null);

			var containerBuilder = new ContainerBuilder();
			containerBuilder.Update(_container);
			containerBuilder.RegisterInstance(fake).As(paramInfo.ParameterType);

			return fake;
		}

		private object CreateFake(ParameterInfo paramInfo)
		{
			var method = typeof (A)
				.GetMethod(nameof(A.Fake), Type.EmptyTypes)
				.MakeGenericMethod(paramInfo.ParameterType);

			var fake = method.Invoke(null, null);

			var containerBuilder = new ContainerBuilder();
			containerBuilder.Update(_container);
			containerBuilder.RegisterInstance(fake).As(paramInfo.ParameterType);

			return fake;
		}

		public static T CreateStrictFunc<T>()
		{
			return A.Fake<T>(x => x.Strict());
		}
	}
}