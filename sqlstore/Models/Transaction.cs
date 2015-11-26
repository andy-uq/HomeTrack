using System;
using AutoMapper;
using HomeTrack.Mapping;

namespace HomeTrack.SqlStore.Models
{
	public class Transaction : ICustomMapping
	{
		public string Id { get; set; }

		public DateTime Date { get; set; }
		public string Description { get; set; }
		public string Reference { get; set; }

		public decimal Amount { get; set; }

		public void Configure(IConfiguration config)
		{
			config.CreateMap<HomeTrack.Transaction, Transaction>()
				.ForMember(x => x.Id, map => map.ResolveUsing(x => x.Id ?? TransactionId.From(x)));

			config.CreateMap<Transaction, HomeTrack.Transaction>();
		}
	}

	public class TransactionComponent : ICustomMapping
	{
		public string TransactionId { get; set; }
		public string AccountId { get; set; }
		public string EntryTypeName { get; set; }
		public decimal Amount { get; set; }
		public string Annotation { get; set; }
		public int? AppliedByRuleId { get; set; }

		public void Configure(IConfiguration config)
		{
			config.CreateMap<HomeTrack.Amount, TransactionComponent>();
		}
	}
}