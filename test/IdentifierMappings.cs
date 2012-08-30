using AutoMapper;
using HomeTrack.Core;
using HomeTrack.RavenStore;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class IdentifierMappings
	{
		private IMappingEngine _mappingEngine;

		[SetUp]
		public void SetUp()
		{
			var typeMapProvider = new RavenEntityTypeMapProvider();
			_mappingEngine = new MappingProvider(typeMapProvider).Build();
		}

		[Test]
		public void AccountIdentifierToRavenDocument()
		{
			var i = new AccountIdentifier {Account = AccountFactory.Expense("Groceries"), Pattern = new CompositePattern()};
			var document = _mappingEngine.Map<HomeTrack.RavenStore.Documents.AccountIdentifier>(i);

			Assert.That(document.AccountId, Is.EqualTo(i.Account.Id));
			Assert.That(document.AccountName, Is.EqualTo(i.Account.Name));
		}

		[Test]
		public void RavenDocumentToAccountIdentifier()
		{
			var document = new HomeTrack.RavenStore.Documents.AccountIdentifier { AccountId = "groceries", AccountName = "Groceries" };
			var identifier = _mappingEngine.Map<AccountIdentifier>(document);

			Assert.That(identifier.Account.Id, Is.EqualTo(document.AccountId));
			Assert.That(identifier.Account.Name, Is.EqualTo(document.AccountName));
		}

		[Test]
		public void FieldPatternToRavenDocument()
		{
			var pattern = new FieldPattern { Name = "Field", Pattern = "Pattern" };
			_mappingEngine.Map<FieldPattern>(pattern);
		}
	}
}