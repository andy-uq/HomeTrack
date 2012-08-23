using System.Linq;
using AutoMapper;

namespace HomeTrack.Web.ViewModels
{
	public class ViewModelTypeMapProvider : ITypeMapProvider
	{
		public void RegisterTypeMaps(ConfigurationStore map)
		{
			map.CreateMap<HomeTrack.Transaction, TransactionIndexViewModel.Transaction>()
				.ConvertUsing(ToViewModel);

			map.CreateMap<HomeTrack.Transaction, Transaction>();
		}

		private TransactionIndexViewModel.Transaction ToViewModel(HomeTrack.Transaction transaction)
		{
			return new TransactionIndexViewModel.Transaction
			{
				Id = transaction.Id,
				Date = transaction.Date,
				Debit = transaction.Debit.Sum(x => x.Value),
				Credit = transaction.Credit.Sum(x => x.Value),
				Description = transaction.Description,
				ReferenceId = transaction.Id,
			};
		}
	}
}