using System;
using System.Threading;
using FluentAssertions;

namespace HomeTrack.Tests
{
	public class DateTimeServerTests
	{
		public void DateTimeServerReturnsArbritaryTime()
		{
			var t1 = new TestDateTimeServer(now: DateTime.Now);
			DateTimeServer.SetLocal(null);
			DateTimeServer.SetGlobal(t1);

			DateTimeServer.Now.Should().Be(t1.Now);
		}

		public void ThreadGlobalTimeServerIsShared()
		{
			var ts1 = new TestDateTimeServer(now: new DateTime(2011, 1, 1));
			var ts2 = new TestDateTimeServer(now: new DateTime(2012, 1, 1));

			var g1 = new ManualResetEvent(false);
			var g2 = new ManualResetEvent(false);

			DateTime? t1Date = null;

			var t1 = new Thread(() => {
				DateTimeServer.SetGlobal(ts1);
				g1.WaitOne();
				t1Date = DateTimeServer.Now;
				g2.Set();
			});

			var t2 = new Thread(() => {
				DateTimeServer.SetGlobal(ts2);
				g2.Set();
				g1.WaitOne();
			});

			t1.Start();
			t2.Start();

			g2.WaitOne(20).Should().BeTrue();
			g2.Reset();
			g1.Set();
			g2.WaitOne(20).Should().BeTrue();

			t1Date.Should().HaveValue();
			t1Date.Should().Be(ts2.Now);
		}

		public void TheadLocalTimeServerIsNotShared()
		{
			var ts1 = new TestDateTimeServer(now: new DateTime(2011, 1, 1));
			var ts2 = new TestDateTimeServer(now: new DateTime(2012, 1, 1));

			var g1 = new ManualResetEvent(false);
			var g2 = new ManualResetEvent(false);

			DateTime? t1Date = null;

			var t1 = new Thread(() => {
				DateTimeServer.SetLocal(ts1);
				g1.WaitOne();
				t1Date = DateTimeServer.Now;
				g2.Set();
			});

			var t2 = new Thread(() => {
				DateTimeServer.SetLocal(ts2);
				g2.Set();
				g1.WaitOne();
			});

			t1.Start();
			t2.Start();

			g2.WaitOne(20).Should().BeTrue();
			g2.Reset();
			g1.Set();
			g2.WaitOne(20).Should().BeTrue();

			t1Date.Should().HaveValue();
			t1Date.Should().Be(ts1.Now);
		}
	}

	public class TestDateTimeServer : IDateTimeServer
	{
		private readonly Func<DateTime> _nowFunc;
		private DateTime? _now;

		public TestDateTimeServer(DateTime? now = null, Func<DateTime> nowFunc = null)
		{
			_now = now;
			_nowFunc = nowFunc ?? (() => DateTime.Now);
		}

		public DateTime Now
		{
			get { return _now ?? _nowFunc(); }
		}

		public void NewTime(DateTime nextTime)
		{
			_now = nextTime;
		}

		public void NewTime(Func<DateTime, DateTime> nextTime)
		{
			_now = nextTime(Now);
		}
	}
}