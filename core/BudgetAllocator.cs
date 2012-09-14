﻿using System;

namespace HomeTrack.Core
{
	public static class BudgetAllocator
	{
		public static Transaction Allocate(this Budget budgetInfo, Account budgetAccount)
		{
			return new Transaction(budgetInfo.BudgetAccount, budgetAccount, budgetInfo.Amount);
		}

		public static Tuple<Transaction, Transaction> Pay(this Budget budgetInfo, Account budgetAccount, Account payableFrom, decimal value)
		{
			var realTransaction = new Transaction(budgetInfo.RealAccount, payableFrom, value);
			var budgetAdjustment = new Transaction(budgetAccount, budgetInfo.BudgetAccount, value);

			return new Tuple<Transaction, Transaction>(realTransaction, budgetAdjustment);
		}
	}
}