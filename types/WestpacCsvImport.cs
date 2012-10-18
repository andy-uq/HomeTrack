using System;
using System.Collections.Generic;

namespace HomeTrack
{
	public interface IImportRow
	{
		string Id { get; set; }

		DateTime Date { get; }
		decimal Amount { get; }
		string Description { get; }

		IEnumerable<KeyValuePair<string, string>> Properties { get; }
	}
}