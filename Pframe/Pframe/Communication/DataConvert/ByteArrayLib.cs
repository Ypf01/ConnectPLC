using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pframe.Common;

namespace Pframe.DataConvert
{
	public class ByteArrayLib
	{
		public static byte[] GetByteArray(byte[] source, int start, int length)
		{
			byte[] array = new byte[length];
			byte[] result;
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

        public static byte[] Get2ByteArray(byte[] source, int start, DataFormat type = DataFormat.ABCD)
		{
			byte[] array = new byte[2];
			byte[] byteArray = ByteArrayLib.GetByteArray(source, start, 2);
			byte[] result;
			if (byteArray == null)
			{
				result = null;
			}
			else
			{
				switch (type)
				{
				case DataFormat.ABCD:
				case DataFormat.CDAB:
					array[0] = byteArray[1];
					array[1] = byteArray[0];
					break;
				case DataFormat.BADC:
				case DataFormat.DCBA:
					array = byteArray;
					break;
				}
				result = array;
			}
			return result;
		}

		public static byte[] Get4ByteArray(byte[] source, int start, DataFormat type = DataFormat.ABCD)
		{
			byte[] array = new byte[4];
			byte[] byteArray = ByteArrayLib.GetByteArray(source, start, 4);
			byte[] result;
			if (byteArray == null)
			{
				result = null;
			}
			else
			{
				switch (type)
				{
				case DataFormat.ABCD:
					array[0] = byteArray[3];
					array[1] = byteArray[2];
					array[2] = byteArray[1];
					array[3] = byteArray[0];
					break;
				case DataFormat.BADC:
					array[0] = byteArray[2];
					array[1] = byteArray[3];
					array[2] = byteArray[0];
					array[3] = byteArray[1];
					break;
				case DataFormat.CDAB:
					array[0] = byteArray[1];
					array[1] = byteArray[0];
					array[2] = byteArray[3];
					array[3] = byteArray[2];
					break;
				case DataFormat.DCBA:
					array = byteArray;
					break;
				}
				result = array;
			}
			return result;
		}

		public static byte[] Get8ByteArray(byte[] source, int start, DataFormat type = DataFormat.ABCD)
		{
			byte[] array = new byte[8];
			byte[] byteArray = ByteArrayLib.GetByteArray(source, start, 8);
			byte[] result;
			if (byteArray == null)
			{
				result = null;
			}
			else
			{
				switch (type)
				{
				case DataFormat.ABCD:
					array[0] = byteArray[7];
					array[1] = byteArray[6];
					array[2] = byteArray[5];
					array[3] = byteArray[4];
					array[4] = byteArray[3];
					array[5] = byteArray[2];
					array[6] = byteArray[1];
					array[7] = byteArray[0];
					break;
				case DataFormat.BADC:
					array[0] = byteArray[6];
					array[1] = byteArray[7];
					array[2] = byteArray[4];
					array[3] = byteArray[5];
					array[4] = byteArray[2];
					array[5] = byteArray[3];
					array[6] = byteArray[0];
					array[7] = byteArray[1];
					break;
				case DataFormat.CDAB:
					array[0] = byteArray[1];
					array[1] = byteArray[0];
					array[2] = byteArray[3];
					array[3] = byteArray[2];
					array[4] = byteArray[5];
					array[5] = byteArray[4];
					array[6] = byteArray[7];
					array[7] = byteArray[6];
					break;
				case DataFormat.DCBA:
					array = byteArray;
					break;
				}
				result = array;
			}
			return result;
		}

		public static bool ByteArrayEquals(byte[] b1, byte[] b2)
		{
			bool result;
			if (b1 == null || b2 == null)
			{
				result = false;
			}
			else if (b1.Length != b2.Length)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < b1.Length; i++)
				{
					if (b1[i] != b2[i])
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		public static byte[] GetByteArrayFromByte(byte value)
		{
			return new byte[]
			{
				value
			};
		}

		public static byte[] GetByteArrayFromShort(short SetValue, DataFormat dataFormat = DataFormat.ABCD)
		{
			byte[] bytes = BitConverter.GetBytes(SetValue);
			byte[] array = new byte[2];
			switch (dataFormat)
			{
			case DataFormat.ABCD:
			case DataFormat.CDAB:
				array[0] = bytes[1];
				array[1] = bytes[0];
				break;
			case DataFormat.BADC:
			case DataFormat.DCBA:
				array = bytes;
				break;
			}
			return array;
		}

		public static byte[] GetByteArrayFromUShort(ushort SetValue, DataFormat dataFormat = DataFormat.ABCD)
		{
			byte[] bytes = BitConverter.GetBytes(SetValue);
			byte[] array = new byte[2];
			switch (dataFormat)
			{
			case DataFormat.ABCD:
			case DataFormat.CDAB:
				array[0] = bytes[1];
				array[1] = bytes[0];
				break;
			case DataFormat.BADC:
			case DataFormat.DCBA:
				array = bytes;
				break;
			}
			return array;
		}

		public static byte[] GetByteArrayFromShortArray(short[] SetValue, DataFormat dataFormat = DataFormat.ABCD)
		{
			ByteArray byteArray = new ByteArray();
			foreach (short setValue in SetValue)
			{
				byteArray.Add(ByteArrayLib.GetByteArrayFromShort(setValue, dataFormat));
			}
			return byteArray.array;
		}

		public static byte[] GetByteArrayFromUShortArray(ushort[] SetValue, DataFormat dataFormat = DataFormat.ABCD)
		{
			ByteArray byteArray = new ByteArray();
			foreach (ushort setValue in SetValue)
			{
				byteArray.Add(ByteArrayLib.GetByteArrayFromUShort(setValue, dataFormat));
			}
			return byteArray.array;
		}

		public static byte[] GetByteArrayFromInt(int SetValue, DataFormat dataFormat = DataFormat.ABCD)
		{
			byte[] bytes = BitConverter.GetBytes(SetValue);
			byte[] array = new byte[4];
			switch (dataFormat)
			{
			case DataFormat.ABCD:
				array[0] = bytes[3];
				array[1] = bytes[2];
				array[2] = bytes[1];
				array[3] = bytes[0];
				break;
			case DataFormat.BADC:
				array[0] = bytes[2];
				array[1] = bytes[3];
				array[2] = bytes[0];
				array[3] = bytes[1];
				break;
			case DataFormat.CDAB:
				array[0] = bytes[1];
				array[1] = bytes[0];
				array[2] = bytes[3];
				array[3] = bytes[2];
				break;
			case DataFormat.DCBA:
				array = bytes;
				break;
			}
			return array;
		}

		public static byte[] GetByteArrayFromUInt(uint SetValue, DataFormat dataFormat = DataFormat.ABCD)
		{
			byte[] bytes = BitConverter.GetBytes(SetValue);
			byte[] array = new byte[4];
			switch (dataFormat)
			{
			case DataFormat.ABCD:
				array[0] = bytes[3];
				array[1] = bytes[2];
				array[2] = bytes[1];
				array[3] = bytes[0];
				break;
			case DataFormat.BADC:
				array[0] = bytes[2];
				array[1] = bytes[3];
				array[2] = bytes[0];
				array[3] = bytes[1];
				break;
			case DataFormat.CDAB:
				array[0] = bytes[1];
				array[1] = bytes[0];
				array[2] = bytes[3];
				array[3] = bytes[2];
				break;
			case DataFormat.DCBA:
				array = bytes;
				break;
			}
			return array;
		}

		public static byte[] GetByteArrayFromFloat(float SetValue, DataFormat dataFormat = DataFormat.ABCD)
		{
			byte[] bytes = BitConverter.GetBytes(SetValue);
			byte[] array = new byte[4];
			switch (dataFormat)
			{
			case DataFormat.ABCD:
				array[0] = bytes[3];
				array[1] = bytes[2];
				array[2] = bytes[1];
				array[3] = bytes[0];
				break;
			case DataFormat.BADC:
				array[0] = bytes[2];
				array[1] = bytes[3];
				array[2] = bytes[0];
				array[3] = bytes[1];
				break;
			case DataFormat.CDAB:
				array[0] = bytes[1];
				array[1] = bytes[0];
				array[2] = bytes[3];
				array[3] = bytes[2];
				break;
			case DataFormat.DCBA:
				array = bytes;
				break;
			}
			return array;
		}

		public static byte[] GetByteArrayFromDouble(double SetValue, DataFormat dataFormat = DataFormat.ABCD)
		{
			byte[] bytes = BitConverter.GetBytes(SetValue);
			byte[] array = new byte[8];
			switch (dataFormat)
			{
			case DataFormat.ABCD:
				array[0] = bytes[7];
				array[1] = bytes[6];
				array[2] = bytes[5];
				array[3] = bytes[4];
				array[4] = bytes[3];
				array[5] = bytes[2];
				array[6] = bytes[1];
				array[7] = bytes[0];
				break;
			case DataFormat.BADC:
				array[0] = bytes[6];
				array[1] = bytes[7];
				array[2] = bytes[4];
				array[3] = bytes[5];
				array[4] = bytes[2];
				array[5] = bytes[3];
				array[6] = bytes[0];
				array[7] = bytes[1];
				break;
			case DataFormat.CDAB:
				array[0] = bytes[1];
				array[1] = bytes[0];
				array[2] = bytes[3];
				array[3] = bytes[2];
				array[4] = bytes[5];
				array[5] = bytes[4];
				array[6] = bytes[7];
				array[7] = bytes[6];
				break;
			case DataFormat.DCBA:
				array = bytes;
				break;
			}
			return array;
		}

		public static byte[] GetByteArrayFromIntArray(int[] SetValue, DataFormat dataFormat = DataFormat.ABCD)
		{
			ByteArray byteArray = new ByteArray();
			foreach (int setValue in SetValue)
			{
				byteArray.Add(ByteArrayLib.GetByteArrayFromInt(setValue, dataFormat));
			}
			return byteArray.array;
		}

		public static byte[] GetByteArrayFromUIntArray(uint[] SetValue, DataFormat dataFormat = DataFormat.ABCD)
		{
			ByteArray byteArray = new ByteArray();
			foreach (uint setValue in SetValue)
			{
				byteArray.Add(ByteArrayLib.GetByteArrayFromUInt(setValue, dataFormat));
			}
			return byteArray.array;
		}

		public static byte[] GetByteArrayFromFloatArray(float[] SetValue, DataFormat dataFormat = DataFormat.ABCD)
		{
			ByteArray byteArray = new ByteArray();
			foreach (float setValue in SetValue)
			{
				byteArray.Add(ByteArrayLib.GetByteArrayFromFloat(setValue, dataFormat));
			}
			return byteArray.array;
		}

		public static byte[] GetByteArrayFromDoubleArray(double[] SetValue, DataFormat dataFormat = DataFormat.ABCD)
		{
			ByteArray byteArray = new ByteArray();
			foreach (double setValue in SetValue)
			{
				byteArray.Add(ByteArrayLib.GetByteArrayFromDouble(setValue, dataFormat));
			}
			return byteArray.array;
		}

		public static byte[] GetByteArrayFromLong(long SetValue, DataFormat dataFormat = DataFormat.ABCD)
		{
			byte[] bytes = BitConverter.GetBytes(SetValue);
			byte[] array = new byte[8];
			switch (dataFormat)
			{
			case DataFormat.ABCD:
				array[0] = bytes[7];
				array[1] = bytes[6];
				array[2] = bytes[5];
				array[3] = bytes[4];
				array[4] = bytes[3];
				array[5] = bytes[2];
				array[6] = bytes[1];
				array[7] = bytes[0];
				break;
			case DataFormat.BADC:
				array[0] = bytes[6];
				array[1] = bytes[7];
				array[2] = bytes[4];
				array[3] = bytes[5];
				array[4] = bytes[2];
				array[5] = bytes[3];
				array[6] = bytes[0];
				array[7] = bytes[1];
				break;
			case DataFormat.CDAB:
				array[0] = bytes[1];
				array[1] = bytes[0];
				array[2] = bytes[3];
				array[3] = bytes[2];
				array[4] = bytes[5];
				array[5] = bytes[4];
				array[6] = bytes[7];
				array[7] = bytes[6];
				break;
			case DataFormat.DCBA:
				array = bytes;
				break;
			}
			return array;
		}

		public static byte[] GetByteArrayFromULong(ulong SetValue, DataFormat dataFormat = DataFormat.ABCD)
		{
			byte[] bytes = BitConverter.GetBytes(SetValue);
			byte[] array = new byte[8];
			switch (dataFormat)
			{
			case DataFormat.ABCD:
				array[0] = bytes[7];
				array[1] = bytes[6];
				array[2] = bytes[5];
				array[3] = bytes[4];
				array[4] = bytes[3];
				array[5] = bytes[2];
				array[6] = bytes[1];
				array[7] = bytes[0];
				break;
			case DataFormat.BADC:
				array[0] = bytes[6];
				array[1] = bytes[7];
				array[2] = bytes[4];
				array[3] = bytes[5];
				array[4] = bytes[2];
				array[5] = bytes[3];
				array[6] = bytes[0];
				array[7] = bytes[1];
				break;
			case DataFormat.CDAB:
				array[0] = bytes[1];
				array[1] = bytes[0];
				array[2] = bytes[3];
				array[3] = bytes[2];
				array[4] = bytes[5];
				array[5] = bytes[4];
				array[6] = bytes[7];
				array[7] = bytes[6];
				break;
			case DataFormat.DCBA:
				array = bytes;
				break;
			}
			return array;
		}

		public static byte[] GetByteArrayFromLongArray(long[] SetValue, DataFormat dataFormat = DataFormat.ABCD)
		{
			ByteArray byteArray = new ByteArray();
			foreach (long num in SetValue)
			{
				byteArray.Add(ByteArrayLib.GetByteArrayFromDouble((double)num, dataFormat));
			}
			return byteArray.array;
		}

		public static byte[] GetByteArrayFromULongArray(ulong[] SetValue, DataFormat dataFormat = DataFormat.ABCD)
		{
			ByteArray byteArray = new ByteArray();
			foreach (ulong setValue in SetValue)
			{
				byteArray.Add(ByteArrayLib.GetByteArrayFromULong(setValue, dataFormat));
			}
			return byteArray.array;
		}

		public static byte[] GetByteArrayFromString(string SetValue, Encoding encoding)
		{
			return encoding.GetBytes(SetValue);
		}

		public static byte[] GetByteArrayFromHexString(string val, char spilt = ' ')
		{
			val = val.Trim();
			List<byte> list = new List<byte>();
			byte[] result;
			try
			{
				if (val.Contains(spilt))
				{
					string[] array = val.Split(new char[]
					{
						spilt
					}, StringSplitOptions.RemoveEmptyEntries);
					foreach (string text in array)
					{
						list.Add(Convert.ToByte(text.Trim(), 16));
					}
				}
				else
				{
					list.Add(Convert.ToByte(val.Trim(), 16));
				}
				result = list.ToArray();
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		public static byte[] GetByteArrayFromHexStringWithoutSpilt(string val)
		{
			List<byte> list = new List<byte>();
			byte[] result;
			if (val.Length % 2 != 0)
			{
				result = null;
			}
			else
			{
				try
				{
					for (int i = 0; i < val.Length; i += 2)
					{
						string value = val.Substring(i, 2);
						list.Add(Convert.ToByte(value, 16));
					}
					result = list.ToArray();
				}
				catch (Exception)
				{
					result = null;
				}
			}
			return result;
		}

		public static byte[] GetByteArrayFromBoolArray(bool[] val)
		{
			byte[] result;
			if (val == null && val.Length == 0)
			{
				result = null;
			}
			else
			{
				byte[] array = new byte[(val.Length % 8 != 0) ? (val.Length / 8 + 1) : (val.Length / 8)];
				for (int i = 0; i < array.Length; i++)
				{
					int num = (val.Length < 8 * (i + 1)) ? (val.Length - 8 * i) : 8;
					for (int j = 0; j < num; j++)
					{
						array[i] = ByteLib.SetbitValue(array[i], j, val[8 * i + j]);
					}
				}
				result = array;
			}
			return result;
		}

		public static byte[] GetByteArrayFromSiemensString(string SetValue)
		{
			byte[] byteArrayFromString = ByteArrayLib.GetByteArrayFromString(SetValue, Encoding.GetEncoding("GBK"));
			byte[] array = new byte[byteArrayFromString.Length + 2];
			array[1] = (byte)byteArrayFromString.Length;
			Array.Copy(byteArrayFromString, 0, array, 2, byteArrayFromString.Length);
			return array;
		}

		public static byte[] GetByteArrayFromOmronCIPString(string SetValue)
		{
			byte[] byteArrayFromString = ByteArrayLib.GetByteArrayFromString(SetValue, Encoding.ASCII);
			byte[] array = ByteArrayLib.smethod_0(byteArrayFromString);
			byte[] array2 = new byte[array.Length + 2];
			array2[0] = BitConverter.GetBytes(array2.Length - 2)[0];
			array2[1] = BitConverter.GetBytes(array2.Length - 2)[1];
			Array.Copy(array, 0, array2, 2, array.Length);
			return array2;
		}

		private static byte[] smethod_0(byte[] byte_0)
		{
			byte[] result;
			if (byte_0 == null)
			{
				result = new byte[0];
			}
			else if (byte_0.Length % 2 == 1)
			{
				result = ByteArrayLib.smethod_1(byte_0, byte_0.Length + 1);
			}
			else
			{
				result = byte_0;
			}
			return result;
		}

		private static byte[] smethod_1(byte[] byte_0, int int_0)
		{
			byte[] result;
			if (byte_0 == null)
			{
				result = new byte[int_0];
			}
			else if (byte_0.Length == int_0)
			{
				result = byte_0;
			}
			else
			{
				byte[] array = new byte[int_0];
				Array.Copy(byte_0, array, Math.Min(byte_0.Length, array.Length));
				result = array;
			}
			return result;
		}

		public static byte[] GetAsciiBytesFromByteArray(byte[] inBytes)
		{
			return Encoding.ASCII.GetBytes(StringLib.GetHexStringFromByteArray(inBytes, '\0'));
		}

		public static byte[] CombineTwoByteArray(byte[] bytes1, byte[] bytes2)
		{
			byte[] result;
			if (bytes1 == null && bytes2 == null)
			{
				result = null;
			}
			else if (bytes1 == null)
			{
				result = bytes2;
			}
			else if (bytes2 == null)
			{
				result = bytes1;
			}
			else
			{
				byte[] array = new byte[bytes1.Length + bytes2.Length];
				bytes1.CopyTo(array, 0);
				bytes2.CopyTo(array, bytes1.Length);
				result = array;
			}
			return result;
		}

		public static byte[] CombineThreeByteArray(byte[] bytes1, byte[] bytes2, byte[] bytes3)
		{
			return ByteArrayLib.CombineTwoByteArray(ByteArrayLib.CombineTwoByteArray(bytes1, bytes2), bytes3);
		}

		public static byte[] GetBytesArrayFromASCIIByteArray(byte[] inBytes)
		{
			return ByteArrayLib.GetByteArrayFromHexStringWithoutSpilt(Encoding.ASCII.GetString(inBytes));
		}

		public static byte[] SetByteArray(byte[] src, object value, int start, int offset)
		{
			string name = value.GetType().Name;
			string text = name.ToLower();
			string text2 = text;
			uint num = PrivateImplementationDetails.ComputeStringHash(text2);
			if (num <= 1871168131U)
			{
				if (num <= 653523161U)
				{
					if (num <= 132346577U)
					{
						if (num != 64103268U)
						{
							if (num == 132346577U)
							{
								if (text2 == "int16")
								{
									Array.Copy(ByteArrayLib.GetByteArrayFromShort(Convert.ToInt16(value), DataFormat.ABCD), 0, src, start, 2);
								}
							}
						}
						else if (text2 == "int64")
						{
							Array.Copy(ByteArrayLib.GetByteArrayFromLong(Convert.ToInt64(value), DataFormat.ABCD), 0, src, start, 8);
						}
					}
					else if (num != 596452977U)
					{
						if (num == 653523161U)
						{
							if (text2 == "int16[]")
							{
								byte[] array = ByteArrayLib.GetByteArrayFromShortArray(ShortLib.GetShortArrayFromString(value.ToString(), ' '), DataFormat.ABCD);
								Array.Copy(array, 0, src, start, array.Length);
							}
						}
					}
					else if (text2 == "single[]")
					{
						byte[] array = ByteArrayLib.GetByteArrayFromFloatArray(FloatLib.GetFloatArrayFromString(value.ToString(), ' '), DataFormat.ABCD);
						Array.Copy(array, 0, src, start, array.Length);
					}
				}
				else if (num <= 1361392351U)
				{
					if (num != 848563180U)
					{
						if (num == 1361392351U)
						{
							if (text2 == "uint64[]")
							{
								byte[] array = ByteArrayLib.GetByteArrayFromULongArray(ULongLib.GetULongArrayFromString(value.ToString(), ' '), DataFormat.ABCD);
								Array.Copy(array, 0, src, start, array.Length);
							}
						}
					}
					else if (text2 == "uint32")
					{
						Array.Copy(ByteArrayLib.GetByteArrayFromUInt(Convert.ToUInt32(value), DataFormat.ABCD), 0, src, start, 4);
					}
				}
				else if (num != 1683620383U)
				{
					if (num != 1710517951U)
					{
						if (num == 1871168131U)
						{
							if (text2 == "byte[]")
							{
								byte[] array = ByteArrayLib.GetByteArrayFromHexString(value.ToString(), ' ');
								Array.Copy(array, 0, src, start, array.Length);
							}
						}
					}
					else if (text2 == "boolean")
					{
						Array.Copy(ByteArrayLib.GetByteArrayFromByte(ByteLib.SetbitValue(src[start], offset, Convert.ToBoolean(value))), 0, src, start, 1);
					}
				}
				else if (text2 == "byte")
				{
					Array.Copy(ByteArrayLib.GetByteArrayFromByte(Convert.ToByte(value)), 0, src, start, 1);
				}
			}
			else if (num <= 2928590578U)
			{
				if (num <= 2133018345U)
				{
					if (num != 2109420386U)
					{
						if (num == 2133018345U)
						{
							if (text2 == "single")
							{
								Array.Copy(ByteArrayLib.GetByteArrayFromFloat(Convert.ToSingle(value), DataFormat.ABCD), 0, src, start, 4);
							}
						}
					}
					else if (text2 == "uint16[]")
					{
						byte[] array = ByteArrayLib.GetByteArrayFromUShortArray(UShortLib.GetUShortArrayFromString(value.ToString(), ' '), DataFormat.ABCD);
						Array.Copy(array, 0, src, start, array.Length);
					}
				}
				else if (num != 2574967888U)
				{
					if (num != 2699759368U)
					{
						if (num == 2928590578U)
						{
							if (text2 == "uint16")
							{
								Array.Copy(ByteArrayLib.GetByteArrayFromUShort(Convert.ToUInt16(value), DataFormat.ABCD), 0, src, start, 2);
							}
						}
					}
					else if (text2 == "double")
					{
						Array.Copy(ByteArrayLib.GetByteArrayFromDouble(Convert.ToDouble(value), DataFormat.ABCD), 0, src, start, 8);
					}
				}
				else if (text2 == "int64[]")
				{
					byte[] array = ByteArrayLib.GetByteArrayFromLongArray(LongLib.GetLongArrayFromString(value.ToString(), ' '), DataFormat.ABCD);
					Array.Copy(array, 0, src, start, array.Length);
				}
			}
			else if (num <= 3233567832U)
			{
				if (num != 2929723411U)
				{
					if (num == 3233567832U)
					{
						if (text2 == "uint32[]")
						{
							byte[] array = ByteArrayLib.GetByteArrayFromUIntArray(UIntLib.GetUIntArrayFromString(value.ToString(), ' '), DataFormat.ABCD);
							Array.Copy(array, 0, src, start, array.Length);
						}
					}
				}
				else if (text2 == "uint64")
				{
					Array.Copy(ByteArrayLib.GetByteArrayFromULong(Convert.ToUInt64(value), DataFormat.ABCD), 0, src, start, 8);
				}
			}
			else if (num != 3244117276U)
			{
				if (num != 3914554019U)
				{
					if (num == 4225688255U)
					{
						if (text2 == "int32")
						{
							Array.Copy(ByteArrayLib.GetByteArrayFromInt(Convert.ToInt32(value), DataFormat.ABCD), 0, src, start, 4);
						}
					}
				}
				else if (text2 == "int32[]")
				{
					byte[] array = ByteArrayLib.GetByteArrayFromIntArray(IntLib.GetIntArrayFromString(value.ToString(), ' '), DataFormat.ABCD);
					Array.Copy(array, 0, src, start, array.Length);
				}
			}
			else if (text2 == "double[]")
			{
				byte[] array = ByteArrayLib.GetByteArrayFromDoubleArray(DoubleLib.GetDoubleArrayFromString(value.ToString(), ' '), DataFormat.ABCD);
				Array.Copy(array, 0, src, start, array.Length);
			}
			return src;
		}
	}
}
