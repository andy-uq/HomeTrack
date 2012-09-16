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
	}
}