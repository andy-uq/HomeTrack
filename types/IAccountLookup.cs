using System.Collections.Generic;

namespace HomeTrack
{
	public interface IAccountLookup : IEnumerable<Account>
	{
		 Account this[string accountId] { get; }
	}
}