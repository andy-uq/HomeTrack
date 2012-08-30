namespace HomeTrack.RavenStore.Documents
{
	public class Account
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public AccountType Type { get; set; }
		public decimal Balance { get; set; }
	}
}