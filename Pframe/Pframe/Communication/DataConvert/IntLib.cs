using System;
using System.Collections.Generic;
using System.Linq;
using Pframe.Common;

namespace Pframe.DataConvert
{
	public class IntLib
	{
		public static int GetIntFromByteArray(byte[] source, int start = 0, DataFormat type = DataFormat.ABCD)
		{
			byte[] array = ByteArrayLib.Get4ByteArray(source, start, type);
			return (array == null) ? 0 : BitConverter.ToInt32(array, 0);
		}
        
		public static int[] GetIntArrayFromByteArray(byte[] source, DataFormat type = DataFormat.ABCD)
		{
			int[] array = new int[source.Length / 4];
			for (int i = 0; i < source.Length / 4; i++)
			{
				array[i] = IntLib.GetIntFromByteArray(source, 4 * i, type);
			}
			return array;
		}
        
		public static int[] GetIntArrayFromString(string val, char spilt = ' ')
		{
			val = val.Trim();
			List<int> list = new List<int>();
			if (val.Contains(spilt))
			{
				string[] array = val.Split(new char[]
				{
					spilt
				}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					list.Add(Convert.ToInt32(text.Trim()));
				}
			}
			else
			{
				list.Add(Convert.ToInt32(val.Trim()));
			}
			return list.ToArray();
		}
        
		public static int GetIntFromBoolLength(int boolLength)
		{
			return (boolLength % 8 == 0) ? (boolLength / 8) : (boolLength / 8 + 1);
		}
        
	}
}
