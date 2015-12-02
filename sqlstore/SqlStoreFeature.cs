using System.Reflection;
using Autofac;
using AutoMapper;
using HomeTrack.Ioc;
using HomeTrack.Mapping;

namespace HomeTrack.SqlStore
{
	public class SqlStoreFeature : IFeatureRegistration
	{
		public void Register(ContainerBuilder builder)
		{
			var assembly = typeof(SqlStoreFeature).Assembly;

			builder.RegisterMappings(assembly);
				

			builder.RegisterType<GeneralLedgerRepository>()
				.As<IGeneralLedgerRepository>()
				.AsSelf();

			builder.RegisterType<ImportRepository>()
				.As<IImportRepository>()
				.As<IImportAsyncRepository>()
				.AsSelf();

			builder.RegisterType<AccountIdentifierRepository>()
				.As<IAccountIdentifierRepository>()
				.As<IAccountIdentifierAsyncRepository>()
				.AsSelf();
		}
	}
}