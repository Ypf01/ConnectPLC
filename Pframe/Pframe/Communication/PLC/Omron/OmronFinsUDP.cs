using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Pframe.Base;
using Pframe.Common;
using Pframe.DataConvert;

namespace Pframe.PLC.Omron
{
    public class OmronFinsUDP : NetDeviceBase
    {
        public OmronFinsUDP(string ipAddress, int port, DataFormat dataFormat = DataFormat.CDAB)
        {
            this.ReceiveBufferLength = 2048;
            this.ICF = 128;
            this.RSV = 0;
            this.GCT = 2;
            this.DNA = 0;
            this.DA1 = 19;
            this.DA2 = 0;
            this.SNA = 0;
            this.SA1 = 13;
            this.SID = 0;
            this.IpAddress = ipAddress;
            this.Port = port;
            base.DataFormat = dataFormat;
        }

        public int ReceiveBufferLength { get; set; }

        public string IpAddress
        {
            get
            {
                return this.ipAddress;
            }
            set
            {
                this.DA1 = Convert.ToByte(value.Substring(value.LastIndexOf(".") + 1));
                this.ipAddress = value;
            }
        }
        public int Port { get; set; }

        public byte DNA { get; set; }

        public byte DA1 { get; set; }

        public byte DA2 { get; set; }

        public byte SNA { get; set; }

        public byte SA1 { get; set; }

        public byte SA2 { get; set; }

        public byte SID { get; set; }

        public override CalResult<bool[]> ReadBoolArray(string address, ushort length)
        {
            CalResult<byte[]> xktResult = this.GetReadCommand(address, length, true);
            CalResult<bool[]> result;
            if (!xktResult.IsSuccess)
                result = CalResult.CreateFailedResult<bool[]>(xktResult);
            else
            {
                byte[] byte_ = null;
                if (this.SendAndReceive_child(xktResult.Content, ref byte_, new FinsMessage(), 5000))
                {
                    CalResult<byte[]> xktResult2 = this.ResponseAnalysis(byte_, true);
                    if (!xktResult2.IsSuccess)
                        result = CalResult.CreateFailedResult<bool[]>(xktResult2);
                    else
                        result = CalResult.CreateSuccessResult<bool[]>(xktResult.Content.Select(new Func<byte, bool>(c => c != 0)).ToArray<bool>());
                }
                else
                    result = CalResult.CreateFailedResult<bool[]>(new CalResult());
            }
            return result;
        }

        public override CalResult<byte[]> ReadByteArray(string address, ushort length)
        {
            CalResult<byte[]> xktResult = this.GetReadCommand(address, length, false);
            CalResult<byte[]> result;
            if (!xktResult.IsSuccess)
            {
                result = CalResult.CreateFailedResult<byte[]>(xktResult);
            }
            else
            {
                byte[] byte_ = null;
                if (this.SendAndReceive_child(xktResult.Content, ref byte_, new FinsMessage(), 5000))
                {
                    result = this.ResponseAnalysis(byte_, true);
                }
                else
                {
                    result = CalResult.CreateFailedResult<byte[]>(new CalResult());
                }
            }
            return result;
        }

        public override CalResult WriteByteArray(string address, byte[] value)
        {
            CalResult<byte[]> xktResult = this.GetWriteCommand(address, value, false);
            CalResult result;
            if (!xktResult.IsSuccess)
            {
                result = xktResult;
            }
            else
            {
                byte[] byte_ = null;
                if (this.SendAndReceive_child(xktResult.Content, ref byte_, new FinsMessage(), 5000))
                {
                    CalResult<byte[]> xktResult2 = this.ResponseAnalysis(byte_, false);
                    result = xktResult2;
                }
                else
                {
                    result = CalResult.CreateFailedResult<bool>(xktResult);
                }
            }
            return result;
        }

        public override CalResult WriteBoolArray(string address, bool[] values)
        {
            CalResult<byte[]> xktResult = this.GetWriteCommand(address, values.Select(new Func<bool, byte>(c => (byte)(c ? 1 : 0))).ToArray<byte>(), true);
            CalResult result;
            if (!xktResult.IsSuccess)
                result = xktResult;
            else
            {
                byte[] byte_ = null;
                if (this.SendAndReceive_child(xktResult.Content, ref byte_, new FinsMessage(), 5000))
                {
                    CalResult<byte[]> xktResult2 = this.ResponseAnalysis(byte_, false);
                    if (!xktResult2.IsSuccess)
                        result = xktResult2;
                    else
                        result = CalResult.CreateSuccessResult();
                }
                else
                    result = CalResult.CreateFailedResult();
            }
            return result;
        }

