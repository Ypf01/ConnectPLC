using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Pframe.Common;
using Pframe.DataConvert;

namespace Pframe.Modbus.Message
{
	public class ModbusAsciiMessage : ModbusSerialBase
	{
		public int HeadDataLength
		{
			get
			{
				return 0;
			}
		}
        
		public byte[] HeadData { get; set; }
        
		public byte[] ContentData { get; set; }
        
		public byte[] SendData { get; set; }
        
		public int GetContentLength()
		{
			return this.GetResponseLength();
		}
        
		public bool CheckHeadDataLegal(byte[] token)
		{
			return true;
		}
        
		public ModbusMessage Message { get; set; }
        
		public byte[] BuildMessageFrame()
		{
			ByteArray byteArray = new ByteArray();
			ByteArray byteArray2 = new ByteArray();
			FunctionCode functionCode = this.Message.FunctionCode;
			FunctionCode functionCode2 = functionCode;
			switch (functionCode2)
			{
			case FunctionCode.ReadOutputStatus:
			case FunctionCode.ReadInputStatus:
			case FunctionCode.ReadKeepReg:
			case FunctionCode.ReadInputReg:
				byteArray.Add(58);
				byteArray2.Add(this.Message.SlaveAddress);
				byteArray2.Add((byte)this.Message.FunctionCode);
				byteArray2.Add(this.Message.StartAddress);
				byteArray2.Add(this.Message.NumberOfPoints);
				byteArray2.Add(ParityHelper.CaculateLRC(byteArray2.array, 0, 0));
				byteArray.Add(ByteArrayLib.GetAsciiBytesFromByteArray(byteArray2.array));
				byteArray.Add(new byte[]
				{
					13,
					10
				});
				break;
			case FunctionCode.ForceCoil:
			{
				byteArray.Add(58);
				byteArray2.Add(this.Message.SlaveAddress);
				byteArray2.Add((byte)this.Message.FunctionCode);
				byteArray2.Add(this.Message.StartAddress);
				ByteArray byteArray3 = byteArray2;
				byte[] items;
				if (!this.Message.WriteBool)
				{
					items = new byte[2];
				}
				else
				{
					(items = new byte[2])[0] = byte.MaxValue;
				}
				byteArray3.Add(items);
				byteArray2.Add(ParityHelper.CaculateLRC(byteArray2.array, 0, 0));
				byteArray.Add(ByteArrayLib.GetAsciiBytesFromByteArray(byteArray2.array));
				byteArray.Add(new byte[]
				{
					13,
					10
				});
				break;
			}
			case FunctionCode.PreSetSingleReg:
				byteArray.Add(58);
				byteArray2.Add(this.Message.SlaveAddress);
				byteArray2.Add((byte)this.Message.FunctionCode);
				byteArray2.Add(this.Message.StartAddress);
				byteArray2.Add(this.Message.WriteData);
				byteArray2.Add(ParityHelper.CaculateLRC(byteArray2.array, 0, 0));
				byteArray.Add(ByteArrayLib.GetAsciiBytesFromByteArray(byteArray2.array));
				byteArray.Add(new byte[]
				{
					13,
					10
				});
				break;
			default:
				if (functionCode2 != FunctionCode.ForceMultiCoil)
				{
					if (functionCode2 == FunctionCode.PreSetMultiReg)
					{
						byteArray.Add(58);
						byteArray2.Add(this.Message.SlaveAddress);
						byteArray2.Add((byte)this.Message.FunctionCode);
						byteArray2.Add(this.Message.StartAddress);
						byteArray2.Add(this.Message.NumberOfPoints);
						byteArray2.Add((byte)this.Message.WriteData.Length);
						byteArray2.Add(this.Message.WriteData);
						byteArray2.Add(ParityHelper.CaculateLRC(byteArray2.array, 0, 0));
						byteArray.Add(ByteArrayLib.GetAsciiBytesFromByteArray(byteArray2.array));
						byteArray.Add(new byte[]
						{
							13,
							10
						});
					}
				}
				else
				{
					byteArray.Add(58);
					byteArray2.Add(this.Message.SlaveAddress);
					byteArray2.Add((byte)this.Message.FunctionCode);
					byteArray2.Add(this.Message.StartAddress);
					byteArray2.Add(this.Message.NumberOfPoints);
					byteArray2.Add((byte)this.Message.WriteData.Length);
					byteArray2.Add(this.Message.WriteData);
					byteArray2.Add(ParityHelper.CaculateLRC(byteArray2.array, 0, 0));
					byteArray.Add(ByteArrayLib.GetAsciiBytesFromByteArray(byteArray2.array));
					byteArray.Add(new byte[]
					{
						13,
						10
					});
				}
				break;
			}
			return byteArray.array;
		}
        
