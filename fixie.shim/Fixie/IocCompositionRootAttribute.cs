using System;

namespace FixieShim.Fixie
{
	public class IocCompositionRootAttribute : Attribute
	{
		public IocCompositionRootAttribute(string method = "GetCompositionRoot")
		{
			Method = method;
		}

		public string Method { get; }
	}
}