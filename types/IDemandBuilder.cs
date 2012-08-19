using Autofac;

namespace HomeTrack
{
	public interface IDemandBuilder
	{
		void Build(ContainerBuilder builder);
	}
}