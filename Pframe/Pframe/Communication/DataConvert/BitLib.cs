using System;
using System.Collections.Generic;
using System.Linq;

namespace Pframe.DataConvert
{
	public class BitLib
	{
		public static bool GetBitFromByte(byte b, int offset)
		{
			if (offset < 0 || offset > 7)
			{
				throw new Exception("索引必须为0-7之间");
			}
			return ((int)b & (int)Math.Pow(2.0, (double)offset)) != 0;
		}

		public static bool GetBitFromByteArray(byte[] b, int index, int offset)
		{
			byte byteFromByteArray = ByteLib.GetByteFromByteArray(b, index);
			return BitLib.GetBitFromByte(byteFromByteArray, offset);
		}

		public static bool GetBitFrom2Byte(byte[] b, int offset, bool reverse = false)
		{
			byte b2 = reverse ? b[0] : b[1];
			byte b3 = reverse ? b[1] : b[0];
			bool bitFromByte;
			if (offset >= 0 && offset <= 7)
			{
				bitFromByte = BitLib.GetBitFromByte(b3, offset);
			}
			else
			{
				if (offset < 8 || offset > 15)
				{
					throw new Exception("索引必须为0-15之间");
				}
				bitFromByte = BitLib.GetBitFromByte(b2, offset - 8);
			}
			return bitFromByte;
		}

		public static bool GetBitFrom2ByteArray(byte[] b, int index, int offset, bool reverse = false)
		{
			byte[] byteArray = ByteArrayLib.GetByteArray(b, index, 2);
			if (byteArray == null)
			{
				throw new Exception("请检查字节索引");
			}
			return BitLib.GetBitFrom2Byte(byteArray, offset, reverse);
		}

		public static bool GetBitFromShort(short val, int offset, bool reverse = false)
		{
			return BitLib.GetBitFrom2Byte(BitConverter.GetBytes(val), offset, reverse);
		}

		public static bool GetBitFromUShort(ushort val, int offset, bool reverse = false)
		{
			return BitLib.GetBitFrom2Byte(BitConverter.GetBytes(val), offset, reverse);
		}

		public static bool[] GetBitArrayFromByte(byte b, bool reverse = false)
		{
			bool[] array = new bool[8];
			if (reverse)
			{
				for (int i = 7; i >= 0; i--)
				{
					array[i] = ((b & 1) == 1);
					b = (byte)(b >> 1);
				}
			}
			else
			{
				for (int j = 0; j <= 7; j++)
				{
					array[j] = ((b & 1) == 1);
					b = (byte)(b >> 1);
				}
			}
			return array;
		}

		public static bool[] GetBitArrayFromByteArray(byte[] b, bool reverse = false)
		{
			List<bool> list = new List<bool>();
			foreach (byte b2 in b)
			{
				list.AddRange(BitLib.GetBitArrayFromByte(b2, reverse));
			}
			return list.ToArray();
		}

		public static bool[] GetBitArray(bool[] source, int start, int length)
		{
			bool[] array = new bool[length];
			bool[] result;
			if (source != null && start >= 0 && length > 0 && source.Length >= start + length)
			{
				Array.Copy(source, start, array, 0, length);
				result = array;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static bool[] GetBitArrayFromBitArrayString(string val, char spilt = ' ')
		{
			val = val.Trim();
			List<bool> list = new List<bool>();
			if (val.Contains(spilt))
			{
				string[] array = val.Split(new char[]
				{
					spilt
				}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					list.Add(text.Trim().ToLower() == "true" || text.Trim().ToLower() == "1");
				}
			}
			else
			{
				list.Add(val.Trim().ToLower() == "true" || val.Trim().ToLower() == "1");
			}
			return list.ToArray();
		}
        
		public static bool[] ByteToBoolArray(byte[] InBytes, int length)
		{
			bool[] result;
			if (InBytes == null)
			{
				result = null;
			}
			else
			{
				if (length > InBytes.Length * 8)
				{
					length = InBytes.Length * 8;
				}
				bool[] array = new bool[length];
				for (int i = 0; i < length; i++)
				{
					int num = i / 8;
					int num2 = i % 8;
					byte b = 0;
					switch (num2)
					{
					case 0:
						b = 1;
						break;
					case 1:
						b = 2;
						break;
					case 2:
						b = 4;
						break;
					case 3:
						b = 8;
						break;
					case 4:
						b = 16;
						break;
					case 5:
						b = 32;
						break;
					case 6:
						b = 64;
						break;
					case 7:
						b = 128;
						break;
					}
					if ((InBytes[num] & b) == b)
					{
						array[i] = true;
					}
				}
				result = array;
			}
			return result;
		}
	}
}
