using System;
using System.Linq;
using AutoMapper;

namespace HomeTrack.Web.ViewModels
{
	public class ViewModelTypeMapProvider : ITypeMapProvider
	{
		public void RegisterTypeMaps(ConfigurationStore map)
		{
			map.CreateMap<HomeTrack.Transaction, TransactionDetails>();
		}
	}
}