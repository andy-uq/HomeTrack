using System;
using System.Linq;
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
			var transactions = new[] { new Transaction { Date = new DateTime(2010, 1, 1), Amount = 100, Reference = "ABCD", Description = "Transaction" } };
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
}