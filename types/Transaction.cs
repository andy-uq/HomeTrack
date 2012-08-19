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

		public decimal CreditAmount
		{
			get { return _credit.Sum(x => x.CreditValue); }
		}

		public decimal DebitAmount
		{
			get { return _debit.Sum(x => x.DebitValue); }
		}
		public bool Check()
		{
			var debit = DebitAmount;
			var credit = CreditAmount;

			return credit == debit;
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