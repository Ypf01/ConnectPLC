using System;
using System.Collections.Generic;
using System.Linq;
using Pframe.Common;

namespace Pframe.DataConvert
{
	public class UIntLib
	{
		public static uint GetUIntFromByteArray(byte[] source, int start = 0, DataFormat type = DataFormat.ABCD)
		{
			byte[] array = ByteArrayLib.Get4ByteArray(source, start, type);
			return (array == null) ? 0U : BitConverter.ToUInt32(array, 0);
		}
        
		public static uint[] GetUIntArrayFromByteArray(byte[] source, DataFormat type = DataFormat.ABCD)
		{
			uint[] array = new uint[source.Length / 4];
			for (int i = 0; i < source.Length / 4; i++)
			{
				array[i] = UIntLib.GetUIntFromByteArray(source, 4 * i, type);
			}
			return array;
		}
        
		public static uint[] GetUIntArrayFromString(string val, char spilt = ' ')
		{
			val = val.Trim();
			List<uint> list = new List<uint>();
			if (val.Contains(spilt))
			{
				string[] array = val.Split(new char[]
				{
					spilt
				}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					list.Add(Convert.ToUInt32(text.Trim()));
				}
			}
			else
			{
				list.Add(Convert.ToUInt32(val.Trim()));
			}
			return list.ToArray();
		}
	}
}
