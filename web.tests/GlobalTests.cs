﻿using System;
using System.Collections;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using HomeTrack.Web;
using Moq;
using NUnit.Framework;

namespace web.tests
{
	[TestFixture]
	public class GlobalTests
	{
		[Test]
		public void Start()
		{
			var global = new MvcApplication(p => p);
			global.Start();
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
			Assert.That(routeData, Is.Not.Null);
			Assert.That(routeData.Values["controller"], Is.EqualTo(controller).Using((IEqualityComparer)StringComparer.OrdinalIgnoreCase));
			Assert.That(routeData.Values["action"], Is.EqualTo(action).Using((IEqualityComparer) StringComparer.OrdinalIgnoreCase));
			Assert.That(routeData.Values["id"], Is.EqualTo((object) id ?? UrlParameter.Optional));
		}
	}
}