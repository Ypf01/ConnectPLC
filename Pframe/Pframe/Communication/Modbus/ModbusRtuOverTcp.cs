using System;
using Pframe.Common;
using Pframe.DataConvert;
using Pframe.Modbus.Message;

namespace Pframe.Modbus
{
	public class ModbusRtuOverTcp : ModbusRtuOverTcpMessage
	{
		public override byte[] ReadOutputStatus(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
			ModbusMessage modbusMessage = new ModbusMessage
			{
				FunctionCode = FunctionCode.ReadOutputStatus,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				NumberOfPoints = numberOfPoints
			};
			byte[] sendByte = base.BuildMessageFrame(modbusMessage);
			byte[] response = null;
			if (base.SendAndReceive(sendByte, ref response, new ModbusRTUMsg(modbusMessage), 5000))
			{
				modbusMessage.Response = response;
				if (base.ValidateResponse(modbusMessage))
				{
					return base.ReturnData(modbusMessage);
				}
			}
			return null;
		}
        
		public override byte[] ReadInputStatus(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
			ModbusMessage modbusMessage = new ModbusMessage
			{
				FunctionCode = FunctionCode.ReadInputStatus,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				NumberOfPoints = numberOfPoints
			};
			byte[] sendByte = base.BuildMessageFrame(modbusMessage);
			byte[] response = null;
			if (base.SendAndReceive(sendByte, ref response, new ModbusRTUMsg(modbusMessage), 5000))
			{
				modbusMessage.Response = response;
				if (base.ValidateResponse(modbusMessage))
				{
					return base.ReturnData(modbusMessage);
				}
			}
			return null;
		}
        
		public override byte[] ReadKeepReg(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
			ModbusMessage modbusMessage = new ModbusMessage
			{
				FunctionCode = FunctionCode.ReadKeepReg,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				NumberOfPoints = numberOfPoints
			};
			byte[] sendByte = base.BuildMessageFrame(modbusMessage);
			byte[] response = null;
			if (base.SendAndReceive(sendByte, ref response, new ModbusRTUMsg(modbusMessage), 5000))
			{
				modbusMessage.Response = response;
				if (base.ValidateResponse(modbusMessage))
				{
					return base.ReturnData(modbusMessage);
				}
			}
			return null;
		}
        
		public override byte[] ReadInputReg(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
			ModbusMessage modbusMessage = new ModbusMessage
			{
				FunctionCode = FunctionCode.ReadInputReg,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				NumberOfPoints = numberOfPoints
			};
			byte[] sendByte = base.BuildMessageFrame(modbusMessage);
			byte[] response = null;
			if (base.SendAndReceive(sendByte, ref response, new ModbusRTUMsg(modbusMessage), 5000))
			{
				modbusMessage.Response = response;
				if (base.ValidateResponse(modbusMessage))
				{
					return base.ReturnData(modbusMessage);
				}
			}
			return null;
		}
        
		public override bool ForceCoil(byte slaveAddress, ushort startAddress, bool value)
		{
			ModbusMessage modbusMessage = new ModbusMessage
			{
				FunctionCode = FunctionCode.ForceCoil,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				WriteBool = value
			};
			byte[] sendByte = base.BuildMessageFrame(modbusMessage);
			byte[] response = null;
			if (base.SendAndReceive(sendByte, ref response, new ModbusRTUMsg(modbusMessage), 5000))
			{
				modbusMessage.Response = response;
				if (base.ValidateResponse(modbusMessage))
				{
					return true;
				}
			}
			return false;
		}
        
		public override bool ForceMultiCoil(byte slaveAddress, ushort startAddress, bool[] data)
		{
			ModbusMessage modbusMessage = new ModbusMessage
			{
				FunctionCode = FunctionCode.ForceMultiCoil,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				NumberOfPoints = (ushort)data.Length,
				WriteData = ByteArrayLib.GetByteArrayFromBoolArray(data)
			};
			byte[] sendByte = base.BuildMessageFrame(modbusMessage);
			byte[] response = null;
			if (base.SendAndReceive(sendByte, ref response, new ModbusRTUMsg(modbusMessage), 5000))
			{
				modbusMessage.Response = response;
				if (base.ValidateResponse(modbusMessage))
				{
					return true;
				}
			}
			return false;
		}
        
		public override bool PreSetSingleReg(byte slaveAddress, ushort startAddress, byte[] value)
		{
			ModbusMessage modbusMessage = new ModbusMessage
			{
				FunctionCode = FunctionCode.PreSetSingleReg,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				WriteData = value
			};
			byte[] sendByte = base.BuildMessageFrame(modbusMessage);
			byte[] response = null;
			if (base.SendAndReceive(sendByte, ref response, new ModbusRTUMsg(modbusMessage), 5000))
			{
				modbusMessage.Response = response;
				if (base.ValidateResponse(modbusMessage))
				{
					return true;
				}
			}
			return false;
		}
        
		public override bool PreSetMultiReg(byte slaveAddress, ushort startAddress, byte[] data)
		{
			ModbusMessage modbusMessage = new ModbusMessage
			{
				FunctionCode = FunctionCode.PreSetMultiReg,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				NumberOfPoints = (ushort)(data.Length / 2),
				WriteData = data
			};
			byte[] sendByte = base.BuildMessageFrame(modbusMessage);
			byte[] response = null;
			if (base.SendAndReceive(sendByte, ref response, new ModbusRTUMsg(modbusMessage), 5000))
			{
				modbusMessage.Response = response;
				if (base.ValidateResponse(modbusMessage))
				{
					return true;
				}
			}
			return false;
		}
        
	}
}
