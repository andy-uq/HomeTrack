using System.Reflection;
using Autofac;
using HomeTrack.Ioc;
using HomeTrack.Logging;
using HomeTrack.Mapping;
using NUnit.Framework;
using Serilog;

namespace HomeTrack.SqlStore.Tests
{
	[SetUpFixture]
	public class AssemblyContext
	{
		private IContainer _container;

		[SetUp]
		public void OnAssemblyStart()
		{
			var builder = new ContainerBuilder();
			builder.RegisterFeature<MappingFeature>();
			//builder.RegisterMappings(Assembly.GetAssembly(typeof(Models.Tracking.RawTrackingEvent)));

			builder.RegisterFeature<TestLogging>();

			_container = builder.Build();
		}

		[TearDown]
		public void OnAssemblyComplete()
		{
			_container.Dispose();
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