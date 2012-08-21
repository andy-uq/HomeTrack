namespace HomeTrack.Web.ViewModels
{
	public static class ViewExtensions
	{
		public static string AsAmount(this decimal amount, string format = null)
		{
			return AsAmount((decimal?) amount, format);
		}

		public static string AsAmount(this decimal? amount, string format = null)
		{
			if (amount == null)
			{
				return string.Empty;
			}

			return amount >= 0M 
				? amount.Value.ToString(format ?? "n2") 
				: string.Concat("(", (-amount.Value).ToString(format ?? "n2"), ")");
		}
	}
}