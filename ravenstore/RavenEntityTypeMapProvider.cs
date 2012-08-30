using System;
using System.Text.RegularExpressions;
using AutoMapper;
using HomeTrack.RavenStore.Documents;

namespace HomeTrack.RavenStore
{
	public class RavenEntityTypeMapProvider : ITypeMapProvider
	{
		#region ITypeMapProvider Members

		public void RegisterTypeMaps(ConfigurationStore map)
		{
			MapAccount(map);
			MapTransaction(map);
			MapIdentifiers(map);
		}

		#endregion

		private object ToAccount(Documents.Amount x)
		{
			return new Account
			{
				Id = x.AccountId.Substring(x.AccountId.LastIndexOf('/') + 1),
				Name = x.AccountName
			};
		}

		private object ToAccount(Documents.AccountIdentifier x)
		{
			return new Account
			{
				Id = x.AccountId.Substring(x.AccountId.LastIndexOf('/') + 1),
				Name = x.AccountName
			};
		}

		private object GetRavenId(Amount x)
		{
			return GetRavenId(x.Account);
		}

		private object GetRavenId(Account x)
		{
			return string.Concat("accounts/", x.Id ?? Regex.Replace(x.Name, @"\W+", string.Empty).ToLowerInvariant());
		}

		private object GetEntityId(Documents.Account x)
		{
			return x.Id.Substring(x.Id.LastIndexOf('/') + 1);
		}

		private void MapIdentifiers(ConfigurationStore map)
		{
			map.CreateMap<Documents.AccountIdentifier, AccountIdentifier>()
				.ForMember(x => x.Account, m => m.ResolveUsing(ToAccount));

			map.CreateMap<AccountIdentifier, Documents.AccountIdentifier>();
		}

		private void MapAccount(ConfigurationStore map)
		{
			map.CreateMap<Documents.Account, Account>()
				.ForMember(x => x.Id, m => m.ResolveUsing(GetEntityId));

			map.CreateMap<Account, Documents.Account>()
				.ForMember(x => x.Id, m => m.ResolveUsing(GetRavenId));
		}

		private void MapTransaction(ConfigurationStore map)
		{
			map.CreateMap<Documents.Transaction, Transaction>()
				.ConstructUsing((Func<Documents.Transaction, Transaction>) (_ => new Transaction()));

			map.CreateMap<Transaction, Documents.Transaction>();

			map.CreateMap<Documents.Amount, Amount>()
				.ForMember(x => x.Account, m => m.ResolveUsing(ToAccount));

			map.CreateMap<Amount, Documents.Amount>()
				.ForMember(x => x.AccountId, m => m.ResolveUsing(GetRavenId));
		}
	}
}