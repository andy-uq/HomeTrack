using System;
using System.Collections.Generic;
using Autofac;

namespace HomeTrack.Core
{
	public class PatternBuilder
	{
		private readonly Func<Dictionary<string, string>, IPattern> _build;

		public string Name { get; set; }
		public List<string> Properties { get; set; }

		private PatternBuilder(string name, IEnumerable<string> properties, Func<Dictionary<string, string>, IPattern> build)
		{
			Name = name;
			Properties = new List<string>(properties);
			_build = build;
		}

		public static void Register(ContainerBuilder builder)
		{
			foreach ( var b in GetPatterns() )
				builder.RegisterInstance(b);
		}

		public static IEnumerable<PatternBuilder> GetPatterns()
		{
			yield return new PatternBuilder("Amount", new[] {"Amount"}, p => new AmountPattern { Amount = Convert.ToDecimal(p["Amount"]) });
			yield return new PatternBuilder("Amount Range", new[] {"Min","Max"}, p => new AmountRangePattern { Min = Convert.ToDecimal(p["Min"]), Max = Convert.ToDecimal(p["Min"]) });
			yield return new PatternBuilder("Field", new[] {"Name","Pattern"}, p => new FieldPattern { Name = p["Name"], Pattern = p["Pattern"] });
		}

		public IPattern Build(Dictionary<string, string> properties)
		{
			return _build(properties);
		}
	}
}