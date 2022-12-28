using System;
using Pframe.Base;
using Pframe.DataConvert;

namespace Pframe.Custom
{
	public class MT850H : SerialDeviceBase
	{
		public bool LinkStatus(ref string ErrorCode)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.Add(new byte[]
			{
				40,
				61,
				0,
				7,
				119,
				1,
				99,
				119,
				0,
				0
			});
			byteArray.Add(MT850H.CalcSumInverse(byteArray.array, 4, 6));
			byte[] array = new byte[9];
			bool result;
			if (base.SendAndReceive(byteArray.array, ref array, 0))
			{
				if (array.Length == 9)
				{
					if (array[6] == 97 && array[7] == 107 && array[8] == 187)
					{
						result = true;
					}
					else if (array[6] == 110 && array[7] == 107)
					{
						ErrorCode = array[8].ToString() + array[9].ToString();
						result = false;
					}
					else
					{
						ErrorCode = "FFFF";
						result = false;
					}
				}
				else
				{
					ErrorCode = "FFFF";
					result = false;
				}
			}
			else
			{
				ErrorCode = "FFFF";
				result = false;
			}
			return result;
		}

		public byte[] ReadSingleReg(string iAddress)
		{
			byte[] result;
			if (!this.VerifyAddress(iAddress))
			{
				result = null;
			}
			else
			{
				ByteArray byteArray = new ByteArray();
				byteArray.Add(new byte[]
				{
					40,
					61,
					0,
					11,
					119,
					1,
					110,
					114,
					0,
					0
				});
				byte[] byteArrayFromHexStringWithoutSpilt = ByteArrayLib.GetByteArrayFromHexStringWithoutSpilt(iAddress);
				byteArray.Add(byteArrayFromHexStringWithoutSpilt);
				byteArray.Add(MT850H.CalcSumInverse(byteArray.array, 4, 10));
				byte[] array = new byte[19];
				if (base.SendAndReceive(byteArray.array, ref array, 0))
				{
					if (array.Length == 19)
					{
						if (array[6] == 110 && array[7] == 114 && array[10] == byteArrayFromHexStringWithoutSpilt[0] && array[11] == byteArrayFromHexStringWithoutSpilt[1] && array[12] == byteArrayFromHexStringWithoutSpilt[2] && array[13] == byteArrayFromHexStringWithoutSpilt[3])
						{
							result = ByteArrayLib.GetByteArray(array, 14, 4);
						}
						else
						{
							result = null;
						}
					}
					else
					{
						result = null;
					}
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		private bool VerifyAddress(string string_0)
		{
			bool result;
			if (string_0.Length == 8)
			{
				if (string_0.StartsWith("040700"))
				{
					int num = (int)Convert.ToInt16(string_0.Substring(6), 16);
					result = (num % 4 == 0);
				}
				else
				{
					result = false;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static byte CalcSumInverse(byte[] Cmd, int start, int len)
		{
			byte b = 0;
			byte result;
			if (Cmd.Length >= 1)
			{
				for (int i = start; i < start + len; i++)
				{
					b += Cmd[i];
				}
				result = (byte)(b ^ byte.MaxValue);
			}
			else
			{
				result = 0;
			}
			return result;
		}
	}
}
