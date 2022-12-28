using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Pframe.Base;
using Pframe.Common;
using Pframe.DataConvert;

namespace Pframe.PLC.Panasonic
{
    public class Mewtocol : SerialDeviceBase
    {
        public Mewtocol(DataFormat DataFormat = DataFormat.DCBA, byte station = 238)
        {
            this.hexCharList = new List<char>
            {
                '0',
                '1',
                '2',
                '3',
                '4',
                '5',
                '6',
                '7',
                '8',
                '9',
                'A',
                'B',
                'C',
                'D',
                'E',
                'F'
            };
            base.DataFormat = DataFormat;
            this.Station = station;
        }

        public Mewtocol(byte station)
        {
            this.hexCharList = new List<char>
            {
                '0',
                '1',
                '2',
                '3',
                '4',
                '5',
                '6',
                '7',
                '8',
                '9',
                'A',
                'B',
                'C',
                'D',
                'E',
                'F'
            };
            base.DataFormat = DataFormat.DCBA;
            this.Station = station;
        }

        public Mewtocol(DataFormat DataFormat)
        {
            this.hexCharList = new List<char>
            {
                '0',
                '1',
                '2',
                '3',
                '4',
                '5',
                '6',
                '7',
                '8',
                '9',
                'A',
                'B',
                'C',
                'D',
                'E',
                'F'
            };
            base.DataFormat = DataFormat;
            this.Station = 238;
        }

        public byte Station { get; set; }

        public override CalResult<byte[]> ReadByteArray(string address, ushort length)
        {
            byte[] array = this.BuildReadCommand(this.Station, address, length);
            CalResult<byte[]> result;
            if (array == null)
                result = CalResult.CreateFailedResult<byte[]>(new CalResult());
            else
            {
                byte[] byte_ = null;
                if (base.SendAndReceive(array, ref byte_, 0))
                    result = this.ExtraActualData(byte_);
                else
                    result = CalResult.CreateFailedResult<byte[]>(new CalResult());
            }
            return result;
        }

        public override CalResult<bool[]> ReadBoolArray(string address, ushort length)
        {
            CalResult<byte[]> xktResult = this.ReadByteArray(address, length);
            CalResult<bool[]> result;
            if (!xktResult.IsSuccess)
                result = null;
            else
                result = CalResult.CreateSuccessResult<bool[]>(BitLib.GetBitArray(BitLib.GetBitArrayFromByteArray(xktResult.Content, false), 0, (int)length));
            return result;
        }

        public bool ReadSingleBool(string address, ref bool val)
        {
            byte[] array = this.BuildReadOneCoil(this.Station, address);
            bool result;
            if (array == null)
            {
                result = false;
            }
            else
            {
                byte[] byte_ = null;
                result = (base.SendAndReceive(array, ref byte_, 0) && this.ExtraActualBool(byte_, ref val));
            }
            return result;
        }

        public override CalResult WriteByteArray(string address, byte[] value)
        {
            byte[] array = this.BuildWriteCommand(this.Station, address, value, -1);
            CalResult result;
            if (array == null)
                result = CalResult.CreateFailedResult();
            else
            {
                byte[] byte_ = null;
                if (base.SendAndReceive(array, ref byte_, 0) && this.ExtraActualData(byte_) != null)
                    result = CalResult.CreateSuccessResult();
                else
                    result = CalResult.CreateFailedResult();
            }
            return result;
        }

        public override CalResult WriteBoolArray(string address, bool[] values)
        {
            byte[] byteArrayFromBoolArray = ByteArrayLib.GetByteArrayFromBoolArray(values);
            byte[] array = this.BuildWriteCommand(this.Station, address, this.BytesReverseByWord(byteArrayFromBoolArray), (short)values.Length);
            CalResult result;
            if (array == null)
                result = CalResult.CreateFailedResult();
            else
            {
                byte[] byte_ = null;
                if (base.SendAndReceive(array, ref byte_, 0) && this.ExtraActualData(byte_) != null)
                    result = CalResult.CreateSuccessResult();
                else
                    result = CalResult.CreateFailedResult();
            }
            return result;
        }

