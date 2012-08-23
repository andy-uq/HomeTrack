using System.Linq;
using System.Web.Mvc;

namespace HomeTrack.Web.Controllers
{
	public static class ControllerExtensions
	{
		public static JsonResult JsonValidation(this ModelStateDictionary state)
		{
			var errors =
				(
					from e in state
					where e.Value.Errors.Count > 0
					select new
					{
						Name = e.Key,
						Errors = e.Value.Errors.Where(x => x.Exception == null).Select(x => x.ErrorMessage)
						.Concat(e.Value.Errors.Where(x => x.Exception != null).Select(x => x.Exception.Message))
					}
				);

			return new ValidationJsonResult
			{
				Data = new
				{
					Tag = "ValidationError",
					State = errors
				}
			};
		}
	}

	public class ValidationJsonResult : JsonResult
	{
	}
}