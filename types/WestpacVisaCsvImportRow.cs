using System;
using System.Collections.Generic;

namespace HomeTrack
{
	public class WestpacVisaCsvImportRow : IImportRow
	{
		public string Id { get; set; }
		public DateTime Date { get { return ProcessDate; } }
		public decimal Amount { get; set; }

		public string Description
		{
			get { return OtherParty; }
		}

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
}