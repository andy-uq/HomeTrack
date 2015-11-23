using System;
using System.Linq;
using Autofac;
using HomeTrack.Ioc;
using Serilog;
using Serilog.Events;

namespace HomeTrack.Logging
{
	public sealed class LoggingFeature : IFeatureRegistration
	{
		public void Register(ContainerBuilder builder)
		{
			builder.Register(CreateLoggingConfiguration)
				.SingleInstance();

			builder.Register(CreateLogger)
				.SingleInstance();

			builder.RegisterType<LoggingActivator>()
				.As<IStartable>();
		}

		private ILogger CreateLogger(IComponentContext resolver)
		{
			var configurations = resolver.ResolveAll<LogSink.Configuration>();
			var baseConfiguration = resolver.Resolve<LoggerConfiguration>();

			return configurations.Aggregate(baseConfiguration, (_, configure) => configure(_)).CreateLogger();
		}

		private LoggerConfiguration CreateLoggingConfiguration(IComponentContext resolver)
		{
			var config = new LoggerConfiguration()
				.WriteTo.Trace();

			if (Environment.UserInteractive)
			{
				config = config
					.WriteTo.ColoredConsole(restrictedToMinimumLevel: LogEventLevel.Information);
			}

			return config;
		}

		private class LoggingActivator : IStartable
		{
			private readonly ILogger _logger;

			public LoggingActivator(ILogger logger)
			{
				_logger = logger;
			}

			public void Start()
			{
				Log.Logger = _logger;
			}
		}
	}
}