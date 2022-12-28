using System;
using Pframe.Common;

namespace Pframe.DataConvert
{
	public static class Timers
	{
		public static double FromByteArray(byte[] bytes)
		{
			short shortFromByteArray = ShortLib.GetShortFromByteArray(new byte[]
			{
				bytes[1],
				bytes[0]
			}, 0, DataFormat.ABCD);
			string text = Conversion.ValToBinString(shortFromByteArray);
			double num = (double)Conversion.BinStringToInt(text.Substring(4, 4)) * 100.0;
			num += (double)Conversion.BinStringToInt(text.Substring(8, 4)) * 10.0;
			num += (double)Conversion.BinStringToInt(text.Substring(12, 4));
			string text2 = text.Substring(2, 2);
			string a = text2;
			if (!(a == "00"))
			{
				if (!(a == "01"))
				{
					if (!(a == "10"))
					{
						if (a == "11")
						{
							num *= 10.0;
						}
					}
					else
					{
						num *= 1.0;
					}
				}
				else
				{
					num *= 0.1;
				}
			}
			else
			{
				num *= 0.01;
			}
			return num;
		}
        
		public static byte[] ToByteArray(ushort value)
		{
			byte[] array = new byte[2];
			int num = 2;
			long num2 = (long)((ulong)value);
			for (int i = 0; i < num; i++)
			{
				long num3 = (long)Math.Pow(256.0, (double)i);
				long num4 = num2 / num3;
				array[num - i - 1] = (byte)(num4 & 255L);
				num2 -= (long)((ulong)array[num - i - 1] * (ulong)num3);
			}
			return array;
		}
        
		public static byte[] ToByteArray(ushort[] value)
		{
			ByteArray byteArray = new ByteArray();
			foreach (ushort value2 in value)
			{
				byteArray.Add(Timers.ToByteArray(value2));
			}
			return byteArray.array;
		}
        
		public static double[] ToArray(byte[] bytes)
		{
			double[] array = new double[bytes.Length / 2];
			int num = 0;
			for (int i = 0; i < bytes.Length / 2; i++)
			{
				array[i] = Timers.FromByteArray(new byte[]
				{
					bytes[num++],
					bytes[num++]
				});
			}
			return array;
		}
	}
}
