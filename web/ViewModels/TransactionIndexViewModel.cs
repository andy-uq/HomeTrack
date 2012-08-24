using System;
using System.Collections.Generic;

namespace HomeTrack.Web.ViewModels
{
	public class TransactionIndexViewModel
	{
		public Account Account { get; set; }
		public IEnumerable<Transaction> Transactions { get; set; }		
	}
}