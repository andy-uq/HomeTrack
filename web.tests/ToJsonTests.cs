using HomeTrack.Tests;
using NUnit.Framework;
using Newtonsoft.Json;

namespace HomeTrack.Web.Tests
{
	[TestFixture]
	public class ToJsonTests
	{
		[Test]
		public void CanSerialise()
		{
			var account = AccountFactory.Asset("bank");
			Assert.That(account.ToJson(), Is.EqualTo(@"{
  ""Id"": ""bank"",
  ""Name"": ""bank"",
  ""Description"": null,
  ""Type"": 1,
  ""Direction"": 1,
  ""Balance"": 0.0
}"));
		}
	}
}