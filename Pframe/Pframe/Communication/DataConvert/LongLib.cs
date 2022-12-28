using System;
using System.Collections.Generic;
using System.Linq;
using Pframe.Common;

namespace Pframe.DataConvert
{
	public class LongLib
	{
		public static long GetLongFromByteArray(byte[] source, int start = 0, DataFormat type = DataFormat.ABCD)
		{
			byte[] array = ByteArrayLib.Get8ByteArray(source, start, type);
			return (array == null) ? 0L : BitConverter.ToInt64(array, 0);
		}
		public static long[] GetLongArrayFromByteArray(byte[] source, DataFormat type = DataFormat.ABCD)
		{
			long[] array = new long[source.Length / 8];
			for (int i = 0; i < source.Length / 8; i++)
			{
				array[i] = LongLib.GetLongFromByteArray(source, 8 * i, type);
			}
			return array;
		}
        
		public static long[] GetLongArrayFromString(string val, char spilt = ' ')
		{
			val = val.Trim();
			List<long> list = new List<long>();
			if (val.Contains(spilt))
			{
				string[] array = val.Split(new char[]
				{
					spilt
				}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					list.Add(Convert.ToInt64(text.Trim()));
				}
			}
			else
			{
				list.Add(Convert.ToInt64(val.Trim()));
			}
			return list.ToArray();
		}
        
	}
}
