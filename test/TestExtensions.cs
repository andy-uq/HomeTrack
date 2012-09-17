using Newtonsoft.Json;

namespace HomeTrack.Tests
{
	public static class TestExtensions
	{
		public static string Dump<T>(this T objectGraph)
		{
			return JsonConvert.SerializeObject(objectGraph, Formatting.Indented);
		}
	}
}