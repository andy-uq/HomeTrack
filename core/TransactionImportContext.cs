using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeTrack.Core
{
	public class TransactionImportContext
	{
		private readonly GeneralLedger _general;
		private readonly AccountIdentifier[] _patterns;
		private readonly IImportRepository _repository;

		public GeneralLedger General
		{
			get { return _general; }
		}

		public IImportRepository Repository
		{
			get { return _repository; }
		}

		public IEnumerable<AccountIdentifier> Patterns
		{
			get { return _patterns; }
		}

		public TransactionImportContext(GeneralLedger general, IEnumerable<AccountIdentifier> patterns, IImportRepository repository)
		{
			_general = general;
			_repository = repository;
			_patterns = patterns.ToArray();

			Array.ForEach(_patterns, p => p.Account = general[p.Account.Id]);
		}

		public TransactionImport CreateImport(Account source, Account unclassifiedDestination = null)
		{
			return new TransactionImport(this, source) { UnclassifedDestination = unclassifiedDestination };
		}
	}
}