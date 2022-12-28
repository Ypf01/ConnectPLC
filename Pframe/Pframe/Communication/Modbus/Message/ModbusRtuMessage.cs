using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Pframe.Common;
using Pframe.DataConvert;

namespace Pframe.Modbus.Message
{
    public class ModbusRtuMessage : ModbusSerialBase
    {
        public ModbusMessage Message { get; set; }

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

        public byte[] BuildMessageFrame()
        {
            ByteArray byteArray = new ByteArray();
            FunctionCode functionCode = this.Message.FunctionCode;
            FunctionCode functionCode2 = functionCode;
            switch (functionCode2)
            {
                case FunctionCode.ReadOutputStatus:
                case FunctionCode.ReadInputStatus:
                case FunctionCode.ReadKeepReg:
                case FunctionCode.ReadInputReg:
                    byteArray.Add(this.Message.SlaveAddress);
                    byteArray.Add((byte)this.Message.FunctionCode);
                    byteArray.Add(this.Message.StartAddress);
                    byteArray.Add(this.Message.NumberOfPoints);
                    byteArray.Add(ParityHelper.CalculateCRC(byteArray.array, byteArray.Length));
                    break;
                case FunctionCode.ForceCoil:
                    {
                        byteArray.Add(this.Message.SlaveAddress);
                        byteArray.Add((byte)this.Message.FunctionCode);
                        byteArray.Add(this.Message.StartAddress);
                        ByteArray byteArray2 = byteArray;
                        byte[] items;
                        if (!this.Message.WriteBool)
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
                    byteArray.Add(this.Message.SlaveAddress);
                    byteArray.Add((byte)this.Message.FunctionCode);
                    byteArray.Add(this.Message.StartAddress);
                    byteArray.Add(this.Message.WriteData);
                    byteArray.Add(ParityHelper.CalculateCRC(byteArray.array, byteArray.Length));
                    break;
                default:
                    if (functionCode2 != FunctionCode.ForceMultiCoil)
                    {
                        if (functionCode2 == FunctionCode.PreSetMultiReg)
                        {
                            byteArray.Add(this.Message.SlaveAddress);
                            byteArray.Add((byte)this.Message.FunctionCode);
                            byteArray.Add(this.Message.StartAddress);
                            byteArray.Add(this.Message.NumberOfPoints);
                            byteArray.Add((byte)this.Message.WriteData.Length);
                            byteArray.Add(this.Message.WriteData);
                            byteArray.Add(ParityHelper.CalculateCRC(byteArray.array, byteArray.Length));
                        }
                    }
                    else
                    {
                        byteArray.Add(this.Message.SlaveAddress);
                        byteArray.Add((byte)this.Message.FunctionCode);
                        byteArray.Add(this.Message.StartAddress);
                        byteArray.Add(this.Message.NumberOfPoints);
                        byteArray.Add((byte)this.Message.WriteData.Length);
                        byteArray.Add(this.Message.WriteData);
                        byteArray.Add(ParityHelper.CalculateCRC(byteArray.array, byteArray.Length));
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
                    return 5 + this.Message.DataCount;
                case FunctionCode.ReadKeepReg:
                case FunctionCode.ReadInputReg:
                    this.Message.DataCount = (int)(this.Message.NumberOfPoints * 2);
                    return 5 + this.Message.DataCount;
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

        public bool ValidateResponse()
        {
            return this.Message.Response != null && this.Message.Response.Length == this.GetResponseLength() && this.method_5() && ParityHelper.CheckCRC(this.Message.Response);
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
    }
}
