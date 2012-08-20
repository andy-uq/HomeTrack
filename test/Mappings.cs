﻿using AutoMapper;
using HomeTrack.RavenStore;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	[TestFixture]
	public class Mappings
	{
		private MappingEngine _mappingEngine;

		[SetUp]
		public void SetUp()
		{
			var registerMappings = new RegisterMappings();
			var configuration = registerMappings.GetMappings();

			_mappingEngine = new AutoMapper.MappingEngine(configuration);
		}

		[Test]
		public void AccountEntityToRaven()
		{
			var account = AccountFactory.Debit("Bank");
			var raven = _mappingEngine.Map<HomeTrack.RavenStore.Account>(account);

			Assert.That(raven.Name, Is.EqualTo(account.Name));
			Assert.That(raven.Description, Is.EqualTo(account.Description));
			Assert.That(raven.Type, Is.EqualTo(account.Type));
			Assert.That(raven.Balance, Is.EqualTo(account.Balance));
			Assert.That(raven.Id, Is.EqualTo(account.Name.ToLower()));
		}

		[Test]
		public void AmountEntityToRaven()
		{
			var account = AccountFactory.Debit("Bank");
			account.Id = "bank";

			var amount = new Amount(account, 100M);

			var raven = _mappingEngine.Map<HomeTrack.RavenStore.Amount>(amount);

			Assert.That(raven.AccountId, Is.EqualTo("accounts/bank"));
			Assert.That(raven.AccountName, Is.EqualTo(account.Name));
			Assert.That(raven.Direction, Is.EqualTo(amount.Direction));
			Assert.That(raven.Value, Is.EqualTo(amount.Value));
		}

		[Test]
		public void TransactionEntityToRaven()
		{
			var debit = AccountFactory.Debit("Bank");
			debit.Id = "bank";
			var credit = AccountFactory.Credit("Mortgage");
			credit.Id = "mortgage";

			var transaction = new Transaction(debit, credit, 100M);
			var raven = _mappingEngine.Map<HomeTrack.RavenStore.Transaction>(transaction);

			Assert.That(raven.Amount, Is.EqualTo(100M));
			Assert.That(raven.Credit, Is.Not.Empty);
			Assert.That(raven.Credit[0].AccountId, Is.EqualTo("accounts/mortgage"));
			Assert.That(raven.Credit[0].AccountName, Is.EqualTo("Mortgage"));
			Assert.That(raven.Credit[0].Direction, Is.EqualTo(EntryType.Credit));
			Assert.That(raven.Credit[0].Value, Is.EqualTo(100M));
			
			Assert.That(raven.Debit, Is.Not.Empty);
			Assert.That(raven.Debit[0].AccountId, Is.EqualTo("accounts/bank"));
			Assert.That(raven.Debit[0].AccountName, Is.EqualTo("Bank"));
			Assert.That(raven.Debit[0].Direction, Is.EqualTo(EntryType.Debit));
			Assert.That(raven.Debit[0].Value, Is.EqualTo(100M));
		}

		[Test]
		public void RavenAmountToEntity()
		{
			var amount = new HomeTrack.RavenStore.Amount { AccountId = "accounts/bank", AccountName = "Bank", Direction = EntryType.Debit, Value = 100M };
			var entity = _mappingEngine.Map<HomeTrack.Amount>(amount);

			Assert.That(entity.Account.Id, Is.EqualTo("bank"));
			Assert.That(entity.Account.Name, Is.EqualTo("Bank"));
			Assert.That(entity.Direction, Is.EqualTo(EntryType.Debit));
			Assert.That(entity.Value, Is.EqualTo(100M));
		}

		[Test]
		public void AccountRavenToEntity()
		{
			var account = new HomeTrack.RavenStore.Account { Id = "accounts/bank", Name = "Bank", Balance = 1000M, Type = AccountType.Asset };
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