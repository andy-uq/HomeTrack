using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Raven.Client.Linq;

namespace HomeTrack.RavenStore
{
	public class GeneralLedgerRepository : IGeneralLedgerRepository
	{
		private readonly RavenRepository _repository;
		private readonly IMappingEngine _mappingEngine;

		public GeneralLedgerRepository(RavenRepository repository, IMappingEngine mappingEngine)
		{
			_repository = repository;
			_mappingEngine = mappingEngine;
		}

		public IEnumerable<HomeTrack.Account> Accounts
		{
			get
			{
				var accounts = _repository.UseOnceTo(x => x.Query<Documents.Account>().ToArray());
				return accounts.Hydrate<HomeTrack.Account>(_mappingEngine);
			}
		}

		public IEnumerable<HomeTrack.Account> DebitAccounts
		{
			get
			{
				var accounts = _repository.UseOnceTo(session => session.Query<Documents.Account>().ToArray().Where(x => x.Type.IsDebitOrCredit() == EntryType.Debit));
				return accounts.Hydrate<HomeTrack.Account>(_mappingEngine);
			}
		}

		public IEnumerable<HomeTrack.Account> CreditAccounts
		{
			get
			{
				var accounts = _repository.UseOnceTo(session => session.Query<Documents.Account>().ToArray().Where(x => x.Type.IsDebitOrCredit() == EntryType.Credit));
				return accounts.Hydrate<HomeTrack.Account>(_mappingEngine);
			}
		}

		public HomeTrack.Account GetAccount(string accountId)
		{
			var qualifiedId = QualifiedId("accounts", accountId);
			var account = _repository.UseOnceTo(x => x.Load<Documents.Account>(qualifiedId));
			return _mappingEngine.Map<HomeTrack.Account>(account);
		}

		public IEnumerable<Budget> GetBudgetsForAccount(string accountId)
		{
			using ( var session = _repository.DocumentStore.OpenSession() )
			{
				var budgets =
					(
						from budget in session.Query<Documents.Budget>()
						where budget.AccountId == accountId
						select budget
					).ToArray();

				return budgets.Hydrate<HomeTrack.Budget>(_mappingEngine);
			}
		}

		public string Add(HomeTrack.Account account)
		{
			var document = _mappingEngine.Map<Documents.Account>(account);
			_repository.UseOnceTo(x => x.Store(document), saveChanges: true);

			return FromQualifiedId(document.Id);
		}

		public void AddBudget(Budget budget)
		{
			var document = _mappingEngine.Map<Documents.Budget>(budget);
			_repository.UseOnceTo(x => x.Store(document), saveChanges: true);
		}

		public bool Post(Transaction transaction)
		{
			if (transaction.Credit.Concat(transaction.Debit).Any(x => x.Account == null || x.Account.Id == null))
				throw new InvalidOperationException("Cannot add a transaction where one or more accounts have a null Id");

			if (transaction.Check())
			{
				using (var session = _repository.DocumentStore.OpenSession())
				{
					var accountIds = transaction.Credit
						.Concat(transaction.Debit)
						.Select(x => QualifiedId("accounts", x.Account.Id))
						.Distinct()
						.ToArray();

					var accounts = session.Load<Documents.Account>(accountIds).ToArray();
					
					for (var i = 0; i < accountIds.Length; i++)
					{
						if (accounts[i] != null)
						{
							continue;
						}
						
						var allAccounts = session.Query<Documents.Account>().Select(x => x.Id);
						throw new InvalidOperationException(string.Format("Cannot find account {0} from ({1})",
						                                                  accountIds[i],
						                                                  string.Join(", ", allAccounts)));
					}

					var accountLookup = accounts.ToDictionary(x => FromQualifiedId(x.Id));

					foreach (var value in transaction.Debit)
					{
						value.Post();
						accountLookup[value.Account.Id].Balance = value.Account.Balance;
					}

					foreach (var value in transaction.Credit)
					{
						value.Post();
						accountLookup[value.Account.Id].Balance = value.Account.Balance;
					}

					var ravenEntity = _mappingEngine.Map<Documents.Transaction>(transaction);
					session.Store(ravenEntity);
					session.SaveChanges();

					transaction.Id = ravenEntity.Id;
					return true;
				}
			}

			return false;
		}

		public IEnumerable<HomeTrack.Transaction> GetTransactions(string accountId)
		{
			accountId = QualifiedId("accounts", accountId);

			using ( var session = _repository.DocumentStore.OpenSession() )
			{
				var query =
					(
						from t in session.Query<Documents.Transaction>()
						where
							t.Credit.Any(x => x.AccountId == accountId)
							|| t.Debit.Any(x => x.AccountId == accountId)
						orderby t.Date, t.Id
						select t
					);

				return query
					.Hydrate<HomeTrack.Transaction>(_mappingEngine)
					.ToArray();
			}
		}

		public HomeTrack.Transaction GetTransaction(int id)
		{
			return _repository.UseOnceTo(s => s.Load<Documents.Transaction>(id).Hydrate<HomeTrack.Transaction>(_mappingEngine));
		}

		private static string QualifiedId(string @namespace, string id)
		{
			return string.Concat(@namespace + "/", id);
		}

		private static string FromQualifiedId(string id)
		{
			return id.Substring(id.LastIndexOf('/') + 1);
		}

		public void Dispose()
		{
			_repository.Dispose();
		}
	}
}