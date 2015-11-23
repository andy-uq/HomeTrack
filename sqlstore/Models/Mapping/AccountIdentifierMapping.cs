using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using HomeTrack.Core;
using HomeTrack.Mapping;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HomeTrack.SqlStore.Models.Mapping
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

		Models.AccountIdentifier ITypeConverter<HomeTrack.AccountIdentifier, Models.AccountIdentifier>.Convert(ResolutionContext context)
		{
			var accountIdentifier = (HomeTrack.AccountIdentifier )context.SourceValue;

			var result = Convert(accountIdentifier.Pattern).ToArray();
			return new Models.AccountIdentifier
			{
				AccountId = accountIdentifier.Account.Id,
				Id = accountIdentifier.Id,
				Patterns = result
			};
		}

		HomeTrack.AccountIdentifier ITypeConverter<Models.AccountIdentifier, HomeTrack.AccountIdentifier>.Convert(ResolutionContext context)
		{
			var models = (Models.AccountIdentifier ) context.SourceValue;
			return Convert(models);
		}

		private HomeTrack.AccountIdentifier Convert(Models.AccountIdentifier model)
		{
			var pattern = ToPattern(model.Patterns);
			return new HomeTrack.AccountIdentifier
			{
				Id = model.Id,
				Account =  AccountLookup[model.AccountId],
				Pattern = pattern
			};
		}

		private IPattern ToPattern(AccountIdentifierPattern[] patterns)
		{
			if (patterns.Length > 1)
			{
				return new CompositePattern(patterns.Select(p => ToPattern(p.Name, p.PropertiesJson)));
			}

			var name = patterns[0].Name;
			var propertiesJson = patterns[0].PropertiesJson;
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

		private IEnumerable<Models.AccountIdentifierPattern> Convert(IPattern pattern)
		{
			foreach (var p in  PatternBuilder.Parse(pattern))
			{
				yield return new Models.AccountIdentifierPattern
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

			config.CreateMap<Models.AccountIdentifier, HomeTrack.AccountIdentifier>()
				.ConvertUsing(this);
		}
	}
}