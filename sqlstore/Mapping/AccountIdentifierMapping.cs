using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using HomeTrack.Collections;
using HomeTrack.Core;
using HomeTrack.Mapping;
using HomeTrack.SqlStore.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace HomeTrack.SqlStore.Mappings
{
	public class AccountIdentifierMapping : ICustomMapping, ITypeConverter<HomeTrack.AccountIdentifier, Models.AccountIdentifier>, ITypeConverter<Models.AccountIdentifier, HomeTrack.AccountIdentifier>
	{
		private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { Converters = new [] { new StringEnumConverter(), }};
		private static readonly Formatting _jsonFormatting = Formatting.None;

		public AccountIdentifierMapping(IAccountLookup accountLookup, IEnumerable<PatternBuilder> patternBuilders)
		{
			AccountLookup = accountLookup;
			PatternBuilders = patternBuilders.ToDictionary(p => p.Name);
		}

		private Dictionary<string, PatternBuilder> PatternBuilders { get; }
		private IAccountLookup AccountLookup { get; }

		Models.AccountIdentifier ITypeConverter<AccountIdentifier, Models.AccountIdentifier>.Convert(ResolutionContext context)
		{
			var accountIdentifier = (HomeTrack.AccountIdentifier )context.SourceValue;

			var result = Convert(accountIdentifier.Pattern).ToList();
			return new Models.AccountIdentifier
			{
				AccountId = accountIdentifier.Account.Id,
				Id = accountIdentifier.Id,
				Primary = result.First(),
				Secondaries = result.Skip(1).ToList()
			};
		}

		AccountIdentifier ITypeConverter<Models.AccountIdentifier, AccountIdentifier>.Convert(ResolutionContext context)
		{
			var models = (Models.AccountIdentifier ) context.SourceValue;
			return Convert(models);
		}

		private AccountIdentifier Convert(Models.AccountIdentifier model)
		{
			var pattern = ToPattern(model.Primary, model.Secondaries);
			return new AccountIdentifier
			{
				Id = model.Id,
				Account =  AccountLookup[model.AccountId],
				Pattern = pattern
			};
		}

		private IPattern ToPattern(Models.AccountIdentifierRow parent, IEnumerable<Models.AccountIdentifierRow> child)
		{
			var patterns = child.Select(c => ToPattern(c, Enumerable.Empty<Models.AccountIdentifierRow>())).ToList();
			if (patterns.Any())
			{
				return new CompositePattern(patterns);
			}

			var name = parent.Name;
			var propertiesJson = parent.PropertiesJson;
			return ToPattern(name, propertiesJson);
		}

		private IPattern ToPattern(string name, string propertiesJson)
		{
			if (!PatternBuilders.ContainsKey(name))
				throw new ArgumentOutOfRangeException(nameof(name), $"Pattern builder \"{name}\" not found");

			var builder = PatternBuilders[name];
			var properties = JsonConvert.DeserializeObject<Dictionary<string, string>>(propertiesJson);
			return builder.Build(properties);
		}

		private IEnumerable<Models.AccountIdentifierRow> Convert(IPattern pattern)
		{
			var patterns = PatternBuilder.Parse(pattern).ToList();

			if (patterns.Count > 1)
			{
				yield return new Models.AccountIdentifierRow
				{
					PropertiesJson = "",
					Name = "Composite",
				};
			}

			foreach (var p in  patterns)
			{
				yield return new Models.AccountIdentifierRow
				{
					PropertiesJson = JsonConvert.SerializeObject(p.Properties, _jsonFormatting, _jsonSerializerSettings),
					Name = p.Name,
				};
			}
		}

		public void Configure(IConfiguration config)
		{
			config.CreateMap<HomeTrack.AccountIdentifier, Models.AccountIdentifier>()
				.ConvertUsing(this);
		}
	}
}