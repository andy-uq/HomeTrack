using System;
using System.Collections;
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

		public Transaction(Account debit, Account credit, decimal amount) : this()
		{
			Ensure.That(() => debit).IsNotNull();
			Ensure.That(() => credit).IsNotNull();
			
			_debit.Add(new Amount(debit, amount));
			_credit.Add(new Amount(credit, amount));

			Amount = Math.Abs(amount);
		}

		public ISet<Amount> Debit
		{
			get { return _debit; }
		}

		public ISet<Amount> Credit
		{
			get { return _credit; }
		}

		public int Id { get; set; }
		public DateTime Date { get; set; }
		public string Description { get; set; }
		public decimal Amount { get; set; }

		public bool Check()
		{
			var debit = _debit.Sum(x => x.DebitValue);
			var credit = _credit.Sum(x => x.CreditValue);

			return Amount == Math.Abs(credit) && Amount == Math.Abs(debit);
		}

		public bool Is(Account account)
		{
			return Debit.Concat(Credit).Any(x => x.Account == account);
		}

		public IEnumerable<Amount> RightHandSide()
		{
			if (_debit.Count <= _credit.Count)
				return _credit;

			return _debit;
		}
	}
}