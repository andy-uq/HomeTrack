using System;
using EnsureThat;

namespace HomeTrack
{
	public class Account
	{
		private decimal _balance;

		public Account()
		{
		}

		public Account(string name, AccountType type)
		{
			Ensure.That(() => name).IsNotNullOrEmpty();

			Name = name;
			Type = type;
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public AccountType Type { get; set; }

		public EntryType Direction { get { return Type.IsDebitOrCredit(); } }

		public decimal Balance
		{
			get { return _balance; }
		}

		public void Post(decimal amount, EntryType entryType)
		{
			_balance += (entryType == Direction) 
				? amount 
				: -amount;
		}

		public void Debit(decimal amount)
		{
			Post(amount, EntryType.Debit);
		}

		public void Credit(decimal amount)
		{
			Post(amount, EntryType.Credit);
		}
	}
}