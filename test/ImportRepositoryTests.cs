using System;
using System.Linq;
using HomeTrack.RavenStore;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class ImportRepositoryTests : RavenRepositoryTests
	{
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			ImportRepository = new ImportRepository(Repository, MappingEngine);
		}

		private ImportRepository ImportRepository { get; set; }

		[Test]
		public void SaveImportResult()
		{
			var result = new ImportResult {Date = DateTimeServer.Now, Name = "I", TransactionCount = 2, UnclassifiedTransactions = 0};
			ImportRepository.Save(result, new[] { new Transaction { Id = 101, Reference = "I/1" }, new Transaction { Id = 102, Reference = "I/2" },  });

			Assert.That(Repository.UseOnceTo(x => x.Query<RavenStore.Documents.ImportResult>().Customize(options => options.WaitForNonStaleResults())), Is.Not.Empty);
		}

		[Test]
		public void GetImportResults()
		{
			AddData();

			var results = ImportRepository.GetAll();
			Assert.That(results.Select(x => x.Name), Is.EqualTo(new[] { "I1", "I2", "I3" }));
		}

		[Test]
		public void GetTransactions()
		{
			AddData();

			var transactions = ImportRepository.GetTransactions("i1").ToArray();
			Assert.That(transactions.Length, Is.EqualTo(2));
			Assert.That(transactions, Has.None.Null);
			Assert.That(transactions.Select(x => x.Id), Is.EqualTo(new[] { 101, 102 }));
		}

		private void AddData()
		{
			var t1 = new Transaction { Id = 101, Reference = "I1/1" };
			var t2 = new Transaction { Id = 102, Reference = "I1/2" };

			Repository.DocumentStore.UseOnceTo(s => s.Store(MappingEngine.Map<RavenStore.Documents.Transaction>(t1)), saveChanges: true);
			Repository.DocumentStore.UseOnceTo(s => s.Store(MappingEngine.Map<RavenStore.Documents.Transaction>(t2)), saveChanges: true);

			var result = new ImportResult {Date = DateTimeServer.Now, Name = "I1", TransactionCount = 2, UnclassifiedTransactions = 0};
			ImportRepository.Save(result, new[] {t1, t2});

			result.Name = "I2";
			ImportRepository.Save(result, new[] {new Transaction {Id = 101, Reference = "I2/1"}, new Transaction {Id = 102, Reference = "I2/2"},});

			result.Name = "I3";
			ImportRepository.Save(result, new[] {new Transaction {Id = 101, Reference = "I3/1"}, new Transaction {Id = 102, Reference = "I3/2"},});
		}
	}
}