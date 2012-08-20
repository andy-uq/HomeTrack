using System;
using Raven.Client;

namespace HomeTrack.RavenStore
{
	public static class RavenExtensions
	{
		public static T UseOnceTo<T>(this RavenRepository respository, Func<IDocumentSession, T> func)
		{
			using (var unitOfWork = respository.DocumentStore.OpenSession())
			{
				return func(unitOfWork);
			}
		}

		public static void UseOnceTo(this RavenRepository respository, Action<IDocumentSession> func, bool saveChanges = false)
		{
			using (var unitOfWork = respository.DocumentStore.OpenSession())
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