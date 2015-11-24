using System;
using Autofac;
using Ploeh.AutoFixture;

namespace FixieShim.AutoFixture
{
	public class SlowTestsCustomization : ICustomization
	{
		private readonly object _testFixture;
		private readonly IContainer _container;

		public SlowTestsCustomization(object testFixture, IContainer container)
		{
			_testFixture = testFixture;
			_container = container;
		}

		public void Customize(IFixture fixture)
		{
			fixture.Register(() => _testFixture);
			fixture.Customizations.Add(new AutofacSpecimenContainerBuilder(_container));
			fixture.Customizations.Add(new FakeSpecimenBuilder(_container));
		}
	}
}