        private byte[] BuildReadCommand(byte station, string address, ushort length)
        {
            byte[] result;
            if (address == null)
                result = null;
            else
            {
                CalResult<string, int> xktResult = this.AnalysisAddress(address);
                if (!xktResult.IsSuccess)
                    result = null;
                else
                {
                    StringBuilder stringBuilder = new StringBuilder("%");
                    stringBuilder.Append(station.ToString("X2"));
                    stringBuilder.Append("#");
                    if (xktResult.Content1 == "X" || xktResult.Content1 == "Y" || xktResult.Content1 == "R" || xktResult.Content1 == "L")
                    {
                        stringBuilder.Append("RCC");
                        stringBuilder.Append(xktResult.Content1);
                        stringBuilder.Append(xktResult.Content2.ToString("D4"));
                        stringBuilder.Append((xktResult.Content2 + (int)length - 1).ToString("D4"));
                    }
                    else if (xktResult.Content1 == "D" || xktResult.Content1 == "L" || xktResult.Content1 == "F")
                    {
                        stringBuilder.Append("RD");
                        stringBuilder.Append(xktResult.Content1);
                        stringBuilder.Append(xktResult.Content2.ToString("D5"));
                        stringBuilder.Append((xktResult.Content2 + (int)length - 1).ToString("D5"));
                    }
                    else if (xktResult.Content1 == "IX" || xktResult.Content1 == "IY" || xktResult.Content1 == "ID")
                    {
                        stringBuilder.Append("RD");
                        stringBuilder.Append(xktResult.Content1);
                        stringBuilder.Append("000000000");
                    }
                    else
                    {
                        if (!(xktResult.Content1 == "C") && !(xktResult.Content1 == "T"))
                        {
                            return null;
                        }
                        stringBuilder.Append("RS");
                        stringBuilder.Append(xktResult.Content2.ToString("D4"));
                        stringBuilder.Append((xktResult.Content2 + (int)length - 1).ToString("D4"));
                    }
                    stringBuilder.Append(this.CalculateCrc(stringBuilder));
                    stringBuilder.Append('\r');
                    result = Encoding.ASCII.GetBytes(stringBuilder.ToString());
                }
            }
            return result;
        }

        private byte[] BuildWriteCommand(byte station, string address, byte[] values, short short_0 = -1)
        {
            byte[] result;
            if (address == null)
                result = null;
            else
            {
                CalResult<string, int> xktResult = this.AnalysisAddress(address);
                if (!xktResult.IsSuccess)
                {
                    result = null;
                }
                else
                {
                    values = this.ArrayExpandToLengthEven<byte>(values);
                    if (short_0 == -1)
                    {
                        short_0 = (short)(values.Length / 2);
                    }
                    StringBuilder stringBuilder = new StringBuilder("%");
                    stringBuilder.Append(station.ToString("X2"));
                    stringBuilder.Append("#");
                    if (xktResult.Content1 == "X" || xktResult.Content1 == "Y" || xktResult.Content1 == "R" || xktResult.Content1 == "L")
                    {
                        stringBuilder.Append("WCC");
                        stringBuilder.Append(xktResult.Content1);
                        stringBuilder.Append(xktResult.Content2.ToString("D4"));
                        stringBuilder.Append((xktResult.Content2 + (int)short_0 - 1).ToString("D4"));
                    }
                    else if (xktResult.Content1 == "D" || xktResult.Content1 == "L" || xktResult.Content1 == "F")
                    {
                        stringBuilder.Append("WD");
                        stringBuilder.Append(xktResult.Content1);
                        stringBuilder.Append(xktResult.Content2.ToString("D5"));
                        stringBuilder.Append((xktResult.Content2 + (int)short_0 - 1).ToString("D5"));
                    }
                    else if (xktResult.Content1 == "IX" || xktResult.Content1 == "IY" || xktResult.Content1 == "ID")
                    {
                        stringBuilder.Append("WD");
                        stringBuilder.Append(xktResult.Content1);
                        stringBuilder.Append(xktResult.Content2.ToString("D9"));
                        stringBuilder.Append((xktResult.Content2 + (int)short_0 - 1).ToString("D9"));
                    }
                    else if (xktResult.Content1 == "C" || xktResult.Content1 == "T")
                    {
                        stringBuilder.Append("WS");
                        stringBuilder.Append(xktResult.Content2.ToString("D4"));
                        stringBuilder.Append((xktResult.Content2 + (int)short_0 - 1).ToString("D4"));
                    }
                    stringBuilder.Append(this.ByteToHexString(values));
                    stringBuilder.Append(this.CalculateCrc(stringBuilder));
                    stringBuilder.Append('\r');
                    result = Encoding.ASCII.GetBytes(stringBuilder.ToString());
                }
            }
            return result;
        }

