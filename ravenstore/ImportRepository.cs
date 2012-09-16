using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace HomeTrack.RavenStore
{
	public class ImportRepository : IImportRepository
	{
		private readonly RavenRepository _repository;
		private readonly IMappingEngine _mappingEngine;

		public ImportRepository(RavenRepository repository, IMappingEngine mappingEngine)
		{
			_repository = repository;
			_mappingEngine = mappingEngine;
		}

		public void Save(ImportResult result, IEnumerable<Transaction> transactions)
		{
			using (var session = _repository.DocumentStore.OpenSession())
			{
				var document = _mappingEngine.Map<Documents.ImportResult>(result);
				document.Transactions =
					(
						from t in transactions
						select new Documents.ImportedTransaction
						{
							Id = t.Reference, 
							TransactionId = t.Id
						}
					).ToArray();

				session.Store(document);
				session.SaveChanges();
			}
		}
	}
}