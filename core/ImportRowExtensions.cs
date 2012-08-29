using System.Collections.Generic;
using System.Linq;

namespace HomeTrack.Core
{
	public static class ImportRowExtensions
	{
		public static Account IdentifyAccount(this IImportRow row, IEnumerable<AccountIdentifier> accountIdentifiers )
		{
			return 
				(
					from identifer in accountIdentifiers 
					where identifer.IsMatch(row) 
					select identifer.Account
				).FirstOrDefault();
		}
	}
}