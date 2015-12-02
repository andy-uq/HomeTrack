using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeTrack
{
	public static class Base32
	{
		private static readonly char[] CharSet = "0123456789bcdfghjklmnpqrstvwxyz_".ToCharArray();
		private static readonly int[] Lookup = Enumerable.Range(0, 256).Select(i => Array.IndexOf(CharSet, (char)i)).ToArray();

		public static Guid GuidFromBase32(string base32)
		{
			var guidData = FromBase32(base32);
			return new Guid(guidData);
		}

		public static string ToBase32(this Guid guid)
		{
			var guidData = guid.ToByteArray();
			return ToBase32(guidData);
		}

		public static byte[] FromBase32(string base32)
		{
			var output = new List<byte>(16);
			var decoded = base32
				.Select(c => Char.ToLowerInvariant(c) & 0xff)
				.Select(c => Lookup[c])
				.Where(val => val >= 0);

			var buffer = 0;
			var bitsLeft = 0;
			foreach (var val in decoded)
			{
				buffer <<= 5;
				buffer |= val;
				bitsLeft += 5;
					
				if (bitsLeft >= 8)
				{
					var b = ((buffer >> (bitsLeft - 8)) & 0xFF);
					output.Add((byte) b);
					bitsLeft -= 8;
				}
			}

			return output.ToArray();
		}

		public static string ToBase32(this byte[] bytes)
		{
			var base32 = new List<char>();
			var encode = new Action<int>(b => base32.Add(CharSet[b]));

			for (var index = 0; index < bytes.Length; index += 5)
			{
				var b = new Func<int, int>(offset => (index + offset < bytes.Length) ? bytes[index + offset] : 0);

				encode(b(0) >> 3);
				encode(((b(0) & 0x07) << 2) | ((b(1) & 0xc0) >> 6));
				if (index + 1 < bytes.Length)
				{
					encode((b(1) & 0x3e) >> 1);
					encode(((b(1) & 0x01) << 4) | ((b(2) & 0xf0) >> 4));
					
					if (index + 2 < bytes.Length)
					{
						encode(((b(2) & 0x0f) << 1) | ((b(3) & 0x80) >> 7));
						
						if (index + 3 < bytes.Length)
						{
							encode((b(3) & 0x7c) >> 2);
							encode(((b(3) & 0x03) << 3) | ((b(4) & 0xe0) >> 5));
							
							if (index + 4 < bytes.Length)
							{
								encode(b(4) & 0x1f);
							}
						}
					}
				}
			}

			return new string(base32.ToArray());
		}
	}
}