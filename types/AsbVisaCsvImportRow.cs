using System;
using System.Collections.Generic;

namespace HomeTrack
{
	public class AsbVisaCsvImportRow : IImportRow
	{
		public string Id { get { return "visa/" + UniqueId; } set { } }
		public DateTime Date { get { return DateProcessed; } }
		public decimal Amount { get; set; }
		public string Description { get; set; }

		public IEnumerable<KeyValuePair<string, string>> Properties
		{
			get
			{
				yield return new KeyValuePair<string, string>("Transaction Date", DateOfTransaction.ToString("yyyy-MM-dd"));
				yield return new KeyValuePair<string, string>("Transaction Type", TranType);
				yield return new KeyValuePair<string, string>("Unique Id", UniqueId);
				yield return new KeyValuePair<string, string>("Reference", Reference);
			}
		}

		public DateTime DateProcessed { get; set; }
		public DateTime DateOfTransaction { get; set; }
		public string UniqueId { get; set; }
		public string TranType { get; set; }
		public string Reference { get; set; }
	}
}