        private CalResult<byte[]> ResponseAnalysis(byte[] response, bool isRead)
        {
            CalResult<byte[]> result;
            if (response.Length >= 14)
            {
                int num = (int)response[12] * 256 + (int)response[13];
                if (num != 0)
                {
                    CalResult<byte[]> xktResult = new CalResult<byte[]>();
                    xktResult.IsSuccess = false;
                    xktResult.ErrorCode = num;
                    ErrorStatus errorStatus = (ErrorStatus)num;
                    xktResult.Message = errorStatus.ToString();
                    result = xktResult;
                }
                else
                {
                    result = (isRead ? CalResult.CreateSuccessResult<byte[]>(ByteArrayLib.GetByteArray(response, 14, response.Length - 14)) : CalResult.CreateSuccessResult<byte[]>(new byte[0]));
                }
            }
            else
            {
                result = new CalResult<byte[]>("数据接收异常");
            }
            return result;
        }

        private CalResult<byte[]> GetReadCommand(string address, ushort length, bool isBit)
        {
            CalResult<FinsDataType, byte[]> xktResult = this.AnalysisAddress(address, isBit);
            CalResult<byte[]> result;
            if (!xktResult.IsSuccess)
                result = CalResult.CreateFailedResult<byte[]>(xktResult);
            else
            {
                ByteArray byteArray = new ByteArray();
                byteArray.Add(new byte[]
                {
                    this.ICF,
                    this.RSV,
                    this.GCT
                });
                byteArray.Add(new byte[]
                {
                    this.DNA,
                    this.DA1,
                    this.DA2,
                    this.SNA,
                    this.SA1,
                    this.SA2,
                    this.SID
                });
                byteArray.Add(new byte[]
                {
                    1,
                    1
                });
                byteArray.Add(isBit ? xktResult.Content1.BitCode : xktResult.Content1.WordCode);
                byteArray.Add(xktResult.Content2);
                byteArray.Add(new byte[]
                {
                    (byte)(length / 256),
                    (byte)(length % 256)
                });
                result = CalResult.CreateSuccessResult<byte[]>(byteArray.array);
            }
            return result;
        }

        private CalResult<byte[]> GetWriteCommand(string address, byte[] value, bool isBit)
        {
            CalResult<FinsDataType, byte[]> xktResult = this.AnalysisAddress(address, isBit);
            CalResult<byte[]> result;
            if (!xktResult.IsSuccess)
                result = CalResult.CreateFailedResult<byte[]>(xktResult);
            else
            {
                ByteArray byteArray = new ByteArray();
                byteArray.Add(new byte[]
                {
                    this.ICF,
                    this.RSV,
                    this.GCT
                });
                byteArray.Add(new byte[]
                {
                    this.DNA,
                    this.DA1,
                    this.DA2,
                    this.SNA,
                    this.SA1,
                    this.SA2,
                    this.SID
                });
                byteArray.Add(new byte[]
                {
                    1,
                    2
                });
                byteArray.Add(isBit ? xktResult.Content1.BitCode : xktResult.Content1.WordCode);
                byteArray.Add(xktResult.Content2);
                ByteArray byteArray2 = byteArray;
                byte[] items;
                if (!isBit)
                {
                    byte[] array = new byte[2];
                    array[0] = (byte)(value.Length / 2 / 256);
                    items = array;
                    array[1] = (byte)(value.Length / 2 % 256);
                }
                else
                {
                    byte[] array2 = new byte[2];
                    array2[0] = (byte)(value.Length / 256);
                    items = array2;
                    array2[1] = (byte)(value.Length % 256);
                }
                byteArray2.Add(items);
                byteArray.Add(value);
                result = CalResult.CreateSuccessResult<byte[]>(byteArray.array);
            }
            return result;
        }

