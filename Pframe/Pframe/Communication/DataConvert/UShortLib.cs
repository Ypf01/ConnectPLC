using System;
using System.Collections.Generic;
using System.Linq;
using Pframe.Common;

namespace Pframe.DataConvert
{
	public class UShortLib
	{
		public static ushort GetUShortFromByteArray(byte[] source, int start = 0, DataFormat type = DataFormat.ABCD)
		{
			byte[] array = ByteArrayLib.Get2ByteArray(source, start, type);
			return (ushort)((array == null) ? 0 : BitConverter.ToUInt16(array, 0));
		}
        
		public static ushort[] GetUShortArrayFromByteArray(byte[] source, DataFormat type = DataFormat.ABCD)
		{
			ushort[] array = new ushort[source.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = UShortLib.GetUShortFromByteArray(source, i * 2, type);
			}
			return array;
		}
        
		public static ushort[] GetUShortArrayFromString(string val, char spilt = ' ')
		{
			val = val.Trim();
			List<ushort> list = new List<ushort>();
			if (val.Contains(spilt))
			{
				string[] array = val.Split(new char[]
				{
					spilt
				}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					list.Add(Convert.ToUInt16(text.Trim()));
				}
			}
			else
			{
				list.Add(Convert.ToUInt16(val.Trim()));
			}
			return list.ToArray();
		}
        
		public static ushort SetbitValueFromUShort(ushort value, int bit, bool val, DataFormat dataFormat = DataFormat.ABCD)
		{
			byte[] byteArrayFromUShort = ByteArrayLib.GetByteArrayFromUShort(value, dataFormat);
			if (bit >= 0 && bit <= 7)
			{
				byteArrayFromUShort[1] = ByteLib.SetbitValue(byteArrayFromUShort[1], bit, val);
			}
			else
			{
				byteArrayFromUShort[0] = ByteLib.SetbitValue(byteArrayFromUShort[0], bit - 8, val);
			}
			return UShortLib.GetUShortFromByteArray(byteArrayFromUShort, 0, dataFormat);
		}
        
		public static ushort SetbitValueFrom2ByteArray(byte[] bt, int bit, bool val, DataFormat dataFormat = DataFormat.ABCD)
		{
			if (bit >= 0 && bit <= 7)
			{
				bt[1] = ByteLib.SetbitValue(bt[1], bit, val);
			}
			else
			{
				bt[0] = ByteLib.SetbitValue(bt[0], bit - 8, val);
			}
			return UShortLib.GetUShortFromByteArray(bt, 0, dataFormat);
		}
        
	}
}
