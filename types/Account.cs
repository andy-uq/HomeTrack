using System;
using EnsureThat;

namespace HomeTrack
{
	public class Account
	{
		private decimal _balance;

		public Account(string name, EntryType entryType)
		{
			Ensure.That(() => name).IsNotNullOrEmpty();

			Name = name;
			Type = entryType;
		}

		public string Name { get; set; }
		public EntryType Type { get; set; }

		public decimal Balance
		{
			get { return _balance; }
		}

		public void Post(decimal amount, EntryType entryType)
		{
			_balance += (entryType == Type) 
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