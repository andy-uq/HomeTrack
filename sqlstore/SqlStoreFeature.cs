using Autofac;
using HomeTrack.Ioc;
using HomeTrack.Mapping;

namespace HomeTrack.SqlStore
{
	public class SqlStoreFeature : IFeatureRegistration
	{
		public void Register(ContainerBuilder builder)
		{
			builder.RegisterMappings(typeof(SqlStoreFeature).Assembly);

			builder.RegisterType<GeneralLedgerRepository>()
				.As<IGeneralLedgerRepository>()
				.AsSelf();

			builder.RegisterType<ImportRepository>()
				.As<IImportRepository>()
				.AsSelf();

			builder.RegisterType<AccountIdentifierRepository>()
				.As<IAccountIdentifierRepository>()
				.AsSelf();
		}
	}
}