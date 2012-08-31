using System.Collections.Generic;
using HomeTrack.Core;

namespace HomeTrack.Web.ViewModels
{
	public class AccountIdentifierViewModel
	{
		public IEnumerable<Account> Accounts { get; set; }
		public IEnumerable<PatternBuilder> Patterns { get; set; }
	}

	public class AccountIdentifierArgs
	{
		public string AccountId { get; set; }
		public PatternArgs[] Patterns { get; set; }
	}

	public class PatternArgs
	{
		public PatternArgs()
		{
			Properties = new Dictionary<string, string>();
		}

		public string Name { get; set; }
		public Dictionary<string, string> Properties { get; set; }
	}
}