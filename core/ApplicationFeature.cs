using Autofac;
using HomeTrack.Ioc;

namespace HomeTrack.Core
{
	public class ApplicationFeature : IFeatureRegistration
	{
		public void Register(ContainerBuilder builder)
		{
			builder.RegisterFeature<PatternsFeature>();

			builder.RegisterInstance(new WestpacCsvImportDetector()).As<IImportDetector>();
			builder.RegisterInstance(new AsbOrbitFastTrackCsvImportDetector()).As<IImportDetector>();
			builder.RegisterInstance(new AsbVisaCsvImportDetector()).As<IImportDetector>();
			builder.RegisterInstance(new WestpacVisaCsvImportDetector()).As<IImportDetector>();

			builder.RegisterType<ImportDetector>();
			builder.RegisterType<TransactionImportContext>();

			builder.RegisterType<GeneralLedger>();
		}
	}
}