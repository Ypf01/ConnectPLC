using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Pframe.Base;
using Pframe.Common;

namespace Pframe.PLC.Melsec
{
	public class MelsecMCA1E : NetDeviceBase
	{
		public MelsecMCA1E(DataFormat DataFormat = DataFormat.DCBA)
		{
			this.byte_0 = byte.MaxValue;
			base.DataFormat = DataFormat;
		}
          
		public override CalResult<byte[]> ReadByteArray(string address, ushort length)
		{
			CalResult<byte[]> xktResult = this.BuildReadCommand(address, length, this.byte_0);
			CalResult<byte[]> result;
			if (!xktResult.IsSuccess)
			{
				result = CalResult.CreateFailedResult<byte[]>(xktResult);
			}
			else
			{
				byte[] array = null;
				IMessage message = new A1EBinaryMessage
				{
					SendData = xktResult.Content
				};
				if (base.SendAndReceive(xktResult.Content, ref array, message, 5000) && (array != null && array.Length != 0))
				{
					if (array[1] > 0)
					{
						result = CalResult.CreateFailedResult<byte[]>(new CalResult());
					}
					else
					{
						result = this.ExtractActualData(array, xktResult.Content[0] == 0);
					}
				}
				else
				{
					result = CalResult.CreateFailedResult<byte[]>(new CalResult());
				}
			}
			return result;
		}
        
		public override CalResult<bool[]> ReadBoolArray(string address, ushort length)
		{
			CalResult<MelsecA1EDataType, ushort> xktResult = MelsecHelper.McA1EAnalysisAddress(address);
			CalResult<bool[]> result;
			if (!xktResult.IsSuccess)
			{
				result = CalResult.CreateFailedResult<bool[]>(xktResult);
			}
			else if (xktResult.Content1.DataType > 0)
			{
				CalResult<byte[]> xktResult2 = this.ReadByteArray(address, length);
				if (!xktResult2.IsSuccess)
				{
					result = CalResult.CreateFailedResult<bool[]>(xktResult2);
				}
				else
				{
					result = CalResult.CreateSuccessResult<bool[]>(xktResult2.Content.Select(new Func<byte, bool>(c => c == 1)).ToArray<bool>());
				}
			}
			else
			{
				result = CalResult.CreateFailedResult<bool[]>(new CalResult());
			}
			return result;
		}
        
		public override CalResult WriteByteArray(string address, byte[] value)
		{
			CalResult<byte[]> xktResult = this.BuildWriteCommand(address, value, this.byte_0);
			CalResult result;
			if (!xktResult.IsSuccess)
			{
				result = xktResult;
			}
			else
			{
				byte[] array = null;
				IMessage message = new A1EBinaryMessage
				{
					SendData = xktResult.Content
				};
				if (base.SendAndReceive(xktResult.Content, ref array, message, 5000) && array != null)
				{
					if (array[1] > 0)
					{
						result = CalResult.CreateFailedResult();
					}
					else
					{
						result = CalResult.CreateSuccessResult();
					}
				}
				else
				{
					result = CalResult.CreateFailedResult();
				}
			}
			return result;
		}
        
		public override CalResult WriteBoolArray(string address, bool[] values)
		{
			return this.WriteByteArray(address, values.Select(new Func<bool, byte>(c => (byte)(c ? 1 : 0))).ToArray());
		}
        
		private CalResult<byte[]> BuildReadCommand(string address, ushort length, byte plcNumber)
		{
			CalResult<MelsecA1EDataType, ushort> xktResult = MelsecHelper.McA1EAnalysisAddress(address);
			CalResult<byte[]> result;
			if (!xktResult.IsSuccess)
			{
				result = CalResult.CreateFailedResult<byte[]>(xktResult);
			}
			else if (xktResult.Content1 == null)
			{
				result = CalResult.CreateFailedResult<byte[]>(xktResult);
			}
			else
			{
				byte b = (byte)((xktResult.Content1.DataType == 1) ? 0 : 1);
				result = CalResult.CreateSuccessResult<byte[]>(new byte[]
				{
					b,
                    plcNumber,
					10,
					0,
					(byte)(xktResult.Content2 % 256),
					(byte)(xktResult.Content2 / 256),
					0,
					0,
					xktResult.Content1.DataCode[1],
					xktResult.Content1.DataCode[0],
					(byte)(length % 256),
					0
				});
			}
			return result;
		}
        
		private CalResult<byte[]> BuildWriteCommand(string address, byte[] value, byte byte_2)
		{
			CalResult<MelsecA1EDataType, ushort> xktResult = MelsecHelper.McA1EAnalysisAddress(address);
			CalResult<byte[]> result;
			if (!xktResult.IsSuccess)
			{
				result = CalResult.CreateFailedResult<byte[]>(xktResult);
			}
			else
			{
				int num = -1;
				if (xktResult.Content1.DataType == 1)
				{
					num = value.Length;
                    value = MelsecHelper.TransBoolArrayToByteData(value);
				}
				byte b = (byte)((xktResult.Content1.DataType == 1) ? 2 : 3);
				byte[] array = new byte[12 + value.Length];
				array[0] = b;
				array[1] = byte_2;
				array[2] = 10;
				array[3] = 0;
				array[4] = (byte)(xktResult.Content2 % 256);
				array[5] = (byte)(xktResult.Content2 / 256);
				array[6] = 0;
				array[7] = 0;
				array[8] = xktResult.Content1.DataCode[1];
				array[9] = xktResult.Content1.DataCode[0];
				array[10] = (byte)(num % 256);
				array[11] = 0;
				if (xktResult.Content1.DataType == 1)
				{
					if (num > 0)
						array[10] = (byte)(num % 256);
					else
						array[10] = (byte)(value.Length * 2 % 256);
				}
				else
					array[10] = (byte)(value.Length / 2 % 256);
				Array.Copy(value, 0, array, 12, value.Length);
				result = CalResult.CreateSuccessResult<byte[]>(array);
			}
			return result;
		}
        
		private CalResult<byte[]> ExtractActualData(byte[] response, bool isBit)
		{
			CalResult<byte[]> result;
			if (isBit)
			{
				byte[] array = new byte[(response.Length - 2) * 2];
				for (int i = 2; i < response.Length; i++)
				{
					if ((response[i] & 16) == 16)
					{
						array[(i - 2) * 2] = 1;
					}
					if ((response[i] & 1) == 1)
					{
						array[(i - 2) * 2 + 1] = 1;
					}
				}
				result = CalResult.CreateSuccessResult<byte[]>(array);
			}
			else
			{
				byte[] array2 = new byte[response.Length - 2];
				Array.Copy(response, 2, array2, 0, array2.Length);
				result = CalResult.CreateSuccessResult<byte[]>(array2);
			}
			return result;
		}
        private byte byte_0 { get; set; }
    }
}
