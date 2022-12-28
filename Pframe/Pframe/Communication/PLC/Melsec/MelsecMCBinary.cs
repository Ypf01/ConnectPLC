using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Pframe.Base;
using Pframe.Common;

namespace Pframe.PLC.Melsec
{
    public class MelsecMCBinary : NetDeviceBase
    {
        public MelsecMCBinary(DataFormat DataFormat = DataFormat.DCBA)
        {
            this.IsFx5U = false;
            this.NetworkNumber = 0;
            this.NetworkStationNumber = 0;
            base.DataFormat = DataFormat;
        }

        public bool IsFx5U { get; set; }

        public override CalResult<byte[]> ReadByteArray(string address, ushort length)
        {
            CalResult<byte[]> xktResult = this.BuildReadCommand(address, length, this.NetworkNumber, this.NetworkStationNumber);
            CalResult<byte[]> result;
            if (!xktResult.IsSuccess)
            {
                result = CalResult.CreateFailedResult<byte[]>(xktResult);
            }
            else
            {
                byte[] array = null;
                IMessage message = new MCBinaryMessage
                {
                    SendData = xktResult.Content
                };
                if (base.SendAndReceive(xktResult.Content, ref array, message, 5000) && (array != null && array.Length > 10))
                {
                    ushort num = BitConverter.ToUInt16(array, 9);
                    if (num > 0)
                        result = CalResult.CreateFailedResult<byte[]>(new CalResult());
                    else
                        result = this.ExtractActualData(array, xktResult.Content[13] == 1);
                }
                else
                    result = CalResult.CreateFailedResult<byte[]>(new CalResult());
            }
            return result;
        }

        public override CalResult<bool[]> ReadBoolArray(string address, ushort length)
        {
            CalResult<MelsecMcDataType, int> xktResult = MelsecHelper.McAnalysisAddress(address, this.IsFx5U);
            CalResult<bool[]> result;
            if (!xktResult.IsSuccess)
                result = CalResult.CreateFailedResult<bool[]>(xktResult);
            else if (xktResult.Content1.DataType == 0)
                result = CalResult.CreateFailedResult<bool[]>(xktResult);
            else
            {
                CalResult<byte[]> xktResult2 = this.ReadByteArray(address, length);
                if (!xktResult2.IsSuccess)
                    result = CalResult.CreateFailedResult<bool[]>(xktResult2);
                else
                    result = CalResult.CreateSuccessResult<bool[]>(xktResult2.Content.Select(new Func<byte, bool>(c => c == 1)).ToArray<bool>());
            }
            return result;
        }

        public override CalResult WriteByteArray(string address, byte[] value)
        {
            CalResult<byte[]> xktResult = this.BuildWriteCommand(address, value, this.NetworkNumber, this.NetworkStationNumber);
            CalResult result;
            if (!xktResult.IsSuccess)
                result = xktResult;
            else
            {
                byte[] array = null;
                IMessage message = new MCBinaryMessage
                {
                    SendData = xktResult.Content
                };
                if (base.SendAndReceive(xktResult.Content, ref array, message, 5000) && array != null)
                {
                    ushort num = BitConverter.ToUInt16(array, 9);
                    result = num > 0 ? CalResult.CreateFailedResult() : CalResult.CreateSuccessResult();
                }
                else
                    result = CalResult.CreateFailedResult();
            }
            return result;
        }

        public override CalResult WriteBoolArray(string address, bool[] values)
        {
            return this.WriteByteArray(address, values.Select(new Func<bool, byte>(c => (byte)(c ? 1 : 0))).ToArray<byte>());
        }

