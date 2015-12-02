using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeTrack.Core
{
	public class TransactionImportContext
	{
		private readonly AccountIdentifier[] _patterns;

		public GeneralLedger General { get; }
		public IImportAsyncRepository Repository { get; }
		public IEnumerable<AccountIdentifier> Patterns => _patterns;

		public TransactionImportContext(GeneralLedger general, IEnumerable<AccountIdentifier> patterns, IImportAsyncRepository repository)
		{
			General = general;
			Repository = repository;
			_patterns = patterns.ToArray();
		}

		public TransactionImport CreateImport(Account source, Account unclassifiedDestination = null)
		{
			return new TransactionImport(this, source) { UnclassifedDestination = unclassifiedDestination };
		}
	}
}