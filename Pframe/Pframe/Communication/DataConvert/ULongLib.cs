using System;
using System.Collections.Generic;
using System.Linq;
using Pframe.Common;

namespace Pframe.DataConvert
{
	public class ULongLib
	{
		public static ulong GetULongFromByteArray(byte[] source, int start = 0, DataFormat type = DataFormat.ABCD)
		{
			byte[] array = ByteArrayLib.Get8ByteArray(source, start, type);
			return (array == null) ? 0UL : BitConverter.ToUInt64(array, 0);
		}

		public static ulong[] GetULongArrayFromByteArray(byte[] source, DataFormat type = DataFormat.ABCD)
		{
			ulong[] array = new ulong[source.Length / 8];
			for (int i = 0; i < source.Length / 8; i++)
			{
				array[i] = ULongLib.GetULongFromByteArray(source, 8 * i, type);
			}
			return array;
		}
        
		public static ulong[] GetULongArrayFromString(string val, char spilt = ' ')
		{
			val = val.Trim();
			List<ulong> list = new List<ulong>();
			if (val.Contains(spilt))
			{
				string[] array = val.Split(new char[]
				{
					spilt
				});
				foreach (string text in array)
				{
					list.Add(Convert.ToUInt64(text.Trim()));
				}
			}
			else
			{
				list.Add(Convert.ToUInt64(val.Trim()));
			}
			return list.ToArray();
		}
        
	}
}
