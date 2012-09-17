using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace HomeTrack.RavenStore.Indexes
{
	public class TransactionByAccountIds : AbstractIndexCreationTask<Documents.Transaction>
	{
		public override IndexDefinition CreateIndexDefinition()
		{
			var builder = new IndexDefinitionBuilder<Documents.Transaction>
			{
				Map = transactions =>
					(
						from t in transactions
						from credit in t.Credit
						from debit in t.Debit
						select new { Credit_AccountId = credit.AccountId, Debit_AccountId = debit.AccountId }
					)
			};

			return builder.ToIndexDefinition(Conventions);
		}
	}
}