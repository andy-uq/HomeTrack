using System;
using EnsureThat;

namespace HomeTrack
{
	public class Account : IEquatable<Account>
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

		public bool Equals(Account other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			
			return Id == null 
				? Equals(other.Name, Name) 
				: Equals(other.Id, Id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Account)) return false;
			return Equals((Account) obj);
		}

		public override int GetHashCode()
		{
			return (Id ?? Name).GetHashCode();
		}

		public static bool operator ==(Account left, Account right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Account left, Account right)
		{
			return !Equals(left, right);
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