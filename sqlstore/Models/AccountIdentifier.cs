using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using HomeTrack.Mapping;

namespace HomeTrack.SqlStore.Models
{
	public class AccountIdentifier
	{
		public AccountIdentifier()
		{
			Patterns = new AccountIdentifierPattern[0];
		}

		public int Id { get; set; }
		public string AccountId { get; set; }

		public AccountIdentifierPattern[] Patterns { get; set; }
	}

	public class AccountIdentifierPattern
	{
		public int AccountIdentifierId { get; set; }
		public string Name { get; set; }
		public string PropertiesJson { get; set; }
	}
}