using System;
using System.Threading;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class DateTimeServerTests
	{
		[Test]
		public void DateTimeServerReturnsArbritaryTime()
		{
			var t1 = new TestDateTimeServer(now: DateTime.Now);
			DateTimeServer.SetLocal(null);
			DateTimeServer.SetGlobal(t1);

			Assert.That(DateTimeServer.Now, Is.EqualTo(t1.Now));
		}

		[Test]
		public void TheadGlobalTimeServerIsShared()
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

			Assert.That(g2.WaitOne(20), Is.True);
			g2.Reset();
			g1.Set();
			Assert.That(g2.WaitOne(20), Is.True);

			Assert.That(t1Date, Is.Not.Null);
			Assert.That(t1Date, Is.EqualTo(ts2.Now));
		}

		[Test]
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

			var t2 = new Thread(() =>
			{
				DateTimeServer.SetLocal(ts2);
				g2.Set();
				g1.WaitOne();
			});

			t1.Start();
			t2.Start();

			Assert.That(g2.WaitOne(20), Is.True);
			g2.Reset();
			g1.Set();
			Assert.That(g2.WaitOne(20), Is.True);

			Assert.That(t1Date, Is.Not.Null);
			Assert.That(t1Date, Is.EqualTo(ts1.Now));
		}
	}

	public class TestDateTimeServer : IDateTimeServer
	{
		private DateTime? _now;
		private readonly Func<DateTime> _nowFunc;

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