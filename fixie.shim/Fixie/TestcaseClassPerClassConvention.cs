using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fixie;
using Ploeh.AutoFixture.Kernel;
using Fixture = Ploeh.AutoFixture.Fixture;

namespace FixieShim.Fixie
{
	public class TestcaseClassPerClassConvention : Convention
	{
		public TestcaseClassPerClassConvention()
		{
			Classes
				.NameEndsWith("Tests")
				.Where(t =>
					t.GetConstructors()
						.All(ci => ci.GetParameters().Length == 0)
				);

			Methods
				.Where(mi =>
					mi.IsPublic
					&& (mi.IsVoid()
						|| mi.IsAsync() && mi.Name.EndsWith("Async"))
				);

			Parameters.Add(FillFromFixture);
		}

		private IEnumerable<object[]> FillFromFixture(MethodInfo method)
		{
			var fixture = new Fixture();
			yield return GetParameterData(method.GetParameters(), fixture);
		}

		private object[] GetParameterData(ParameterInfo[] parameters, Fixture fixture)
		{
			return parameters
				.Select(p => new SpecimenContext(fixture).Resolve(p.ParameterType))
				.ToArray();
		}
	}
}