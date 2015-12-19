using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeTrack.Core
{
	public class TransactionImportContext
	{
		private readonly AsyncGeneralLedger _generalLedger;
		private readonly AccountIdentifier[] _patterns;

		public IImportAsyncRepository Repository { get; }
		public IEnumerable<AccountIdentifier> Patterns => _patterns;

		public TransactionImportContext(AsyncGeneralLedger generalLedger, IEnumerable<AccountIdentifier> patterns, IImportAsyncRepository repository)
		{
			_generalLedger = generalLedger;
			Repository = repository;
			_patterns = patterns.ToArray();
		}
		
		public TransactionImport CreateImport(Account source, Account unclassifiedDestination = null)
		{
			return new TransactionImport(this, source) { UnclassifedDestination = unclassifiedDestination };
		}

		public Task<bool> PostAsync(Transaction transaction)
		{
			return _generalLedger.PostAsync(transaction);
		}

		public Task<Account> GetAccountAsync(string account)
		{
			return _generalLedger.GetAccountAsync(account);
		}
	}
}