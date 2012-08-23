using System.Collections.Generic;

namespace HomeTrack.Tests
{
	public class TransactionComparer : IEqualityComparer<Transaction>
	{
		public bool Equals(Transaction x, Transaction y)
		{
			return x.Id == y.Id;
		}

		public int GetHashCode(Transaction obj)
		{
			return obj.Id.GetHashCode();
		}
	}
}