using System;
using System.Collections.Generic;
using System.Linq;
using Pframe.Common;

namespace Pframe.DataConvert
{
	public class FloatLib
	{
		public static float GetFloatFromByteArray(byte[] source, int start = 0, DataFormat type = DataFormat.ABCD)
		{
			byte[] array = ByteArrayLib.Get4ByteArray(source, start, type);
			return (array == null) ? 0f : BitConverter.ToSingle(array, 0);
		}
        
		public static float[] GetFloatArrayFromByteArray(byte[] source, DataFormat type = DataFormat.ABCD)
		{
			float[] array = new float[source.Length / 4];
			for (int i = 0; i < source.Length / 4; i++)
			{
				array[i] = FloatLib.GetFloatFromByteArray(source, 4 * i, type);
			}
			return array;
		}
		public static float[] GetFloatArrayFromString(string val, char spilt = ' ')
		{
			val = val.Trim();
			List<float> list = new List<float>();
			if (val.Contains(spilt))
			{
				string[] array = val.Split(new char[]
				{
					spilt
				}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					list.Add(Convert.ToSingle(text.Trim()));
				}
			}
			else
			{
				list.Add(Convert.ToSingle(val.Trim()));
			}
			return list.ToArray();
		}
        
	}
}
