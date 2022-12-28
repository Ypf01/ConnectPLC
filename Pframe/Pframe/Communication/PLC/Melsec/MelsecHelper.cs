using System;
using System.Text;

namespace Pframe.PLC.Melsec
{
	public class MelsecHelper
	{
		public static CalResult<MelsecA1EDataType, ushort> McA1EAnalysisAddress(string address)
		{
			CalResult<MelsecA1EDataType, ushort> xktResult = new CalResult<MelsecA1EDataType, ushort>();
			try
			{
				char c = address[0];
				char c2 = c;
				if (c2 <= 'Y')
				{
					if (c2 <= 'R')
					{
						if (c2 == 'D')
						{
							goto IL_C2;
						}
						if (c2 == 'M')
						{
							goto IL_96;
						}
						if (c2 != 'R')
						{
							goto IL_178;
						}
					}
					else
					{
						if (c2 == 'S')
						{
							goto IL_151;
						}
						if (c2 == 'X')
						{
							goto IL_128;
						}
						if (c2 != 'Y')
						{
							goto IL_178;
						}
						goto IL_FF;
					}
				}
				else if (c2 <= 'r')
				{
					if (c2 == 'd')
					{
						goto IL_C2;
					}
					if (c2 == 'm')
					{
						goto IL_96;
					}
					if (c2 != 'r')
					{
						goto IL_178;
					}
				}
				else
				{
					if (c2 == 's')
					{
						goto IL_151;
					}
					if (c2 == 'x')
					{
						goto IL_128;
					}
					if (c2 != 'y')
					{
						goto IL_178;
					}
					goto IL_FF;
				}
				xktResult.Content1 = MelsecA1EDataType.R;
				xktResult.Content2 = Convert.ToUInt16(address.Substring(1), MelsecA1EDataType.R.FromBase);
				goto IL_178;
				IL_96:
				xktResult.Content1 = MelsecA1EDataType.M;
				xktResult.Content2 = Convert.ToUInt16(address.Substring(1), MelsecA1EDataType.M.FromBase);
				goto IL_178;
				IL_C2:
				xktResult.Content1 = MelsecA1EDataType.D;
				xktResult.Content2 = Convert.ToUInt16(address.Substring(1), MelsecA1EDataType.D.FromBase);
				goto IL_178;
				IL_FF:
				xktResult.Content1 = MelsecA1EDataType.Y;
				xktResult.Content2 = Convert.ToUInt16(address.Substring(1), MelsecA1EDataType.Y.FromBase);
				goto IL_178;
				IL_128:
				xktResult.Content1 = MelsecA1EDataType.X;
				xktResult.Content2 = Convert.ToUInt16(address.Substring(1), MelsecA1EDataType.X.FromBase);
				goto IL_178;
				IL_151:
				xktResult.Content1 = MelsecA1EDataType.S;
				xktResult.Content2 = Convert.ToUInt16(address.Substring(1), MelsecA1EDataType.S.FromBase);
				IL_178:;
			}
			catch (Exception ex)
			{
				xktResult.Message = ex.Message;
				return xktResult;
			}
			xktResult.IsSuccess = true;
			return xktResult;
		}
        
