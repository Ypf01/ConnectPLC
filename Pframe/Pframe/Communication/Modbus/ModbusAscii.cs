using System;
using Pframe.Common;
using Pframe.DataConvert;
using Pframe.Modbus.Message;

namespace Pframe.Modbus
{
	public class ModbusAscii : ModbusAsciiMessage
	{
		public override byte[] ReadOutputStatus(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
			base.Message = new ModbusMessage
			{
				FunctionCode = FunctionCode.ReadOutputStatus,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				NumberOfPoints = numberOfPoints
			};
			byte[] sendByte = base.BuildMessageFrame();
			byte[] value = null;
			if (base.SendAndReceive(sendByte, ref value, base.GetResponseLength()))
			{
				base.Message.Response = base.GetCoreFromAsciiMsg(value);
				if (base.ValidateResponse())
				{
					return base.ReturnData();
				}
			}
			return null;
		}
        
		public override byte[] ReadInputStatus(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
			base.Message = new ModbusMessage
			{
				FunctionCode = FunctionCode.ReadInputStatus,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				NumberOfPoints = numberOfPoints
			};
			byte[] sendByte = base.BuildMessageFrame();
			byte[] value = null;
			if (base.SendAndReceive(sendByte, ref value, base.GetResponseLength()))
			{
				base.Message.Response = base.GetCoreFromAsciiMsg(value);
				if (base.ValidateResponse())
				{
					return base.ReturnData();
				}
			}
			return null;
		}
        
		public override byte[] ReadKeepReg(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
			base.Message = new ModbusMessage
			{
				FunctionCode = FunctionCode.ReadKeepReg,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				NumberOfPoints = numberOfPoints
			};
			byte[] sendByte = base.BuildMessageFrame();
			byte[] value = null;
			if (base.SendAndReceive(sendByte, ref value, base.GetResponseLength()))
			{
				base.Message.Response = base.GetCoreFromAsciiMsg(value);
				if (base.ValidateResponse())
				{
					return base.ReturnData();
				}
			}
			return null;
		}
        
		public override byte[] ReadInputReg(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
			base.Message = new ModbusMessage
			{
				FunctionCode = FunctionCode.ReadInputReg,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				NumberOfPoints = numberOfPoints
			};
			byte[] sendByte = base.BuildMessageFrame();
			byte[] value = null;
			if (base.SendAndReceive(sendByte, ref value, base.GetResponseLength()))
			{
				base.Message.Response = base.GetCoreFromAsciiMsg(value);
				if (base.ValidateResponse())
				{
					return base.ReturnData();
				}
			}
			return null;
		}
        
		public override bool ForceCoil(byte slaveAddress, ushort startAddress, bool value)
		{
			base.Message = new ModbusMessage
			{
				FunctionCode = FunctionCode.ForceCoil,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				WriteBool = value
			};
			byte[] sendByte = base.BuildMessageFrame();
			byte[] value2 = null;
			if (base.SendAndReceive(sendByte, ref value2, base.GetResponseLength()))
			{
				base.Message.Response = base.GetCoreFromAsciiMsg(value2);
				if (base.ValidateResponse())
				{
					return true;
				}
			}
			return false;
		}
        
		public override bool ForceMultiCoil(byte slaveAddress, ushort startAddress, bool[] data)
		{
			base.Message = new ModbusMessage
			{
				FunctionCode = FunctionCode.ForceMultiCoil,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				NumberOfPoints = (ushort)data.Length,
				WriteData = ByteArrayLib.GetByteArrayFromBoolArray(data)
			};
			byte[] sendByte = base.BuildMessageFrame();
			byte[] value = null;
			if (base.SendAndReceive(sendByte, ref value, base.GetResponseLength()))
			{
				base.Message.Response = base.GetCoreFromAsciiMsg(value);
				if (base.ValidateResponse())
				{
					return true;
				}
			}
			return false;
		}
        
		public override bool PreSetSingleReg(byte slaveAddress, ushort startAddress, byte[] value)
		{
			base.Message = new ModbusMessage
			{
				FunctionCode = FunctionCode.PreSetSingleReg,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				WriteData = value
			};
			byte[] sendByte = base.BuildMessageFrame();
			byte[] value2 = null;
			if (base.SendAndReceive(sendByte, ref value2, base.GetResponseLength()))
			{
				base.Message.Response = base.GetCoreFromAsciiMsg(value2);
				if (base.ValidateResponse())
				{
					return true;
				}
			}
			return false;
		}
        
		public override bool PreSetMultiReg(byte slaveAddress, ushort startAddress, byte[] data)
		{
			base.Message = new ModbusMessage
			{
				FunctionCode = FunctionCode.PreSetMultiReg,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				NumberOfPoints = (ushort)(data.Length / 2),
				WriteData = data
			};
			byte[] sendByte = base.BuildMessageFrame();
			byte[] value = null;
			if (base.SendAndReceive(sendByte, ref value, base.GetResponseLength()))
			{
				base.Message.Response = base.GetCoreFromAsciiMsg(value);
				if (base.ValidateResponse())
				{
					return true;
				}
			}
			return false;
		}
        
	}
}
