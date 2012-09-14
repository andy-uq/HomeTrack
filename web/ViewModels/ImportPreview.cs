using System.Collections.Generic;
using HomeTrack.Core;

namespace HomeTrack.Web.ViewModels
{
	public class ImportPreview
	{
		public string FileName { get; set; }

		public IEnumerable<Account> Accounts { get; set; }
		public IEnumerable<AccountIdentifier> AccountIdentifiers { get; set; }
		public Import Import { get; set; }
	}
}