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
	}
}