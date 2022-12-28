using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Pframe.Common;
using Pframe.DataConvert;
using Pframe.Tools;

namespace Pframe.Base
{
    /// <summary>
    /// 网络设备基类
    /// </summary>
    public class NetDeviceBase
    {
        /// <summary>
        /// 发送超时时间
        /// </summary>
        public int SendTimeOut { get; set; }
        /// <summary>
        /// 接收超时时间
        /// </summary>
        public int ReceiveTimeOut { get; set; }
        /// <summary>
        /// 字节大小端顺序
        /// </summary>
        public DataFormat DataFormat { get; set; }
        /// <summary>
        /// 接受返回报文等待次数，默认为10次
        /// </summary>
        public int WaitTimes { get; set; }
        /// <summary>
        /// 接受返回报文等待时间，每次为10ms
        /// </summary>
        public int SleepTime { get; set; }
        /// <summary>
        /// 连接超时时间
        /// </summary>
        public int ConnectTimeOut { get; set; }
        /// <summary>
        /// 建立Socket连接
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口号</param>
        /// <returns></returns>
        public bool Connect(string ip, int port)
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, this.ReceiveTimeOut);
            this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, this.SendTimeOut);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
            IAsyncResult asyncResult = this.socket.BeginConnect(remoteEP, null, null);
            bool result;
            if (!(result = asyncResult.AsyncWaitHandle.WaitOne(this.ConnectTimeOut, true)))
                this.socket.Close();
            return result;
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        public void DisConnect()
        {
            Socket socket = this.socket;
            if (socket != null)
                socket.Close();
        }
        /// <summary>
        /// 发送报文并接收返回值
        /// </summary>
        /// <param name="SendByte">字节数组</param>
        /// <param name="response">返回报文</param>
        /// <param name="message">返回长度</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>是否成功</returns>
        public bool SendAndReceive(byte[] SendByte, ref byte[] response, IMessage message = null, int timeout = 5000)
        {
            bool result;
            this.InteractiveLock.Enter();
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                this.socket.ReceiveTimeout = timeout;
                this.socket.Send(SendByte, SendByte.Length, SocketFlags.None);
                if (message == null)
                {
                    int num = 0;
                    byte[] buffer = new byte[1024];
                    while (true)
                    {
                        Thread.Sleep(this.SleepTime);
                        if (this.socket.Available > 0)
                        {
                            int count = this.socket.Receive(buffer, SocketFlags.None);
                            memoryStream.Write(buffer, 0, count);
                        }
                        else
                        {
                            num++;
                            if (num >= this.WaitTimes)
                                goto IL_B6;
                            if (memoryStream.Length > 0L)
                                break;
                        }
                    }
                    goto IL_112;
                    IL_B6:
                    return false;
                }
                if (message.HeadDataLength > 0)
                {
                    byte[] array = this.CheckLength(message.HeadDataLength);
                    if (array == null)
                        return false;
                    message.HeadData = array;
                    memoryStream.Write(array, 0, array.Length);
                }
                int contentLength = message.GetContentLength();
                byte[] array2 = this.CheckLength(contentLength);
                memoryStream.Write(array2, 0, array2.Length);
                IL_112:
                response = memoryStream.ToArray();
                memoryStream.Dispose();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            finally
            {
                this.InteractiveLock.Leave();
            }
            return result;
        }
        private byte[] CheckLength(int legth)
        {
            byte[] array = new byte[legth];
            int i = 0;
            while (i < legth)
            {
                int size = Math.Min(legth - i, 8192);
                int num = this.socket.Receive(array, i, size, SocketFlags.None);
                i += num;
                if (num == 0)
                    throw new Exception("Read resulted in 0 bytes returned.");
            }
            return array;
        }
        /// <summary>
        ///  读取字节数组接口
        /// </summary>
        /// <param name="address"> 变量地址</param>
        /// <param name="length">长度</param>
        /// <returns>带字节数组的操作结果</returns>
        public virtual CalResult<byte[]> ReadByteArray(string address, ushort length)
        {
            return CalResult.CreateFailedResult<byte[]>(new CalResult());
        }
        /// <summary>
        /// 读取布尔数组接口
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="length">长度</param>
        /// <returns>带布尔数组的操作结果</returns>
        public virtual CalResult<bool[]> ReadBoolArray(string address, ushort length)
        {
            return CalResult.CreateFailedResult<bool[]>(new CalResult());
        }
        /// <summary>
        /// 写入字节数组接口
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="value">字节数组</param>
        /// <returns>操作结果</returns>
        public virtual CalResult WriteByteArray(string address, byte[] value)
        {
            return CalResult.CreateFailedResult();
        }
        /// <summary>
        /// 写入布尔数组接口
        /// </summary>
        /// <param name="address"> 变量地址</param>
        /// <param name="value">布尔数组</param>
        /// <returns> 操作结果</returns>
        public virtual CalResult WriteBoolArray(string address, bool[] value)
        {
            return CalResult.CreateFailedResult();
        }
        /// <summary>
        /// 读取单个布尔
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="value">返回值</param>
        /// <returns>是否成功</returns>
        public bool ReadBool(string address, ref bool value)
        {
            bool[] array = this.ReadBool(address, 1);
            bool result;
            if (array != null)
            {
                value = array[0];
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 读取布尔数组
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="length">长度</param>
        /// <returns>是否成功</returns>
        public bool[] ReadBool(string address, ushort length)
        {
            CalResult<bool[]> xktResult = this.ReadBoolArray(address, length);
            return xktResult.IsSuccess ? xktResult.Content : null;
        }
        /// <summary>
        /// 读取单个字节
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="value"> 返回值</param>
        /// <returns>是否成功</returns>
        public bool ReadByte(string address, ref byte value)
        {
            byte[] array = this.ReadBytes(address, 1);
            bool result;
            if (array != null)
            {
                value = array[0];
                result = true;
            }
            else
                result = false;
            return result;
        }
        /// <summary>
        /// 读取字节数组
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="length">长度</param>
        /// <returns>字节数组</returns>
        public byte[] ReadBytes(string address, ushort length)
        {
            CalResult<byte[]> xktResult = this.ReadByteArray(address, length);
            byte[] result;
            if (xktResult.IsSuccess)
            {
                result = xktResult.Content;
            }
            else
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// 读取单个Short
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="value">返回值</param>
        /// <returns></returns>
        public bool ReadShort(string address, ref short value)
        {
            short[] array = this.ReadShort(address, 1);
            bool result;
            if (array != null)
            {
                value = array[0];
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 读取Short数组
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="length">长度</param>
        /// <returns>Short数组</returns>
        public short[] ReadShort(string address, ushort length)
        {
            CalResult<byte[]> xktResult = this.ReadByteArray(address, length);
            short[] result;
            if (xktResult.IsSuccess)
            {
                result = ShortLib.GetShortArrayFromByteArray(xktResult.Content, this.DataFormat);
            }
            else
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// 读取单个UShort
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="value">返回值</param>
        /// <returns>是否成功</returns>
        public bool ReadUShort(string address, ref ushort value)
        {
            ushort[] array = this.ReadUShort(address, 1);
            bool result;
            if (array != null)
            {
                value = array[0];
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 读取UShort数组
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="length">长度</param>
        /// <returns>UShort数组</returns>
        public ushort[] ReadUShort(string address, ushort length)
        {
            CalResult<byte[]> xktResult = this.ReadByteArray(address, length);
            ushort[] result;
            if (xktResult.IsSuccess)
            {
                result = UShortLib.GetUShortArrayFromByteArray(xktResult.Content, this.DataFormat);
            }
            else
                result = null;
            return result;
        }
        /// <summary>
        /// 读取单个Int
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="value">返回值</param>
        /// <returns>是否成功</returns>
        public bool ReadInt(string address, ref int value)
        {
            int[] array = this.ReadInt(address, 1);
            bool result;
            if (array != null)
            {
                value = array[0];
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 读取Int数组
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="length">长度</param>
        /// <returns>Int数组</returns>
        public int[] ReadInt(string address, ushort length)
        {
            CalResult<byte[]> xktResult = this.ReadByteArray(address, (ushort)(length * 2));
            int[] result;
            if (xktResult.IsSuccess)
            {
                result = IntLib.GetIntArrayFromByteArray(xktResult.Content, this.DataFormat);
            }
            else
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// 读取单个UInt
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="value">返回值</param>
        /// <returns>是否成功</returns>
        public bool ReadUInt(string address, ref uint value)
        {
            uint[] array = this.ReadUInt(address, 1);
            bool result;
            if (array != null)
            {
                value = array[0];
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 读取UInt数组
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="length">长度</param>
        /// <returns>UInt数组</returns>
        public uint[] ReadUInt(string address, ushort length)
        {
            CalResult<byte[]> xktResult = this.ReadByteArray(address, (ushort)(length * 2));
            uint[] result;
            if (xktResult.IsSuccess)
            {
                result = UIntLib.GetUIntArrayFromByteArray(xktResult.Content, this.DataFormat);
            }
            else
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// 读取单个Float
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="value">返回值</param>
        /// <returns>是否成功</returns>
        public bool ReadFloat(string address, ref float value)
        {
            float[] array = this.ReadFloat(address, 1);
            bool result;
            if (array != null)
            {
                value = array[0];
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 读取Float数组
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="length"> 长度</param>
        /// <returns></returns>
        public float[] ReadFloat(string address, ushort length)
        {
            CalResult<byte[]> xktResult = this.ReadByteArray(address, (ushort)(length * 2));
            float[] result;
            if (xktResult.IsSuccess)
            {
                result = FloatLib.GetFloatArrayFromByteArray(xktResult.Content, this.DataFormat);
            }
            else
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// 读取单个Long
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="value">返回值</param>
        /// <returns>是否成功</returns>
        public bool ReadLong(string address, ref long value)
        {
            long[] array = this.ReadLong(address, 1);
            bool result;
            if (array != null)
            {
                value = array[0];
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 读取Long数组
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="length">长度</param>
        /// <returns>Long数组</returns>
        public long[] ReadLong(string address, ushort length)
        {
            CalResult<byte[]> xktResult = this.ReadByteArray(address, (ushort)(length * 4));
            long[] result;
            if (xktResult.IsSuccess)
            {
                result = LongLib.GetLongArrayFromByteArray(xktResult.Content, this.DataFormat);
            }
            else
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// 读取单个ULong
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="value">返回值</param>
        /// <returns>是否成功</returns>
        public bool ReadULong(string address, ref ulong value)
        {
            ulong[] array = this.ReadULong(address, 1);
            bool result;
            if (array != null)
            {
                value = array[0];
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 读取ULong数组
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="length">长度</param>
        /// <returns>ULong数组</returns>
        public ulong[] ReadULong(string address, ushort length)
        {
            CalResult<byte[]> xktResult = this.ReadByteArray(address, (ushort)(length * 4));
            ulong[] result;
            if (xktResult.IsSuccess)
            {
                result = ULongLib.GetULongArrayFromByteArray(xktResult.Content, this.DataFormat);
            }
            else
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// 读取Double数组
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="value">长度</param>
        /// <returns>是否成功</returns>
        public bool ReadDouble(string address, ref double value)
        {
            double[] array = this.ReadDouble(address, 1);
            bool result;
            if (array != null)
            {
                value = array[0];
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 读取Double数组
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="length">长度</param>
        /// <returns>Double数组</returns>
        public double[] ReadDouble(string address, ushort length)
        {
            CalResult<byte[]> xktResult = this.ReadByteArray(address, (ushort)(length * 4));
            double[] result;
            if (xktResult.IsSuccess)
            {
                result = DoubleLib.GetDoubleArrayFromByteArray(xktResult.Content, this.DataFormat);
            }
            else
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// 读取String
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="length">长度</param>
        /// <param name="value">返回值</param>
        /// <returns>是否成功</returns>
        public bool ReadString(string address, int length, ref string value)
        {
            CalResult<byte[]> xktResult = this.ReadByteArray(address, (ushort)length);
            if (xktResult.IsSuccess)
            {
                value = StringLib.GetStringFromByteArray(xktResult.Content, 0, xktResult.Content.Length * 2, Encoding.ASCII);
            }
            return xktResult.IsSuccess;
        }
        /// <summary>
        /// 通用写入方法
        /// </summary>
        /// <param name="address">变量地址</param>
        /// <param name="value">变量值</param>
        /// <param name="dataType">数据类型</param>
        /// <returns></returns>
        public CalResult Write(string address, object value, DataType dataType)
        {
            CalResult xktResult = new CalResult();
            try
            {
                switch (dataType)
                {
                    case DataType.Bool:
                        if (address.Contains('.'))
                        {
                            xktResult.IsSuccess = this.Write(address, CommonMethods.IsBoolean(value.ToString()), true);
                        }
                        else
                        {
                            xktResult.IsSuccess = this.Write(address, CommonMethods.IsBoolean(value.ToString()), false);
                        }
                        break;
                    case DataType.Byte:
                        xktResult.IsSuccess = this.Write(address, Convert.ToByte(value));
                        break;
                    case DataType.Short:
                        xktResult.IsSuccess = this.Write(address, Convert.ToInt16(value));
                        break;
                    case DataType.UShort:
                        xktResult.IsSuccess = this.Write(address, Convert.ToUInt16(value));
                        break;
                    case DataType.Int:
                        xktResult.IsSuccess = this.Write(address, Convert.ToInt32(value));
                        break;
                    case DataType.UInt:
                        xktResult.IsSuccess = this.Write(address, Convert.ToUInt32(value));
                        break;
                    case DataType.Float:
                        xktResult.IsSuccess = this.Write(address, Convert.ToSingle(value));
                        break;
                    case DataType.Double:
                        xktResult.IsSuccess = this.Write(address, Convert.ToDouble(value));
                        break;
                    case DataType.Long:
                        xktResult.IsSuccess = this.Write(address, Convert.ToInt64(value));
                        break;
                    case DataType.ULong:
                        xktResult.IsSuccess = this.Write(address, Convert.ToUInt64(value));
                        break;
                    case DataType.String:
                        xktResult.IsSuccess = this.Write(address.Substring(0, address.IndexOf('.')), value.ToString());
                        break;
                    default:
                        xktResult = CalResult.CreateFailedResult();
                        break;
                }
            }
            catch (Exception ex)
            {
                xktResult.IsSuccess = false;
                xktResult.Message = ex.Message;
            }
            return xktResult;
        }

        public bool Write(string address, bool value)
        {
            return this.Write(address, new bool[]
            {
                value
            });
        }

        public bool Write(string address, bool value, bool IsRegBool = false)
        {
            bool result;
            if (IsRegBool)
            {
                if (address.Contains('.'))
                {
                    string[] array = address.Split(new char[]
                    {
                        '.'
                    });
                    if (array.Length == 2)
                    {
                        ushort value2 = 0;
                        if (this.ReadUShort(array[0], ref value2))
                        {
                            int bit = Convert.ToInt32(array[1], 16);
                            ushort value3 = UShortLib.SetbitValueFromUShort(value2, bit, value, DataFormat.ABCD);
                            return this.Write(array[0], value3);
                        }
                    }
                }
                result = false;
            }
            else
            {
                result = this.Write(address, false);
            }
            return result;
        }

        public bool Write(string address, bool[] value)
        {
            return this.WriteBoolArray(address, value).IsSuccess;
        }

        public bool Write(string address, byte value)
        {
            return this.Write(address, new byte[]
            {
                value
            });
        }

        public bool Write(string address, byte[] value)
        {
            return this.WriteByteArray(address, value).IsSuccess;
        }

        public bool Write(string address, short[] values)
        {
            return this.WriteByteArray(address, ByteArrayLib.GetByteArrayFromShortArray(values, this.DataFormat)).IsSuccess;
        }

        public bool Write(string address, short value)
        {
            return this.Write(address, new short[]
            {
                value
            });
        }

        public bool Write(string address, ushort[] values)
        {
            return this.WriteByteArray(address, ByteArrayLib.GetByteArrayFromUShortArray(values, this.DataFormat)).IsSuccess;
        }

        public bool Write(string address, ushort value)
        {
            return this.Write(address, new ushort[]
            {
                value
            });
        }

        public bool Write(string address, int[] values)
        {
            return this.WriteByteArray(address, ByteArrayLib.GetByteArrayFromIntArray(values, this.DataFormat)).IsSuccess;
        }

        public bool Write(string address, int value)
        {
            return this.Write(address, new int[]
            {
                value
            });
        }

        public bool Write(string address, uint[] values)
        {
            return this.WriteByteArray(address, ByteArrayLib.GetByteArrayFromUIntArray(values, this.DataFormat)).IsSuccess;
        }

        public bool Write(string address, uint value)
        {
            return this.Write(address, new uint[]
            {
                value
            });
        }

        public bool Write(string address, float[] values)
        {
            return this.WriteByteArray(address, ByteArrayLib.GetByteArrayFromFloatArray(values, this.DataFormat)).IsSuccess;
        }

        public bool Write(string address, float value)
        {
            return this.Write(address, new float[]
            {
                value
            });
        }

        public bool Write(string address, long[] values)
        {
            return this.WriteByteArray(address, ByteArrayLib.GetByteArrayFromLongArray(values, this.DataFormat)).IsSuccess;
        }

        public bool Write(string address, long value)
        {
            return this.Write(address, new long[]
            {
                value
            });
        }

        public bool Write(string address, ulong[] values)
        {
            return this.WriteByteArray(address, ByteArrayLib.GetByteArrayFromULongArray(values, this.DataFormat)).IsSuccess;
        }

        public bool Write(string address, ulong value)
        {
            return this.Write(address, new ulong[]
            {
                value
            });
        }

        public bool Write(string address, double[] values)
        {
            return this.WriteByteArray(address, ByteArrayLib.GetByteArrayFromDoubleArray(values, this.DataFormat)).IsSuccess;
        }

        public bool Write(string address, double value)
        {
            return this.Write(address, new double[]
            {
                value
            });
        }

        public bool Write(string address, string value, Encoding encoding)
        {
            return this.WriteByteArray(address, ByteArrayLib.GetByteArrayFromString(value, encoding)).IsSuccess;
        }

        public bool Write(string address, string value)
        {
            return this.Write(address, value, Encoding.ASCII);
        }

        public NetDeviceBase()
        {
            this.InteractiveLock = new SimpleHybirdLock();
            this.SendTimeOut = 2000;
            this.ReceiveTimeOut = 2000;
            this.DataFormat = DataFormat.ABCD;
            this.WaitTimes = 10;
            this.SleepTime = 10;
            this.ConnectTimeOut = 2000;
        }

        private Socket socket;
        /// <summary>
        /// 一次正常的交互的互斥锁
        /// </summary>
        public SimpleHybirdLock InteractiveLock;
    }
}
