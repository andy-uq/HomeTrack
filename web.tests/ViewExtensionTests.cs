using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using HomeTrack.Web.ViewModels;
using NUnit.Framework;

namespace HomeTrack.Web.Tests
{
	[TestFixture]
	public class ViewExtensionTests
	{
		[Test]	 
		public void AsAmount()
		{
			Assert.That(10M.AsAmount(), Is.EqualTo("10.00"));
			Assert.That(((decimal?)null).AsAmount(), Is.Empty);
		}		
		
		[Test]
		public void Format()
		{
			Assert.That(DateTime.Parse("2012-1-1").Format(), Is.EqualTo("1 Jan"));
			Assert.That(DateTime.Parse("2012-1-1").Format(fullDate:true), Is.EqualTo("1 Jan, 2012"));
			Assert.That(DateTime.Parse("2012-1-1 9:30").Format(fullDate:true, showTime:true), Is.EqualTo("1 Jan, 2012, 9:30 a.m."));
		}

		[Test]
		public void AsSelectList()
		{
			var list = new[]
			{
				new Tuple<string, int>("A", 1), 
				new Tuple<string, int>("B", 2),
			};

			var items = list.AsSelectList(v => v.Item1, t => t.Item2.ToString(CultureInfo.InvariantCulture), s => s.Item2 == 2).ToArray();
			Assert.That(items[0].Text, Is.EqualTo("1"));
			Assert.That(items[0].Value, Is.EqualTo("A"));
			Assert.That(items[0].Selected, Is.False);
			
			Assert.That(items[1].Text, Is.EqualTo("2"));
			Assert.That(items[1].Value, Is.EqualTo("B"));
			Assert.That(items[1].Selected, Is.True);
		}
	}
}