using System;
using AutoMapper;
using HomeTrack.RavenStore;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class TransactionMappings
	{
		private IMappingEngine _mappingEngine;

		[SetUp]
		public void SetUp()
		{
			var typeMapProvider = new RavenEntityTypeMapProvider();
			_mappingEngine = new MappingProvider(typeMapProvider).Build();
		}

		[Test]
		public void AccountToRavenDocument()
		{
			var account = AccountFactory.Asset("Bank");
			var document = _mappingEngine.Map<HomeTrack.RavenStore.Documents.Account>(account);

			Assert.That(document.Name, Is.EqualTo(account.Name));
			Assert.That(document.Description, Is.EqualTo(account.Description));
			Assert.That(document.Type, Is.EqualTo(account.Type));
			Assert.That(document.Balance, Is.EqualTo(account.Balance));
			Assert.That(document.Id, Is.EqualTo("accounts/bank"));
		}

		[Test]
		public void AmountToRavenDocument()
		{
			var account = AccountFactory.Asset("Bank");
			account.Id = "bank";

			var amount = new Amount { Account = account, Direction = EntryType.Debit, Value = 10M };
			var document = _mappingEngine.Map<HomeTrack.RavenStore.Documents.Amount>(amount);

			Assert.That(document.AccountId, Is.EqualTo("accounts/bank"));
			Assert.That(document.AccountName, Is.EqualTo(account.Name));
			Assert.That(document.Direction, Is.EqualTo(amount.Direction));
			Assert.That(document.Value, Is.EqualTo(amount.Value));
		}

		[Test]
		public void TransactionToRavenDocument()
		{
			var debit = AccountFactory.Asset("Bank");
			debit.Id = "bank";
			var credit = AccountFactory.Liability("Mortgage");
			credit.Id = "mortgage";

			var transaction = new Transaction(debit, credit, 100M) { Description = "description", Reference = "r1" };
			var document = _mappingEngine.Map<HomeTrack.RavenStore.Documents.Transaction>(transaction);

			Assert.That(document.Amount, Is.EqualTo(100M));
			Assert.That(document.Description, Is.EqualTo("description"));
			Assert.That(document.Reference, Is.EqualTo("r1"));
			
			Assert.That(document.Credit, Is.Not.Empty);
			Assert.That(document.Credit[0].AccountId, Is.EqualTo("accounts/mortgage"));
			Assert.That(document.Credit[0].AccountName, Is.EqualTo("Mortgage"));
			Assert.That(document.Credit[0].Direction, Is.EqualTo(EntryType.Credit));
			Assert.That(document.Credit[0].Value, Is.EqualTo(100M));
			
			Assert.That(document.Debit, Is.Not.Empty);
			Assert.That(document.Debit[0].AccountId, Is.EqualTo("accounts/bank"));
			Assert.That(document.Debit[0].AccountName, Is.EqualTo("Bank"));
			Assert.That(document.Debit[0].Direction, Is.EqualTo(EntryType.Debit));
			Assert.That(document.Debit[0].Value, Is.EqualTo(100M));
		}

		[Test]
		public void RavenDocumentToTransaction()
		{
			var transaction = new HomeTrack.RavenStore.Documents.Transaction
			{
				Amount = 100M,
				Reference = "R1",
				Debit = new[] {new HomeTrack.RavenStore.Documents.Amount { AccountId = "accounts/bank", Direction = EntryType.Credit, Value = 50M }, new HomeTrack.RavenStore.Documents.Amount { AccountId = "accounts/cashonhand", Direction = EntryType.Credit, Value = 50M }  },
				Credit = new[] {new HomeTrack.RavenStore.Documents.Amount { AccountId = "accounts/mortgage",Direction = EntryType.Debit, Value = 100M } }
			};

			var entity = _mappingEngine.Map<HomeTrack.Transaction>(transaction);
			Assert.That(entity.Amount, Is.EqualTo(100M));
			Assert.That(entity.Reference, Is.EqualTo("R1"));
		}
		
		[Test]
		public void RavenDocumentToAmount()
		{
			var amount = new HomeTrack.RavenStore.Documents.Amount { AccountId = "accounts/bank", AccountName = "Bank", Direction = EntryType.Debit, Value = 100M };
			var entity = _mappingEngine.Map<HomeTrack.Amount>(amount);

			Assert.That(entity.Account.Id, Is.EqualTo("bank"));
			Assert.That(entity.Account.Name, Is.EqualTo("Bank"));
			Assert.That(entity.Direction, Is.EqualTo(EntryType.Debit));
			Assert.That(entity.Value, Is.EqualTo(100M));
		}

		[Test]
		public void RavenDocumentToAccount()
		{
			var account = new HomeTrack.RavenStore.Documents.Account { Id = "accounts/bank", Name = "Bank", Balance = 1000M, Type = AccountType.Asset };
			var entity = _mappingEngine.Map<Account>(account);

			Assert.That(entity.Id, Is.EqualTo("bank"));
			Assert.That(entity.Name, Is.EqualTo(account.Name));
			Assert.That(entity.Description, Is.EqualTo(account.Description));
			Assert.That(entity.Type, Is.EqualTo(account.Type));
			Assert.That(entity.Balance, Is.EqualTo(account.Balance));
			Assert.That(entity.Id, Is.EqualTo(account.Name.ToLower()));
		}
	}
}