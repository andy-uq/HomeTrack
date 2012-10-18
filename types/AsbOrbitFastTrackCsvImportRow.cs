using System;
using System.Collections.Generic;

namespace HomeTrack
{
	public class AsbOrbitFastTrackCsvImportRow : IImportRow
	{
		public string Id { get { return "asb/" + UniqueId; } set { } }
		public DateTime Date { get; set; }
		public decimal Amount { get; set; }

		public string Description
		{
			get { return Payee; }
		}

		public IEnumerable<KeyValuePair<string, string>> Properties
		{
			get
			{
				yield return new KeyValuePair<string, string>("Unique Id", UniqueId);
				yield return new KeyValuePair<string, string>("Transaction Type", TranType);
				yield return new KeyValuePair<string, string>("Cheque Number", ChequeNumber);
				yield return new KeyValuePair<string, string>("Payee", Payee);
				yield return new KeyValuePair<string, string>("Memo", Memo);
			}
		}

		public string UniqueId { get; set; }
		public string TranType { get; set; }
		public string ChequeNumber { get; set; }
		public string Payee { get; set; }
		public string Memo { get; set; }
	}
}