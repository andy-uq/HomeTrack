using System.Collections.Generic;

namespace HomeTrack.RavenStore.Documents
{
	public class AccountIdentifier
	{
		public string Id { get; set; }
		public string AccountId { get; set; } 
		public string AccountName { get; set; }
		public Pattern Pattern { get; set; }
	}

	public class Pattern
	{
		public Pattern()
		{
			Properties = new Dictionary<string, object>();
		}

		public string Name { get; set; }
		public Dictionary<string, object> Properties { get; set; }
		public Pattern[] Child { get; set; }
	}
}