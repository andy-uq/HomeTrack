using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HomeTrack
{
	public class GeneralLedger : IEnumerable<Account>
	{
		private readonly ISet<Account> _accounts;

		public IEnumerable<Account> DebitAccounts
		{
			get { return _accounts.Where(x => x.Type == EntryType.Debit); }
		}

		public IEnumerable<Account> CreditAccounts
		{
			get { return _accounts.Where(x => x.Type == EntryType.Credit); }
		}

		public GeneralLedger()
		{
			_accounts = new HashSet<Account>();
		}
		
		public void Add(Account account)
		{
			_accounts.Add(account);
		}

		public IEnumerator<Account> GetEnumerator()
		{
			return _accounts.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool TrialBalance()
		{
			var debit = DebitAccounts.Sum(x => x.Balance);
			var credit = CreditAccounts.Sum(x => x.Balance);

			return debit == credit;
		}

		public Account this[string name]
		{
			get { return _accounts.Single(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)); }
		}
	}
}