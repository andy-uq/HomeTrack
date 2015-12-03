using System.Threading.Tasks;
using FluentAssertions;

namespace HomeTrack.SqlStore.Tests
{
	public class TransactionAsyncTests
	{
		private readonly GeneralLedgerRepository _repository;

		public TransactionAsyncTests(GeneralLedgerRepository repository)
		{
			_repository = repository;
		}

		public async Task AddTransactionAsync()
		{
			var t1 = new Transaction(TestData.Expenses, TestData.Bank, 10M) { Reference = "A", Description = "Misc. expenses" };
			await _repository.PostAsync(t1);
		}

		public async Task SearchAccountTransactionsAsync()
		{
			var ledger = new AsyncGeneralLedger(_repository);

			var t1 = new Transaction(TestData.Expenses, TestData.Bank, 10M) { Reference = "A", Description = "Buy some stuff" };
			await ledger.PostAsync(t1);

			var t2 = new Transaction(TestData.Bank, TestData.Bank, 10M) { Reference = "B", Description = "Withdraw money" };
			await ledger.PostAsync(t2);

			var bankTransactions = await ledger.GetTransactionsAsync(TestData.Bank.Id);
			bankTransactions.Should().Equal(new[] { t1, t2 }, (x, y) => x.Id == y.Id);

			var mortgageTransactions = await ledger.GetTransactionsAsync(TestData.Expenses.Id);
			mortgageTransactions.Should().Equal(new[] { t1 }, (x, y) => x.Id == y.Id);

			var balanced = await ledger.TrialBalanceAsync();
			balanced.Should().BeTrue();
		}
	}
}