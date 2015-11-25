using System;
using System.Linq;
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
			var transactions = new[] { new Transaction { Date = DateTime.Now, Amount = 100, Reference = "ABCD", Description = "Transaction" } };
			_repository.Save(import, transactions);
		}

		public void GetImport()
		{
			var import = _repository.GetAll().Single(x => x.Name == "Import");
			import.Should().NotBeNull();
		}

		public void GetImportTransactions()
		{
			var import = _repository.GetAll().Single(x => x.Name == "Import");
			var transactions = _repository.GetTransactionIds(import.Id);
		}
	}
}