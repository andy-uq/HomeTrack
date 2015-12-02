using System;
using AutoMapper;
using FluentAssertions;
using HomeTrack.Tests;
using HomeTrack.Web.ViewModels;
using NUnit.Framework;

namespace HomeTrack.Web.Tests
{
	[TestFixture]
	public class ViewMappingTests
	{
		private IMappingEngine _mappingEngine;
		private Account _bank, _mortgage;

		[SetUp]
		public void SetUp()
		{
			DateTimeServer.SetLocal(new TestDateTimeServer(DateTime.Now));

			_bank = AccountFactory.Asset("Bank");
			_mortgage = AccountFactory.Asset("Mortgage");

			var mapping = new MappingProvider(new[] {new ViewModelTypeMapProvider()});
			_mappingEngine = mapping.Build();
		}

		[Test]
		public void Transaction()
		{
			var t1 = new Transaction(_mortgage, _bank, 100M) { Description = "Description" };
			var model = _mappingEngine.Map<ViewModels.TransactionDetails>(t1);

			model.Amount.Should().Be(100M);
			model.Date.Should().Be(DateTimeServer.Now);
			model.Description.Should().Be("Description");
		}
	}
}