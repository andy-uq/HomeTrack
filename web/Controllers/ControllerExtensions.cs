using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace HomeTrack.Web.Controllers
{
	public static class ControllerExtensions
	{
		private static RouteCollection _routes;

		public static void InitRoutes(RouteCollection routes)
		{
			_routes = routes;
		}

		public static JsonResult JsonRedirect(this Controller controller, string redirectUrl)
		{
			return new JsonResult { Data = new { redirectUrl }};
		}

		public static JsonResult ToJson(this RedirectToRouteResult state, ControllerContext context)
		{
			var redirectUrl = (_routes == null || context == null)
			                  	? null
			                  	: UrlHelper.GenerateUrl(state.RouteName, actionName: null, controllerName: null,
			                  	                        routeValues: state.RouteValues, routeCollection: _routes,
			                  	                        requestContext: context.RequestContext, includeImplicitMvcValues: false);
			return new JsonResult
			{
				Data = new
				{
					actionName = state.RouteValues["action"],
					controllerName = state.RouteValues["controller"],
					routeValues = state.RouteValues,
					redirectUrl
				}
			};
		}

		public static JsonResult ToJson(this ModelStateDictionary state)
		{
			var errors =
				(
					from e in state
					where e.Value.Errors.Count > 0
					select new
					{
						Name = e.Key,
						Errors = e.Value.Errors.Where(x => x.Exception == null).Select(x => x.ErrorMessage)
						.Concat(e.Value.Errors.Where(x => x.Exception != null).Select(x => x.Exception.Message))
					}
				);

			return new JsonResult
			{
				Data = new
				{
					Tag = "ValidationError",
					State = errors
				}
			};
		}
	}
}