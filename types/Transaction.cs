using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
			Date = DateTimeServer.Now;
		}

		public ISet<Amount> Debit { get; set; }
		public ISet<Amount> Credit { get; set; }

		public string Id { get; set; }
		public DateTime Date { get; set; }
		public string Description { get; set; }
		public string Reference { get; set; }
		public decimal Amount { get; set; }

		public override string ToString()
		{
			return string.Format("{0:yyyy-MM-dd} {0:HH:mm} - {1} {2:c}", Date, Description, Amount);
		}

		public bool Check()
		{
			var debit = Debit.Sum(x => x.DebitValue);
			var credit = Credit.Sum(x => x.CreditValue);

			return debit > 0 && credit > 0 && debit == credit;
		}

		public IEnumerable<Account> RelatedAccounts()
		{
			var set = new HashSet<Account>();

			foreach ( var amount in Debit )
			{
				var account = amount.Account;

				if (set.Add(account))
					yield return account;
			}

			foreach ( var amount in Credit )
			{
				var account = amount.Account;

				if (set.Add(account))
					yield return amount.Account;
			}
		}

		public bool Is(Account account)
		{
			return Debit.Concat(Credit).Any(x => x.Account == account);
		}

		public bool IsDebitAccount(Account account)
		{
			return Debit.Any(x => x.Account == account);
		}

		public bool IsCreditAccount(Account account)
		{
			return Credit.Any(x => x.Account == account);
		}
	}

	public static class TransactionId
	{
		public static string From(Transaction transaction)
		{
			var date = transaction.Date;
			var amount = transaction.Amount;
			var reference = transaction.Reference;
			var description = transaction.Description;

			return From(date, amount, reference, description);
		}

		public static string From(DateTime date, decimal amount, string reference, string description)
		{
			using (var sha1 = System.Security.Cryptography.SHA1.Create())
			{
				var raw = Encoding.ASCII.GetBytes($"{date.ToString("o")}|{amount:F4)}|{reference}|{description}");
				var hash = sha1.ComputeHash(raw);

				return hash.ToBase32();
			}
		}
	}
}