		public static CalResult<MelsecMcDataType, int> McAnalysisAddress(string address, bool IsFx5U)
		{
			CalResult<MelsecMcDataType, int> xktResult = new CalResult<MelsecMcDataType, int>();
			try
			{
				char c = address[0];
				char c2 = c;
				if (c2 <= 'Z')
				{
					switch (c2)
					{
					case 'B':
						goto IL_2EB;
					case 'C':
					case 'E':
						goto IL_364;
					case 'D':
						goto IL_314;
					case 'F':
						goto IL_33D;
					default:
						switch (c2)
						{
						case 'L':
							break;
						case 'M':
							goto IL_10F;
						case 'N':
						case 'O':
						case 'P':
						case 'Q':
						case 'T':
						case 'U':
							goto IL_364;
						case 'R':
							goto IL_13B;
						case 'S':
							goto IL_167;
						case 'V':
							goto IL_193;
						case 'W':
							goto IL_1BF;
						case 'X':
							goto IL_1EB;
						case 'Y':
							goto IL_232;
						case 'Z':
							goto IL_279;
						default:
							goto IL_364;
						}
						break;
					}
				}
				else
				{
					switch (c2)
					{
					case 'b':
						goto IL_2EB;
					case 'c':
					case 'e':
						goto IL_364;
					case 'd':
						goto IL_314;
					case 'f':
						goto IL_33D;
					default:
						switch (c2)
						{
						case 'l':
							break;
						case 'm':
							goto IL_10F;
						case 'n':
						case 'o':
						case 'p':
						case 'q':
						case 't':
						case 'u':
							goto IL_364;
						case 'r':
							goto IL_13B;
						case 's':
							goto IL_167;
						case 'v':
							goto IL_193;
						case 'w':
							goto IL_1BF;
						case 'x':
							goto IL_1EB;
						case 'y':
							goto IL_232;
						case 'z':
							goto IL_279;
						default:
							goto IL_364;
						}
						break;
					}
				}
				xktResult.Content1 = MelsecMcDataType.L;
				xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(1), MelsecMcDataType.L.FromBase);
				goto IL_364;
				IL_10F:
				xktResult.Content1 = MelsecMcDataType.M;
				xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(1), MelsecMcDataType.M.FromBase);
				goto IL_364;
				IL_13B:
				xktResult.Content1 = MelsecMcDataType.R;
				xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(1), MelsecMcDataType.R.FromBase);
				goto IL_364;
				IL_167:
				xktResult.Content1 = MelsecMcDataType.S;
				xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(1), MelsecMcDataType.S.FromBase);
				goto IL_364;
				IL_193:
				xktResult.Content1 = MelsecMcDataType.V;
				xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(1), MelsecMcDataType.V.FromBase);
				goto IL_364;
				IL_1BF:
				xktResult.Content1 = MelsecMcDataType.W;
				xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(1), MelsecMcDataType.W.FromBase);
				goto IL_364;
				IL_1EB:
				xktResult.Content1 = MelsecMcDataType.X;
				xktResult.Content2 = (int)(IsFx5U ? Convert.ToUInt16(address.Substring(1), MelsecMcDataType.X5U.FromBase) : Convert.ToUInt16(address.Substring(1), MelsecMcDataType.X.FromBase));
				goto IL_364;
				IL_232:
				xktResult.Content1 = MelsecMcDataType.Y;
				xktResult.Content2 = (int)(IsFx5U ? Convert.ToUInt16(address.Substring(1), MelsecMcDataType.Y5U.FromBase) : Convert.ToUInt16(address.Substring(1), MelsecMcDataType.Y.FromBase));
				goto IL_364;
				IL_279:
				if (address.StartsWith("ZR") || address.StartsWith("zr"))
				{
					xktResult.Content1 = MelsecMcDataType.ZR;
					xktResult.Content2 = Convert.ToInt32(address.Substring(2), MelsecMcDataType.ZR.FromBase);
					goto IL_364;
				}
				xktResult.Content1 = MelsecMcDataType.Z;
				xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(1), MelsecMcDataType.Z.FromBase);
				goto IL_364;
				IL_2EB:
				xktResult.Content1 = MelsecMcDataType.B;
				xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(1), MelsecMcDataType.B.FromBase);
				goto IL_364;
				IL_314:
				xktResult.Content1 = MelsecMcDataType.D;
				xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(1), MelsecMcDataType.D.FromBase);
				goto IL_364;
				IL_33D:
				xktResult.Content1 = MelsecMcDataType.F;
				xktResult.Content2 = (int)Convert.ToUInt16(address.Substring(1), MelsecMcDataType.F.FromBase);
				IL_364:;
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
        
		public static CalResult<MelsecMcDataType, int> PanasonicAnalysisAddress(string address)
		{
			CalResult<MelsecMcDataType, int> xktResult = new CalResult<MelsecMcDataType, int>();
			try
			{
				char c = address[0];
				char c2 = c;
				if (c2 <= 'Y')
				{
					if (c2 <= 'L')
					{
						if (c2 == 'C')
						{
							goto IL_144;
						}
						if (c2 == 'D')
						{
							goto IL_E1;
						}
						if (c2 != 'L')
						{
							goto IL_2E5;
						}
					}
					else if (c2 <= 'T')
					{
						if (c2 == 'R')
						{
							goto IL_258;
						}
						if (c2 != 'T')
						{
							goto IL_2E5;
						}
						goto IL_1DB;
					}
					else
					{
						if (c2 == 'X')
						{
							goto IL_2C8;
						}
						if (c2 != 'Y')
						{
							goto IL_2E5;
						}
						goto IL_2A9;
					}
				}
				else if (c2 <= 'l')
				{
					if (c2 == 'c')
					{
						goto IL_144;
					}
					if (c2 == 'd')
					{
						goto IL_E1;
					}
					if (c2 != 'l')
					{
						goto IL_2E5;
					}
				}
				else if (c2 <= 't')
				{
					if (c2 == 'r')
					{
						goto IL_258;
					}
					if (c2 != 't')
					{
						goto IL_2E5;
					}
					goto IL_1DB;
				}
				else
				{
					if (c2 == 'x')
					{
						goto IL_2C8;
					}
					if (c2 != 'y')
					{
						goto IL_2E5;
					}
					goto IL_2A9;
				}
				if (address[1] == 'D' || address[1] == 'd')
				{
					xktResult.Content1 = MelsecMcDataType.Panasonic_LD;
					xktResult.Content2 = Convert.ToInt32(address.Substring(2));
					goto IL_2E5;
				}
				xktResult.Content1 = MelsecMcDataType.Panasonic_L;
				xktResult.Content2 = MelsecHelper.CalculateComplexAddress(address.Substring(1));
				goto IL_2E5;
				IL_E1:
				int num = Convert.ToInt32(address.Substring(1));
				if (num < 90000)
				{
					xktResult.Content1 = MelsecMcDataType.Panasonic_DT;
					xktResult.Content2 = Convert.ToInt32(address.Substring(1));
					goto IL_2E5;
				}
				xktResult.Content1 = MelsecMcDataType.Panasonic_SD;
				xktResult.Content2 = Convert.ToInt32(address.Substring(1)) - 90000;
				goto IL_2E5;
				IL_144:
				if (address[1] == 'N' || address[1] == 'n')
				{
					xktResult.Content1 = MelsecMcDataType.Panasonic_CN;
					xktResult.Content2 = Convert.ToInt32(address.Substring(2));
					goto IL_2E5;
				}
				if (address[1] == 'S' || address[1] == 's')
				{
					xktResult.Content1 = MelsecMcDataType.Panasonic_CS;
					xktResult.Content2 = Convert.ToInt32(address.Substring(2));
					goto IL_2E5;
				}
				goto IL_2E5;
				IL_1DB:
				if (address[1] == 'N' || address[1] == 'n')
				{
					xktResult.Content1 = MelsecMcDataType.Panasonic_TN;
					xktResult.Content2 = Convert.ToInt32(address.Substring(2));
					goto IL_2E5;
				}
				if (address[1] == 'S' || address[1] == 's')
				{
					xktResult.Content1 = MelsecMcDataType.Panasonic_TS;
					xktResult.Content2 = Convert.ToInt32(address.Substring(2));
					goto IL_2E5;
				}
				goto IL_2E5;
				IL_258:
				int num2 = MelsecHelper.CalculateComplexAddress(address.Substring(1));
				if (num2 < 14400)
				{
					xktResult.Content1 = MelsecMcDataType.Panasonic_R;
					xktResult.Content2 = num2;
					goto IL_2E5;
				}
				xktResult.Content1 = MelsecMcDataType.Panasonic_SM;
				xktResult.Content2 = num2 - 14400;
				goto IL_2E5;
				IL_2A9:
				xktResult.Content1 = MelsecMcDataType.Y;
				xktResult.Content2 = MelsecHelper.CalculateComplexAddress(address.Substring(1));
				goto IL_2E5;
				IL_2C8:
				xktResult.Content1 = MelsecMcDataType.X;
				xktResult.Content2 = MelsecHelper.CalculateComplexAddress(address.Substring(1));
				IL_2E5:;
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
        
		public static int CalculateComplexAddress(string address)
		{
			int num;
			if (address.IndexOf(".") < 0)
			{
				if (address.Length == 1)
				{
					num = Convert.ToInt32(address, 16);
				}
				else
				{
					num = Convert.ToInt32(address.Substring(0, address.Length - 1)) * 16 + Convert.ToInt32(address.Substring(address.Length - 1), 16);
				}
			}
			else
			{
				num = Convert.ToInt32(address.Substring(0, address.IndexOf("."))) * 16;
				string text = address.Substring(address.IndexOf(".") + 1);
				if (text.Contains("A") || text.Contains("B") || text.Contains("C") || text.Contains("D") || text.Contains("E") || text.Contains("F"))
				{
					num += Convert.ToInt32(text, 16);
				}
				else
				{
					num += Convert.ToInt32(text);
				}
			}
			return num;
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
        
		internal static byte[] BuildBytesFromAddress(int address, MelsecMcDataType type)
		{
			return Encoding.ASCII.GetBytes(address.ToString((type.FromBase == 10) ? "D6" : "X6"));
		}
        
		internal static byte[] BuildBytesFromData(byte[] value)
		{
			byte[] array = new byte[value.Length * 2];
			for (int i = 0; i < value.Length; i++)
			{
				MelsecHelper.BuildBytesFromData(value[i]).CopyTo(array, 2 * i);
			}
			return array;
		}
        
		internal static byte[] TransBoolArrayToByteData(byte[] value)
		{
			int num = (value.Length % 2 == 0) ? (value.Length / 2) : (value.Length / 2 + 1);
			byte[] array = new byte[num];
			for (int i = 0; i < num; i++)
			{
				if (value[i * 2] > 0)
				{
					byte[] array2 = array;
					int num2 = i;
					array2[num2] += 16;
				}
				if (i * 2 + 1 < value.Length && value[i * 2 + 1] > 0)
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
			return MelsecHelper.BuildBytesFromData((byte)num);
		}
        
		internal static bool CheckCRC(byte[] data)
		{
			byte[] array = MelsecHelper.FxCalculateCRC(data);
			return array[0] == data[data.Length - 2] && array[1] == data[data.Length - 1];
		}
        
	}
}
