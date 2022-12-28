using System;
using Pframe.Common;
using Pframe.DataConvert;

namespace Pframe.Modbus.Message
{
	public class ModbusAsciiOverTcpMessage : ModbusNetBase
	{
		public byte[] BuildMessageFrame(ModbusMessage Message)
		{
			ByteArray byteArray = new ByteArray();
			ByteArray byteArray2 = new ByteArray();
			FunctionCode functionCode = Message.FunctionCode;
			FunctionCode functionCode2 = functionCode;
			switch (functionCode2)
			{
			case FunctionCode.ReadOutputStatus:
			case FunctionCode.ReadInputStatus:
			case FunctionCode.ReadKeepReg:
			case FunctionCode.ReadInputReg:
				byteArray.Add(58);
				byteArray2.Add(Message.SlaveAddress);
				byteArray2.Add((byte)Message.FunctionCode);
				byteArray2.Add(Message.StartAddress);
				byteArray2.Add(Message.NumberOfPoints);
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
				byteArray2.Add(Message.SlaveAddress);
				byteArray2.Add((byte)Message.FunctionCode);
				byteArray2.Add(Message.StartAddress);
				ByteArray byteArray3 = byteArray2;
				byte[] items;
				if (!Message.WriteBool)
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
				byteArray2.Add(Message.SlaveAddress);
				byteArray2.Add((byte)Message.FunctionCode);
				byteArray2.Add(Message.StartAddress);
				byteArray2.Add(Message.WriteData);
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
						byteArray2.Add(Message.SlaveAddress);
						byteArray2.Add((byte)Message.FunctionCode);
						byteArray2.Add(Message.StartAddress);
						byteArray2.Add(Message.NumberOfPoints);
						byteArray2.Add((byte)Message.WriteData.Length);
						byteArray2.Add(Message.WriteData);
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
					byteArray2.Add(Message.SlaveAddress);
					byteArray2.Add((byte)Message.FunctionCode);
					byteArray2.Add(Message.StartAddress);
					byteArray2.Add(Message.NumberOfPoints);
					byteArray2.Add((byte)Message.WriteData.Length);
					byteArray2.Add(Message.WriteData);
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

		public byte[] ReturnData(ModbusMessage Message)
		{
			FunctionCode functionCode = Message.FunctionCode;
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
					result = ByteArrayLib.GetByteArray(Message.Response, 3, Message.DataCount);
				}
			}
			else
			{
				result = ByteArrayLib.GetByteArray(Message.Response, 3, Message.DataCount);
			}
			return result;
		}
        
		public int GetResponseLength(ModbusMessage Message)
		{
			FunctionCode functionCode = Message.FunctionCode;
			FunctionCode functionCode2 = functionCode;
			switch (functionCode2)
			{
			case FunctionCode.ReadOutputStatus:
			case FunctionCode.ReadInputStatus:
				Message.DataCount = IntLib.GetIntFromBoolLength((int)Message.NumberOfPoints);
				return 11 + Message.DataCount * 2;
			case FunctionCode.ReadKeepReg:
			case FunctionCode.ReadInputReg:
				Message.DataCount = (int)(Message.NumberOfPoints * 2);
				return 11 + Message.DataCount * 2;
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
        
		public int GetValidateResponseLength(ModbusMessage Message)
		{
			FunctionCode functionCode = Message.FunctionCode;
			FunctionCode functionCode2 = functionCode;
			switch (functionCode2)
			{
			case FunctionCode.ReadOutputStatus:
			case FunctionCode.ReadInputStatus:
			{
				int num = IntLib.GetIntFromBoolLength((int)Message.NumberOfPoints);
				return 3 + num;
			}
			case FunctionCode.ReadKeepReg:
			case FunctionCode.ReadInputReg:
			{
				int num = (int)(Message.NumberOfPoints * 2);
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
        
		public bool ValidateResponse(ModbusMessage Message)
		{
			return Message.Response != null && Message.Response.Length == this.GetValidateResponseLength(Message) && this.method_5(Message);
		}
        
		private bool method_5(ModbusMessage modbusMessage_0)
		{
			if (modbusMessage_0.Response[0] == modbusMessage_0.SlaveAddress && modbusMessage_0.Response[1] == (byte)modbusMessage_0.FunctionCode)
			{
				FunctionCode functionCode = modbusMessage_0.FunctionCode;
				FunctionCode functionCode2 = functionCode;
				if (functionCode2 - FunctionCode.ReadOutputStatus <= 3)
				{
					return (int)modbusMessage_0.Response[2] == modbusMessage_0.DataCount;
				}
				if (functionCode2 - FunctionCode.ForceCoil <= 1 || functionCode2 - FunctionCode.ForceMultiCoil <= 1)
				{
					return modbusMessage_0.StartAddress == (ushort)modbusMessage_0.Response[2] * 256 + (ushort)modbusMessage_0.Response[3];
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
