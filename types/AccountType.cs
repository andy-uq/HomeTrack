using System;

namespace HomeTrack
{
	public enum AccountType
	{
		NotSet,
		Asset,
		Liability,
		Equity,
		Income,
		Expense
	}

	public static class AccountTypeExtensions
	{
		public static EntryType IsDebitOrCredit(this AccountType accountType)
		{
			switch ( accountType )
			{
				case AccountType.Asset:
				case AccountType.Expense:
					return EntryType.Debit;
				case AccountType.Equity:
				case AccountType.Income:
				case AccountType.Liability:
					return EntryType.Credit;
				default:
					throw new ArgumentOutOfRangeException("accountType");
			}
		}

		public static string ToCrDrString(this AccountType accountType)
		{
			switch ( accountType )
			{
				case AccountType.Asset:
				case AccountType.Expense:
					return "Dr";
				case AccountType.Equity:
				case AccountType.Income:
				case AccountType.Liability:
					return "Cr";
				default:
					throw new ArgumentOutOfRangeException("accountType");
			}
		}

		public static decimal? ToDr(this AccountType accountType, decimal value)
		{
			switch ( accountType )
			{
				case AccountType.Asset:
				case AccountType.Expense:
					return value >= 0 ? value : (decimal?) null;
				case AccountType.Equity:
				case AccountType.Income:
				case AccountType.Liability:
					return value < 0 ? -value : (decimal?)null;
				default:
					throw new ArgumentOutOfRangeException("accountType");
			}
		}

		public static decimal? ToCr(this AccountType accountType, decimal value)
		{
			switch ( accountType )
			{
				case AccountType.Asset:
				case AccountType.Expense:
					return value < 0 ? -value : (decimal?) null;
				case AccountType.Equity:
				case AccountType.Income:
				case AccountType.Liability:
					return value >= 0 ? value : (decimal?)null;
				default:
					throw new ArgumentOutOfRangeException("accountType");
			}
		}
	}

}