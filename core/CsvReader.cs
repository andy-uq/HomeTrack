using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HomeTrack.Core
{
	public class CsvReader : IDisposable
	{
		private readonly StreamReader _stream;
		private string[] _header;

		public CsvReader(string filename)
		{
			_stream = File.OpenText(filename);
		}

		public CsvReader(Stream stream)
		{
			_stream = new StreamReader(stream);
		}


		public IEnumerable<string> GetLines()
		{
			string line;
			while ((line = _stream.ReadLine()) != null)
			{
				if (!string.IsNullOrEmpty(line))
				{
					yield return line;
				}
			}
		}

		public IEnumerable<string> GetHeader(int skip = 0)
		{
			if ( _header == null )
			{
				while ( skip > 0 )
				{
					_stream.ReadLine();
					skip--;
				}

				var line = _stream.ReadLine();
				_header = Decode(line).ToArray();
			}

			return _header;
		}

		public IEnumerable<string[]> GetData()
		{
			return GetLines().Select(line => Decode(line).ToArray());
		}

		public IEnumerable<T> GetData<T>() where T : new()
		{
			GetHeader();

			var type = typeof (T);
			var propertyMap = type.GetProperties()
				.ToDictionary(
				p => p.Name, 
				v => (Action<T, string>)((instance, value) => {
					var nativeValue = Convert.ChangeType(value, v.PropertyType);
					v.SetValue(instance, nativeValue, null);
				}));

			Func<string[], T> map = data => {
				var o = new T();

				for (int i = 0; i < _header.Length; i++)
				{
					var key = Regex.Replace(_header[i], @"\W+", string.Empty);
					var value = data[i];
					propertyMap[key](o, value);
				}

				return o;
			};

			return GetLines().Select(line => map(Decode(line).ToArray()));
		}

		private IEnumerable<string> Decode(string line)
		{
			var accumulator = new Accumulator(line, ',') { TextDelimiter = '"' };
			string field;
			while ((field = accumulator.Next()) != null)
				yield return field;
		}

		public void Dispose()
		{
			_stream.Close();
		}
	}
}