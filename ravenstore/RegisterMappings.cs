using System.Text.RegularExpressions;
using AutoMapper;
using AutoMapper.Mappers;

namespace HomeTrack.RavenStore
{
	public class RegisterMappings
	{
		public ConfigurationStore GetMappings()
		{
			var map = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.AllMappers());

			map.CreateMap<Account, HomeTrack.Account>()
				.ForMember(x => x.Id, m => m.ResolveUsing(GetEntityId));

			map.CreateMap<HomeTrack.Account, Account>()
				.ForMember(x => x.Id, m => m.ResolveUsing(GetRavenId));

			map.CreateMap<Amount, HomeTrack.Amount>()
				.ForMember(x => x.Account, m => m.ResolveUsing(ToAccount));

			map.CreateMap<HomeTrack.Amount, Amount>()
				.ForMember(x => x.AccountId, m => m.ResolveUsing(GetRavenId));

			map.CreateMap<Transaction, HomeTrack.Transaction>()
				.ForMember(x => x.Credit, m => m.MapFrom(x => x.Credit))
				.ForMember(x => x.Debit, m => m.MapFrom(x => x.Debit));
			
			map.CreateMap<HomeTrack.Transaction, Transaction>();

			return map;
		}

		private object ToAccount(Amount x)
		{
			return new HomeTrack.Account
			{
				Id = x.AccountId.Substring(x.AccountId.LastIndexOf('/') + 1),
				Name = x.AccountName
			};
		}

		private object GetRavenId(HomeTrack.Amount x)
		{
			return GetRavenId(x.Account);
		}

		private object GetRavenId(HomeTrack.Account x)
		{
			return string.Concat("accounts/", x.Id ?? Regex.Replace(x.Name, @"\W+", string.Empty).ToLowerInvariant());
		}

		private object GetEntityId(Account x)
		{
			return x.Id.Substring(x.Id.LastIndexOf('/') + 1);
		}
	}
}