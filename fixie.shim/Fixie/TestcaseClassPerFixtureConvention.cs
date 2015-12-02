using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Autofac;
using Fixie;
using FixieShim.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Fixture = Ploeh.AutoFixture.Fixture;

namespace FixieShim.Fixie
{
	public class TestcaseClassPerFixtureConvention : Convention
	{
		private readonly SlowTestsCustomization _customisation;

		public TestcaseClassPerFixtureConvention()
		{
			var fixture = new SlowTestFixture();
			_customisation = new SlowTestsCustomization(fixture, fixture.Container);

			Classes
				.NameEndsWith("Tests")
				.Where(t =>
					t.GetConstructors().Count() == 1
					&& t.GetConstructors().Count(ci => ci.GetParameters().Length > 0) == 1
				);

			Methods
				.Where(mi =>
					mi.IsPublic
					&& (mi.IsVoid()
					    || mi.IsAsync() && mi.Name.EndsWith("Async"))
				);

			ClassExecution
				.CreateInstancePerClass()
				.UsingFactory(CreateFromFixture);
		}

		private object CreateFromFixture(Type type)
		{
			var fixture = new Fixture();
			_customisation.Customize(fixture);

			try
			{
				return new SpecimenContext(fixture).Resolve(type);
			}
			catch (TargetInvocationException t)
			{
				ExceptionDispatchInfo.Capture(t.InnerException).Throw();
				throw;
			}
		}
	}
}