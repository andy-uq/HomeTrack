using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace HomeTrack
{
	public class Transaction
	{
		public Transaction()
		{
			Debit = new HashSet<Amount>();
			Credit = new HashSet<Amount>();

			Date = DateTime.Now;
		}

		public Transaction(Account debit, Account credit, decimal amount) : this()
		{
			Ensure.That(() => debit).IsNotNull();
			Ensure.That(() => credit).IsNotNull();
			
			Debit.Add(new Amount(debit, EntryType.Debit, amount));
			Credit.Add(new Amount(credit, EntryType.Credit, amount));

			Amount = Math.Abs(amount);
		}

		public ISet<Amount> Debit { get; set; }
		public ISet<Amount> Credit { get; set; }

		public int Id { get; set; }
		public DateTime Date { get; set; }
		public string Description { get; set; }
		public decimal Amount { get; set; }

		public override string ToString()
		{
			return string.Format("{0} - {1} ({2:n2})", Date, Description, Amount);
		}

		public bool Check()
		{
			var debit = Debit.Sum(x => x.DebitValue);
			var credit = Credit.Sum(x => x.CreditValue);

			return Amount == Math.Abs(credit) && Amount == Math.Abs(debit);
		}

		public bool Is(Account account)
		{
			return Debit.Concat(Credit).Any(x => x.Account == account);
		}

		public IEnumerable<Amount> RightHandSide()
		{
			if (Debit.Count <= Credit.Count)
				return Credit;

			return Debit;
		}
	}
}