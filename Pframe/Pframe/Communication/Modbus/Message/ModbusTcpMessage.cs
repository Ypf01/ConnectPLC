using System;
using Pframe.Common;
using Pframe.DataConvert;
using Pframe.Modbus.Interface;

namespace Pframe.Modbus.Message
{
	public class ModbusTcpMessage : ModbusNetBase, IModbusMessage
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
				byteArray.Add(this.method_6());
				byteArray.Add(0);
				byteArray.Add(6);
				byteArray.Add(Message.SlaveAddress);
				byteArray.Add((byte)Message.FunctionCode);
				byteArray.Add(Message.StartAddress);
				byteArray.Add(Message.NumberOfPoints);
				break;
			case FunctionCode.ForceCoil:
			{
				byteArray.Add(this.method_6());
				byteArray.Add(0);
				byteArray.Add(6);
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
				break;
			}
			case FunctionCode.PreSetSingleReg:
				byteArray.Add(this.method_6());
				byteArray.Add(0);
				byteArray.Add(6);
				byteArray.Add(Message.SlaveAddress);
				byteArray.Add((byte)Message.FunctionCode);
				byteArray.Add(Message.StartAddress);
				byteArray.Add(Message.WriteData);
				break;
			default:
				if (functionCode2 != FunctionCode.ForceMultiCoil)
				{
					if (functionCode2 == FunctionCode.PreSetMultiReg)
					{
						byteArray.Add(this.method_6());
						byteArray.Add(0);
						byteArray.Add((byte)(7 + Message.WriteData.Length));
						byteArray.Add(Message.SlaveAddress);
						byteArray.Add((byte)Message.FunctionCode);
						byteArray.Add(Message.StartAddress);
						byteArray.Add(Message.NumberOfPoints);
						byteArray.Add((byte)Message.WriteData.Length);
						byteArray.Add(Message.WriteData);
					}
				}
				else
				{
					byteArray.Add(this.method_6());
					byteArray.Add(0);
					byteArray.Add((byte)(7 + Message.WriteData.Length));
					byteArray.Add(Message.SlaveAddress);
					byteArray.Add((byte)Message.FunctionCode);
					byteArray.Add(Message.StartAddress);
					byteArray.Add(Message.NumberOfPoints);
					byteArray.Add((byte)Message.WriteData.Length);
					byteArray.Add(Message.WriteData);
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
					result = ByteArrayLib.GetByteArray(Message.Response, 9, Message.DataCount);
				}
			}
			else
			{
				result = ByteArrayLib.GetByteArray(Message.Response, 9, Message.DataCount);
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
				return 9 + Message.DataCount;
			case FunctionCode.ReadKeepReg:
			case FunctionCode.ReadInputReg:
				Message.DataCount = (int)(Message.NumberOfPoints * 2);
				return 9 + Message.DataCount;
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
			return 12;
		}
        
		public bool ValidateResponse(ModbusMessage Message)
		{
			return Message.Response != null && Message.Response.Length == this.GetResponseLength(Message) && this.method_5(Message);
		}
        
		private bool method_5(ModbusMessage modbusMessage_0)
		{
			if (modbusMessage_0.Response[6] == modbusMessage_0.SlaveAddress && modbusMessage_0.Response[7] == (byte)modbusMessage_0.FunctionCode)
			{
				FunctionCode functionCode = modbusMessage_0.FunctionCode;
				FunctionCode functionCode2 = functionCode;
				if (functionCode2 - FunctionCode.ReadOutputStatus <= 3)
				{
					return (int)modbusMessage_0.Response[8] == modbusMessage_0.DataCount;
				}
				if (functionCode2 - FunctionCode.ForceCoil <= 1 || functionCode2 - FunctionCode.ForceMultiCoil <= 1)
				{
					return modbusMessage_0.StartAddress == (ushort)modbusMessage_0.Response[8] * 256 + (ushort)modbusMessage_0.Response[9];
				}
			}
			return false;
		}
        
		private ushort method_6()
		{
			object obj = ModbusTcpMessage.object_0;
			lock (obj)
			{
				ushort num2;
				if (this.numCalu != 65535)
				{
					ushort num = (ushort)(this.numCalu + 1);
					this.numCalu = num;
					num2 = num;
				}
				else
				{
					num2 = 1;
				}
				this.numCalu = num2;
			}
			return this.numCalu;
		}
           
		static ModbusTcpMessage()
		{
			ModbusTcpMessage.object_0 = new object();
		}

		private static readonly object object_0;

		private ushort numCalu;
	}
}
