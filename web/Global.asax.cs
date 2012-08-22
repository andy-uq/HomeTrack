using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;

namespace HomeTrack.Web
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		private readonly Func<string, string> _mapPath;

		public MvcApplication()
		{
		}

		public MvcApplication(Func<string, string> mapPath)
		{
			_mapPath = mapPath;
		}

		private static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			Start();
		}

		public void Start()
		{
			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);

			var builder = new ContainerBuilder();
			RegisterIoc(builder);
		}

		private void RegisterIoc(ContainerBuilder builder)
		{
			var raven = new RavenStore.ConfigureEmbeddedDocumentStore()
			{
				DataDirectory = MapPath("~/App_Data/RavenDb"),
				UseEmbeddedHttpServer = false
			};

			raven.Build(builder);

			builder.Register(r => new GeneralLedger(r.Resolve<IGeneralLedgerRepository>()));
			builder.RegisterControllers(typeof (MvcApplication).Assembly);

			var container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
		}

		private Func<string, string> MapPath
		{
			get { return _mapPath ?? Server.MapPath; }
		}
	}
}