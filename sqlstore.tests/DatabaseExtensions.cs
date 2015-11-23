using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace HomeTrack.SqlStore.Tests
{
	public static class DatabaseExtensions
	{
		public static async Task<bool> HasTableAsync(this SqlConnection database, string tableName)
		{
			return await database.ExecuteScalarAsync<bool>("SELECT CASE WHEN EXISTS (SELECT * FROM sys.tables WHERE name=@0) THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END", tableName);
		}
	}
}