using System;
using Pframe.Common;
using Pframe.DataConvert;

namespace Pframe.Modbus.Message
{
	public class ModbusRtuOverTcpMessage : ModbusNetBase
	{
		public byte[] BuildMessageFrame(ModbusMessage Message)
		{
			ByteArray byteArray = new ByteArray();
			FunctionCode functionCode = Message.FunctionCode;
			FunctionCode functionCode2 = functionCode;
			switch (functionCode2)
			{
			case FunctionCode.ReadOutputStatus:
			case FunctionCode.ReadInputStatus:
			case FunctionCode.ReadKeepReg:
			case FunctionCode.ReadInputReg:
				byteArray.Add(Message.SlaveAddress);
				byteArray.Add((byte)Message.FunctionCode);
				byteArray.Add(Message.StartAddress);
				byteArray.Add(Message.NumberOfPoints);
				byteArray.Add(ParityHelper.CalculateCRC(byteArray.array, byteArray.Length));
				break;
			case FunctionCode.ForceCoil:
			{
				byteArray.Add(Message.SlaveAddress);
				byteArray.Add((byte)Message.FunctionCode);
				byteArray.Add(Message.StartAddress);
				ByteArray byteArray2 = byteArray;
				byte[] items;
				if (!Message.WriteBool)
				{
					items = new byte[2];
				}
				else
				{
					(items = new byte[2])[0] = byte.MaxValue;
				}
				byteArray2.Add(items);
				byteArray.Add(ParityHelper.CalculateCRC(byteArray.array, byteArray.Length));
				break;
			}
			case FunctionCode.PreSetSingleReg:
				byteArray.Add(Message.SlaveAddress);
				byteArray.Add((byte)Message.FunctionCode);
				byteArray.Add(Message.StartAddress);
				byteArray.Add(Message.WriteData);
				byteArray.Add(ParityHelper.CalculateCRC(byteArray.array, byteArray.Length));
				break;
			default:
				if (functionCode2 != FunctionCode.ForceMultiCoil)
				{
					if (functionCode2 == FunctionCode.PreSetMultiReg)
					{
						byteArray.Add(Message.SlaveAddress);
						byteArray.Add((byte)Message.FunctionCode);
						byteArray.Add(Message.StartAddress);
						byteArray.Add(Message.NumberOfPoints);
						byteArray.Add((byte)Message.WriteData.Length);
						byteArray.Add(Message.WriteData);
						byteArray.Add(ParityHelper.CalculateCRC(byteArray.array, byteArray.Length));
					}
				}
				else
				{
					byteArray.Add(Message.SlaveAddress);
					byteArray.Add((byte)Message.FunctionCode);
					byteArray.Add(Message.StartAddress);
					byteArray.Add(Message.NumberOfPoints);
					byteArray.Add((byte)Message.WriteData.Length);
					byteArray.Add(Message.WriteData);
					byteArray.Add(ParityHelper.CalculateCRC(byteArray.array, byteArray.Length));
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
				return 5 + Message.DataCount;
			case FunctionCode.ReadKeepReg:
			case FunctionCode.ReadInputReg:
				Message.DataCount = (int)(Message.NumberOfPoints * 2);
				return 5 + Message.DataCount;
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
			return 8;
		}
        
		public bool ValidateResponse(ModbusMessage Message)
		{
			return Message.Response != null && Message.Response.Length == this.GetResponseLength(Message) && this.method_5(Message) && ParityHelper.CheckCRC(Message.Response);
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
        
	}
}
