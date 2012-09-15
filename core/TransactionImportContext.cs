using System.Collections.Generic;
using System.Linq;

namespace HomeTrack.Core
{
	public class TransactionImportContext
	{
		private readonly GeneralLedger _general;
		private readonly AccountIdentifier[] _patterns;

		public GeneralLedger General
		{
			get { return _general; }
		}

		public IEnumerable<AccountIdentifier> Patterns
		{
			get { return _patterns; }
		}

		public TransactionImportContext(GeneralLedger general, IEnumerable<AccountIdentifier> patterns)
		{
			_general = general;
			_patterns = patterns.ToArray();
		}

		public TransactionImport CreateImport(Account source, Account unclassifiedDestination = null)
		{
			return new TransactionImport(this, source) { UnclassifedDestination = unclassifiedDestination };
		}
	}
}