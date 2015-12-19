using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace HomeTrack.Mapping
{
	public static class MappingExtensions
	{
		private static IMappingEngine _mappingEngine;

		public static void SetMappingEngine(IMappingEngine mappingEngine)
		{
			if (_mappingEngine == null)
			{
				_mappingEngine = mappingEngine;
			}
		}

		public static T Map<T>(this object source)
		{
			if (_mappingEngine == null)
				throw new InvalidOperationException("Mapping engine is not initialised");

			return _mappingEngine.Map<T>(source);
		}

		public static TTo Map<TFrom, TTo>(this TFrom source, TTo dest)
		{
			if (_mappingEngine == null)
				throw new InvalidOperationException("Mapping engine is not initialised");

			return _mappingEngine.Map(source, dest);
		}

		public static IEnumerable<TTo> MapAll<TTo>(this IEnumerable source)
		{
			return
				from object o in source
				select _mappingEngine.Map<TTo>(o);
		}
	}
}