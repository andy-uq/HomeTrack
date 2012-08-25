using System.Collections.Generic;

namespace HomeTrack.Core
{
	public class Accumulator
	{
		private readonly char _delimiter;
		private char[] _data;
		private int _position;

		public Accumulator(string data, char delimiter)
		{
			_delimiter = delimiter;
			_data = data.ToCharArray();
			_position = 0;
		}

		public char TextDelimiter { get; set; }

		public string Next()
		{
			if ( _position >= _data.Length )
				return null;

			var local = new List<char>(_data.Length - _position);

			bool inDelimitedString = false;

			while ( _position < _data.Length )
			{
				var c = _data[_position++];

				if ( c == TextDelimiter )
				{
					if ( inDelimitedString )
					{
						if ( _data[_position] == TextDelimiter )
						{
							local.Add(c);
							_position++;
						}

						inDelimitedString = false;
					}
					else
					{
						inDelimitedString = true;
					}
				}
				else
				{

					if (!inDelimitedString && c == _delimiter)
						break;

					local.Add(c);
				}
			}

			return new string(local.ToArray());
		}
	}
}