using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Dapper;
using Rosella.Tests.Integration.Helpers;

namespace HomeTrack.SqlStore.Tests
{
	public class ReadOnlyTestDatabase : TestDatabase
	{
		public async Task SetReadOnlyAsync()
		{
			string dbName = Database.Database;
			Database.ChangeDatabase("master");

			await Database.ExecuteAsync($"ALTER DATABASE [{dbName}] SET READ_ONLY WITH NO_WAIT");

			Database.ChangeDatabase(dbName);
		}
	}

	public class TestDatabase : IDisposable
	{
		private string _dbFileName;
		private string _connectionString;
		private SqlConnection _database;

		public TestDatabase()
		{
			CreateDatabase();
		}

		public TestDatabase ApplyMigrations()
		{
			MigrationManager.CreateDatabase(_connectionString, typeof(CreateDatabase).Assembly, showSql: true);
			MigrationManager.ApplyProfiles(_connectionString, typeof(CreateTestData).Assembly);
			return this;
		}

		private void CreateDatabase()
		{
			_dbFileName = Path.Combine(Environment.CurrentDirectory, $"HomeTrack_Test_{Guid.NewGuid():n}.mdf");
			_connectionString = $"Server=(LocalDB)\\v12.0;AttachDbFileName={_dbFileName};MultipleActiveResultSets=True";

			if (File.Exists(_dbFileName))
			{
				File.Delete(_dbFileName);
			}

			Console.WriteLine("Attaching {0}", _dbFileName);
			File.Copy(IntegrationSettings.DatabaseLocation, _dbFileName);
		}

		public string ConnectionString => _connectionString;
		public SqlConnection Database => _database ?? (_database = new SqlConnection(ConnectionString));

		public void Dispose()
		{
			_database?.Close();
			_database?.Dispose();

			try
			{
				if (File.Exists(_dbFileName))
				{
					File.Delete(_dbFileName);
				}
			}
			catch (IOException)
			{
			}
		}
	}
}