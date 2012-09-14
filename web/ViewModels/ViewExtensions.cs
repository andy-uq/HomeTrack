using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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

		public static IEnumerable<SelectListItem> AsSelectList<T>(this IEnumerable<T> source, Func<T, string> valueSelector, Func<T, string> textSelector = null, Func<T, bool> isSelected = null)
		{
			Func<T, SelectListItem> convert = item => new SelectListItem
			{
				Selected = isSelected != null && isSelected(item),
				Value = valueSelector(item),
				Text = (textSelector == null) ? item.ToString() : textSelector(item)
			};

			return source.Select(convert);
		}
	}
}