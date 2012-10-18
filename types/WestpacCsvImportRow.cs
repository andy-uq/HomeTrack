using System;
using System.Collections.Generic;

namespace HomeTrack
{
	public class WestpacCsvImportRow : IImportRow
	{
		public string Id { get; set; }
		public DateTime Date { get; set; }
		public decimal Amount { get; set; }

		public IEnumerable<KeyValuePair<string, string>> Properties
		{
			get
			{
				yield return new KeyValuePair<string, string>("Other Party", OtherParty);
				yield return new KeyValuePair<string, string>("Description", Description);
				yield return new KeyValuePair<string, string>("Reference", Reference);
				yield return new KeyValuePair<string, string>("Particulars", Particulars);
				yield return new KeyValuePair<string, string>("Analysis Code", AnalysisCode);
			}
		}
		
		public string OtherParty { get; set; }
		public string Description { get; set; }
		public string Reference { get; set; }
		public string Particulars { get; set; }
		public string AnalysisCode { get; set; }
	}
}