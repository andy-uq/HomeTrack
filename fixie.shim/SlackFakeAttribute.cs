using System;

namespace FixieShim
{
	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class SlackFakeAttribute : Attribute
	{
	}
}