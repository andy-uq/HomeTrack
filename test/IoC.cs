using Autofac;
using FixieShim.Fixie;
using HomeTrack.Core;
using HomeTrack.Ioc;
using HomeTrack.Logging;
using HomeTrack.Tests;
using Serilog;

namespace HomeTrack.SqlStore.Tests
{
	[IocCompositionRoot]
	public static class IoC
	{
		public static ContainerBuilder GetCompositionRoot()
		{
			var builder = new ContainerBuilder();
			builder.RegisterFeature<ApplicationFeature>();

			builder.RegisterFeature<TestLogging>();

			builder.RegisterType<InMemoryRepository>()
				.AsImplementedInterfaces();

			builder.RegisterType<TestData.AccountLookup>()
				.As<IAccountLookup>();

			return builder;
		}

		private class TestLogging : IFeatureRegistration
		{
			public void Register(ContainerBuilder builder)
			{
				builder.RegisterFeature<LoggingFeature>();
				builder.Register(resolver => LogSink.Configure(ConfigureTestLogging));
			}

			private LoggerConfiguration ConfigureTestLogging(LoggerConfiguration config)
			{
				return config.MinimumLevel.Verbose();
			}
		}
	}
}