        private byte[] BuildWriteOneCoil(byte station, string address, bool value)
        {
            StringBuilder stringBuilder = new StringBuilder("%");
            stringBuilder.Append(station.ToString("X2"));
            stringBuilder.Append("#WCS");
            CalResult<string, int> xktResult = this.AnalysisAddress(address);
            byte[] result;
            if (!xktResult.IsSuccess)
            {
                result = null;
            }
            else
            {
                stringBuilder.Append(xktResult.Content1);
                if (xktResult.Content1 == "X" || xktResult.Content1 == "Y" || xktResult.Content1 == "R" || xktResult.Content1 == "L")
                {
                    stringBuilder.Append((xktResult.Content2 / 16).ToString("D3"));
                    stringBuilder.Append((xktResult.Content2 % 16).ToString("X1"));
                }
                else
                {
                    if (!(xktResult.Content1 == "T") && !(xktResult.Content1 == "C"))
                    {
                        return null;
                    }
                    stringBuilder.Append("0");
                    stringBuilder.Append(xktResult.Content2.ToString("D3"));
                }
                stringBuilder.Append(value ? '1' : '0');
                stringBuilder.Append(this.CalculateCrc(stringBuilder));
                stringBuilder.Append('\r');
                result = Encoding.ASCII.GetBytes(stringBuilder.ToString());
            }
            return result;
        }

        private byte[] BuildReadOneCoil(byte station, string address)
        {
            byte[] result;
            if (address == null)
                result = null;
            else if (address.Length < 1 || address.Length > 8)
                result = null;
            else
            {
                StringBuilder stringBuilder = new StringBuilder("%");
                stringBuilder.Append(station.ToString("X2"));
                stringBuilder.Append("#RCS");
                CalResult<string, int> xktResult = this.AnalysisAddress(address);
                if (!xktResult.IsSuccess)
                {
                    result = null;
                }
                else
                {
                    stringBuilder.Append(xktResult.Content1);
                    if (xktResult.Content1 == "X" || xktResult.Content1 == "Y" || xktResult.Content1 == "R" || xktResult.Content1 == "L")
                    {
                        stringBuilder.Append((xktResult.Content2 / 16).ToString("D3"));
                        stringBuilder.Append((xktResult.Content2 % 16).ToString("X1"));
                    }
                    else
                    {
                        if (!(xktResult.Content1 == "T") && !(xktResult.Content1 == "C"))
                        {
                            return null;
                        }
                        stringBuilder.Append("0");
                        stringBuilder.Append(xktResult.Content2.ToString("D3"));
                    }
                    stringBuilder.Append(this.CalculateCrc(stringBuilder));
                    stringBuilder.Append('\r');
                    result = Encoding.ASCII.GetBytes(stringBuilder.ToString());
                }
            }
            return result;
        }

        private CalResult<byte[]> ExtraActualData(byte[] response)
        {
            CalResult<byte[]> result;
            if (response.Length < 9)
            {
                result = new CalResult<byte[]>(this.PanasonicReceiveLengthMustLargerThan9());
            }
            else if (response[3] == 36)
            {
                byte[] array = new byte[response.Length - 9];
                if (array.Length != 0)
                {
                    Array.Copy(response, 6, array, 0, array.Length);
                    array = this.HexStringToBytes(Encoding.ASCII.GetString(array));
                }
                result = CalResult.CreateSuccessResult<byte[]>(array);
            }
            else if (response[3] == 33)
            {
                int num = int.Parse(Encoding.ASCII.GetString(response, 4, 2));
                result = new CalResult<byte[]>(num, this.GetErrorDescription(num));
            }
            else
            {
                result = new CalResult<byte[]>(this.UnknownError());
            }
            return result;
        }

