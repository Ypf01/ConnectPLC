using System;
using System.Collections.Generic;
using System.Linq;
using Pframe.Common;

namespace Pframe.DataConvert
{
	public class DoubleLib
	{
		public static double GetDoubleFromByteArray(byte[] source, int start = 0, DataFormat type = DataFormat.ABCD)
		{
			byte[] array = ByteArrayLib.Get8ByteArray(source, start, type);
			return (array == null) ? 0.0 : BitConverter.ToDouble(array, 0);
		}
        
		public static double[] GetDoubleArrayFromByteArray(byte[] source, DataFormat type = DataFormat.ABCD)
		{
			double[] array = new double[source.Length / 8];
			for (int i = 0; i < source.Length / 8; i++)
			{
				array[i] = DoubleLib.GetDoubleFromByteArray(source, 8 * i, type);
			}
			return array;
		}
        
		public static double[] GetDoubleArrayFromString(string val, char spilt = ' ')
		{
			val = val.Trim();
			List<double> list = new List<double>();
			if (val.Contains(spilt))
			{
				string[] array = val.Split(new char[]
				{
					spilt
				}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					list.Add(Convert.ToDouble(text.Trim()));
				}
			}
			else
			{
				list.Add(Convert.ToDouble(val.Trim()));
			}
			return list.ToArray();
		}
	}
}
