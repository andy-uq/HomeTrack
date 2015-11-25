using System;
using System.Collections.Generic;
using System.Globalization;
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
						select _mappingEngine.Map<ImportedTransaction>(t)
					).ToArray();

				session.Store(document);
				session.SaveChanges();
			}
		}

		public IEnumerable<ImportResult> GetAll()
		{
			var results = _repository.UseOnceTo(x => x.Query<Documents.ImportResult>().ToArray());
			return results.Hydrate<HomeTrack.ImportResult>(_mappingEngine);
		}

		public IEnumerable<ImportedTransaction> GetTransactionIds(int importId)
		{
			using (var session = _repository.DocumentStore.OpenSession())
			{
				var import = session.Load<Documents.ImportResult>(QualifiedId("imports", importId));
				return import.Transactions;
			}
		}

		public IEnumerable<Transaction> GetTransactions(int importId)
		{
			using (var session = _repository.DocumentStore.OpenSession())
			{
				var import = session.Load<Documents.ImportResult>(QualifiedId("imports", importId));
				var transactions = session.Load<Documents.Transaction>(import.Transactions.Select(x => QualifiedId("transactions", x.TransactionId)));
				return transactions.Hydrate<Transaction>(_mappingEngine);
			}
		}

		private static string QualifiedId(string @namespace, object id)
		{
			return string.Concat(@namespace + "/", id);
		}

		private static string FromQualifiedId(string id)
		{
			return id.Substring(id.LastIndexOf('/') + 1);
		}
	}
}