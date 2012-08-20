using System.Collections.Generic;

namespace HomeTrack.Web.ViewModels
{
	public class AccountViewModel
	{
		public Account Account { get; set; }
		public IEnumerable<HomeTrack.Transaction> Transactions { get; set; }

		public string Id
		{
			get { return Account.Id; }
		}

		public string Name
		{
			get { return Account.Name; }
		}
	}
}