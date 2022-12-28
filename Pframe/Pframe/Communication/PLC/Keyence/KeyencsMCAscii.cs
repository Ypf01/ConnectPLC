using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Pframe.Base;
using Pframe.Common;

namespace Pframe.PLC.Keyence
{
    /// <summary>
    ///  基恩士MC ASCII通信类
    /// </summary>
    public class KeyencsMCAscii : NetDeviceBase
    {
        public KeyencsMCAscii(DataFormat DataFormat = DataFormat.DCBA)
        {
            this.NetworkNumber = 0;
            this.NetworkStationNumber = 0;
            base.DataFormat = DataFormat;
        }

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
                IMessage message = new MCAsciiMessage
                {
                    SendData = xktResult.Content
                };
                if (base.SendAndReceive(xktResult.Content, ref array, message, 5000) && (array != null && array.Length != 0))
                {
                    ushort num = Convert.ToUInt16(Encoding.ASCII.GetString(array, 18, 4), 16);
                    if (num > 0)
                        result = CalResult.CreateFailedResult<byte[]>(new CalResult());
                    else
                        result = this.ExtractActualData(array, xktResult.Content[29] == 49);
                }
                else
                    result = CalResult.CreateFailedResult<byte[]>(new CalResult());
            }
            return result;
        }

        public override CalResult<bool[]> ReadBoolArray(string address, ushort length)
        {
            CalResult<KeyenceMcDataType, int> xktResult = KeyenceHelper.KeyenceAnalysisAddress(address);
            CalResult<bool[]> result;
            if (!xktResult.IsSuccess)
                result = CalResult.CreateFailedResult<bool[]>(xktResult);
            else
            {
                if (xktResult.Content1.DataType == 0)
                    CalResult.CreateFailedResult<bool[]>(new CalResult());
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
                IMessage message = new MCAsciiMessage
                {
                    SendData = xktResult.Content
                };
                if (base.SendAndReceive(xktResult.Content, ref array, message, 5000) && array != null)
                {
                    ushort num = Convert.ToUInt16(Encoding.ASCII.GetString(array, 18, 4), 16);
                    if (num > 0)
                        result = CalResult.CreateFailedResult();
                    else
                        result = CalResult.CreateSuccessResult();
                }
                else
                    result = CalResult.CreateFailedResult();
            }
            return result;
        }

        public override CalResult WriteBoolArray(string address, bool[] values)
        {
            return this.WriteByteArray(address, values.Select(new Func<bool, byte>(c => (byte)(c ? 1 : 1))).ToArray<byte>());
        }

        private CalResult<byte[]> BuildReadCommand(string address, ushort length, byte networkNumber = 0, byte networkStationNumber = 0)
        {
            CalResult<KeyenceMcDataType, int> xktResult = KeyenceHelper.KeyenceAnalysisAddress(address);
            CalResult<byte[]> result;
            if (!xktResult.IsSuccess)
            {
                result = CalResult.CreateFailedResult<byte[]>(xktResult);
            }
            else
            {
                result = CalResult.CreateSuccessResult<byte[]>(new byte[]
                {
                    53,
                    48,
                    48,
                    48,
                    KeyenceHelper.BuildBytesFromData(networkNumber)[0],
                    KeyenceHelper.BuildBytesFromData(networkNumber)[1],
                    70,
                    70,
                    48,
                    51,
                    70,
                    70,
                    KeyenceHelper.BuildBytesFromData(networkStationNumber)[0],
                    KeyenceHelper.BuildBytesFromData(networkStationNumber)[1],
                    48,
                    48,
                    49,
                    56,
                    48,
                    48,
                    49,
                    48,
                    48,
                    52,
                    48,
                    49,
                    48,
                    48,
                    48,
                    (byte)((xktResult.Content1.DataType == 0) ? 48 : 49),
                    Encoding.ASCII.GetBytes(xktResult.Content1.AsciiCode)[0],
                    Encoding.ASCII.GetBytes(xktResult.Content1.AsciiCode)[1],
                    KeyenceHelper.BuildBytesFromAddress(xktResult.Content2, xktResult.Content1)[0],
                    KeyenceHelper.BuildBytesFromAddress(xktResult.Content2, xktResult.Content1)[1],
                    KeyenceHelper.BuildBytesFromAddress(xktResult.Content2, xktResult.Content1)[2],
                    KeyenceHelper.BuildBytesFromAddress(xktResult.Content2, xktResult.Content1)[3],
                    KeyenceHelper.BuildBytesFromAddress(xktResult.Content2, xktResult.Content1)[4],
                    KeyenceHelper.BuildBytesFromAddress(xktResult.Content2, xktResult.Content1)[5],
                    KeyenceHelper.BuildBytesFromData(length)[0],
                    KeyenceHelper.BuildBytesFromData(length)[1],
                    KeyenceHelper.BuildBytesFromData(length)[2],
                    KeyenceHelper.BuildBytesFromData(length)[3]
                });
            }
            return result;
        }

        private CalResult<byte[]> BuildWriteCommand(string address, byte[] value, byte networkNumber = 0, byte networkStationNumber = 0)
        {
            CalResult<KeyenceMcDataType, int> xktResult = KeyenceHelper.KeyenceAnalysisAddress(address);
            CalResult<byte[]> result;
            if (!xktResult.IsSuccess)
            {
                result = CalResult.CreateFailedResult<byte[]>(xktResult);
            }
            else
            {
                if (xktResult.Content1.DataType == 1)
                    value = value.Select(new Func<byte, byte>(c => (byte)(c == 0 ? 48 : 49))).ToArray<byte>();
                else
                {
                    byte[] array = new byte[value.Length * 2];
                    for (int i = 0; i < value.Length / 2; i++)
                        KeyenceHelper.BuildBytesFromData(BitConverter.ToUInt16(value, i * 2)).CopyTo(array, 4 * i);
                    value = array;
                }
                byte[] array2 = new byte[42 + value.Length];
                array2[0] = 53;
                array2[1] = 48;
                array2[2] = 48;
                array2[3] = 48;
                array2[4] = KeyenceHelper.BuildBytesFromData(networkNumber)[0];
                array2[5] = KeyenceHelper.BuildBytesFromData(networkNumber)[1];
                array2[6] = 70;
                array2[7] = 70;
                array2[8] = 48;
                array2[9] = 51;
                array2[10] = 70;
                array2[11] = 70;
                array2[12] = KeyenceHelper.BuildBytesFromData(networkStationNumber)[0];
                array2[13] = KeyenceHelper.BuildBytesFromData(networkStationNumber)[1];
                array2[14] = KeyenceHelper.BuildBytesFromData((ushort)(array2.Length - 18))[0];
                array2[15] = KeyenceHelper.BuildBytesFromData((ushort)(array2.Length - 18))[1];
                array2[16] = KeyenceHelper.BuildBytesFromData((ushort)(array2.Length - 18))[2];
                array2[17] = KeyenceHelper.BuildBytesFromData((ushort)(array2.Length - 18))[3];
                array2[18] = 48;
                array2[19] = 48;
                array2[20] = 49;
                array2[21] = 48;
                array2[22] = 49;
                array2[23] = 52;
                array2[24] = 48;
                array2[25] = 49;
                array2[26] = 48;
                array2[27] = 48;
                array2[28] = 48;
                array2[29] = (byte)((xktResult.Content1.DataType == 0) ? 48 : 49);
                array2[30] = Encoding.ASCII.GetBytes(xktResult.Content1.AsciiCode)[0];
                array2[31] = Encoding.ASCII.GetBytes(xktResult.Content1.AsciiCode)[1];
                array2[32] = KeyenceHelper.BuildBytesFromAddress(xktResult.Content2, xktResult.Content1)[0];
                array2[33] = KeyenceHelper.BuildBytesFromAddress(xktResult.Content2, xktResult.Content1)[1];
                array2[34] = KeyenceHelper.BuildBytesFromAddress(xktResult.Content2, xktResult.Content1)[2];
                array2[35] = KeyenceHelper.BuildBytesFromAddress(xktResult.Content2, xktResult.Content1)[3];
                array2[36] = KeyenceHelper.BuildBytesFromAddress(xktResult.Content2, xktResult.Content1)[4];
                array2[37] = KeyenceHelper.BuildBytesFromAddress(xktResult.Content2, xktResult.Content1)[5];
                if (xktResult.Content1.DataType == 1)
                {
                    array2[38] = KeyenceHelper.BuildBytesFromData((ushort)value.Length)[0];
                    array2[39] = KeyenceHelper.BuildBytesFromData((ushort)value.Length)[1];
                    array2[40] = KeyenceHelper.BuildBytesFromData((ushort)value.Length)[2];
                    array2[41] = KeyenceHelper.BuildBytesFromData((ushort)value.Length)[3];
                }
                else
                {
                    array2[38] = KeyenceHelper.BuildBytesFromData((ushort)(value.Length / 4))[0];
                    array2[39] = KeyenceHelper.BuildBytesFromData((ushort)(value.Length / 4))[1];
                    array2[40] = KeyenceHelper.BuildBytesFromData((ushort)(value.Length / 4))[2];
                    array2[41] = KeyenceHelper.BuildBytesFromData((ushort)(value.Length / 4))[3];
                }
                Array.Copy(value, 0, array2, 42, value.Length);
                result = CalResult.CreateSuccessResult<byte[]>(array2);
            }
            return result;
        }

        private CalResult<byte[]> ExtractActualData(byte[] response, bool isBit)
        {
            CalResult<byte[]> result;
            if (isBit)
            {
                byte[] array = new byte[response.Length - 22];
                for (int i = 22; i < response.Length; i++)
                    array[i - 22] = (byte)((response[i] == 48) ? 0 : 1);
                result = CalResult.CreateSuccessResult<byte[]>(array);
            }
            else
            {
                byte[] array2 = new byte[(response.Length - 22) / 2];
                for (int j = 0; j < array2.Length / 2; j++)
                {
                    ushort value = Convert.ToUInt16(Encoding.ASCII.GetString(response, j * 4 + 22, 4), 16);
                    BitConverter.GetBytes(value).CopyTo(array2, j * 2);
                }
                result = CalResult.CreateSuccessResult<byte[]>(array2);
            }
            return result;
        }

        private byte NetworkNumber { get; set; }

        private byte NetworkStationNumber { get; set; }
    }
}
