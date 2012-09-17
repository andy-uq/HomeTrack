using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace HomeTrack.RavenStore.Indexes
{
	public class ImportsByImportType  : AbstractIndexCreationTask<Documents.ImportResult>
	{
		public override IndexDefinition CreateIndexDefinition()
		{
			var builder = new IndexDefinitionBuilder<Documents.ImportResult>
			{
				Map = imports =>
					(
						from t in imports
						select new { t.ImportType }
					)
			};
			
			return builder.ToIndexDefinition(Conventions);
		}
	}
}