		public byte[] ReturnData()
		{
			FunctionCode functionCode = this.Message.FunctionCode;
			FunctionCode functionCode2 = functionCode;
			byte[] result;
			if (functionCode2 - FunctionCode.ReadOutputStatus > 1)
			{
				if (functionCode2 - FunctionCode.ReadKeepReg > 1)
				{
					result = null;
				}
				else
				{
					result = ByteArrayLib.GetByteArray(this.Message.Response, 3, this.Message.DataCount);
				}
			}
			else
			{
				result = ByteArrayLib.GetByteArray(this.Message.Response, 3, this.Message.DataCount);
			}
			return result;
		}
        
		public int GetResponseLength()
		{
			FunctionCode functionCode = this.Message.FunctionCode;
			FunctionCode functionCode2 = functionCode;
			switch (functionCode2)
			{
			case FunctionCode.ReadOutputStatus:
			case FunctionCode.ReadInputStatus:
				this.Message.DataCount = IntLib.GetIntFromBoolLength((int)this.Message.NumberOfPoints);
				return 11 + this.Message.DataCount * 2;
			case FunctionCode.ReadKeepReg:
			case FunctionCode.ReadInputReg:
				this.Message.DataCount = (int)(this.Message.NumberOfPoints * 2);
				return 11 + this.Message.DataCount * 2;
			case FunctionCode.ForceCoil:
			case FunctionCode.PreSetSingleReg:
				break;
			default:
				if (functionCode2 - FunctionCode.ForceMultiCoil > 1)
				{
					return 0;
				}
				break;
			}
			return 17;
		}
        
		public int GetValidateResponseLength()
		{
			FunctionCode functionCode = this.Message.FunctionCode;
			FunctionCode functionCode2 = functionCode;
			switch (functionCode2)
			{
			case FunctionCode.ReadOutputStatus:
			case FunctionCode.ReadInputStatus:
			{
				int num = IntLib.GetIntFromBoolLength((int)this.Message.NumberOfPoints);
				return 3 + num;
			}
			case FunctionCode.ReadKeepReg:
			case FunctionCode.ReadInputReg:
			{
				int num = (int)(this.Message.NumberOfPoints * 2);
				return 3 + num;
			}
			case FunctionCode.ForceCoil:
			case FunctionCode.PreSetSingleReg:
				break;
			default:
				if (functionCode2 - FunctionCode.ForceMultiCoil > 1)
				{
					return 0;
				}
				break;
			}
			return 6;
		}
        
		public bool ValidateResponse()
		{
			return this.Message.Response != null && this.Message.Response.Length == this.GetValidateResponseLength() && this.method_5();
		}
        
		private bool method_5()
		{
			if (this.Message.Response[0] == this.Message.SlaveAddress && this.Message.Response[1] == (byte)this.Message.FunctionCode)
			{
				FunctionCode functionCode = this.Message.FunctionCode;
				FunctionCode functionCode2 = functionCode;
				if (functionCode2 - FunctionCode.ReadOutputStatus <= 3)
				{
					return (int)this.Message.Response[2] == this.Message.DataCount;
				}
				if (functionCode2 - FunctionCode.ForceCoil <= 1 || functionCode2 - FunctionCode.ForceMultiCoil <= 1)
				{
					return this.Message.StartAddress == (ushort)this.Message.Response[2] * 256 + (ushort)this.Message.Response[3];
				}
			}
			return false;
		}
        
		public byte[] GetCoreFromAsciiMsg(byte[] value)
		{
			byte[] result;
			if (value[0] != 58 || value[value.Length - 2] != 13 || value[value.Length - 1] != 10)
			{
				result = null;
			}
			else
			{
				byte[] byteArray = ByteArrayLib.GetByteArray(value, 1, value.Length - 3);
				byte[] bytesArrayFromASCIIByteArray = ByteArrayLib.GetBytesArrayFromASCIIByteArray(byteArray);
				if (ParityHelper.CheckLRC(bytesArrayFromASCIIByteArray))
				{
					result = ByteArrayLib.GetByteArray(bytesArrayFromASCIIByteArray, 0, bytesArrayFromASCIIByteArray.Length - 1);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}       
	}
}
