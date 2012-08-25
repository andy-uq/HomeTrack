using System.Collections;
using System.Collections.Generic;
using AutoMapper;
using AutoMapper.Mappers;

namespace HomeTrack
{
	public interface ITypeMapProvider
	{
		void RegisterTypeMaps(ConfigurationStore map);
	}

	public class MappingProvider
	{
		private readonly HashSet<ITypeMapProvider> _providers;

		public MappingProvider()
		{
			_providers = new HashSet<ITypeMapProvider>();
		}

		public MappingProvider(params ITypeMapProvider[] providers) : this()
		{
			foreach ( var provider in providers )
				Add(provider);
		}

		public void Add(ITypeMapProvider typeMapProvider)
		{
			_providers.Add(typeMapProvider);
		}

		public IMappingEngine Build()
		{
			var map = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.AllMappers());
			foreach (var provider in _providers)
				provider.RegisterTypeMaps(map);

			return new MappingEngine(map);
		}
	}
}