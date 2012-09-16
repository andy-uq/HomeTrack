using System;
using EnsureThat;

namespace HomeTrack
{
	public class Account
	{
		public Account()
		{
		}

		public Account(string name, AccountType type)
		{
			Ensure.That(() => name).IsNotNullOrEmpty();

			Name = name;
			Type = type;
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public AccountType Type { get; set; }

		public EntryType Direction
		{
			get
			{
				if ( Type == AccountType.NotSet )
				{
					throw new InvalidOperationException("The account \"" + Name + "\" does not have an account type set.");
				}
				
				return Type.IsDebitOrCredit();
			}
		}

		public decimal Balance { get; set; }

		public void Post(decimal amount, EntryType entryType)
		{
			Balance += (entryType == Direction) 
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

		public override string ToString()
		{
			return Name ?? Id;
		}
	}
}