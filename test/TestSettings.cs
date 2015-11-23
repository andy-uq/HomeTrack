using System;
using System.Collections.Generic;

namespace HomeTrack.Tests
{
	public static class TestSettings
	{
		private static readonly Dictionary<string, string> _mapPath = new Dictionary<string, string>()
		{
			{ "ANDYCLARKE", @"C:\Users\andy.clarke\Documents\GitHub\HomeTrack" },
			{ "ARCHANGEL", @"C:\dotnet\HomeTrack" }
		};

		public static string GetFilename(string filename)
		{
			if (!filename.StartsWith("~"))
				return filename;
			
			string root;
			if ( _mapPath.TryGetValue(Environment.MachineName, out root) )
			{
				return System.IO.Path.Combine(root, filename.Substring(2));
			}

			throw new InvalidOperationException("Cannot resolve virtual path");
		}
	}
}