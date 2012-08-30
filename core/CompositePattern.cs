using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HomeTrack.Core
{
	public class CompositePattern : IEnumerable<IPattern>, IPattern
	{
		private readonly List<IPattern> _patterns;

		public CompositePattern()
		{
			_patterns = new List<IPattern>();
		}

		public void Add(IPattern pattern)
		{
			_patterns.Add(pattern);
		}

		public IEnumerator<IPattern> GetEnumerator()
		{
			return _patterns.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool IsMatch(IImportRow importRow)
		{
			return this.All(x => x.IsMatch(importRow));
		}

		public override string ToString()
		{
			return string.Join(" AND ", this.Select(x => string.Format("({0})", x.ToString())));
		}
	}
}