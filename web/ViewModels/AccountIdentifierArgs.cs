using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using HomeTrack.Core;

namespace HomeTrack.Web.ViewModels
{
	public class AccountIdentifierViewModel
	{
		public AccountIdentifierViewModel()
		{
			Patterns = Enumerable.Empty<PatternBuilder>();
		}

		public string AccountId { get; set; }
		public IEnumerable<Account> Accounts { get; set; }
		public IEnumerable<PatternBuilder> AvailablePatterns { get; set; }
		
		public IEnumerable<PatternBuilder> Patterns { get; set; }

	}

	public class AccountIdentifierArgs
	{
		[Required]
		public string AccountId { get; set; }
		
		[Required]
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