using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HomeTrack.Collections
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> AsEnumerable<T>(this T value)
		{
			return Enumerable.Repeat(value, 1);
		}

		public static IList<T> AsList<T>(this IEnumerable<T> source)
		{
			return (source as IList<T>) ?? source.ToList();
		}

		public static ISet<T> AsSet<T>(this IEnumerable<T> source)
		{
			return (source as ISet<T>) ?? new HashSet<T>(source);
		}
	}
}