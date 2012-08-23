using System;

namespace HomeTrack
{
	public static class DateTimeServer
	{
		private static IDateTimeServer _global = new RealTimeServer();
		private class RealTimeServer : IDateTimeServer
		{
			DateTime IDateTimeServer.Now
			{
				get { return DateTime.Now; }
			}
		}

		[ThreadStatic]
		private static IDateTimeServer _local;

		public static DateTime Now
		{
			get { return (_local ?? _global).Now; }
		}

		public static void SetGlobal(IDateTimeServer dateTimeServer)
		{
			_global = dateTimeServer;
		}

		public static void SetLocal(IDateTimeServer dateTimeServer)
		{
			_local = dateTimeServer;
		}
	}

	public interface IDateTimeServer
	{
		DateTime Now { get; }
	}
}