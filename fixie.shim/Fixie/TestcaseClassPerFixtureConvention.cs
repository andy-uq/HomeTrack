using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
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
				.Where(t => !t.IsAbstract && t.GetConstructors().All(c => c.GetParameters().Length > 1));

			Methods
				.Where(mi =>
					mi.IsPublic
					&& (mi.IsVoid() || mi.IsAsync())
					&& mi.Name != "InitialiseAsync"
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
				var instance = new SpecimenContext(fixture).Resolve(type);

				var asyncTest = instance as IAsyncTest;
				if (asyncTest != null)
				{
					var task = asyncTest.InitialiseAsync();
					var awaiter = task.GetAwaiter();
					awaiter.GetResult();
				}

				return instance;
			}
			catch (TargetInvocationException t)
			{
				ExceptionDispatchInfo.Capture(t.InnerException).Throw();
				throw;
			}
		}
	}
}