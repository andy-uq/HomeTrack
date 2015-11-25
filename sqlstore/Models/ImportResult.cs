using System;
using AutoMapper;
using HomeTrack.Mapping;

namespace HomeTrack.SqlStore.Models
{
	public class ImportResult : ICustomMapping
	{
		public string Id { get; set; }

		public string Name { get; set; }
		public string ImportType { get; set; }

		public DateTime Date { get; set; }

		public void Configure(IConfiguration config)
		{
			config.CreateMap<HomeTrack.ImportResult, Models.ImportResult>();
			config.CreateMap<ImportResult, HomeTrack.ImportResult>();

			config.CreateMap<ImportedTransaction, HomeTrack.ImportedTransaction>();

			config.CreateMap<HomeTrack.Transaction, Models.ImportedTransaction>()
				.ForMember(x => x.Id, map => map.ResolveUsing(x => x.Id ?? TransactionId.From(x)));
		}
	}

	public class ImportedTransaction
	{
		public string Id { get; set; }
		public bool Unclassified { get; set; }
		public decimal Amount { get; set; }
	}
}