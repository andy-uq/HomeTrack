using System;
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
			using ( var unitOfWork = _repository.DocumentStore.OpenSession() )
			{
				unitOfWork.Store(document);
				unitOfWork.SaveChanges();

				identifier.Id = document.Id;
			}
		}

		public IEnumerable<AccountIdentifier> GetAll()
		{
			var documents = _repository.UseOnceTo(x => x.Query<Documents.AccountIdentifier>().ToArray());
			return documents.Hydrate<AccountIdentifier>(_mappingEngine);
		}

		public void Remove(string id)
		{
			if ( string.IsNullOrEmpty(id) )
				throw new ArgumentNullException("id");

			_repository.UseOnceTo(x => {
				var e = x.Load<Documents.AccountIdentifier>(id);
				x.Delete(e);
			}, saveChanges: true);
		}

		#endregion
	}
}