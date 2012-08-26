﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using HomeTrack.Core;
using HomeTrack.RavenStore;

namespace HomeTrack.Web
{
	public class MvcApplication : HttpApplication
	{
		private readonly Func<string, string> _mapPath;
		private ConfigureEmbeddedDocumentStore _ravenDb;

		public MvcApplication()
		{
		}

		public MvcApplication(Func<string, string> mapPath)
		{
			_mapPath = mapPath;
		}

		public IContainer Container { get; private set; }

		private Func<string, string> MapPath
		{
			get { return _mapPath ?? Server.MapPath; }
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
				new {controller = "Home", action = "Index", id = UrlParameter.Optional} // Parameter defaults
				);
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			Start(GlobalFilters.Filters, RouteTable.Routes);
		}

		public void Start(GlobalFilterCollection filters, RouteCollection routes)
		{
			RegisterGlobalFilters(filters);
			RegisterRoutes(routes);

			var builder = new ContainerBuilder();
			Container = RegisterIoc(builder);
		}

		private IContainer RegisterIoc(ContainerBuilder builder)
		{
			EmbeddedDocumentStore.Build(builder);

			builder.RegisterType<MappingProvider>();
			builder.RegisterType<ViewModels.ViewModelTypeMapProvider>().As<ITypeMapProvider>();
			builder.Register(c => c.Resolve<MappingProvider>().Build());

			var path = MapPath("~/App_Data");
			builder.Register(c => new DirectoryExplorer(path));

			builder.RegisterInstance(new WestpacCsvImportDetector()).As<IImportDetector>();
			builder.RegisterInstance(new AsbCsvImportDetector()).As<IImportDetector>();
			builder.RegisterType<ImportDetector>();

			builder.RegisterType<GeneralLedger>();
			builder.RegisterControllers(typeof (MvcApplication).Assembly);

			IContainer container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

			return container;
		}

		public ConfigureEmbeddedDocumentStore EmbeddedDocumentStore
		{
			get
			{
				return _ravenDb ??
				       (
				       	_ravenDb = new ConfigureEmbeddedDocumentStore
				       	{
				       		DataDirectory = MapPath("~/App_Data/RavenDb"),
				       		UseEmbeddedHttpServer = false
				       	}
				       );
			}
		}
	}
}