namespace HomeTrack
{
	public class AutomaticPayment
	{
		public Account Debit { get; set; }
		public Account Credit { get; set; }
		public decimal Amount { get; set; }
	}
}