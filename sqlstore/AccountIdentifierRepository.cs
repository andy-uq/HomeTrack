using System.Collections.Generic;
using HomeTrack;

namespace sqlstore
{
	public class AccountIdentifierRepository : IAccountIdentifierRepository
	{
		public void AddOrUpdate(AccountIdentifier identifier)
		{
		}

		public IEnumerable<AccountIdentifier> GetAll()
		{
			yield break;
		}

		public void Remove(int id)
		{
		}

		public AccountIdentifier GetById(int id)
		{
			return null;
		}
	}
}