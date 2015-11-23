using System;
using Autofac;

namespace HomeTrack.Ioc
{
	public interface IFeatureRegistration
	{
		void Register(ContainerBuilder builder);
	}

	public static class FeatureRegistrationExtensions
	{
		public static ContainerBuilder RegisterFeature(this ContainerBuilder containerBuilder, IFeatureRegistration dependencyRegistration)
		{
			dependencyRegistration.Register(containerBuilder);
			return containerBuilder;
		}

		public static ContainerBuilder RegisterFeature<T>(this ContainerBuilder containerBuilder) where T : IFeatureRegistration
		{
			var feature = (IFeatureRegistration)Activator.CreateInstance<T>();
			feature.Register(containerBuilder);

			return containerBuilder;
		}

		public static ContainerBuilder RegisterFeature<T>(this ContainerBuilder containerBuilder, params object[] constructorArgs) where T : IFeatureRegistration
		{
			var feature = (IFeatureRegistration)Activator.CreateInstance(typeof(T), constructorArgs);
			feature.Register(containerBuilder);

			return containerBuilder;
		}
	}
}