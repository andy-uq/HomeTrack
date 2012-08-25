using System;

namespace HomeTrack
{
	public class WestpacCsvImportRow
	{
		public DateTime Date { get; set; }
		public decimal Amount { get; set; }
		public string OtherParty { get; set; }
		public string Description { get; set; }
		public string Reference { get; set; }
		public string Particulars { get; set; }
		public string AnalysisCode { get; set; }
	}

	public class AsbCsvImportRow
	{
		public DateTime Date { get; set; }
		public decimal Amount { get; set; }
		public string UniqueId { get;set; }
		public string TranType { get; set; }
		public string ChequeNumber { get; set; }
		public string Payee { get; set; }
		public string Memo { get; set; }
	}
}