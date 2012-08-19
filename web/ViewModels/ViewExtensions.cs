using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

			return amount.Value.ToString(format ?? "n2");
		}
	}
}