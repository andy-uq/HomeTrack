using AutoMapper;
using HomeTrack.RavenStore;
using NUnit.Framework;

namespace HomeTrack.Tests
{
	public abstract class RavenRepositoryTests
	{
		private RavenRepository _repository;
		private GeneralLedger _generalLedger;

		protected RavenRepository Repository
		{
			get { return _repository; }
		}

		protected GeneralLedger GeneralLedger
		{
			get { return _generalLedger; }
		}

		[SetUp]
		public virtual void SetUp()
		{
			_repository = RavenStore.CreateRepository();

			var mappingEngine = (new MappingProvider {new RavenEntityTypeMapProvider()}).Build();
			_generalLedger = new GeneralLedger(new GeneralLedgerRepository(_repository, mappingEngine));
		}

		[TearDown]
		public void TearDown()
		{
			_repository.Dispose();
		}
	}
}