        private string GetErrorDescription(int err)
        {
            switch (err)
            {
                case 20:
                    return this.PanasonicMewStatus20();
                case 21:
                    return this.PanasonicMewStatus21();
                case 22:
                    return this.PanasonicMewStatus22();
                case 23:
                    return this.PanasonicMewStatus23();
                case 24:
                    return this.PanasonicMewStatus24();
                case 25:
                    return this.PanasonicMewStatus25();
                case 26:
                    return this.PanasonicMewStatus26();
                case 27:
                    return this.PanasonicMewStatus27();
                case 28:
                    return this.PanasonicMewStatus28();
                case 29:
                    return this.PanasonicMewStatus29();
                case 30:
                    return this.PanasonicMewStatus30();
                case 40:
                    return this.PanasonicMewStatus40();
                case 41:
                    return this.PanasonicMewStatus41();
                case 42:
                    return this.PanasonicMewStatus42();
                case 43:
                    return this.PanasonicMewStatus43();
                case 50:
                    return this.PanasonicMewStatus50();
                case 51:
                    return this.PanasonicMewStatus51();
                case 52:
                    return this.PanasonicMewStatus52();
                case 53:
                    return this.PanasonicMewStatus53();
                case 60:
                    return this.PanasonicMewStatus60();
                case 61:
                    return this.PanasonicMewStatus61();
                case 62:
                    return this.PanasonicMewStatus62();
                case 63:
                    return this.PanasonicMewStatus63();
                case 65:
                    return this.PanasonicMewStatus65();
                case 66:
                    return this.PanasonicMewStatus66();
                case 67:
                    return this.PanasonicMewStatus67();
            }
            return this.UnknownError();
        }

        private string CalculateCrc(StringBuilder sb)
        {
            byte b = (byte)sb[0];
            for (int i = 1; i < sb.Length; i++)
                b ^= (byte)sb[i];
            return this.ByteToHexString(new byte[]
            {
                b
            });
        }

        private bool ExtraActualBool(byte[] response, ref bool val)
        {
            bool result;
            if (response.Length < 9)
            {
                result = false;
            }
            else if (response[3] == 36)
            {
                val = (response[6] == 49);
                result = true;
            }
            else if (response[3] == 33)
            {
                int.Parse(Encoding.ASCII.GetString(response, 4, 2));
                result = false;
            }
            else
            {
                result = false;
            }
            return result;
        }

        private CalResult<string, int> AnalysisAddress(string address)
        {
            CalResult<string, int> xktResult = new CalResult<string, int>();
            try
            {
                xktResult.Content2 = 0;
                if (address.StartsWith("IX") || address.StartsWith("ix"))
                {
                    xktResult.Content1 = "IX";
                    xktResult.Content2 = int.Parse(address.Substring(2));
                }
                else if (address.StartsWith("IY") || address.StartsWith("iy"))
                {
                    xktResult.Content1 = "IY";
                    xktResult.Content2 = int.Parse(address.Substring(2));
                }
                else if (address.StartsWith("ID") || address.StartsWith("id"))
                {
                    xktResult.Content1 = "ID";
                    xktResult.Content2 = int.Parse(address.Substring(2));
                }
                else if (address[0] == 'X' || address[0] == 'x')
                {
                    xktResult.Content1 = "X";
                    xktResult.Content2 = this.CalculateComplexAddress(address.Substring(1));
                }
                else if (address[0] == 'Y' || address[0] == 'y')
                {
                    xktResult.Content1 = "Y";
                    xktResult.Content2 = this.CalculateComplexAddress(address.Substring(1));
                }
                else if (address[0] == 'R' || address[0] == 'r')
                {
                    xktResult.Content1 = "R";
                    xktResult.Content2 = this.CalculateComplexAddress(address.Substring(1));
                }
                else if (address[0] == 'T' || address[0] == 't')
                {
                    xktResult.Content1 = "T";
                    xktResult.Content2 = (int)ushort.Parse(address.Substring(1));
                }
                else if (address[0] == 'C' || address[0] == 'c')
                {
                    xktResult.Content1 = "C";
                    xktResult.Content2 = (int)ushort.Parse(address.Substring(1));
                }
                else if (address[0] == 'L' || address[0] == 'l')
                {
                    xktResult.Content1 = "L";
                    xktResult.Content2 = this.CalculateComplexAddress(address.Substring(1));
                }
                else if (address[0] == 'D' || address[0] == 'd')
                {
                    xktResult.Content1 = "D";
                    xktResult.Content2 = (int)ushort.Parse(address.Substring(1));
                }
                else if (address[0] == 'F' || address[0] == 'f')
                {
                    xktResult.Content1 = "F";
                    xktResult.Content2 = (int)ushort.Parse(address.Substring(1));
                }
                else if (address[0] == 'S' || address[0] == 's')
                {
                    xktResult.Content1 = "S";
                    xktResult.Content2 = (int)ushort.Parse(address.Substring(1));
                }
                else
                {
                    if (address[0] != 'K' && address[0] != 'k')
                        throw new Exception(this.NotSupportedDataType());
                    xktResult.Content1 = "K";
                    xktResult.Content2 = (int)ushort.Parse(address.Substring(1));
                }
            }
            catch (Exception ex)
            {
                xktResult.Message = ex.Message;
                return xktResult;
            }
            xktResult.IsSuccess = true;
            return xktResult;
        }

