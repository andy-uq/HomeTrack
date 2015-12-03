using System.Collections.Generic;
using FluentAssertions;

namespace HomeTrack.SqlStore.Tests
{
	public class TransactionTests
	{
		private readonly IGeneralLedgerRepository _repository;

		public TransactionTests(IGeneralLedgerRepository repository)
		{
			_repository = repository;
		}

		public void AddTransaction()
		{
			var t1 = new Transaction(TestData.Expenses, TestData.Bank, 10M) { Reference = "A", Description = "Misc. expenses" };
			_repository.Post(t1);
		}

		public void SearchAccountTransactions()
		{
			var ledger = new GeneralLedger(_repository);

			var t1 = new Transaction(TestData.Expenses, TestData.Bank, 10M) { Reference = "A", Description = "Buy some stuff" };
			ledger.Post(t1);

			var t2 = new Transaction(TestData.Bank, TestData.Bank, 10M) { Reference = "B", Description = "Withdraw money" };
			ledger.Post(t2);

			var bankTransactions = ledger.GetTransactions(TestData.Bank.Id);
			bankTransactions.Should().Equal(new[] { t1, t2 }, (x, y) => x.Id == y.Id);

			var mortgageTransactions = ledger.GetTransactions(TestData.Expenses.Id);
			mortgageTransactions.Should().Equal(new[] { t1 }, (x, y) => x.Id == y.Id);
		}
	}
}