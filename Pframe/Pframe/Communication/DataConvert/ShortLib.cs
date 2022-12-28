using System;
using System.Collections.Generic;
using System.Linq;
using Pframe.Common;

namespace Pframe.DataConvert
{
	public class ShortLib
	{
		public static short GetShortFromByteArray(byte[] source, int start = 0, DataFormat type = DataFormat.ABCD)
		{
			byte[] array = ByteArrayLib.Get2ByteArray(source, start, type);
			return(short)((array == null) ? 0 : BitConverter.ToInt16(array, 0));
		}

		public static short[] GetShortArrayFromByteArray(byte[] source, DataFormat type = DataFormat.ABCD)
		{
			short[] array = new short[source.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ShortLib.GetShortFromByteArray(source, i * 2, type);
			}
			return array;
		}
        
		public static short[] GetShortArrayFromString(string val, char spilt = ' ')
		{
			val = val.Trim();
			List<short> list = new List<short>();
			if (val.Contains(spilt))
			{
				string[] array = val.Split(new char[]
				{
					spilt
				}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					list.Add(Convert.ToInt16(text.Trim()));
				}
			}
			else
			{
				list.Add(Convert.ToInt16(val.Trim()));
			}
			return list.ToArray();
		}
        
		public static short SetbitValueFromShort(short value, int bit, bool val, DataFormat dataFormat = DataFormat.ABCD)
		{
			byte[] byteArrayFromShort = ByteArrayLib.GetByteArrayFromShort(value, dataFormat);
			if (bit >= 0 && bit <= 7)
			{
				byteArrayFromShort[1] = ByteLib.SetbitValue(byteArrayFromShort[1], bit, val);
			}
			else
			{
				byteArrayFromShort[0] = ByteLib.SetbitValue(byteArrayFromShort[0], bit - 8, val);
			}
			return ShortLib.GetShortFromByteArray(byteArrayFromShort, 0, dataFormat);
		}
        
		public static short SetbitValueFrom2ByteArray(byte[] bt, int bit, bool val, DataFormat dataFormat = DataFormat.ABCD)
		{
			if (bit >= 0 && bit <= 7)
			{
				bt[1] = ByteLib.SetbitValue(bt[1], bit, val);
			}
			else
			{
				bt[0] = ByteLib.SetbitValue(bt[0], bit - 8, val);
			}
			return ShortLib.GetShortFromByteArray(bt, 0, dataFormat);
		}
	}
}
