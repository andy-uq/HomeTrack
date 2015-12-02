using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.SqlServer;

namespace HomeTrack.SqlStore.Tests
{
	public class MigrationManager
	{
		private readonly string _connectionString;
		private readonly Assembly _migrationAssembly;
		private readonly bool _showSql;

		private MigrationManager(string connectionString, Assembly migrationAssembly, bool showSql = true)
		{
			_connectionString = connectionString;
			_migrationAssembly = migrationAssembly;
			_showSql = showSql;
		}

		public static void CreateDatabase(string connectionString, Assembly migrationAssembly, bool showSql = true)
		{
			new MigrationManager(connectionString, migrationAssembly, showSql).RunMigration(r => r.MigrateUp(true));
		}

		public static void MigrateUp(string connectionString, Assembly migrationAssembly, long targetVersion)
		{
			new MigrationManager(connectionString, migrationAssembly).RunMigration(r => r.MigrateUp(targetVersion, useAutomaticTransactionManagement: true));
		}

		public static void ApplyProfiles(string connectionString, Assembly profileAssembly)
		{
			new MigrationManager(connectionString, profileAssembly).RunMigration(r => r.ApplyProfiles());
		}

		public static void MigrateDown(string connectionString, Assembly migrationAssembly, long targetVersion)
		{
			new MigrationManager(connectionString, migrationAssembly).RunMigration(r => r.MigrateDown(targetVersion, useAutomaticTransactionManagement: true));
		}

		private void RunMigration(Action<MigrationRunner> runAction)
		{
			Announcer announcer = CreateAnnouncer();
			using (IMigrationProcessor processor = CreateMigrationProcessor(announcer))
			{
				IRunnerContext migrationContext = new RunnerContext(announcer);
				migrationContext.Profile = "Integration";
				var runner = new MigrationRunner(_migrationAssembly, migrationContext, processor);
				runAction(runner);
			}
		}

		private Announcer CreateAnnouncer()
		{
			return _showSql
				? (Announcer)new TextWriterAnnouncer(s => Debug.WriteLine(s)) { ShowSql = true }
				: new NullAnnouncer();
		}

		private IMigrationProcessor CreateMigrationProcessor(Announcer announcer)
		{
			var options = new ProcessorOptions
			{
				PreviewOnly = false, // set to true to see the SQL
				Timeout = 60,
			};

			EnsureConnectionAvailable(_connectionString);

			var factory = new SqlServer2012ProcessorFactory();
			IMigrationProcessor processor = factory.Create(_connectionString, announcer, options);
			return processor;
		}

		private static void EnsureConnectionAvailable(string connectionString)
		{
			Trace.WriteLine("Connecting to " + connectionString);
			try
			{
				using (var connection = new SqlConnection(connectionString))
				{
					connection.Open();
					Trace.WriteLine($"Connection to {connection.DataSource}[{connection.ServerVersion}] {connection.Database} successful.");
				}
			}
			catch (SqlException sqlEx)
			{
				throw new InvalidOperationException("Cannot open SQL database using connection string: " + connectionString, sqlEx);
			}
		}

		public static void ApplyMigrations()
		{
			ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings["Application"];
			if (connectionString == null)
				return;

			CreateDatabase(connectionString.ConnectionString, typeof(MigrationManager).Assembly, showSql: true);
		}
	}
}