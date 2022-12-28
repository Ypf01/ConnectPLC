using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using Pframe.Base;
using Pframe.Common;
using Pframe.DataConvert;
using Pframe.PLC.Common;
using System.Net;

namespace Pframe.PLC.AB
{
    public class ABCIP : NetABS7DeviceBase
    {
        private IMessage GetNewNetMessage()
        {
            return new ABMessage();
        }

        private string AllenBradley04()
        {
            return "它没有正确生成或匹配标记不存在。";
        }

        private string AllenBradley05()
        {
            return "引用的特定项（通常是实例）无法找到。";
        }
        private string AllenBradley06()
        {
            return "请求的数据量不适合响应缓冲区。 发生了部分数据传输。";
        }
        private string AllenBradley0A()
        {
            return "尝试处理其中一个属性时发生错误。";
        }

        private string AllenBradley13()
        {
            return "命令中没有提供足够的命令数据/参数来执行所请求的服务。";
        }

        private string AllenBradley1C()
        {
            return "与属性计数相比，提供的属性数量不足。";
        }

        private string AllenBradley1E()
        {
            return "此服务中的服务请求出错。";
        }

        private string AllenBradley26()
        {
            return "IOI字长与处理的IOI数量不匹配。";
        }

        private string AllenBradleySessionStatus00()
        {
            return "成功";
        }

        private string AllenBradleySessionStatus01()
        {
            return "发件人发出无效或不受支持的封装命令。";
        }
        private string AllenBradleySessionStatus02()
        {
            return "接收器中的内存资源不足以处理命令。 这不是一个应用程序错误。 相反，只有在封装层无法获得所需内存资源的情况下才会导致此问题。";
        }

        private string AllenBradleySessionStatus03()
        {
            return "封装消息的数据部分中的数据形成不良或不正确。";
        }

        private string AllenBradleySessionStatus64()
        {
            return "向目标发送封装消息时，始发者使用了无效的会话句柄。";
        }

        private string AllenBradleySessionStatus65()
        {
            return "目标收到一个无效长度的信息。";
        }

        private string AllenBradleySessionStatus69()
        {
            return "不支持的封装协议修订。";
        }

        private string UnknownError()
        {
            return "未知错误。";
        }

        public int SendTimeOut { get; set; }

        public int ReceiveTimeOut { get; set; }

        public DataFormat DataFormat { get; set; }

        public int WaitTimes { get; set; }

        public int SleepTime { get; set; }

        public uint SessionHandle { get; protected set; }

        public byte Slot { get; set; }

        public byte[] PortSlot { get; set; }
        public ushort CipCommand { get; set; }

