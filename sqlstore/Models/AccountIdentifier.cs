using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using HomeTrack.Mapping;
using HomeTrack.SqlStore.Mappings;

namespace HomeTrack.SqlStore.Models
{
	public class AccountIdentifier
	{
		public AccountIdentifier()
		{
			Primary = new AccountIdentifierRow();
			Secondaries = Enumerable.Empty<AccountIdentifierRow>();
		}

		public int Id { get; set; }
		public string AccountId { get; set; }

		public AccountIdentifierRow Primary { get; set; }
		public IEnumerable<AccountIdentifierRow> Secondaries { get; set; }
	}

	public class AccountIdentifierRow
	{
		public string Name { get; set; }
		public string PropertiesJson { get; set; }
	}
}