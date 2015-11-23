using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using AutoMapper;
using AutoMapper.Mappers;
using HomeTrack.Collections;
using HomeTrack.Ioc;

namespace HomeTrack.Mapping
{
	public class MappingFeature : IFeatureRegistration
	{
		public void Register(ContainerBuilder builder)
		{
			builder.RegisterType<TypeMapFactory>()
				.AsImplementedInterfaces();

			builder.RegisterType<ConfigurationStore>()
				.WithNamedParameter("mappers", GetObjectMappers)
				.OnActivated(RegisterMappingProviders)
				.AsImplementedInterfaces()
				.SingleInstance();

			builder.RegisterType<MappingEngine>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder.RegisterType<MappingActivator>()
				.As<IStartable>();

			builder.RegisterType<TypeMappings>()
				.As<ICustomMapping>();
		}

		private object GetObjectMappers(IComponentContext resolveContext)
		{
			var mappers = new List<IObjectMapper>(resolveContext.Resolve<IEnumerable<IObjectMapper>>());
			//mappers.AddRange(MapperRegistry.Mappers);

			return mappers;
		}

		private void RegisterMappingProviders(IActivatedEventArgs<ConfigurationStore> obj)
		{
			var config = obj.Instance;

			var customMappings = obj.Context.Resolve<IEnumerable<ICustomMapping>>();
			foreach (var mappingProvider in customMappings)
			{
				mappingProvider.Configure(config);
				System.Diagnostics.Trace.WriteLine($"Registered custom mapping provider: {mappingProvider.GetType()}");
			}
		}

		private class MappingActivator : IStartable
		{
			private readonly IMappingEngine _mappingEngine;

			public MappingActivator(IMappingEngine mappingEngine)
			{
				_mappingEngine = mappingEngine;
			}

			public void Start()
			{
				MappingExtensions.SetMappingEngine(_mappingEngine);
			}
		}
	}

	public static class MappingRegistration
	{
		public static void RegisterMappings(this ContainerBuilder builder, Assembly assembly, params Assembly[] additionalAssemblies)
		{
			var assemblies = assembly.AsEnumerable().Concat(additionalAssemblies).ToArray();

			builder.RegisterAssemblyTypes(assemblies)
				.AssignableTo<IObjectMapper>()
				.AsImplementedInterfaces();

			builder.RegisterAssemblyTypes(assemblies)
				.AssignableTo<ICustomMapping>()
				.AsImplementedInterfaces();
		}
	}
}