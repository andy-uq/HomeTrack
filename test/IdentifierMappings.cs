using AutoMapper;
using HomeTrack.Core;
using HomeTrack.RavenStore;
using HomeTrack.RavenStore.Documents;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class IdentifierMappings
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			var typeMapProvider = new RavenEntityTypeMapProvider();
			_mappingEngine = new MappingProvider(typeMapProvider).Build();
		}

		#endregion

		private IMappingEngine _mappingEngine;

		[Test]
		public void AccountIdentifierToRavenDocument()
		{
			var i = new AccountIdentifier {Account = AccountFactory.Expense("Groceries"), Pattern = new CompositePattern()};
			var document = _mappingEngine.Map<RavenStore.Documents.AccountIdentifier>(i);

			Assert.That(document.AccountId, Is.EqualTo(i.Account.Id));
			Assert.That(document.AccountName, Is.EqualTo(i.Account.Name));
		}

		[Test]
		public void AmountPatternToRavenDocument()
		{
			var p = new AmountPattern {Amount = 10M};
			var document = _mappingEngine.Map<Pattern>(p);

			Assert.That(document.Name, Is.EqualTo("Amount"));
			Assert.That(document.Properties, Has.Some.Property("Key").EqualTo("Amount").With.Some.Property("Value").EqualTo(10M));
		}

		[Test]
		public void FieldPatternToRavenDocument()
		{
			var pattern = new FieldPattern {Name = "Field", Pattern = "Pattern"};
			var document = _mappingEngine.Map<Pattern>(pattern);

			Assert.That(document.Name, Is.EqualTo("Field"));
			Assert.That(document.Properties.Values, Is.EquivalentTo(new[] { "Field", "Pattern" }));
		}

		[Test]
		public void RavenDocumentToFieldPattern()
		{
			var document = new Pattern { Name = "Field", Properties = { { "Name", "Field" }, { "Pattern", "Pattern" } } };
			var p = _mappingEngine.Map<IPattern>(document);

			Assert.That(p, Is.InstanceOf<FieldPattern>());

			var fieldPattern = (FieldPattern)p;
			Assert.That(fieldPattern.Name, Is.EqualTo("Field"));
			Assert.That(fieldPattern.Pattern, Is.EqualTo("Pattern"));
		}

		[Test]
		public void RavenDocumentToCompositePattern()
		{
			var c1 = new Pattern { Name = "Field", Properties = { { "Name", "Field" }, { "Pattern", "Pattern" } } };
			var c2 = new Pattern { Name = "Amount", Properties = { { "Amount", 10M } } };
			var document = new Pattern { Name = "Composite", Child = new[] { c1, c2 }};
			var p = _mappingEngine.Map<IPattern>(document);

			Assert.That(p, Is.InstanceOf<CompositePattern>());

			var compositePattern = (CompositePattern)p;
			Assert.That(compositePattern, Is.Not.Empty);
			Assert.That(compositePattern, Has.Member(new FieldPattern { Name = "Field", Pattern = "Pattern" }).Using(new PatternComparer()));
			Assert.That(compositePattern, Has.Member(new AmountPattern { Amount = 10M }).Using(new PatternComparer()));
		}

		[Test]
		public void RavenDocumentToAccountIdentifier()
		{
			var document = new RavenStore.Documents.AccountIdentifier {AccountId = "groceries", AccountName = "Groceries"};
			var identifier = _mappingEngine.Map<AccountIdentifier>(document);

			Assert.That(identifier.Account.Id, Is.EqualTo(document.AccountId));
			Assert.That(identifier.Account.Name, Is.EqualTo(document.AccountName));
		}

		[Test]
		public void RavenDocumentToAmountPattern()
		{
			var document = new Pattern {Name = "Amount", Properties = {{"Amount", 10M}}};
			var p = _mappingEngine.Map<IPattern>(document);

			Assert.That(p, Is.InstanceOf<AmountPattern>());

			var amount = (AmountPattern) p;
			Assert.That(amount.Amount, Is.EqualTo(10M));
		}
	}
}