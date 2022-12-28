using System;
using System.Text;

namespace Pframe.PLC.Keyence
{
	public class KeyenceHelper
	{
		public static CalResult<KeyenceMcDataType, int> KeyenceAnalysisAddress(string address)
		{
			CalResult<KeyenceMcDataType, int> xktResult = new CalResult<KeyenceMcDataType, int>();
			try
			{
				char c = address[0];
				char c2 = c;
				if (c2 <= 'W')
				{
					if (c2 <= 'M')
					{
						switch (c2)
						{
						case 'B':
							goto IL_13B;
						case 'C':
							goto IL_167;
						case 'D':
							goto IL_1DC;
						default:
							if (c2 == 'L')
							{
								goto IL_EF;
							}
							if (c2 != 'M')
							{
								goto IL_350;
							}
							break;
						}
					}
					else
					{
						if (c2 == 'R')
						{
							goto IL_294;
						}
						if (c2 == 'T')
						{
							goto IL_268;
						}
						if (c2 != 'W')
						{
							goto IL_350;
						}
						goto IL_23C;
					}
				}
				else if (c2 <= 'm')
				{
					switch (c2)
					{
					case 'b':
						goto IL_13B;
					case 'c':
						goto IL_167;
					case 'd':
						goto IL_1DC;
					default:
						if (c2 == 'l')
						{
							goto IL_EF;
						}
						if (c2 != 'm')
						{
							goto IL_350;
						}
						break;
					}
				}
				else
				{
					if (c2 == 'r')
					{
						goto IL_294;
					}
					if (c2 == 't')
					{
						goto IL_268;
					}
					if (c2 != 'w')
					{
						goto IL_350;
					}
					goto IL_23C;
				}
				if (address.StartsWith("MR") || address.StartsWith("mr"))
				{
					xktResult.Content1 = KeyenceMcDataType.M;
					xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(2), KeyenceMcDataType.M.FromBase);
					xktResult.Content2 = KeyenceMcDataType.TranslateKeyenceMCRAddress(xktResult.Content2.ToString());
					goto IL_350;
				}
				goto IL_350;
				IL_EF:
				if (address.StartsWith("LR") || address.StartsWith("lr"))
				{
					xktResult.Content1 = KeyenceMcDataType.L;
					xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(2), KeyenceMcDataType.L.FromBase);
					goto IL_350;
				}
				goto IL_350;
				IL_13B:
				xktResult.Content1 = KeyenceMcDataType.B;
				xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(1), KeyenceMcDataType.B.FromBase);
				goto IL_350;
				IL_167:
				if (address.StartsWith("CR") || address.StartsWith("cr"))
				{
					xktResult.Content1 = KeyenceMcDataType.S;
					xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(2), KeyenceMcDataType.S.FromBase);
					goto IL_350;
				}
				xktResult.Content1 = KeyenceMcDataType.CN;
				xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(1), KeyenceMcDataType.F.FromBase);
				goto IL_350;
				IL_1DC:
				if (address.StartsWith("DM") || address.StartsWith("dm"))
				{
					xktResult.Content1 = KeyenceMcDataType.D;
					xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(2), KeyenceMcDataType.D.FromBase);
					goto IL_350;
				}
				goto IL_350;
				IL_23C:
				xktResult.Content1 = KeyenceMcDataType.W;
				xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(1), KeyenceMcDataType.W.FromBase);
				goto IL_350;
				IL_268:
				xktResult.Content1 = KeyenceMcDataType.TN;
				xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(1), KeyenceMcDataType.F.FromBase);
				goto IL_350;
				IL_294:
				if (address.StartsWith("RX") || address.StartsWith("rx"))
				{
					xktResult.Content1 = KeyenceMcDataType.X;
					xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(2), KeyenceMcDataType.X.FromBase);
					xktResult.Content2 = KeyenceMcDataType.TranslateKeyenceMCRAddress(xktResult.Content2.ToString());
				}
				else if (address.StartsWith("RY") || address.StartsWith("ry"))
				{
					xktResult.Content1 = KeyenceMcDataType.Y;
					xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(2), KeyenceMcDataType.Y.FromBase);
					xktResult.Content2 = KeyenceMcDataType.TranslateKeyenceMCRAddress(xktResult.Content2.ToString());
				}
				IL_350:;
			}
			catch (Exception ex)
			{
				xktResult.Message = ex.Message;
				return xktResult;
			}
			xktResult.IsSuccess = true;
			xktResult.Message = "Success";
			return xktResult;
		}
        
		internal static byte[] BuildBytesFromData(byte value)
		{
			return Encoding.ASCII.GetBytes(value.ToString("X2"));
		}
        
		internal static byte[] BuildBytesFromData(short value)
		{
			return Encoding.ASCII.GetBytes(value.ToString("X4"));
		}
        
		internal static byte[] BuildBytesFromData(ushort value)
		{
			return Encoding.ASCII.GetBytes(value.ToString("X4"));
		}
        
		internal static byte[] BuildBytesFromAddress(int value, KeyenceMcDataType keyenceMcDataType_0)
		{
			return Encoding.ASCII.GetBytes(value.ToString((keyenceMcDataType_0.FromBase == 10) ? "D6" : "X6"));
		}
        
		internal static byte[] BuildBytesFromData(byte[] value)
		{
			byte[] array = new byte[value.Length * 2];
			for (int i = 0; i < value.Length; i++)
			{
				KeyenceHelper.BuildBytesFromData(value[i]).CopyTo(array, 2 * i);
			}
			return array;
		}
        
		internal static byte[] TransBoolArrayToByteData(byte[] values)
		{
			int num = (values.Length % 2 == 0) ? (values.Length / 2) : (values.Length / 2 + 1);
			byte[] array = new byte[num];
			for (int i = 0; i < num; i++)
			{
				if (values[i * 2] > 0)
				{
					byte[] array2 = array;
					int num2 = i;
					array2[num2] += 16;
				}
				if (i * 2 + 1 < values.Length && values[i * 2 + 1] > 0)
				{
					byte[] array3 = array;
					int num3 = i;
					array3[num3] += 1;
				}
			}
			return array;
		}
        
		internal static byte[] FxCalculateCRC(byte[] data)
		{
			int num = 0;
			for (int i = 1; i < data.Length - 2; i++)
			{
				num += (int)data[i];
			}
			return KeyenceHelper.BuildBytesFromData((byte)num);
		}
        
		internal static bool CheckCRC(byte[] data)
		{
			byte[] array = KeyenceHelper.FxCalculateCRC(data);
			return array[0] == data[data.Length - 2] && array[1] == data[data.Length - 1];
		}
        
	}
}
