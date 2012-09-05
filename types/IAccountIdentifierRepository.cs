using System.Collections.Generic;

namespace HomeTrack
{
	public interface IAccountIdentifierRepository
	{
		void AddOrUpdate(AccountIdentifier identifier);
		IEnumerable<AccountIdentifier> GetAll();
		void Remove(int id);
		AccountIdentifier GetById(int id);
	}
}