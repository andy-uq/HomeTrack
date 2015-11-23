using AutoMapper;
using HomeTrack.Mapping;

namespace HomeTrack.SqlStore.Models
{
	public class Account : ICustomMapping
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string AccountTypeName { get; set; }

		public void Configure(IConfiguration config)
		{
			config.CreateMap<HomeTrack.Account, Models.Account>()
				.ForMember(x => x.AccountTypeName, map => map.MapFrom(x => x.Type));

			config.CreateMap<Models.Account, HomeTrack.Account>()
				.ForMember(x => x.Type, map => map.MapFrom(x => x.AccountTypeName));
		}
	}
}