using System;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using HomeTrack.Core;
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
			MapBudget(map);
			MapImport(map);
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
				Name = x.AccountName,
				Type = x.AccountType,
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

		private Documents.Pattern ToPattern(AmountPattern pattern)
		{
			return new Pattern
			{
				Name = "Amount",
				Properties =
					{
						{"Amount", pattern.Amount}
					}
			};
		}

		private Documents.Pattern ToPattern(AmountRangePattern pattern)
		{
			return new Pattern
			{
				Name = "AmountRange",
				Properties =
					{
						{"Min", pattern.Min},
						{"Max", pattern.Max}
					}
			};
		}

		private Documents.Pattern ToPattern(DayOfMonthPattern pattern)
		{
			return new Pattern
			{
				Name = "DayOfMonth",
				Properties =
					{
						{"DaysOfMonth", pattern.DaysOfMonth}
					}
			};
		}

		private Documents.Pattern ToPattern(FieldPattern pattern)
		{
			return new Pattern
			{
				Name = "Field",
				Properties =
					{
						{"Name", pattern.Name},
						{"Pattern", pattern.Pattern},
					}
			};
		}

		private IPattern ToPattern(Pattern pattern)
		{
			if ( pattern == null )
				return null;

			if ( pattern.Child == null )
			{
				var name = string.Concat("HomeTrack.Core.", pattern.Name, "Pattern, HomeTrack.Core");
				var type = Type.GetType(name, true);
				var instance = Activator.CreateInstance(type);

				foreach (var property in type.GetProperties().Where(x => x.CanWrite))
				{
					object value;
					if (pattern.Properties.TryGetValue(property.Name, out value))
					{
						if ( property.PropertyType == typeof(decimal) )
						{
							value = Convert.ToDecimal(value);
						}

						if (property.PropertyType == typeof(int[]))
						{
							value = ((System.Collections.IEnumerable) value).Cast<int>().ToArray();
						}

						property.SetValue(instance, value, null);
					}
				}

				return (IPattern) instance;
			}

			return new CompositePattern(pattern.Child.Select(ToPattern));
		}

		private void MapIdentifiers(ConfigurationStore map)
		{
			map.CreateMap<Documents.AccountIdentifier, AccountIdentifier>()
				.ForMember(x => x.Account, m => m.ResolveUsing(ToAccount));

			map.CreateMap<Documents.Pattern, IPattern>()
				.ConvertUsing(ToPattern);

			map.CreateMap<AmountPattern, Documents.Pattern>()
				.ConvertUsing(ToPattern);

			map.CreateMap<AmountRangePattern, Documents.Pattern>()
				.ConvertUsing(ToPattern);

			map.CreateMap<DayOfMonthPattern, Documents.Pattern>()
				.ConvertUsing(ToPattern);

			map.CreateMap<FieldPattern, Documents.Pattern>()
				.ConvertUsing(ToPattern);

			map.CreateMap<CompositePattern, Documents.Pattern>()
				.ForMember(x => x.Name, m => m.UseValue("Composite"))
				.ForMember(x => x.Child, m => m.MapFrom(p => p));

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
		
		private void MapBudget(ConfigurationStore map)
		{
			map.CreateMap<Budget, Documents.Budget>()
				.ForMember(x => x.AccountId, m => m.MapFrom(x => x.RealAccount.Id));

			map.CreateMap<Documents.Budget, Budget>()
				.ForMember(x => x.RealAccount, m => m.ResolveUsing(x => new Account { Id = x.AccountId }))
				.ForMember(x => x.BudgetAccount, m => m.ResolveUsing(x => new Account { Id = x.BudgetAccountId }));
		}

		private void MapImport(ConfigurationStore map)
		{
			map.CreateMap<ImportResult, Documents.ImportResult>()
				.ForMember(x => x.Id, m => m.ResolveUsing(x => string.Concat("imports/", x.Name.ToLowerInvariant())));

			map.CreateMap<Documents.ImportResult, ImportResult>();

			map.CreateMap<Transaction, ImportedTransaction>()
				.ForMember(x => x.TransactionId, m => m.MapFrom(x => x.Id))
				.ForMember(x => x.Id, m => m.MapFrom(x => x.Reference));

		}
	}
}