using System.Collections.Generic;

namespace HomeTrack
{
	public interface IAccountIdentifierRepository
	{
		void Add(AccountIdentifier identifier);
		IEnumerable<AccountIdentifier> GetAll();
	}
}