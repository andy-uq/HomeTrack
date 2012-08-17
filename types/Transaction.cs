using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace HomeTrack
{
	public class Transaction
	{
		private readonly ISet<Amount> _debit;
		private readonly ISet<Amount> _credit;

		public Transaction()
		{
			_debit = new HashSet<Amount>();
			_credit = new HashSet<Amount>();
		}

		public ISet<Amount> Debit
		{
			get { return _debit; }
		}

		public ISet<Amount> Credit
		{
			get { return _credit; }
		}

		public Transaction(Account debit, Account credit, decimal amount) : this()
		{
			Ensure.That(() => debit).IsNotNull();
			Ensure.That(() => credit).IsNotNull();
			
			_debit.Add(new Amount(debit, amount));
			_credit.Add(new Amount(credit, amount));
		}

		public bool Check()
		{
			var debit = _debit.Sum(x => x.DebitValue);
			var credit = _credit.Sum(x => x.CreditValue);

			return credit == debit;
		}
	}
}