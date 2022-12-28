using System;
using System.Text;

namespace Pframe.DataConvert
{
	public class StringLib
	{
		public static string GetStringFromByteArrayByBitConvert(byte[] source, int start, int count)
		{
			return BitConverter.ToString(source, start, count);
		}
        
		public static string GetStringFromByteArray(byte[] source, int start, int count, Encoding encoding)
		{
			return encoding.GetString(ByteArrayLib.GetByteArray(source, start, count));
		}
        
		public static string GetStringFromByteArray(byte[] source, int start, int count)
		{
			byte[] byteArray = ByteArrayLib.GetByteArray(source, start, count);
			string result;
			if (byteArray != null)
			{
				result = Encoding.ASCII.GetString(byteArray);
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}
        
		public static string GetHexStringFromByteArray(byte[] source, int start, int count, char segment = ' ')
		{
			byte[] byteArray = ByteArrayLib.GetByteArray(source, start, count);
			StringBuilder stringBuilder = new StringBuilder();
			if (byteArray.Length != 0)
			{
				foreach (byte b in byteArray)
				{
					if (segment == '\0')
					{
						stringBuilder.Append(string.Format("{0:X2}", b));
					}
					else
					{
						stringBuilder.Append(string.Format("{0:X2}{1}", b, segment));
					}
				}
			}
			if (segment != '\0' && stringBuilder.Length > 1 && stringBuilder[stringBuilder.Length - 1] == segment)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			return stringBuilder.ToString();
		}
        
		public static string GetHexStringFromByteArray(byte[] source, char segment = ' ')
		{
			return StringLib.GetHexStringFromByteArray(source, 0, source.Length, segment);
		}
        
		public static string GetSiemensStringFromByteArray(byte[] source, int start, int length)
		{
			byte[] byteArray = ByteArrayLib.GetByteArray(source, start, length + 2);
			int num = (int)byteArray[1];
			string result;
			if (num > 0)
			{
				result = Encoding.GetEncoding("GBK").GetString(ByteArrayLib.GetByteArray(byteArray, 2, num));
			}
			else
			{
				result = "empty";
			}
			return result;
		}
        
		public static string GetStringFromBitArray(bool[] source, bool IsTrueFormat = true, char segment = ' ')
		{
			return StringLib.GetStringFromBitArray(source, 0, source.Length, IsTrueFormat, segment);
		}
        
		public static string GetStringFromBitArray(bool[] source, int start, int count, bool IsTrueFormat = true, char segment = ' ')
		{
			bool[] bitArray = BitLib.GetBitArray(source, start, count);
			StringBuilder stringBuilder = new StringBuilder();
			if (bitArray.Length != 0)
			{
				foreach (bool flag in bitArray)
				{
					if (IsTrueFormat)
					{
						stringBuilder.Append(flag.ToString() + segment.ToString());
					}
					else
					{
						stringBuilder.Append(flag ? ("1" + segment.ToString()) : ("0" + segment.ToString()));
					}
				}
			}
			if (stringBuilder.Length > 1 && stringBuilder[stringBuilder.Length - 1] == segment)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			return stringBuilder.ToString();
		}
        
		public static string GetStringFromValueArray<T>(T[] source, char segment = ' ')
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (source.Length != 0)
			{
				foreach (T t in source)
				{
					if (segment == '\0')
					{
						stringBuilder.Append(t.ToString());
					}
					else
					{
						stringBuilder.Append(t.ToString() + segment.ToString());
					}
				}
			}
			if (segment != '\0' && stringBuilder.Length > 1 && stringBuilder[stringBuilder.Length - 1] == segment)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			return stringBuilder.ToString();
		}
	}
}