        public int ConnectTimeOut { get; set; }
        public bool Connect_A(string ip, int port)
        {
            this.tcpclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.tcpclient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, this.ReceiveTimeOut);
            this.tcpclient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, this.SendTimeOut);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
            IAsyncResult asyncResult = this.tcpclient.BeginConnect(remoteEP, null, null);
            bool result;
            if (!(result = asyncResult.AsyncWaitHandle.WaitOne(this.ConnectTimeOut, true)))
                this.tcpclient.Close();
            return result;
        }
        public bool Connect(string ip, int port = 44818)
        {
            if (!Connect_A(ip, port))
                return false;
            byte[] array = this.SendAndReceive_Child(this.RegisterSessionHandle());
            bool result;
            if (array == null)
                result = false;
            else
            {
                CalResult xktResult = this.CheckResponse(array);
                if (!xktResult.IsSuccess)
                    result = false;
                else
                {
                    this.SessionHandle = UIntLib.GetUIntFromByteArray(array, 4, this.DataFormat);
                    result = true;
                }
            }
            return result;
        }

        private byte[] RegisterSessionHandle()
        {
            byte[] array = new byte[4];
            array[0] = 1;
            byte[] byte_ = array;
            return this.PackRequestHeader(101, 0U, byte_);
        }

        private byte[] UnRegisterSessionHandle()
        {
            return this.PackRequestHeader(102, this.SessionHandle, new byte[0]);
        }

        public bool DisConnect()
        {
            byte[] array = this.SendAndReceive_Child(this.UnRegisterSessionHandle());
            bool result;
            if (array == null)
                result = false;
            else
            {
                CalResult xktResult = this.CheckResponse(array);
                if (!xktResult.IsSuccess)
                    result = false;
                else
                {
                    this.DisConnect();
                    result = true;
                }
            }
            return result;
        }

        public CalResult<string> ReadString(string address, ushort length, Encoding encoding)
        {
            CalResult<byte[]> xktResult = this.Read(address, length);
            CalResult<string> result;
            if (!xktResult.IsSuccess)
                result = CalResult.CreateFailedResult<string>(xktResult);
            else
                result = CalResult.CreateSuccessResult<string>(encoding.GetString(xktResult.Content));
            return result;
        }

        public CalResult<byte[]> Read(string address, ushort length)
        {
            CalResult<byte[]> result;
            if (length > 1)
                result = this.ReadSegment(address, 0, (int)length);
            else
            {
                result = this.Read(new string[]
                {
                    address
                }, new int[]
                {
                    (int)length
                });
            }
            return result;
        }

        private CalResult<byte[]> ReadSegment(string address, int startIndex, int length)
        {
            CalResult<byte[]> result;
            try
            {
                List<byte> list = new List<byte>();
                CalResult<byte[]> xktResult;
                CalResult<byte[], ushort, bool> xktResult2;
                while (true)
                {
                    xktResult = this.ReadCipFromServer(new byte[][]
                    {
                        this.PackRequestReadSegment(address, startIndex, length)
                    });
                    if (!xktResult.IsSuccess)
                    {
                        break;
                    }
                    xktResult2 = this.ExtractActualData(xktResult.Content, true);
                    if (!xktResult2.IsSuccess)
                    {
                        goto IL_6D;
                    }
                    startIndex += xktResult2.Content1.Length;
                    list.AddRange(xktResult2.Content1);
                    if (!xktResult2.Content3)
                        goto IL_76;
                }
                return xktResult;
                IL_6D:
                return CalResult.CreateFailedResult<byte[]>(xktResult2);
                IL_76:
                result = CalResult.CreateSuccessResult<byte[]>(list.ToArray());
            }
            catch (Exception ex)
            {
                result = new CalResult<byte[]>("Address Wrong:" + ex.Message);
            }
            return result;
        }

        private CalResult<byte[]> ReadCipFromServer(params byte[][] cips)
        {
            byte[][] array = new byte[2][];
            array[0] = new byte[4];
            int num = 1;
            byte[] byte_;
            if ((byte_ = this.PortSlot) == null)
            {
                byte[] array2 = new byte[2];
                array2[0] = 1;
                byte_ = array2;
                array2[1] = this.Slot;
            }
            array[num] = this.PackCommandService(byte_, cips.ToArray<byte[]>());
            byte[] byte_2 = this.PackCommandSpecificData(array);
            byte[] byte_3 = this.PackRequestHeader(this.CipCommand, this.SessionHandle, byte_2);
            byte[] array3 = this.SendAndReceive_Child(byte_3);
            CalResult<byte[]> result;
            if (array3 == null)
            {
                result = CalResult.CreateFailedResult<byte[]>(new CalResult<byte[]>("Read Error"));
            }
            else
            {
                CalResult xktResult = this.CheckResponse(array3);
                if (!xktResult.IsSuccess)
                    result = CalResult.CreateFailedResult<byte[]>(xktResult);
                else
                    result = CalResult.CreateSuccessResult<byte[]>(array3);
            }
            return result;
        }

        public CalResult<byte[]> Read(string[] address, int[] length)
        {
            CalResult<byte[], ushort, bool> xktResult = this.ReadWithType(address, length);
            CalResult<byte[]> result;
            if (!xktResult.IsSuccess)
                result = CalResult.CreateFailedResult<byte[]>(xktResult);
            else
                result = CalResult.CreateSuccessResult<byte[]>(xktResult.Content1);
            return result;
        }

        public CalResult<byte[]> Read(string[] address)
        {
            CalResult<byte[]> result;
            if (address == null)
            {
                result = new CalResult<byte[]>("address can not be null");
            }
            else
            {
                int[] array = new int[address.Length];
                for (int i = 0; i < array.Length; i++)
                    array[i] = 1;
                result = this.Read(address, array);
            }
            return result;
        }

        private CalResult<byte[], ushort, bool> ReadWithType(string[] address, int[] length)
        {
            CalResult<byte[]> xktResult = this.BuildReadCommand(address, length);
            CalResult<byte[], ushort, bool> result;
            if (!xktResult.IsSuccess)
            {
                result = CalResult.CreateFailedResult<byte[], ushort, bool>(xktResult);
            }
            else
            {
                byte[] array = this.SendAndReceive_Child(xktResult.Content);
                if (array == null)
                {
                    result = CalResult.CreateFailedResult<byte[], ushort, bool>(new CalResult<byte[]>("Read Error"));
                }
                else
                {
                    CalResult xktResult2 = this.CheckResponse(array);
                    if (!xktResult2.IsSuccess)
                    {
                        result = CalResult.CreateFailedResult<byte[], ushort, bool>(xktResult2);
                    }
                    else
                    {
                        result = this.ExtractActualData(array, true);
                    }
                }
            }
            return result;
        }

        private CalResult CheckResponse(byte[] response)
        {
            CalResult result;
            try
            {
                int intFromByteArray = IntLib.GetIntFromByteArray(response, 8, this.DataFormat);
                if (intFromByteArray == 0)
                {
                    result = CalResult.CreateSuccessResult();
                }
                else
                {
                    string msg = string.Empty;
                    int num = intFromByteArray;
                    int num2 = num;
                    if (num2 <= 100)
                    {
                        switch (num2)
                        {
                            case 1:
                                msg = this.AllenBradleySessionStatus01();
                                goto IL_97;
                            case 2:
                                msg = this.AllenBradleySessionStatus02();
                                goto IL_97;
                            case 3:
                                msg = this.AllenBradleySessionStatus03();
                                goto IL_97;
                            default:
                                if (num2 == 100)
                                {
                                    msg = this.AllenBradleySessionStatus64();
                                    goto IL_97;
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (num2 == 101)
                        {
                            msg = this.AllenBradleySessionStatus65();
                            goto IL_97;
                        }
                        if (num2 == 105)
                        {
                            msg = this.AllenBradleySessionStatus69();
                            goto IL_97;
                        }
                    }
                    msg = this.UnknownError();
                    IL_97:
                    result = new CalResult(intFromByteArray, msg);
                }
            }
            catch (Exception ex)
            {
                result = new CalResult(ex.Message);
            }
            return result;
        }

        private CalResult<byte[]> BuildReadCommand(string[] address, int[] length)
        {
            CalResult<byte[]> result;
            if (address == null || length == null)
            {
                result = new CalResult<byte[]>("address or length is null");
            }
            else if (address.Length != length.Length)
            {
                result = new CalResult<byte[]>("address and length is not same array");
            }
            else
            {
                try
                {
                    List<byte[]> list = new List<byte[]>();
                    for (int i = 0; i < address.Length; i++)
                    {
                        list.Add(this.PackRequsetRead(address[i], length[i]));
                    }
                    byte[][] array = new byte[2][];
                    array[0] = new byte[4];
                    int num = 1;
                    byte[] byte_;
                    if ((byte_ = this.PortSlot) == null)
                    {
                        byte[] array2 = new byte[2];
                        array2[0] = 1;
                        byte_ = array2;
                        array2[1] = this.Slot;
                    }
                    array[num] = this.PackCommandService(byte_, list.ToArray());
                    byte[] byte_2 = this.PackCommandSpecificData(array);
                    result = CalResult.CreateSuccessResult<byte[]>(this.PackRequestHeader(this.CipCommand, this.SessionHandle, byte_2));
                }
                catch (Exception ex)
                {
                    result = new CalResult<byte[]>("Address Wrong:" + ex.Message);
                }
            }
            return result;
        }

        public CalResult Write(string address, object value, ComplexDataType datatype)
        {
            CalResult result;
            try
            {
                switch (datatype)
                {
                    case ComplexDataType.Bool:
                        {
                            ushort ushort_ = 193;
                            byte[] byte_;
                            if (!(value.ToString() == "1") && !(value.ToString().ToLower() == "true"))
                            {
                                byte_ = new byte[2];
                            }
                            else
                            {
                                byte[] array = new byte[2];
                                array[0] = byte.MaxValue;
                                byte_ = array;
                                array[1] = byte.MaxValue;
                            }
                            return this.WriteTag(address, ushort_, byte_, 1);
                        }
                    case ComplexDataType.Byte:
                        {
                            ushort ushort_2 = 194;
                            byte[] array2 = new byte[2];
                            array2[0] = Convert.ToByte(value);
                            return this.WriteTag(address, ushort_2, array2, 1);
                        }
                    case ComplexDataType.Short:
                        return this.WriteTag(address, 195, ByteArrayLib.GetByteArrayFromShort(Convert.ToInt16(value), this.DataFormat), 1);
                    case ComplexDataType.UShort:
                        return this.WriteTag(address, 195, ByteArrayLib.GetByteArrayFromUShort(Convert.ToUInt16(value), this.DataFormat), 1);
                    case ComplexDataType.Int:
                        return this.WriteTag(address, 196, ByteArrayLib.GetByteArrayFromInt(Convert.ToInt32(value), this.DataFormat), 1);
                    case ComplexDataType.UInt:
                        return this.WriteTag(address, 196, ByteArrayLib.GetByteArrayFromUInt(Convert.ToUInt32(value), this.DataFormat), 1);
                    case ComplexDataType.Float:
                        return this.WriteTag(address, 202, ByteArrayLib.GetByteArrayFromFloat(Convert.ToSingle(value), this.DataFormat), 1);
                    case ComplexDataType.Double:
                        return this.WriteTag(address, 203, ByteArrayLib.GetByteArrayFromDouble(Convert.ToDouble(value), this.DataFormat), 1);
                    case ComplexDataType.Long:
                        return this.WriteTag(address, 197, ByteArrayLib.GetByteArrayFromLong(Convert.ToInt64(value), this.DataFormat), 1);
                    case ComplexDataType.ULong:
                        return this.WriteTag(address, 197, ByteArrayLib.GetByteArrayFromULong(Convert.ToUInt64(value), this.DataFormat), 1);
                    case ComplexDataType.String:
                        return this.WriteTag(address, 208, ByteArrayLib.GetByteArrayFromOmronCIPString(value.ToString()), 1);
                    case ComplexDataType.ByteArray:
                        return this.WriteTag(address, 208, ByteArrayLib.GetByteArrayFromHexString(value.ToString(), ' '), 1);
                }
                result = CalResult.CreateFailedResult();
            }
            catch (Exception ex)
            {
                CalResult xktResult = new CalResult
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
                result = xktResult;
            }
            return result;
        }

        private CalResult WriteTag(string address, ushort typeCode, byte[] value, int length = 1)
        {
            CalResult<byte[]> xktResult = this.BuildWriteCommand(address, typeCode, value, length);
            CalResult result;
            if (!xktResult.IsSuccess)
                result = xktResult;
            else
            {
                byte[] array = this.SendAndReceive_Child(xktResult.Content);
                if (array == null)
                    result = CalResult.CreateFailedResult();
                else
                {
                    CalResult xktResult2 = this.CheckResponse(array);
                    if (!xktResult2.IsSuccess)
                        result = CalResult.CreateFailedResult<byte[]>(xktResult2);
                    else
                        result = this.ExtractActualData(array, false);
                }
            }
            return result;
        }

        private CalResult<byte[]> BuildWriteCommand(string address, ushort typeCode, byte[] data, int length = 1)
        {
            CalResult<byte[]> result;
            try
            {
                byte[] array = this.PackRequestWrite(address, typeCode, data, length);
                byte[][] array2 = new byte[2][];
                array2[0] = new byte[4];
                int num = 1;
                byte[] byte_3;
                if ((byte_3 = this.PortSlot) == null)
                {
                    byte[] array3 = new byte[2];
                    array3[0] = 1;
                    byte_3 = array3;
                    array3[1] = this.Slot;
                }
                array2[num] = this.PackCommandService(byte_3, new byte[][]
                {
                    array
                });
                byte[] byte_4 = this.PackCommandSpecificData(array2);
                result = CalResult.CreateSuccessResult<byte[]>(this.PackRequestHeader(this.CipCommand, this.SessionHandle, byte_4));
            }
            catch (Exception ex)
            {
                result = new CalResult<byte[]>("Address Wrong:" + ex.Message);
            }
            return result;
        }

        private byte[] SendAndReceive_Child(byte[] SendByte)
        {
            this.InteractiveLock.Enter();
            new MemoryStream();
            byte[] result;
            try
            {
                IMessage message = this.GetNewNetMessage();
                if (message != null)
                    message.SendData = SendByte;
                this.tcpclient.Send(SendByte, SendByte.Length, SocketFlags.None);
                CalResult<byte[]> xktResult = base.ReceiveByMessage(this.tcpclient, this.ReceiveTimeOut, message, null);
                if (!xktResult.IsSuccess)
                    result = null;
                else if (message != null && !message.CheckHeadDataLegal(base.Token.ToByteArray()))
                    result = null;
                else
                    result = xktResult.Content;
            }
            catch (Exception)
            {
                result = null;
            }
            finally
            {
                this.InteractiveLock.Leave();
            }
            return result;
        }

        private byte[] PackRequestHeader(ushort command, uint session, byte[] commandSpecificData)
        {
            byte[] array = new byte[commandSpecificData.Length + 24];
            Array.Copy(commandSpecificData, 0, array, 24, commandSpecificData.Length);
            BitConverter.GetBytes(command).CopyTo(array, 0);
            BitConverter.GetBytes(session).CopyTo(array, 4);
            BitConverter.GetBytes((ushort)commandSpecificData.Length).CopyTo(array, 2);
            return array;
        }

        private byte[] PackRequsetRead(string address, int length)
        {
            byte[] array = new byte[1024];
            array[0] = 76;
            byte[] array2 = this.BuildRequestPathCommand(address);
            array2.CopyTo(array, 2);
            int num = 2 + array2.Length;
            array[1] = (byte)((num - 2) / 2);
            array[num++] = BitConverter.GetBytes(length)[0];
            array[num++] = BitConverter.GetBytes(length)[1];
            byte[] array3 = new byte[num];
            Array.Copy(array, 0, array3, 0, num);
            return array3;
        }

        private byte[] BuildRequestPathCommand(string address)
        {
            byte[] result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                string[] array = address.Split(new char[]
                {
                    '.'
                }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < array.Length; i++)
                {
                    string text = string.Empty;
                    int num = array[i].IndexOf('[');
                    int num2 = array[i].IndexOf(']');
                    if (num > 0 && num2 > 0 && num2 > num)
                    {
                        text = array[i].Substring(num + 1, num2 - num - 1);
                        array[i] = array[i].Substring(0, num);
                    }
                    memoryStream.WriteByte(145);
                    memoryStream.WriteByte((byte)array[i].Length);
                    byte[] bytes = Encoding.ASCII.GetBytes(array[i]);
                    memoryStream.Write(bytes, 0, bytes.Length);
                    if (bytes.Length % 2 == 1)
                    {
                        memoryStream.WriteByte(0);
                    }
                    if (!string.IsNullOrEmpty(text))
                    {
                        string[] array2 = text.Split(new char[]
                        {
                            ','
                        }, StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 0; j < array2.Length; j++)
                        {
                            int num3 = Convert.ToInt32(array2[j]);
                            if (num3 < 256)
                            {
                                memoryStream.WriteByte(40);
                                memoryStream.WriteByte((byte)num3);
                            }
                            else
                            {
                                memoryStream.WriteByte(41);
                                memoryStream.WriteByte(0);
                                memoryStream.WriteByte(BitConverter.GetBytes(num3)[0]);
                                memoryStream.WriteByte(BitConverter.GetBytes(num3)[1]);
                            }
                        }
                    }
                }
                result = memoryStream.ToArray();
            }
            return result;
        }

        private string ParseRequestPathCommand(byte[] pathCommand)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < pathCommand.Length; i++)
            {
                if (pathCommand[i] == 145)
                {
                    string @string = Encoding.ASCII.GetString(pathCommand, i + 2, (int)pathCommand[i + 1]);
                    stringBuilder.Append(@string);
                    int num = 2 + @string.Length;
                    if (@string.Length % 2 == 1)
                    {
                        num++;
                    }
                    if (pathCommand.Length > num + i)
                    {
                        if (pathCommand[i + num] == 40)
                        {
                            stringBuilder.Append(string.Format("[{0}]", pathCommand[i + num + 1]));
                        }
                        else if (pathCommand[i + num] == 41)
                        {
                            stringBuilder.Append(string.Format("[{0}]", BitConverter.ToUInt16(pathCommand, i + num + 2)));
                        }
                    }
                    stringBuilder.Append(".");
                }
            }
            if (stringBuilder[stringBuilder.Length - 1] == '.')
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            return stringBuilder.ToString();
        }

        private byte[] GetEnumeratorCommand(ushort startInstance)
        {
            return new byte[]
            {
                85,
                3,
                32,
                107,
                37,
                0,
                BitConverter.GetBytes(startInstance)[0],
                BitConverter.GetBytes(startInstance)[1],
                2,
                0,
                1,
                0,
                2,
                0
            };
        }

        private byte[] GetStructHandleCommand(ushort symbolType)
        {
            byte[] array = new byte[18];
            byte[] bytes = BitConverter.GetBytes(symbolType);
            bytes[1] = (byte)(bytes[1] & 15);
            array[0] = 3;
            array[1] = 3;
            array[2] = 32;
            array[3] = 108;
            array[4] = 37;
            array[5] = 0;
            array[6] = bytes[0];
            array[7] = bytes[1];
            array[8] = 4;
            array[9] = 0;
            array[10] = 4;
            array[11] = 0;
            array[12] = 5;
            array[13] = 0;
            array[14] = 2;
            array[15] = 0;
            array[16] = 1;
            array[17] = 0;
            return array;
        }

        private byte[] GetStructItemNameType(ushort symbolType, ABStruct structHandle)
        {
            byte[] array = new byte[14];
            ushort value = (ushort)(structHandle.TemplateObjectDefinitionSize * 4U - 21U);
            byte[] bytes = BitConverter.GetBytes(symbolType);
            bytes[1] = (byte)(bytes[1] & 15);
            byte[] bytes2 = BitConverter.GetBytes(0);
            byte[] bytes3 = BitConverter.GetBytes(value);
            array[0] = 76;
            array[1] = 3;
            array[2] = 32;
            array[3] = 108;
            array[4] = 37;
            array[5] = 0;
            array[6] = bytes[0];
            array[7] = bytes[1];
            array[8] = bytes2[0];
            array[9] = bytes2[1];
            array[10] = bytes2[2];
            array[11] = bytes2[3];
            array[12] = bytes3[0];
            array[13] = bytes3[1];
            return array;
        }

        private byte[] PackRequestReadSegment(string address, int startIndex, int length)
        {
            byte[] array = new byte[1024];
            array[0] = 82;
            byte[] array2 = this.BuildRequestPathCommand(address);
            array2.CopyTo(array, 2);
            int num = 2 + array2.Length;
            array[1] = (byte)((num - 2) / 2);
            array[num++] = BitConverter.GetBytes(length)[0];
            array[num++] = BitConverter.GetBytes(length)[1];
            array[num++] = BitConverter.GetBytes(startIndex)[0];
            array[num++] = BitConverter.GetBytes(startIndex)[1];
            array[num++] = BitConverter.GetBytes(startIndex)[2];
            array[num++] = BitConverter.GetBytes(startIndex)[3];
            byte[] array3 = new byte[num];
            Array.Copy(array, 0, array3, 0, num);
            return array3;
        }

        private byte[] PackRequestWrite(string address, ushort typeCode, byte[] value, int length = 1)
        {
            byte[] array = new byte[1024];
            array[0] = 77;
            byte[] array2 = this.BuildRequestPathCommand(address);
            array2.CopyTo(array, 2);
            int num = 2 + array2.Length;
            array[1] = (byte)((num - 2) / 2);
            array[num++] = BitConverter.GetBytes(typeCode)[0];
            array[num++] = BitConverter.GetBytes(typeCode)[1];
            array[num++] = BitConverter.GetBytes(length)[0];
            array[num++] = BitConverter.GetBytes(length)[1];
            value.CopyTo(array, num);
            num += value.Length;
            byte[] array3 = new byte[num];
            Array.Copy(array, 0, array3, 0, num);
            return array3;
        }

        private byte[] PackCommandService(byte[] portSlot, params byte[][] cips)
        {
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.WriteByte(178);
            memoryStream.WriteByte(0);
            memoryStream.WriteByte(0);
            memoryStream.WriteByte(0);
            memoryStream.WriteByte(82);
            memoryStream.WriteByte(2);
            memoryStream.WriteByte(32);
            memoryStream.WriteByte(6);
            memoryStream.WriteByte(36);
            memoryStream.WriteByte(1);
            memoryStream.WriteByte(10);
            memoryStream.WriteByte(240);
            memoryStream.WriteByte(0);
            memoryStream.WriteByte(0);
            int num = 0;
            if (cips.Length == 1)
            {
                memoryStream.Write(cips[0], 0, cips[0].Length);
                num += cips[0].Length;
            }
            else
            {
                memoryStream.WriteByte(10);
                memoryStream.WriteByte(2);
                memoryStream.WriteByte(32);
                memoryStream.WriteByte(2);
                memoryStream.WriteByte(36);
                memoryStream.WriteByte(1);
                num += 8;
                memoryStream.Write(BitConverter.GetBytes((ushort)cips.Length), 0, 2);
                ushort num2 = (ushort)(2 + 2 * cips.Length);
                num += 2 * cips.Length;
                for (int i = 0; i < cips.Length; i++)
                {
                    memoryStream.Write(BitConverter.GetBytes(num2), 0, 2);
                    num2 = (ushort)((int)num2 + cips[i].Length);
                }
                for (int j = 0; j < cips.Length; j++)
                {
                    memoryStream.Write(cips[j], 0, cips[j].Length);
                    num += cips[j].Length;
                }
            }
            memoryStream.WriteByte((byte)((portSlot.Length + 1) / 2));
            memoryStream.WriteByte(0);
            memoryStream.Write(portSlot, 0, portSlot.Length);
            if (portSlot.Length % 2 == 1)
            {
                memoryStream.WriteByte(0);
            }
            byte[] array = memoryStream.ToArray();
            memoryStream.Dispose();
            BitConverter.GetBytes((short)num).CopyTo(array, 12);
            BitConverter.GetBytes((short)(array.Length - 4)).CopyTo(array, 2);
            return array;
        }

        private byte[] PackCleanCommandService(byte[] portSlot, params byte[][] cips)
        {
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.WriteByte(178);
            memoryStream.WriteByte(0);
            memoryStream.WriteByte(0);
            memoryStream.WriteByte(0);
            if (cips.Length == 1)
            {
                memoryStream.Write(cips[0], 0, cips[0].Length);
            }
            else
            {
                memoryStream.WriteByte(10);
                memoryStream.WriteByte(2);
                memoryStream.WriteByte(32);
                memoryStream.WriteByte(2);
                memoryStream.WriteByte(36);
                memoryStream.WriteByte(1);
                memoryStream.Write(BitConverter.GetBytes((ushort)cips.Length), 0, 2);
                ushort num = (ushort)(2 + 2 * cips.Length);
                for (int i = 0; i < cips.Length; i++)
                {
                    memoryStream.Write(BitConverter.GetBytes(num), 0, 2);
                    num = (ushort)((int)num + cips[i].Length);
                }
                for (int j = 0; j < cips.Length; j++)
                {
                    memoryStream.Write(cips[j], 0, cips[j].Length);
                }
            }
            memoryStream.WriteByte((byte)((portSlot.Length + 1) / 2));
            memoryStream.WriteByte(0);
            memoryStream.Write(portSlot, 0, portSlot.Length);
            if (portSlot.Length % 2 == 1)
            {
                memoryStream.WriteByte(0);
            }
            byte[] array = memoryStream.ToArray();
            memoryStream.Dispose();
            BitConverter.GetBytes((short)(array.Length - 4)).CopyTo(array, 2);
            return array;
        }

        private byte[] PackCommandResponse(byte[] data, bool isRead)
        {
            byte[] result;
            if (data == null)
            {
                byte[] array = new byte[6];
                array[2] = 4;
                result = array;
            }
            else
            {
                byte[] array2 = new byte[6];
                array2[0] = (byte)(isRead ? 204 : 205);
                result = ByteArrayLib.CombineTwoByteArray(array2, data);
            }
            return result;
        }

        private byte[] PackCommandSpecificData(params byte[][] service)
        {
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.WriteByte(0);
            memoryStream.WriteByte(0);
            memoryStream.WriteByte(0);
            memoryStream.WriteByte(0);
            memoryStream.WriteByte(1);
            memoryStream.WriteByte(0);
            memoryStream.WriteByte(BitConverter.GetBytes(service.Length)[0]);
            memoryStream.WriteByte(BitConverter.GetBytes(service.Length)[1]);
            for (int i = 0; i < service.Length; i++)
            {
                memoryStream.Write(service[i], 0, service[i].Length);
            }
            byte[] result = memoryStream.ToArray();
            memoryStream.Dispose();
            return result;
        }
        private byte[] PackCommandSingleService(byte[] command)
        {
            if (command == null)
            {
                command = new byte[0];
            }
            byte[] array = new byte[4 + command.Length];
            array[0] = 178;
            array[1] = 0;
            array[2] = BitConverter.GetBytes(command.Length)[0];
            array[3] = BitConverter.GetBytes(command.Length)[1];
            command.CopyTo(array, 4);
            return array;
        }

        private CalResult<byte[], ushort, bool> ExtractActualData(byte[] response, bool isRead)
        {
            List<byte> list = new List<byte>();
            int num = 38;
            bool value = false;
            ushort value2 = 0;
            ushort num2 = BitConverter.ToUInt16(response, 38);
            if (BitConverter.ToInt32(response, 40) != 138)
            {
                byte b = response[num + 4];
                byte b2 = b;
                byte b3 = b2;
                if (b3 <= 19)
                {
                    switch (b3)
                    {
                        case 0:
                            break;
                        case 1:
                        case 2:
                        case 3:
                            goto IL_3F1;
                        case 4:
                            return new CalResult<byte[], ushort, bool>
                            {
                                ErrorCode = (int)b,
                                Message = this.AllenBradley04()
                            };
                        case 5:
                            return new CalResult<byte[], ushort, bool>
                            {
                                ErrorCode = (int)b,
                                Message = this.AllenBradley05()
                            };
                        case 6:
                            value = true;
                            break;
                        default:
                            if (b3 == 10)
                            {
                                return new CalResult<byte[], ushort, bool>
                                {
                                    ErrorCode = (int)b,
                                    Message = this.AllenBradley0A()
                                };
                            }
                            if (b3 != 19)
                            {
                                goto IL_3F1;
                            }
                            return new CalResult<byte[], ushort, bool>
                            {
                                ErrorCode = (int)b,
                                Message = this.AllenBradley13()
                            };
                    }
                    if (response[num + 2] == 205 || response[num + 2] == 211)
                    {
                        return CalResult.CreateSuccessResult<byte[], ushort, bool>(list.ToArray(), value2, value);
                    }
                    if (response[num + 2] == 204 || response[num + 2] == 210)
                    {
                        for (int i = num + 8; i < num + 2 + (int)num2; i++)
                        {
                            list.Add(response[i]);
                        }
                        value2 = BitConverter.ToUInt16(response, num + 6);
                        goto IL_3CB;
                    }
                    if (response[num + 2] == 213)
                    {
                        for (int j = num + 6; j < num + 2 + (int)num2; j++)
                        {
                            list.Add(response[j]);
                        }
                        goto IL_3CB;
                    }
                    goto IL_3CB;
                }
                else
                {
                    if (b3 == 28)
                    {
                        return new CalResult<byte[], ushort, bool>
                        {
                            ErrorCode = (int)b,
                            Message = this.AllenBradley1C()
                        };
                    }
                    if (b3 == 30)
                    {
                        return new CalResult<byte[], ushort, bool>
                        {
                            ErrorCode = (int)b,
                            Message = this.AllenBradley1E()
                        };
                    }
                    if (b3 == 38)
                    {
                        return new CalResult<byte[], ushort, bool>
                        {
                            ErrorCode = (int)b,
                            Message = this.AllenBradley26()
                        };
                    }
                }
                IL_3F1:
                return new CalResult<byte[], ushort, bool>
                {
                    ErrorCode = (int)b,
                    Message = this.UnknownError()
                };
            }
            num = 44;
            int num3 = (int)BitConverter.ToUInt16(response, 44);
            int k = 0;
            while (k < num3)
            {
                int num4 = (int)BitConverter.ToUInt16(response, num + 2 + k * 2) + num;
                int num5 = (k == num3 - 1) ? response.Length : ((int)BitConverter.ToUInt16(response, num + 4 + k * 2) + num);
                ushort num6 = BitConverter.ToUInt16(response, num4 + 2);
                ushort num7 = num6;
                ushort num8 = num7;
                if (num8 <= 19)
                {
                    switch (num8)
                    {
                        case 0:
                            break;
                        case 1:
                        case 2:
                        case 3:
                            goto IL_1CF;
                        case 4:
                            return new CalResult<byte[], ushort, bool>
                            {
                                ErrorCode = (int)num6,
                                Message = this.AllenBradley04()
                            };
                        case 5:
                            return new CalResult<byte[], ushort, bool>
                            {
                                ErrorCode = (int)num6,
                                Message = this.AllenBradley05()
                            };
                        case 6:
                            if (response[num + 2] == 210 || response[num + 2] == 204)
                            {
                                return new CalResult<byte[], ushort, bool>
                                {
                                    ErrorCode = (int)num6,
                                    Message = this.AllenBradley06()
                                };
                            }
                            break;
                        default:
                            if (num8 == 10)
                            {
                                return new CalResult<byte[], ushort, bool>
                                {
                                    ErrorCode = (int)num6,
                                    Message = this.AllenBradley0A()
                                };
                            }
                            if (num8 != 19)
                            {
                                goto IL_1CF;
                            }
                            return new CalResult<byte[], ushort, bool>
                            {
                                ErrorCode = (int)num6,
                                Message = this.AllenBradley13()
                            };
                    }
                    if (isRead)
                    {
                        for (int l = num4 + 6; l < num5; l++)
                        {
                            list.Add(response[l]);
                        }
                    }
                    k++;
                    continue;
                }
                if (num8 == 28)
                {
                    return new CalResult<byte[], ushort, bool>
                    {
                        ErrorCode = (int)num6,
                        Message = this.AllenBradley1C()
                    };
                }
                if (num8 == 30)
                {
                    return new CalResult<byte[], ushort, bool>
                    {
                        ErrorCode = (int)num6,
                        Message = this.AllenBradley1E()
                    };
                }
                if (num8 == 38)
                {
                    return new CalResult<byte[], ushort, bool>
                    {
                        ErrorCode = (int)num6,
                        Message = this.AllenBradley26()
                    };
                }
                IL_1CF:
                return new CalResult<byte[], ushort, bool>
                {
                    ErrorCode = (int)num6,
                    Message = this.UnknownError()
                };
            }
            IL_3CB:
            return CalResult.CreateSuccessResult<byte[], ushort, bool>(list.ToArray(), value2, value);
        }

        public ABCIP()
        {
            this.InteractiveLock = new SimpleHybirdLock();
            this.SendTimeOut = 2000;
            this.ReceiveTimeOut = 2000;
            this.DataFormat = DataFormat.DCBA;
            this.WaitTimes = 10;
            this.SleepTime = 20;
            this.Slot = 0;
            this.CipCommand = 111;
            this.ConnectTimeOut = 2000;
        }

        private readonly SimpleHybirdLock InteractiveLock;

    }
}
