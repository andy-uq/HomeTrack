using System.Data.SqlClient;
using Autofac;
using FixieShim.Fixie;
using HomeTrack.Core;
using HomeTrack.Ioc;

namespace HomeTrack.SqlStore.Tests
{
	[IocCompositionRoot]
	public static class IoC
	{
		public static ContainerBuilder GetCompositionRoot()
		{
			var builder = new ContainerBuilder();
			builder.RegisterFeature<ApplicationFeature>();

			builder.RegisterFeature<SqlStoreFeature>();

			builder.RegisterType<TestDatabase>()
				.OnActivated(args => args.Instance.ApplyMigrations())
				.InstancePerLifetimeScope();

			builder.Register(resolver => resolver.Resolve<TestDatabase>().Database);

			builder.RegisterType<TestData.AccountLookup>()
				.As<IAccountLookup>();

			return builder;
		}
	}
}