        private int CalculateComplexAddress(string address)
        {
            int num;
            if (address.IndexOf(".") < 0)
                num = Convert.ToInt32(address, 16);
            else
            {
                num = Convert.ToInt32(address.Substring(0, address.IndexOf("."))) * 16;
                string text = address.Substring(address.IndexOf(".") + 1);
                if (text.Contains("A") || text.Contains("B") || text.Contains("C") || text.Contains("D") || text.Contains("E") || text.Contains("F"))
                    num += Convert.ToInt32(text, 16);
                else
                    num += Convert.ToInt32(text);
            }
            return num;
        }

        private byte[] HexStringToBytes(string hex)
        {
            hex = hex.ToUpper();
            MemoryStream memoryStream = new MemoryStream();
            for (int i = 0; i < hex.Length; i++)
            {
                if (i + 1 < hex.Length && (this.hexCharList.Contains(hex[i]) && this.hexCharList.Contains(hex[i + 1])))
                {
                    memoryStream.WriteByte((byte)(this.hexCharList.IndexOf(hex[i]) * 16 + this.hexCharList.IndexOf(hex[i + 1])));
                    i++;
                }
            }
            byte[] result = memoryStream.ToArray();
            memoryStream.Dispose();
            return result;
        }

        private byte[] BytesToAsciiBytes(byte[] inBytes)
        {
            return Encoding.ASCII.GetBytes(this.ByteToHexString(inBytes));
        }

        private string ByteToHexString(byte[] InBytes)
        {
            return this.ByteToHexString(InBytes, '\0');
        }

