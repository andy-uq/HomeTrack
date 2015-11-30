using Autofac;
using HomeTrack.Ioc;

namespace HomeTrack.SqlStore
{
	public class SqlStoreFeature : IFeatureRegistration
	{
		public void Register(ContainerBuilder builder)
		{
			builder.RegisterType<GeneralLedgerRepository>()
				.As<IGeneralLedgerRepository>()
				.AsSelf();

			builder.RegisterType<ImportRepository>()
				.As<IImportRepository>()
				.AsSelf();
		}
	}
}