using System;
using System.Collections.Generic;

namespace HomeTrack
{
	public interface IImportRow
	{
		DateTime Date { get; }
		decimal Amount { get; }

		IEnumerable<KeyValuePair<string, string>> Properties { get; }
	}

	public class WestpacCsvImportRow : IImportRow
	{
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

	public class VisaCsvImportRow : IImportRow
	{
		public DateTime Date { get { return ProcessDate; } }
		public decimal Amount { get; set; }

		public IEnumerable<KeyValuePair<string, string>> Properties
		{
			get
			{
				yield return new KeyValuePair<string, string>("Other Party", OtherParty);
				yield return new KeyValuePair<string, string>("Transaction Date", TransactionDate.ToString("yyyy-MM-dd"));
				yield return new KeyValuePair<string, string>("Credit Plan Name", CreditPlanName);
				yield return new KeyValuePair<string, string>("Foreign Details", ForeignDetails);
				yield return new KeyValuePair<string, string>("City", City);
				yield return new KeyValuePair<string, string>("Country Code", CountryCode);
			}
		}

		public DateTime ProcessDate { get; set; }
		public string OtherParty { get; set; }
		public string CreditPlanName { get; set; }
		public DateTime TransactionDate { get; set; }
		public string ForeignDetails { get; set; }
		public string City { get; set; }
		public string CountryCode { get; set; }
	}

	public class AsbCsvImportRow : IImportRow
	{
		public DateTime Date { get; set; }
		public decimal Amount { get; set; }

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

		public string UniqueId { get;set; }
		public string TranType { get; set; }
		public string ChequeNumber { get; set; }
		public string Payee { get; set; }
		public string Memo { get; set; }
	}
}