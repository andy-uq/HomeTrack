using System;
using AutoMapper;
using HomeTrack.RavenStore;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class ImportMappingTests
	{
		private IMappingEngine _mappingEngine;

		[SetUp]
		public void SetUp()
		{
			var typeMapProvider = new RavenEntityTypeMapProvider();
			_mappingEngine = new MappingProvider(typeMapProvider).Build();
		}

		[Test]
		public void TransactionToImportedTransaction()
		{
			var t = new Transaction { Id = "101", Reference = "I1" };
			var document = _mappingEngine.Map<ImportedTransaction>(t);

			Assert.That(document.Id, Is.EqualTo("I1"));
			Assert.That(document.TransactionId, Is.EqualTo("101"));
		}

		[Test]
		public void ImportResultToRavenDocument()
		{
			var now = DateTime.Now;
			var importResult = new ImportResult {Date = now, Name = "Name", ImportType = "ImportType", TransactionCount = 100, UnclassifiedTransactions = 10};
			var document = _mappingEngine.Map<RavenStore.Documents.ImportResult>(importResult);

			Assert.That(document.Date, Is.EqualTo(now));
			Assert.That(document.Name, Is.EqualTo("Name"));
			Assert.That(document.ImportType, Is.EqualTo("ImportType"));
			Assert.That(document.Id, Is.EqualTo("imports/name"));
			Assert.That(document.TransactionCount, Is.EqualTo(100));
			Assert.That(document.UnclassifiedTransactions, Is.EqualTo(10));
		}

		[Test]
		public void RavenDocumentToImportResult()
		{
			var now = DateTime.Now;
			var document = new RavenStore.Documents.ImportResult { Date = DateTime.Now, Name = "Name", TransactionCount = 100, UnclassifiedTransactions = 10 };
			var result = _mappingEngine.Map<ImportResult>(document);

			Assert.That(result.Date, Is.EqualTo(now));
			Assert.That(result.Name, Is.EqualTo("Name"));
			Assert.That(result.TransactionCount, Is.EqualTo(100));
			Assert.That(result.UnclassifiedTransactions, Is.EqualTo(10));
		}
	}
}