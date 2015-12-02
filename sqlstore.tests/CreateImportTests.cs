using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;

namespace HomeTrack.SqlStore.Tests
{
	public class CreateImportTests
	{
		private readonly ImportRepository _repository;

		public CreateImportTests(ImportRepository repository)
		{
			_repository = repository;
			var import = new ImportResult { Date = DateTime.Now, ImportType = "", Name = "Import", };
			var transaction = new Transaction { Date = new DateTime(2010, 1, 1), Amount = 100, Reference = "ABCD", Description = "Transaction" };
			transaction.Credit.Add(new Amount(TestData.Bank, EntryType.Credit, 100));
			transaction.Debit.Add(new Amount(TestData.Expenses, EntryType.Debit, 100));

			var transactions = new[] { transaction };
			_repository.Save(import, transactions);
		}

		public void GetImport()
		{
			var import = _repository.GetAll().Single(x => x.Name == "Import");
			import.Should().NotBeNull();
		}

		public void GetImportedTransactions()
		{
			var import = _repository.GetAll().Single(x => x.Name == "Import");
			var transactions = _repository.GetTransactionIds(import.Id).AsList();
			transactions.Should().NotBeEmpty();
			transactions.Select(t => t.Id).Should().Contain("hzfj51vcn2y6dkft9h642y12b8d5_8zp");
		}

		public void GetTransactions()
		{
			var import = _repository.GetAll().Single(x => x.Name == "Import");
			var transactions = _repository.GetTransactions(import.Id).AsList();
			transactions.Should().NotBeEmpty();
			transactions.Select(t => t.Id).Should().Contain("hzfj51vcn2y6dkft9h642y12b8d5_8zp");
		}
	}

	public class CreateImportAsyncTests
	{
		private readonly ImportRepository _repository;

		public CreateImportAsyncTests(ImportRepository repository)
		{
			_repository = repository;
		}

		private Task CreateImportAsync()
		{
			var import = new ImportResult { Date = DateTime.Now, ImportType = "", Name = "Import", };
			var transaction = new Transaction { Date = new DateTime(2010, 1, 1), Amount = 100, Reference = "ABCD", Description = "Transaction" };
			transaction.Credit.Add(new Amount(TestData.Bank, EntryType.Credit, 100));
			transaction.Debit.Add(new Amount(TestData.Expenses, EntryType.Debit, 100));

			var transactions = new[] { transaction };
			return _repository.SaveAsync(import, transactions);
		}

		public async Task GetImportAsync()
		{
			await CreateImportAsync();

			var imports = await _repository.GetAllAsync();

			var import = imports.Single(x => x.Name == "Import");
			import.Should().NotBeNull();
		}

		public async Task GetImportedTransactionsAsync()
		{
			await CreateImportAsync();

			var imports = await _repository.GetAllAsync();
			var import = imports.Single(x => x.Name == "Import");

			var transactions = (await _repository.GetTransactionIdsAsync(import.Id)).AsList();
			transactions.Should().NotBeEmpty();
			transactions.Select(t => t.Id).Should().Contain("hzfj51vcn2y6dkft9h642y12b8d5_8zp");
		}

		public async Task GetTransactionsAsync()
		{
			await CreateImportAsync();

			var import = _repository.GetAll().Single(x => x.Name == "Import");
			var transactions = _repository.GetTransactions(import.Id).AsList();
			transactions.Should().NotBeEmpty();
			transactions.Select(t => t.Id).Should().Contain("hzfj51vcn2y6dkft9h642y12b8d5_8zp");
		}
	}
}