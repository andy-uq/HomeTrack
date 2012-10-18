using System.Collections.Generic;
using System.IO;

namespace HomeTrack.Core
{
	public interface IImportDetector
	{
		string Name { get; }
		bool Matches(string filename);
		IEnumerable<IImportRow> Import(Stream stream);

		IEnumerable<string> GetPropertyNames();
	}
}