using System;
using Autofac;
using Ploeh.AutoFixture.Kernel;

namespace FixieShim.AutoFixture
{
	public class AutofacSpecimenContainerBuilder : ISpecimenBuilder
	{
		private readonly ILifetimeScope _container;

		public AutofacSpecimenContainerBuilder(ILifetimeScope container)
		{
			_container = container;
		}

		public object Create(object request, ISpecimenContext context)
		{
			var type = request as Type;

			if (type == null || type.IsPrimitive)
			{
				return new NoSpecimen();
			}

			object instance;
			_container.TryResolve(type, out instance);

			return instance ?? new NoSpecimen();
		}
	}
}