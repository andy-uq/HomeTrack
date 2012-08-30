using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace HomeTrack.RavenStore
{
	public class AccountIdentifierRepository : IAccountIdentifierRepository
	{
		private readonly IMappingEngine _mappingEngine;
		private readonly RavenRepository _repository;

		public AccountIdentifierRepository(RavenRepository repository, IMappingEngine mappingEngine)
		{
			_repository = repository;
			_mappingEngine = mappingEngine;
		}

		#region IAccountIdentifierRepository Members

		public void Add(AccountIdentifier identifier)
		{
			var document = _mappingEngine.Map<Documents.AccountIdentifier>(identifier);
			_repository.UseOnceTo(session => session.Store(document), saveChanges: true);
		}

		public IEnumerable<AccountIdentifier> GetAll()
		{
			var documents = _repository.UseOnceTo(x => x.Query<Documents.AccountIdentifier>().ToArray());
			return documents.Hydrate<AccountIdentifier>(_mappingEngine);
		}

		#endregion
	}
}