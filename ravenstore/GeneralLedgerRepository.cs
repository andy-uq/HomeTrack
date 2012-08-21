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
				var accounts = _repository.UseOnceTo(x => x.Query<Account>().ToArray());
				return accounts.Hydrate<HomeTrack.Account>(_mappingEngine);
			}
		}

		public IEnumerable<HomeTrack.Account> DebitAccounts
		{
			get { throw new System.NotImplementedException(); }
		}

		public IEnumerable<HomeTrack.Account> CreditAccounts
		{
			get { throw new System.NotImplementedException(); }
		}

		public HomeTrack.Account GetAccount(string accountId)
		{
			var qualifiedId = QualifiedId("accounts", accountId);
			var account = _repository.UseOnceTo(x => x.Load<Account>(qualifiedId));
			return _mappingEngine.Map<HomeTrack.Account>(account);
		}

		private static string QualifiedId(string @namespace, string id)
		{
			return string.Concat(@namespace + "/", id);
		}

		private static string FromQualifiedId(string id)
		{
			return id.Substring(id.LastIndexOf('/') + 1);
		}

		public string Add(HomeTrack.Account account)
		{
			var ravenEntity = _mappingEngine.Map<Account>(account);
			_repository.UseOnceTo(x => x.Store(ravenEntity), saveChanges: true);

			return FromQualifiedId(ravenEntity.Id);
		}

		public bool Post(HomeTrack.Transaction transaction)
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

					var accounts = session.Load<Account>(accountIds).ToArray();
					
					for (var i = 0; i < accountIds.Length; i++)
					{
						if ( accounts[i] == null )
						{
							var allAccounts = session.Query<Account>().Select(x => x.Id);
							throw new InvalidOperationException(string.Format("Cannot find account {0} from ({1})",
							                                                  accountIds[i],
							                                                  string.Join(", ", allAccounts)));
						}
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

					var ravenEntity = _mappingEngine.Map<Transaction>(transaction);
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
						from t in session.Query<Transaction>()
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
	}
}