using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Raven.Client;

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

		public void AddOrUpdate(AccountIdentifier identifier)
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

		public void Remove(int id)
		{
			if ( id == 0 )
				throw new ArgumentNullException("id");

			using ( var unitOfWork = _repository.DocumentStore.OpenSession() )
			{
				var e = unitOfWork.Load<Documents.AccountIdentifier>(id);
				if (e == null)
				{
					return;
				}
				
				unitOfWork.Delete(e);
				unitOfWork.SaveChanges();
			}
		}

		public AccountIdentifier GetById(int id)
		{
			return _repository.UseOnceTo(x => x.Load<Documents.AccountIdentifier>(id).Hydrate<AccountIdentifier>(_mappingEngine));
		}

		#endregion
	}
}