using System;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Web.Mvc;
using System.Web.Routing;
using HomeTrack.Web;
using Moq;

namespace web.tests
	{
		public static class MvcMockHelpers
		{
			public static HttpContextBase FakeHttpContext()
			{
				var context = new Mock<HttpContextBase>();
				var request = new Mock<HttpRequestBase>();
				var response = new Mock<HttpResponseBase>();
				var session = new Mock<HttpSessionStateBase>();
				var server = new Mock<HttpServerUtilityBase>();

				context.Setup(ctx => ctx.Request).Returns(request.Object);
				context.Setup(ctx => ctx.Response).Returns(response.Object);
				context.Setup(ctx => ctx.Session).Returns(session.Object);
				context.Setup(ctx => ctx.Server).Returns(server.Object);

				return context.Object;
			}

			public static HttpContextBase FakeHttpContext(string url, bool isAjax = false)
			{
				var context = FakeHttpContext();
				context.Request.SetupRequestUrl(url);
				if ( isAjax )
				{
					var mock = Mock.Get(context.Request);
					var headers = new NameValueCollection();
					headers.Add("X-Requested-With", "XMLHttpRequest");

					mock.SetupGet(x => x.Headers).Returns(headers);
				}

				return context;
			}

			public static void SetFakeControllerContext(this Controller controller, string url, bool isAjax = false)
			{
				var httpContext = FakeHttpContext(url, isAjax);

				var routeTable = new RouteCollection();
				MvcApplication.RegisterRoutes(routeTable);
				var routeData = routeTable.GetRouteData(httpContext) ?? new RouteData();
				
				var context = new ControllerContext(new RequestContext(httpContext, routeData), controller);
				controller.ControllerContext = context;
				controller.Url = new UrlHelper(new RequestContext(httpContext, routeData));
			}

			private static string GetUrlFileName(string url)
			{
				return url.Contains("?") 
					? url.Substring(0, url.IndexOf("?", StringComparison.OrdinalIgnoreCase)) 
					: url;
			}

			static NameValueCollection GetQueryStringParameters(string url)
			{
				if (!url.Contains("?"))
				{
					return null;
				}
				
				var parameters = new NameValueCollection();

				var parts = url.Split("?".ToCharArray());
				var keys = parts[1].Split("&".ToCharArray());

				foreach (var part in keys.Select(key => key.Split('=')))
				{
					parameters.Add(part[0], part[1]);
				}

				return parameters;
			}

			public static void SetHttpMethodResult(this HttpRequestBase request, string httpMethod)
			{
				Mock.Get(request)
					.Setup(req => req.HttpMethod)
					.Returns(httpMethod);
			}

			public static void SetupRequestUrl(this HttpRequestBase request, string url)
			{
				if ( url == null )
					throw new ArgumentNullException("url");

				if ( !url.StartsWith("~/") )
					throw new ArgumentException("Sorry, we expect a virtual url starting with \"~/\".");

				var mock = Mock.Get(request);

				mock.Setup(req => req.QueryString)
					.Returns(GetQueryStringParameters(url));

				mock.Setup(req => req.AppRelativeCurrentExecutionFilePath)
					.Returns(GetUrlFileName(url));

				mock.Setup(req => req.PathInfo)
					.Returns(string.Empty);
			}
		}
	}
