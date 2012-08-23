using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace HomeTrack.RavenStore
{
	public static class MappingExtensions
	{
		public static IEnumerable<T> Hydrate<T>(this IEnumerable<object> enumerable, IMappingEngine mappingEngine)
		{
			return enumerable.Select(mappingEngine.Map<T>);
		}

		public static T Hydrate<T>(this object obj, IMappingEngine mappingEngine)
		{
			return mappingEngine.Map<T>(obj);
		}
	}
}