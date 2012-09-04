using Newtonsoft.Json;

namespace HomeTrack.Web
{
	public static class JsonExtensions
	{
		public static string ToJson<T>(this T objectGraph)
		{
			return JsonConvert.SerializeObject(objectGraph, Formatting.Indented);
		}
	}
}