using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using HomeTrack.Core;
using HomeTrack.Mapping;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace HomeTrack.SqlStore.Models
{
	public class AccountIdentifier : ICustomMapping
	{
		public int Id { get; set; }
		public string AccountId { get; set; } 
		public string Name { get; set; }
		public string PropertiesJson { get; set; }
		public int? ParentId { get; set; }

		public void Configure(IConfiguration config)
		{
			config.CreateMap<HomeTrack.AccountIdentifier, IEnumerable<AccountIdentifier>>()
				.ConvertUsing<AccountIdentifierModelBuilder>();
		}
	}

	public class AccountIdentifierModelBuilder : ITypeConverter<HomeTrack.AccountIdentifier, IEnumerable<AccountIdentifier>>
	{
		private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), Converters = new [] { new StringEnumConverter(), }};
		private static readonly Formatting _jsonFormatting = Formatting.None;

		public IEnumerable<AccountIdentifier> Convert(ResolutionContext context)
		{
			var accountIdentifier = (HomeTrack.AccountIdentifier )context.SourceValue;

			int id = 1;
			return Convert(() => id++, null, accountIdentifier.Pattern, accountIdentifier.Account.Id);
		}

		private IEnumerable<AccountIdentifier> Convert(Func<int> idFunc, int? parentId, IPattern pattern, string accountId)
		{
			var compositePattern = (pattern as IEnumerable<IPattern>) ?? Enumerable.Empty<IPattern>();

			var id = idFunc();
			yield return new AccountIdentifier
			{
				Id = id,
				AccountId = accountId,
				PropertiesJson = pattern is CompositePattern ? "" : JsonConvert.SerializeObject(pattern, _jsonFormatting, _jsonSerializerSettings),
				Name = pattern.GetType().Name,
				ParentId = parentId,
			};
			
			foreach (var child in compositePattern.SelectMany(childPattern => Convert(idFunc, id, childPattern, accountId)))
				yield return child;
		}
	}
}