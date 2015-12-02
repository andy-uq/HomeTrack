using System;

namespace HomeTrack.Core
{
	public static class BudgetAllocator
	{
		public static Transaction Allocate(this Budget budgetInfo, Account budgetAccount)
		{
			return new Transaction(budgetInfo.BudgetAccount, budgetAccount, budgetInfo.Amount);
		}

		public static Transaction Pay(this Budget budgetInfo, Account budgetAccount, Account payableFrom, decimal value)
		{
			var realTransaction = new Transaction(budgetInfo.RealAccount, payableFrom, value);
			realTransaction.Debit.Add(new Amount(budgetAccount, EntryType.Debit, value));
			realTransaction.Credit.Add(new Amount(budgetInfo.BudgetAccount, EntryType.Credit, value));

			return realTransaction;
		}
	}
}