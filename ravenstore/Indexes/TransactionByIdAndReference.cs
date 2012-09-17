using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace HomeTrack.RavenStore.Indexes
{
	public class TransactionByIdAndReference : AbstractIndexCreationTask<Documents.Transaction>
	{
		public override IndexDefinition CreateIndexDefinition()
		{
			var builder = new IndexDefinitionBuilder<Documents.Transaction>
			{
				Map = transactions =>
					(
						from t in transactions
						select new { t.Id, t.Reference }
					)
			};
			
			return builder.ToIndexDefinition(Conventions);
		}
	}
}