        private CalResult<byte[]> BuildReadCommand(string address, ushort length, byte networkNumber = 0, byte networkStationNumber = 0)
        {
            CalResult<MelsecMcDataType, int> xktResult = MelsecHelper.McAnalysisAddress(address, this.IsFx5U);
            CalResult<byte[]> result;
            if (!xktResult.IsSuccess)
            {
                result = CalResult.CreateFailedResult<byte[]>(xktResult);
            }
            else
            {
                result = CalResult.CreateSuccessResult<byte[]>(new byte[]
                {
                    80,
                    0,
                    networkNumber,
                    byte.MaxValue,
                    byte.MaxValue,
                    3,
                    networkStationNumber,
                    12,
                    0,
                    10,
                    0,
                    1,
                    4,
                    xktResult.Content1.DataType,
                    0,
                    BitConverter.GetBytes(xktResult.Content2)[0],
                    BitConverter.GetBytes(xktResult.Content2)[1],
                    BitConverter.GetBytes(xktResult.Content2)[2],
                    xktResult.Content1.DataCode,
                    (byte)(length % 256),
                    (byte)(length / 256)
                });
            }
            return result;
        }

        private CalResult<byte[]> BuildWriteCommand(string address, byte[] value, byte networkNumber = 0, byte networkStationNumber = 0)
        {
            CalResult<MelsecMcDataType, int> xktResult = MelsecHelper.McAnalysisAddress(address, this.IsFx5U);
            CalResult<byte[]> result;
            if (!xktResult.IsSuccess)
                result = CalResult.CreateFailedResult<byte[]>(xktResult);
            else
            {
                int num = -1;
                if (xktResult.Content1.DataType == 1)
                {
                    num = value.Length;
                    value = MelsecHelper.TransBoolArrayToByteData(value);
                }
                byte[] array = new byte[21 + value.Length];
                array[0] = 80;
                array[1] = 0;
                array[2] = networkNumber;
                array[3] = byte.MaxValue;
                array[4] = byte.MaxValue;
                array[5] = 3;
                array[6] = networkStationNumber;
                array[7] = (byte)((array.Length - 9) % 256);
                array[8] = (byte)((array.Length - 9) / 256);
                array[9] = 10;
                array[10] = 0;
                array[11] = 1;
                array[12] = 20;
                array[13] = xktResult.Content1.DataType;
                array[14] = 0;
                array[15] = BitConverter.GetBytes(xktResult.Content2)[0];
                array[16] = BitConverter.GetBytes(xktResult.Content2)[1];
                array[17] = BitConverter.GetBytes(xktResult.Content2)[2];
                array[18] = xktResult.Content1.DataCode;
                if (xktResult.Content1.DataType == 1)
                {
                    if (num > 0)
                    {
                        array[19] = (byte)(num % 256);
                        array[20] = (byte)(num / 256);
                    }
                    else
                    {
                        array[19] = (byte)(value.Length * 2 % 256);
                        array[20] = (byte)(value.Length * 2 / 256);
                    }
                }
                else
                {
                    array[19] = (byte)(value.Length / 2 % 256);
                    array[20] = (byte)(value.Length / 2 / 256);
                }
                Array.Copy(value, 0, array, 21, value.Length);
                result = CalResult.CreateSuccessResult<byte[]>(array);
            }
            return result;
        }

        private CalResult<byte[]> ExtractActualData(byte[] response, bool isBit)
        {
            CalResult<byte[]> result;
            if (isBit)
            {
                byte[] array = new byte[(response.Length - 11) * 2];
                for (int i = 11; i < response.Length; i++)
                {
                    if ((response[i] & 16) == 16)
                        array[(i - 11) * 2] = 1;
                    if ((response[i] & 1) == 1)
                        array[(i - 11) * 2 + 1] = 1;
                }
                result = CalResult.CreateSuccessResult<byte[]>(array);
            }
            else
            {
                byte[] array2 = new byte[response.Length - 11];
                Array.Copy(response, 11, array2, 0, array2.Length);
                result = CalResult.CreateSuccessResult<byte[]>(array2);
            }
            return result;
        }

        private byte NetworkNumber;

        private byte NetworkStationNumber;
    }
}
