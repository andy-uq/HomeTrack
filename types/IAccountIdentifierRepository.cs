using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeTrack
{
	public interface IAccountIdentifierRepository
	{
		void AddOrUpdate(AccountIdentifier identifier);
		IEnumerable<AccountIdentifier> GetAll();
		void Remove(int id);
		AccountIdentifier GetById(int id);
	}

	public interface IAccountIdentifierAsyncRepository
	{
		Task AddOrUpdateAsync(AccountIdentifier identifier);
		Task<IEnumerable<AccountIdentifier>> GetAllAsync();
		Task RemoveAsync(int id);
		Task<AccountIdentifier> GetByIdAsync(int id);
	}
}