        private string ByteToHexString(byte[] InBytes, char segment)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in InBytes)
            {
                if (segment == '\0')
                {
                    stringBuilder.Append(string.Format("{0:X2}", b));
                }
                else
                {
                    stringBuilder.Append(string.Format("{0:X2}{1}", b, segment));
                }
            }
            if (segment != '\0' && stringBuilder.Length > 1 && stringBuilder[stringBuilder.Length - 1] == segment)
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            return stringBuilder.ToString();
        }

        private byte[] BuildAsciiBytesFrom(byte value)
        {
            return Encoding.ASCII.GetBytes(value.ToString("X2"));
        }

        private T[] ArrayExpandToLengthEven<T>(T[] data)
        {
            T[] result;
            if (data == null)
            {
                result = new T[0];
            }
            else if (data.Length % 2 == 1)
            {
                result = this.ArrayExpandToLength<T>(data, data.Length + 1);
            }
            else
            {
                result = data;
            }
            return result;
        }
        
        private T[] ArrayExpandToLength<T>(T[] data, int length)
        {
            T[] result;
            if (data == null)
            {
                result = new T[length];
            }
            else if (data.Length == length)
            {
                result = data;
            }
            else
            {
                T[] array = new T[length];
                Array.Copy(data, array, Math.Min(data.Length, array.Length));
                result = array;
            }
            return result;
        }
        
        private byte[] BytesReverseByWord(byte[] inBytes)
        {
            byte[] result;
            if (inBytes == null)
            {
                result = null;
            }
            else
            {
                byte[] array = this.ArrayExpandToLengthEven<byte>(inBytes);
                for (int i = 0; i < array.Length / 2; i++)
                {
                    byte b = array[i * 2];
                    array[i * 2] = array[i * 2 + 1];
                    array[i * 2 + 1] = b;
                }
                result = array;
            }
            return result;
        }

        private string PanasonicReceiveLengthMustLargerThan9()
        {
            return "接收数据长度必须大于9";
        }

        private string PanasonicAddressParameterCannotBeNull()
        {
            return "地址参数不允许为空";
        }

        private string PanasonicMewStatus20()
        {
            return "错误未知";
        }

        private string PanasonicMewStatus21()
        {
            return "NACK错误，远程单元无法被正确识别，或者发生了数据错误。";
        }

        private string PanasonicMewStatus22()
        {
            return "WACK 错误:用于远程单元的接收缓冲区已满。";
        }

        private string PanasonicMewStatus23()
        {
            return "多重端口错误:远程单元编号(01 至 16)设置与本地单元重复。";
        }

        private string PanasonicMewStatus24()
        {
            return "传输格式错误:试图发送不符合传输格式的数据，或者某一帧数据溢出或发生了数据错误。";
        }

        private string PanasonicMewStatus25()
        {
            return "硬件错误:传输系统硬件停止操作。";
        }

        private string PanasonicMewStatus26()
        {
            return "单元号错误:远程单元的编号设置超出 01 至 63 的范围。";
        }

        private string PanasonicMewStatus27()
        {
            return "不支持错误:接收方数据帧溢出. 试图在不同的模块之间发送不同帧长度的数据。";
        }

        private string PanasonicMewStatus28()
        {
            return "无应答错误:远程单元不存在. (超时)。";
        }

        private string PanasonicMewStatus29()
        {
            return "缓冲区关闭错误:试图发送或接收处于关闭状态的缓冲区。";
        }

        private string PanasonicMewStatus30()
        {
            return "超时错误:持续处于传输禁止状态。";
        }

        private string PanasonicMewStatus40()
        {
            return "BCC 错误:在指令数据中发生传输错误。";
        }

        private string PanasonicMewStatus41()
        {
            return "格式错误:所发送的指令信息不符合传输格式。";
        }

        private string PanasonicMewStatus42()
        {
            return "不支持错误:发送了一个未被支持的指令。向未被支持的目标站发送了指令。";
        }

        private string PanasonicMewStatus43()
        {
            return "处理步骤错误:在处于传输请求信息挂起时,发送了其他指令。";
        }

        private string PanasonicMewStatus50()
        {
            return "链接设置错误:设置了实际不存在的链接编号。";
        }

        private string PanasonicMewStatus51()
        {
            return "同时操作错误:当向其他单元发出指令时,本地单元的传输缓冲区已满。";
        }

        private string PanasonicMewStatus52()
        {
            return "传输禁止错误:无法向其他单元传输。";
        }

        private string PanasonicMewStatus53()
        {
            return "忙错误:在接收到指令时,正在处理其他指令。";
        }

        private string PanasonicMewStatus60()
        {
            return "参数错误:在指令中包含有无法使用的代码,或者代码没有附带区域指定参数(X, Y, D), 等以外。";
        }

        private string PanasonicMewStatus61()
        {
            return "数据错误:触点编号,区域编号,数据代码格式(BCD,hex,等)上溢出, 下溢出以及区域指定错误。";
        }

        private string PanasonicMewStatus62()
        {
            return "寄存器错误:过多记录数据在未记录状态下的操作（监控记录、跟踪记录等。)。";
        }

        private string PanasonicMewStatus63()
        {
            return "PLC 模式错误:当一条指令发出时，运行模式不能够对指令进行处理。";
        }

        private string PanasonicMewStatus65()
        {
            return "保护错误:在存储保护状态下执行写操作到程序区域或系统寄存器。";
        }

        private string PanasonicMewStatus66()
        {
            return "地址错误:地址（程序地址、绝对地址等）数据编码形式（BCD、hex 等）、上溢、下溢或指定范围错误。";
        }

        private string PanasonicMewStatus67()
        {
            return "丢失数据错误:要读的数据不存在。（读取没有写入注释寄存区的数据。。";
        }

        private string NotSupportedDataType()
        {
            return "输入的类型不支持，请重新输入";
        }

        private string UnknownError()
        {
            return "未知错误";
        }

        private List<char> hexCharList;
    }
}
