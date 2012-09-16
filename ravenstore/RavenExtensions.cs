using System;
using Raven.Client;

namespace HomeTrack.RavenStore
{
	public static class RavenExtensions
	{
		public static T UseOnceTo<T>(this IDocumentStore repository, Func<IDocumentSession, T> func, bool saveChanges = false)
		{
			using (var unitOfWork = repository.OpenSession())
			{
				var result = func(unitOfWork);
				if ( saveChanges )
				{
					unitOfWork.SaveChanges();
				}

				return result;
			}
		}

		public static void UseOnceTo(this IDocumentStore repository, Action<IDocumentSession> func, bool saveChanges = false)
		{
			using (var unitOfWork = repository.OpenSession())
			{
				func(unitOfWork);
				if (saveChanges)
				{
					unitOfWork.SaveChanges();
				}
			}
		}
	}
}