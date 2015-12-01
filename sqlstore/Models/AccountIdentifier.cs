using System.Collections.Generic;
using AutoMapper;
using HomeTrack.Mapping;
using HomeTrack.SqlStore.Mappings;

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
				.ConvertUsing<AccountIdentifierMapping>();
		}
	}
}