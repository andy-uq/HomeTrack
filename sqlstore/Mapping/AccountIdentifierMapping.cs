using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using HomeTrack.Collections;
using HomeTrack.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace HomeTrack.SqlStore.Mappings
{
	public class AccountIdentifierMapping : ITypeConverter<HomeTrack.AccountIdentifier, IEnumerable<Models.AccountIdentifier>>, ITypeConverter<IEnumerable<Models.AccountIdentifier>, AccountIdentifier>
	{
		private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), Converters = new [] { new StringEnumConverter(), }};
		private static readonly Formatting _jsonFormatting = Formatting.None;


		public AccountIdentifierMapping(IAccountLookup accountLookup, IEnumerable<PatternBuilder> patternBuilders)
		{
			AccountLookup = accountLookup;
			PatternBuilders = patternBuilders.ToDictionary(p => p.Name);
		}

		private Dictionary<string, PatternBuilder> PatternBuilders { get; }
		private IAccountLookup AccountLookup { get; }

		IEnumerable<Models.AccountIdentifier> ITypeConverter<AccountIdentifier, IEnumerable<Models.AccountIdentifier>>.Convert(ResolutionContext context)
		{
			var accountIdentifier = (HomeTrack.AccountIdentifier )context.SourceValue;

			int id = 1;
			return Convert(() => id++, null, accountIdentifier.Pattern, accountIdentifier.Account.Id);
		}

		AccountIdentifier ITypeConverter<IEnumerable<Models.AccountIdentifier>, AccountIdentifier>.Convert(ResolutionContext context)
		{
			var models = (IEnumerable<Models.AccountIdentifier>) context.SourceValue;
			return Convert(models.AsList());
		}

		private AccountIdentifier Convert(IList<Models.AccountIdentifier> models)
		{
			var parent = models.Single(m => m.ParentId == null);

			IPattern pattern = ToPattern(parent, models.Where(m => m.ParentId == parent.Id));

			return new AccountIdentifier
			{
				Id = parent.Id,
				Account =  AccountLookup[parent.AccountId],
				Pattern = pattern
			};
		}

		private IPattern ToPattern(Models.AccountIdentifier parent, IEnumerable<Models.AccountIdentifier> child)
		{
			var patterns = child.Select(c => ToPattern(c, Enumerable.Empty<Models.AccountIdentifier>())).ToList();
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
			name = Regex.Match(name, @"(\w+)Pattern").Groups[1].Value;

			if (!PatternBuilders.ContainsKey(name))
				throw new ArgumentOutOfRangeException(nameof(name), $"Pattern builder \"{name}\" not found");

			var builder = PatternBuilders[name];
			var properties = JsonConvert.DeserializeObject<Dictionary<string, string>>(propertiesJson);
			return builder.Build(properties);
		}

		private IEnumerable<Models.AccountIdentifier> Convert(Func<int> idFunc, int? parentId, IPattern pattern, string accountId)
		{
			var compositePattern = (pattern as IEnumerable<IPattern>) ?? Enumerable.Empty<IPattern>();

			var id = idFunc();
			yield return new Models.AccountIdentifier
			{
				Id = id,
				AccountId = accountId,
				PropertiesJson = pattern is CompositePattern ? "" : JsonConvert.SerializeObject(pattern, _jsonFormatting, _jsonSerializerSettings),
				Name = pattern.GetType().Name,
				ParentId = parentId,
			};

			foreach (var child in compositePattern.SelectMany(childPattern => Convert(idFunc, id, childPattern, accountId)))
			{
				yield return child;
			}
		}
	}
}