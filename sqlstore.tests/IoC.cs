using System.Data.SqlClient;
using Autofac;
using FixieShim.Fixie;
using HomeTrack.Core;
using HomeTrack.Ioc;
using HomeTrack.Logging;
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

			builder.RegisterFeature<SqlStoreFeature>();
			builder.RegisterFeature<TestLogging>();

			builder.RegisterType<TestDatabase>()
				.OnActivated(args => args.Instance.ApplyMigrations())
				.InstancePerLifetimeScope();

			builder.Register(resolver => resolver.Resolve<TestDatabase>().Database);

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