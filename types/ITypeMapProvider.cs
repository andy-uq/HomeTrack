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

	public class MappingProvider : IEnumerable<ITypeMapProvider>
	{
		private readonly HashSet<ITypeMapProvider> _providers;

		public MappingProvider()
		{
			_providers = new HashSet<ITypeMapProvider>();
		}

		public MappingProvider(IEnumerable<ITypeMapProvider> providers)
			: this()
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

		public IEnumerator<ITypeMapProvider> GetEnumerator()
		{
			return _providers.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}