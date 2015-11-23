using System;
using System.IO;

namespace HomeTrack.SqlStore.Tests
{
	public static class IntegrationSettings
	{
		private static readonly Settings _current = GetIntegrationSettings();

		private class Settings
		{
			public string ConnectionString { get; set; }
			public string DatabaseLocation { get; set; }
			public string BaseDirectory { get; set; }
		}

		private static Settings GetIntegrationSettings()
		{
			var buildDirectory = GetBuildDirectory();
			var databaseLocation = Path.Combine(buildDirectory, "sqlstore.tests/Template.mdf");
			return new Settings
			{
				BaseDirectory = buildDirectory,
				DatabaseLocation = databaseLocation,
				ConnectionString = "Data Source=(LocalDB)\\v12.0;Integrated Security=true;AttachDbFileName={0};MultipleActiveResultSets=True"
			};
		}

		private static string GetBuildDirectory()
		{
			var solution = Environment.GetEnvironmentVariable("NCrunch.OriginalSolutionPath");
			if (!string.IsNullOrEmpty(solution))
				return Path.GetDirectoryName(solution);

			var teamCityLocation = Environment.GetEnvironmentVariable("teamcity.build.checkoutDir");
			if (!string.IsNullOrEmpty(teamCityLocation))
			{
				return teamCityLocation;
			}

			return Path.Combine(Environment.CurrentDirectory, "../../..");
		}

		public static string ConnectionString
		{
			get { return _current.ConnectionString; }
		}

		public static string DatabaseLocation
		{
			get { return _current.DatabaseLocation; }
		}
	}
}