        private CalResult<FinsDataType, byte[]> AnalysisAddress(string address, bool bool_0)
        {
            CalResult<FinsDataType, byte[]> xktResult = new CalResult<FinsDataType, byte[]>();
            try
            {
                string text = address[0].ToString().ToLower();
                string a = text;
                if (!(a == "d"))
                {
                    if (!(a == "c"))
                    {
                        if (!(a == "w"))
                        {
                            if (!(a == "h"))
                            {
                                if (!(a == "a"))
                                {
                                    if (!(a == "e"))
                                    {
                                        throw new Exception("不支持的存储区");
                                    }
                                    string[] array = address.Split(new char[]
                                    {
                                        '.'
                                    }, StringSplitOptions.RemoveEmptyEntries);
                                    int num = Convert.ToInt32(array[0].Substring(1), 16);
                                    if (num < 16)
                                    {
                                        xktResult.Content1 = new FinsDataType((byte)(32 + num), (byte)(160 + num));
                                    }
                                    else
                                    {
                                        xktResult.Content1 = new FinsDataType((byte)(224 + num - 16), (byte)(96 + num - 16));
                                    }
                                }
                                else
                                    xktResult.Content1 = FinsDataType.AR;
                            }
                            else
                                xktResult.Content1 = FinsDataType.HR;
                        }
                        else
                            xktResult.Content1 = FinsDataType.WR;
                    }
                    else
                        xktResult.Content1 = FinsDataType.CIO;
                }
                else
                    xktResult.Content1 = FinsDataType.DM;
                if (address[0].ToString().ToLower() == "e")
                {
                    string[] array2 = address.Split(new char[]
                    {
                        '.'
                    }, StringSplitOptions.RemoveEmptyEntries);
                    if (bool_0)
                    {
                        ushort value = ushort.Parse(array2[1]);
                        xktResult.Content2 = new byte[3];
                        xktResult.Content2[0] = BitConverter.GetBytes(value)[1];
                        xktResult.Content2[1] = BitConverter.GetBytes(value)[0];
                        if (address.Length > 2)
                        {
                            xktResult.Content2[2] = byte.Parse(array2[2]);
                            if (xktResult.Content2[2] > 15)
                                throw new Exception("位值超过15");
                        }
                    }
                    else
                    {
                        ushort value2 = ushort.Parse(array2[1]);
                        xktResult.Content2 = new byte[3];
                        xktResult.Content2[0] = BitConverter.GetBytes(value2)[1];
                        xktResult.Content2[1] = BitConverter.GetBytes(value2)[0];
                    }
                }
                else if (bool_0)
                {
                    string[] array3 = address.Substring(1).Split(new char[]
                    {
                        '.'
                    }, StringSplitOptions.RemoveEmptyEntries);
                    ushort value3 = ushort.Parse(array3[0]);
                    xktResult.Content2 = new byte[3];
                    xktResult.Content2[0] = BitConverter.GetBytes(value3)[1];
                    xktResult.Content2[1] = BitConverter.GetBytes(value3)[0];
                    if (array3.Length > 1)
                    {
                        xktResult.Content2[2] = byte.Parse(array3[1]);
                        if (xktResult.Content2[2] > 15)
                            throw new Exception("位值超过15");
                    }
                }
                else
                {
                    ushort value4 = ushort.Parse(address.Substring(1));
                    xktResult.Content2 = new byte[3];
                    xktResult.Content2[0] = BitConverter.GetBytes(value4)[1];
                    xktResult.Content2[1] = BitConverter.GetBytes(value4)[0];
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

        private bool SendAndReceive_child(byte[] SendByte, ref byte[] response, IMessage imessage_0 = null, int ReceiveTimeout = 5000)
        {
            this.InteractiveLock.Enter();
            bool result;
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(this.IpAddress), this.Port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.SendTo(SendByte, SendByte.Length, SocketFlags.None, remoteEP);
                socket.ReceiveTimeout = ReceiveTimeout;
                IPEndPoint ipendPoint = new IPEndPoint(IPAddress.Any, 0);
                EndPoint endPoint = ipendPoint;
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, ReceiveTimeout);
                byte[] array = new byte[this.ReceiveBufferLength];
                int count = socket.ReceiveFrom(array, ref endPoint);
                response = array.Take(count).ToArray<byte>();
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

        private string ipAddress;

        private byte ICF { get; set; }

        private byte RSV { get; set; }

        private byte GCT { get; set; }
    }
}
