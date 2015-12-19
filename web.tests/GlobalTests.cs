using System;
using System.Collections;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using Autofac;
using FluentAssertions;
using HomeTrack;
using HomeTrack.Core;
using HomeTrack.Ioc;
using HomeTrack.Web;
using Moq;
using NUnit.Framework;

namespace web.tests
{
	[TestFixture]
	public class GlobalTests
	{
		private class StubMvcApplication : MvcApplication
		{
			public StubMvcApplication() : base(path => path)
			{
			}

			protected override void RegisterDataProvider(ContainerBuilder builder)
			{
				builder.Register(resolver => new System.Data.SqlClient.SqlConnection(""));
			}
		}

		[Test]
		public void Start()
		{
			var global = new StubMvcApplication();
			global.Start(new GlobalFilterCollection(), new RouteCollection());
		}

		[Test]
		public void ResolveIoc()
		{
			var global = new StubMvcApplication();
			global.Start(new GlobalFilterCollection(), new RouteCollection());
			global.Container.Resolve<IMappingEngine>();
			global.Container.Resolve<AsyncGeneralLedger>();
			global.Container.Resolve<DirectoryExplorer>();
			global.Container.Resolve<ImportDetector>();
			global.Container.Resolve<TransactionImportContext>();
		}

		[TestCase("~/", "Home", "Index", null)]
		[TestCase("~/account", "Account", "Index", null)]
		[TestCase("~/transaction", "Transaction", "Index", null)]
		[TestCase("~/transaction/detail/1", "Transaction", "Detail", "1")]
		public void RoutesMatch(string url, string controller, string action, string id)
		{
			// Arrange
			var routes = new RouteCollection();
			MvcApplication.RegisterRoutes(routes);

			// Act
			var context = GetMockHttpContext(url);
			RouteData routeData = routes.GetRouteData(context);

			// Assert
			AssertRouteData(routeData, controller, action, id);
		}

		private HttpContextBase GetMockHttpContext(string s)
		{
			return MvcMockHelpers.FakeHttpContext(s);
		}

		private void AssertRouteData(RouteData routeData, string controller, string action, string id = null)
		{
			routeData.Should().NotBeNull();
			routeData.Values["controller"].Should().Match(_ => StringComparer.OrdinalIgnoreCase.Equals(_, controller));
			routeData.Values["action"].Should().Match(_ => StringComparer.OrdinalIgnoreCase.Equals(_, action));
			routeData.Values["id"].Should().Be((object) id ?? UrlParameter.Optional);
		}
	}
}