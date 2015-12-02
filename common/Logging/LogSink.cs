using System;
using Serilog;

namespace HomeTrack.Logging
{
	public static class LogSink
	{
		public delegate LoggerConfiguration Configuration(LoggerConfiguration config);

		public static Configuration Configure<T>(T settings, Func<T, LoggerConfiguration, LoggerConfiguration> configure)
		{
			return config => configure(settings, config);
		}

		public static Configuration Configure(Func<LoggerConfiguration, LoggerConfiguration> configure)
		{
			return config => configure(config);
		}
	}

	
}