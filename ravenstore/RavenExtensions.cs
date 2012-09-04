using System;
using Raven.Client;

namespace HomeTrack.RavenStore
{
	public static class RavenExtensions
	{
		public static T UseOnceTo<T>(this RavenRepository repository, Func<IDocumentSession, T> func)
		{
			using (var unitOfWork = repository.DocumentStore.OpenSession())
			{
				return func(unitOfWork);
			}
		}

		public static void UseOnceTo(this RavenRepository repository, Action<IDocumentSession> func, bool saveChanges = false)
		{
			using (var unitOfWork = repository.DocumentStore.OpenSession())
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