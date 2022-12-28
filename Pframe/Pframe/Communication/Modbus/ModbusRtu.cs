using System;
using Pframe.Common;
using Pframe.DataConvert;
using Pframe.Modbus.Message;

namespace Pframe.Modbus
{
    public class ModbusRtu : ModbusRtuMessage
    {
        private static object theLock = new object();
        public override byte[] ReadOutputStatus(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            lock (theLock)
            {
                base.Message = new ModbusMessage
                {
                    FunctionCode = FunctionCode.ReadOutputStatus,
                    SlaveAddress = slaveAddress,
                    StartAddress = startAddress,
                    NumberOfPoints = numberOfPoints
                };
                byte[] sendByte = base.BuildMessageFrame();
                byte[] response = null;
                if (base.SendAndReceive(sendByte, ref response, base.GetResponseLength()))
                {
                    base.Message.Response = response;
                    if (base.ValidateResponse())
                        return base.ReturnData();
                }
                return null;
            }
        }

        public override byte[] ReadInputStatus(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            lock (theLock)
            {
                base.Message = new ModbusMessage
                {
                    FunctionCode = FunctionCode.ReadInputStatus,
                    SlaveAddress = slaveAddress,
                    StartAddress = startAddress,
                    NumberOfPoints = numberOfPoints
                };
                byte[] sendByte = base.BuildMessageFrame();
                byte[] response = null;
                if (base.SendAndReceive(sendByte, ref response, base.GetResponseLength()))
                {
                    base.Message.Response = response;
                    if (base.ValidateResponse())
                    {
                        return base.ReturnData();
                    }
                }
                return null;
            }
        }

        public override byte[] ReadKeepReg(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            lock (theLock)
            {
                base.Message = new ModbusMessage
                {
                    FunctionCode = FunctionCode.ReadKeepReg,
                    SlaveAddress = slaveAddress,
                    StartAddress = startAddress,
                    NumberOfPoints = numberOfPoints
                };
                byte[] sendByte = base.BuildMessageFrame();
                byte[] response = null;
                if (base.SendAndReceive(sendByte, ref response, base.GetResponseLength()))
                {
                    base.Message.Response = response;
                    if (base.ValidateResponse())
                    {
                        return base.ReturnData();
                    }
                }
                return null;
            }
        }

        public override byte[] ReadInputReg(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            lock (theLock)
            {
                base.Message = new ModbusMessage
                {
                    FunctionCode = FunctionCode.ReadInputReg,
                    SlaveAddress = slaveAddress,
                    StartAddress = startAddress,
                    NumberOfPoints = numberOfPoints
                };
                byte[] sendByte = base.BuildMessageFrame();
                byte[] response = null;
                if (base.SendAndReceive(sendByte, ref response, base.GetResponseLength()))
                {
                    base.Message.Response = response;
                    if (base.ValidateResponse())
                    {
                        return base.ReturnData();
                    }
                }
                return null;
            }
        }

        public override bool ForceCoil(byte slaveAddress, ushort startAddress, bool value)
        {
            lock (theLock)
            {
                base.Message = new ModbusMessage
                {
                    FunctionCode = FunctionCode.ForceCoil,
                    SlaveAddress = slaveAddress,
                    StartAddress = startAddress,
                    WriteBool = value
                };
                byte[] sendByte = base.BuildMessageFrame();
                byte[] response = null;
                if (base.SendAndReceive(sendByte, ref response, base.GetResponseLength()))
                {
                    base.Message.Response = response;
                    if (base.ValidateResponse())
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override bool ForceMultiCoil(byte slaveAddress, ushort startAddress, bool[] data)
        {
            lock (theLock)
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
                byte[] response = null;
                if (base.SendAndReceive(sendByte, ref response, base.GetResponseLength()))
                {
                    base.Message.Response = response;
                    if (base.ValidateResponse())
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override bool PreSetSingleReg(byte slaveAddress, ushort startAddress, byte[] value)
        {
            lock (theLock)
            {
                base.Message = new ModbusMessage
                {
                    FunctionCode = FunctionCode.PreSetSingleReg,
                    SlaveAddress = slaveAddress,
                    StartAddress = startAddress,
                    WriteData = value
                };
                byte[] sendByte = base.BuildMessageFrame();
                byte[] response = null;
                if (base.SendAndReceive(sendByte, ref response, base.GetResponseLength()))
                {
                    base.Message.Response = response;
                    if (base.ValidateResponse())
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override bool PreSetMultiReg(byte slaveAddress, ushort startAddress, byte[] data)
        {
            lock (theLock)
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
                byte[] response = null;
                if (base.SendAndReceive(sendByte, ref response, base.GetResponseLength()))
                {
                    base.Message.Response = response;
                    if (base.ValidateResponse())
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
