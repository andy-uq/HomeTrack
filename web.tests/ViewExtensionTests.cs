using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using FluentAssertions;
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
			10M.AsAmount().Should().Be("10.00");
			Assert.That(((decimal?)null).AsAmount(), Is.Empty);
		}		
		
		[Test]
		public void Format()
		{
			DateTime.Parse("2012-1-1").Format().Should().Be("1 Jan");
			DateTime.Parse("2012-1-1").Format(fullDate:true).Should().Be("1 Jan, 2012");
			var dateTime = DateTime.Parse("2012-1-1 9:30");
			dateTime.Format(fullDate:true, showTime:true).Should().Be(dateTime.ToString("d MMM, yyyy, h:mm tt"));
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
			items[0].Text.Should().Be("1");
			items[0].Value.Should().Be("A");
			Assert.That(items[0].Selected, Is.False);
			
			items[1].Text.Should().Be("2");
			items[1].Value.Should().Be("B");
			items[1].Selected.Should().BeTrue();
		}
	}
}