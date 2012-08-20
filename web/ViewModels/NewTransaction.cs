using System;
using System.Collections.Generic;

namespace HomeTrack.Web.ViewModels
{
	public class Transaction
	{
		public Account Account { get; set; }
		public decimal Amount { get; set; }
		public string Description { get; set; }
		public DateTime Date { get; set; }
		public EditRelatedAccount[] Related { get; set; }
		
		public IEnumerable<Account> Accounts { get; set; }
	}

	public class RelatedAccount
	{
		public string AccountId { get; set; }
		public decimal Amount { get; set; }
	}

	public class EditRelatedAccount : RelatedAccount
	{
		public EditRelatedAccount()
		{
		}

		public EditRelatedAccount(RelatedAccount relatedAccount, IEnumerable<Account> accounts)
		{
			AccountId = relatedAccount.AccountId;
			Amount = relatedAccount.Amount;
			Accounts = accounts;
		}

		public IEnumerable<Account> Accounts { get; set; } 
	}

	public class NewTransaction
	{
		public string AccountId { get; set; }
		public decimal Amount { get; set; }
		public string Description { get; set; }
		public DateTime Date { get; set; }
		public RelatedAccount[] Related { get; set; }
		
	}
}