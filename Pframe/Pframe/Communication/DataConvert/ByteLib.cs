using System;

namespace Pframe.DataConvert
{
	public class ByteLib
	{
		public static byte GetByteFromByteArray(byte[] source, int start)
		{
			byte result;
			if (start < 0)
			{
				result = 0;
			}
			else
			{
				byte[] byteArray = ByteArrayLib.GetByteArray(source, start, 1);
				result = (byte)((byteArray == null) ? 0 : byteArray[0]);
			}
			return result;
		}

		public static byte SetbitValue(byte value, int bit, bool val)
		{
			return val ? (byte)(value | (byte)Math.Pow(2.0, (double)bit)) : (byte)(value & ~(byte)Math.Pow(2.0, (double)bit));
		}
	}
}
