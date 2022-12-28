using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Pframe.Base;
using Pframe.Common;
using Pframe.DataConvert;
using Pframe.PLC.Common;
using Pframe.Tools;

namespace Pframe.PLC.Siemens
{
    public class SiemensS7 : NetABS7DeviceBase
    {
        public short MaxPDUSize { get; set; }

        public int ConnectTimeOut { get; set; }

        public int ReceiveTimeOut { get; set; }

        public bool Connect(string Ip, CPU_Type Cpu, int Rack, int Slot, int Port = 102)
        {
            this.tcpclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.tcpclient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 2000);
            this.tcpclient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 2000);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(Ip), Port);
            IAsyncResult asyncResult = this.tcpclient.BeginConnect(remoteEP, null, null);
            asyncResult.AsyncWaitHandle.WaitOne(this.ConnectTimeOut, true);
            bool result;
            if (!asyncResult.IsCompleted)
            {
                this.tcpclient.Close();
                result = false;
            }
            else
            {
                byte[] array = this.SendAndReceive(this.GetCOTPConnectionRequest(Cpu, Rack, Slot));
                if (array == null)
                    result = false;
                else
                {
                    byte[] array2 = this.SendAndReceive(this.GetS7ConnectionSetup(Cpu));
                    if (array2 == null)
                        result = false;
                    else
                    {
                        if (array2.Length == 27)
                            this.MaxPDUSize = (short)((int)array2[25] * 256 + (int)array2[26]);
                        result = true;
                    }
                }
            }
            return result;
        }

        public void DisConnect()
        {
            if (this.tcpclient != null && this.tcpclient.Connected)
            {
                this.tcpclient.Close();
            }
        }

        private byte[] GetCOTPConnectionRequest(CPU_Type Cpu, int Rack, int Slot)
        {
            byte[] array = new byte[]
            {
                3,
                0,
                0,
                22,
                17,
                224,
                0,
                0,
                0,
                1,
                0,
                192,
                1,
                10,
                193,
                2,
                1,
                2,
                194,
                2,
                1,
                0
            };
            if (Cpu <= CPU_Type.S7200SMART)
            {
                if (Cpu != CPU_Type.S7200)
                {
                    if (Cpu == CPU_Type.S7200SMART)
                    {
                        array[9] = 1;
                        array[10] = 0;
                        array[11] = 193;
                        array[12] = 2;
                        array[13] = 16;
                        array[14] = 0;
                        array[15] = 194;
                        array[16] = 2;
                        array[17] = 3;
                        array[18] = 0;
                        array[19] = 0;
                        array[20] = 1;
                        array[21] = 10;
                    }
                }
            }
            else if (Cpu != CPU_Type.S7300)
            {
                switch (Cpu)
                {
                    case CPU_Type.S7400:
                        array[17] = 0;
                        array[21] = (byte)(Rack * 32 + Slot);
                        break;
                    case CPU_Type.S71200:
                        array[21] = 0;
                        break;
                    case CPU_Type.S71500:
                        array[21] = 0;
                        break;
                }
            }
            else
            {
                array[21] = 2;
            }
            return array;
        }

        private byte[] GetS7ConnectionSetup(CPU_Type Cpu)
        {
            byte[] array = new byte[]
            {
                3,
                0,
                0,
                25,
                2,
                240,
                128,
                50,
                1,
                0,
                0,
                byte.MaxValue,
                byte.MaxValue,
                0,
                8,
                0,
                0,
                240,
                0,
                0,
                3,
                0,
                3,
                7,
                128
            };
            if (Cpu == CPU_Type.S7200SMART)
            {
                array[11] = 204;
                array[12] = 193;
                array[13] = 0;
                array[14] = 8;
                array[15] = 0;
                array[16] = 0;
                array[17] = 240;
                array[18] = 0;
                array[19] = 0;
                array[20] = 1;
                array[21] = 0;
                array[22] = 1;
                array[23] = 3;
                array[24] = 192;
            }
            return array;
        }

        public byte[] ReadBytes(StoreType StoreType, int DB, int StartByteAdr, int count)
        {
            this.InteractiveLock_Child.Enter();
            byte[] result;
            try
            {
                List<byte> list = new List<byte>();
                int num = StartByteAdr;
                while (count > 0)
                {
                    int num2 = Math.Min(count, (int)(this.MaxPDUSize - 18));
                    byte[] array = this.ReadBytesWithSingleRequest(StoreType, DB, num, num2);
                    if (array == null)
                    {
                        return list.ToArray();
                    }
                    list.AddRange(array);
                    count -= num2;
                    num += num2;
                }
                result = ((list.Count > 0) ? list.ToArray() : null);
            }
            catch (Exception)
            {
                result = null;
            }
            finally
            {
                this.InteractiveLock_Child.Leave();
            }
            return result;
        }

        private byte[] ReadBytesWithSingleRequest(StoreType StoreType, int DB, int StartByteAdr, int count)
        {
            DB = ((StoreType == StoreType.DataBlock) ? DB : 0);
            byte[] array = new byte[count];
            byte[] result;
            try
            {
                ByteArray byteArray = new ByteArray();
                ByteArray byteArray2 = byteArray;
                byte[] array2 = new byte[3];
                array2[0] = 3;
                byteArray2.Add(array2);
                byteArray.Add(31);
                byteArray.Add(new byte[]
                {
                    2,
                    240,
                    128,
                    50,
                    1,
                    0,
                    0,
                    0,
                    0,
                    0,
                    14,
                    0,
                    0,
                    4,
                    1,
                    18,
                    10,
                    16
                });
                if (StoreType - StoreType.Counter > 1)
                {
                    byteArray.Add(2);
                }
                else
                {
                    byteArray.Add((byte)StoreType);
                }
                byteArray.Add(ByteArrayLib.GetByteArrayFromUShort((ushort)count, DataFormat.ABCD));
                byteArray.Add(ByteArrayLib.GetByteArrayFromUShort((ushort)DB, DataFormat.ABCD));
                byteArray.Add((byte)StoreType);
                byteArray.Add((byte)(StartByteAdr * 8 / 256 / 256 % 256));
                byteArray.Add((byte)(StartByteAdr * 8 / 256 % 256));
                byteArray.Add((byte)(StartByteAdr * 8 % 256));
                byte[] array3 = this.SendAndReceiveWithoutLock(byteArray.array);
                if (array3 == null)
                {
                    throw new Exception();
                }
                if (array3[21] != 255)
                {
                    throw new Exception();
                }
                for (int i = 0; i < count; i++)
                {
                    array[i] = array3[i + 25];
                }
                result = array;
            }
            catch
            {
                result = null;
            }
            return result;
        }

        public object Read(StoreType StoreType, int DB, int StartByteAdr, VarType VarType, int VarCount)
        {
            switch (VarType)
            {
                case VarType.Byte:
                    {
                        int num = VarCount;
                        if (num < 1)
                        {
                            num = 1;
                        }
                        byte[] array = this.ReadBytes(StoreType, DB, StartByteAdr, num);
                        if (array == null)
                        {
                            return null;
                        }
                        if (VarCount == 1)
                        {
                            return array[0];
                        }
                        return array;
                    }
                case VarType.Word:
                    {
                        int count = VarCount * 2;
                        byte[] array2 = this.ReadBytes(StoreType, DB, StartByteAdr, count);
                        if (array2 == null)
                        {
                            return null;
                        }
                        if (VarCount == 1)
                        {
                            return UShortLib.GetUShortFromByteArray(array2, 0, DataFormat.ABCD);
                        }
                        return UShortLib.GetUShortArrayFromByteArray(array2, DataFormat.ABCD);
                    }
                case VarType.DWord:
                    {
                        int count2 = VarCount * 4;
                        byte[] array3 = this.ReadBytes(StoreType, DB, StartByteAdr, count2);
                        if (array3 == null)
                        {
                            return null;
                        }
                        if (VarCount == 1)
                        {
                            return UIntLib.GetUIntFromByteArray(array3, 0, DataFormat.ABCD);
                        }
                        return UIntLib.GetUIntArrayFromByteArray(array3, DataFormat.ABCD);
                    }
                case VarType.Int:
                    {
                        int count3 = VarCount * 2;
                        byte[] array4 = this.ReadBytes(StoreType, DB, StartByteAdr, count3);
                        if (array4 == null)
                        {
                            return null;
                        }
                        if (VarCount == 1)
                        {
                            return ShortLib.GetShortFromByteArray(array4, 0, DataFormat.ABCD);
                        }
                        return ShortLib.GetShortArrayFromByteArray(array4, DataFormat.ABCD);
                    }
                case VarType.DInt:
                    {
                        int count4 = VarCount * 4;
                        byte[] array5 = this.ReadBytes(StoreType, DB, StartByteAdr, count4);
                        if (array5 == null)
                        {
                            return null;
                        }
                        if (VarCount == 1)
                        {
                            return IntLib.GetIntFromByteArray(array5, 0, DataFormat.ABCD);
                        }
                        return IntLib.GetIntArrayFromByteArray(array5, DataFormat.ABCD);
                    }
                case VarType.Real:
                    {
                        int count5 = VarCount * 4;
                        byte[] array6 = this.ReadBytes(StoreType, DB, StartByteAdr, count5);
                        if (array6 == null)
                        {
                            return null;
                        }
                        if (VarCount == 1)
                        {
                            return FloatLib.GetFloatFromByteArray(array6, 0, DataFormat.ABCD);
                        }
                        return FloatLib.GetFloatArrayFromByteArray(array6, DataFormat.ABCD);
                    }
                case VarType.String:
                    {
                        byte[] array7 = this.ReadBytes(StoreType, DB, StartByteAdr, VarCount);
                        if (array7 == null)
                        {
                            return null;
                        }
                        return Encoding.ASCII.GetString(array7);
                    }
                case VarType.Timer:
                    {
                        int count6 = VarCount * 2;
                        byte[] array8 = this.ReadBytes(StoreType, DB, StartByteAdr, count6);
                        if (array8 == null)
                        {
                            return null;
                        }
                        if (VarCount == 1)
                        {
                            return Timers.FromByteArray(array8);
                        }
                        return Timers.ToArray(array8);
                    }
                case VarType.Counter:
                    {
                        int count7 = VarCount * 2;
                        byte[] array9 = this.ReadBytes(StoreType, DB, StartByteAdr, count7);
                        if (array9 == null)
                        {
                            return null;
                        }
                        if (VarCount == 1)
                        {
                            return UShortLib.GetUShortFromByteArray(array9, 0, DataFormat.ABCD);
                        }
                        return UShortLib.GetUShortArrayFromByteArray(array9, DataFormat.ABCD);
                    }
            }
            return null;
        }

        public object Read(string variable, VarType VarType)
        {
            string text = variable.ToUpper();
            text = text.Replace(" ", "");
            object result;
            try
            {
                string text2 = text.Substring(0, 2);
                string text3 = text2;
                string text4 = text3;
                uint num = PrivateImplementationDetails.ComputeStringHash(text4);
                if (num <= 1458105184U)
                {
                    if (num <= 970965853U)
                    {
                        if (num != 919500163U)
                        {
                            if (num == 970965853U)
                            {
                                if (text4 == "MW")
                                {
                                    if (VarType != VarType.Int)
                                    {
                                        return (ushort)this.Read(StoreType.Marker, 0, int.Parse(text.Substring(2)), VarType.Word, 1);
                                    }
                                    return (short)this.Read(StoreType.Marker, 0, int.Parse(text.Substring(2)), VarType.Int, 1);
                                }
                            }
                        }
                        else if (text4 == "DB")
                        {
                            string[] array = text.Split(new char[]
                            {
                                '.'
                            });
                            if (array.Length < 2)
                            {
                                throw new Exception();
                            }
                            int db = int.Parse(array[0].Substring(2));
                            string text5 = array[1].Substring(0, 3);
                            int count = 0;
                            int num2;
                            if (array[1].Contains('|'))
                            {
                                string[] array2 = array[1].Split(new char[]
                                {
                                    '|'
                                });
                                if (array2.Length != 2)
                                {
                                    throw new Exception();
                                }
                                num2 = int.Parse(array2[0].Substring(3));
                                count = int.Parse(array2[1]);
                            }
                            else
                            {
                                num2 = int.Parse(array[1].Substring(3));
                            }
                            text2 = text5;
                            if (text2 != null)
                            {
                                if (text2 == "DBB")
                                {
                                    byte b = (byte)this.Read(StoreType.DataBlock, db, num2, VarType.Byte, 1);
                                    result = b;
                                    return result;
                                }
                                if (text2 == "DBW")
                                {
                                    if (VarType != VarType.Int)
                                    {
                                        return (ushort)this.Read(StoreType.DataBlock, db, num2, VarType.Word, 1);
                                    }
                                    return (short)this.Read(StoreType.DataBlock, db, num2, VarType.Int, 1);
                                }
                                else if (text2 == "DBD")
                                {
                                    if (VarType == VarType.DInt)
                                    {
                                        return Convert.ToInt32(this.Read(StoreType.DataBlock, db, num2, VarType.DInt, 1));
                                    }
                                    if (VarType != VarType.Real)
                                    {
                                        return Convert.ToUInt32(this.Read(StoreType.DataBlock, db, num2, VarType.DWord, 1));
                                    }
                                    return Convert.ToSingle(this.Read(StoreType.DataBlock, db, num2, VarType.Real, 1));
                                }
                                else if (text2 == "DBX")
                                {
                                    int startByteAdr = num2;
                                    int num3 = int.Parse(array[2]);
                                    if (num3 > 7)
                                    {
                                        throw new Exception();
                                    }
                                    return BitLib.GetBitFromByte(Convert.ToByte(this.Read(StoreType.DataBlock, db, startByteAdr, VarType.Byte, 1)), num3);
                                }
                                else if (text2 == "DBS")
                                {
                                    if (!text.Contains('|'))
                                    {
                                        int startByteAdr2 = num2;
                                        int num4 = int.Parse(array[2]);
                                        byte[] array3 = this.ReadBytes(StoreType.DataBlock, db, startByteAdr2, num4 + 2);
                                        if (array3 == null)
                                        {
                                            throw new Exception();
                                        }
                                        if (array3.Length != num4 + 2)
                                        {
                                            throw new Exception();
                                        }
                                        int num5 = (int)array3[1];
                                        if (num5 > 0)
                                        {
                                            return Encoding.GetEncoding("GBK").GetString(ByteArrayLib.GetByteArray(array3, 2, num5));
                                        }
                                        return "empty";
                                    }
                                    else
                                    {
                                        int startByteAdr3 = num2;
                                        byte[] array4 = this.ReadBytes(StoreType.DataBlock, db, startByteAdr3, count);
                                        if (array4 == null)
                                        {
                                            throw new Exception();
                                        }
                                        return StringLib.GetStringFromByteArray(array4, 0, array4.Length, Encoding.GetEncoding("GBK"));
                                    }
                                }
                            }
                            throw new Exception();
                        }
                    }
                    else if (num != 1155519662U)
                    {
                        if (num != 1189074900U)
                        {
                            if (num == 1458105184U)
                            {
                                if (text4 == "ID")
                                {
                                    if (VarType == VarType.DInt)
                                    {
                                        return Convert.ToInt32(this.Read(StoreType.Input, 0, int.Parse(text.Substring(2)), VarType.DInt, 1));
                                    }
                                    if (VarType != VarType.Real)
                                    {
                                        return Convert.ToUInt32(this.Read(StoreType.Input, 0, int.Parse(text.Substring(2)), VarType.DWord, 1));
                                    }
                                    return Convert.ToSingle(this.Read(StoreType.Input, 0, int.Parse(text.Substring(2)), VarType.Real, 1));
                                }
                            }
                        }
                        else if (text4 == "MD")
                        {
                            if (VarType == VarType.DInt)
                            {
                                return Convert.ToInt32(this.Read(StoreType.Marker, 0, int.Parse(text.Substring(2)), VarType.DInt, 1));
                            }
                            if (VarType != VarType.Real)
                            {
                                return Convert.ToUInt32(this.Read(StoreType.Marker, 0, int.Parse(text.Substring(2)), VarType.DWord, 1));
                            }
                            return Convert.ToSingle(this.Read(StoreType.Marker, 0, int.Parse(text.Substring(2)), VarType.Real, 1));
                        }
                    }
                    else if (text4 == "MB")
                    {
                        byte b2 = (byte)this.Read(StoreType.Marker, 0, int.Parse(text.Substring(2)), VarType.Byte, 1);
                        result = b2;
                        return result;
                    }
                }
                else if (num <= 1776879945U)
                {
                    if (num != 1558770898U)
                    {
                        if (num == 1776879945U)
                        {
                            if (text4 == "IW")
                            {
                                if (VarType != VarType.Int)
                                {
                                    return (ushort)this.Read(StoreType.Input, 0, int.Parse(text.Substring(2)), VarType.Word, 1);
                                }
                                return (short)this.Read(StoreType.Input, 0, int.Parse(text.Substring(2)), VarType.Int, 1);
                            }
                        }
                    }
                    else if (text4 == "IB")
                    {
                        byte b3 = (byte)this.Read(StoreType.Input, 0, int.Parse(text.Substring(2)), VarType.Byte, 1);
                        result = b3;
                        return result;
                    }
                }
                else if (num != 2046601777U)
                {
                    if (num != 2264710824U)
                    {
                        if (num == 2365376538U)
                        {
                            if (text4 == "QB")
                            {
                                byte b4 = (byte)this.Read(StoreType.Output, 0, int.Parse(text.Substring(2)), VarType.Byte, 1);
                                result = b4;
                                return result;
                            }
                        }
                    }
                    else if (text4 == "QD")
                    {
                        if (VarType == VarType.DInt)
                        {
                            return Convert.ToInt32(this.Read(StoreType.Output, 0, int.Parse(text.Substring(2)), VarType.DInt, 1));
                        }
                        if (VarType != VarType.Real)
                        {
                            return Convert.ToUInt32(this.Read(StoreType.Output, 0, int.Parse(text.Substring(2)), VarType.DWord, 1));
                        }
                        return Convert.ToSingle(this.Read(StoreType.Output, 0, int.Parse(text.Substring(2)), VarType.Real, 1));
                    }
                }
                else if (text4 == "QW")
                {
                    if (VarType != VarType.Int)
                    {
                        return (ushort)this.Read(StoreType.Output, 0, int.Parse(text.Substring(2)), VarType.Word, 1);
                    }
                    return (short)this.Read(StoreType.Output, 0, int.Parse(text.Substring(2)), VarType.Int, 1);
                }
                text2 = text.Substring(0, 1);
                if (text2 != null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(8)
                    {
                        {
                            "E",
                            0
                        },
                        {
                            "I",
                            1
                        },
                        {
                            "A",
                            2
                        },
                        {
                            "Q",
                            3
                        },
                        {
                            "M",
                            4
                        },
                        {
                            "T",
                            5
                        },
                        {
                            "Z",
                            6
                        },
                        {
                            "C",
                            7
                        }
                    };
                    int num6;
                    if (dictionary.TryGetValue(text2, out num6))
                    {
                        StoreType storeType;
                        switch (num6)
                        {
                            case 0:
                            case 1:
                                storeType = StoreType.Input;
                                break;
                            case 2:
                            case 3:
                                storeType = StoreType.Output;
                                break;
                            case 4:
                                storeType = StoreType.Marker;
                                break;
                            case 5:
                                return (ushort)this.Read(StoreType.Timer, 0, int.Parse(text.Substring(1)), VarType.Timer, 1);
                            case 6:
                            case 7:
                                return (ushort)this.Read(StoreType.Counter, 0, int.Parse(text.Substring(1)), VarType.Counter, 1);
                            default:
                                goto IL_92F;
                        }
                        string text6 = text.Substring(1);
                        if (text6.IndexOf(".") == -1)
                        {
                            throw new Exception();
                        }
                        int startByteAdr4 = int.Parse(text6.Substring(0, text6.IndexOf(".")));
                        int num7 = int.Parse(text6.Substring(text6.IndexOf(".") + 1));
                        if (num7 > 7)
                        {
                            throw new Exception();
                        }
                        return BitLib.GetBitFromByte(Convert.ToByte(this.Read(storeType, 0, startByteAdr4, VarType.Byte, 1)), num7);
                    }
                }
                IL_92F:
                result = null;
            }
            catch
            {
                result = null;
            }
            return result;
        }

        public bool WriteBytes(StoreType StoreType, int DB, int StartByteAdr, byte[] value)
        {
            this.InteractiveLock_Child.Enter();
            bool result;
            try
            {
                int num = 0;
                int i = value.Length;
                while (i > 0)
                {
                    int num2 = Math.Min(i, 200);
                    if (!this.WriteBytesSingleRequest(StoreType, DB, StartByteAdr + num, value.Skip(num).Take(num2).ToArray<byte>()))
                    {
                        return false;
                    }
                    i -= num2;
                    num += num2;
                }
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            finally
            {
                this.InteractiveLock_Child.Leave();
            }
            return result;
        }

        private bool WriteBytesSingleRequest(StoreType StoreType, int DB, int StartByteAdr, byte[] value)
        {
            byte[] array = new byte[512];
            bool result;
            try
            {
                int num = value.Length;
                int num2 = 35 + value.Length;
                ByteArray byteArray = new ByteArray();
                ByteArray byteArray2 = byteArray;
                byte[] array2 = new byte[3];
                array2[0] = 3;
                byteArray2.Add(array2);
                byteArray.Add((byte)num2);
                byteArray.Add(new byte[]
                {
                    2,
                    240,
                    128,
                    50,
                    1,
                    0,
                    0
                });
                byteArray.Add(ByteArrayLib.GetByteArrayFromUShort((ushort)(num - 1), DataFormat.ABCD));
                byteArray.Add(new byte[]
                {
                    0,
                    14
                });
                byteArray.Add(ByteArrayLib.GetByteArrayFromUShort((ushort)(num + 4), DataFormat.ABCD));
                byteArray.Add(new byte[]
                {
                    5,
                    1,
                    18,
                    10,
                    16,
                    2
                });
                byteArray.Add(ByteArrayLib.GetByteArrayFromUShort((ushort)num, DataFormat.ABCD));
                byteArray.Add(ByteArrayLib.GetByteArrayFromUShort((ushort)DB, DataFormat.ABCD));
                byteArray.Add((byte)StoreType);
                byteArray.Add((byte)(StartByteAdr * 8 / 256 / 256 % 256));
                byteArray.Add((byte)(StartByteAdr * 8 / 256 % 256));
                byteArray.Add((byte)(StartByteAdr * 8 % 256));
                byteArray.Add(new byte[]
                {
                    0,
                    4
                });
                byteArray.Add(ByteArrayLib.GetByteArrayFromUShort((ushort)(num * 8), DataFormat.ABCD));
                byteArray.Add(value);
                array = this.SendAndReceiveWithoutLock(byteArray.array);
                if (array == null)
                {
                    result = false;
                }
                if (array[21] != 255)
                {
                    result = false;
                }
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        private bool WriteBit(StoreType StoreType, int DB, int StartByteAdr, int bitAddress, bool bitValue)
        {
            return bitAddress >= 0 && bitAddress <= 7 && this.WriteBitSingleRequest(StoreType, DB, StartByteAdr, bitAddress, bitValue);
        }

        private bool WriteBitSingleRequest(StoreType StoreType, int DB, int StartByteAdr, int bitAddress, bool bitValue)
        {
            byte[] array = new byte[512];
            byte[] array2 = new byte[]
            {
                (byte)(bitValue ? 1 : 0)
            };
            int num = array2.Length;
            int num2 = 35 + array2.Length;
            ByteArray byteArray = new ByteArray();
            ByteArray byteArray2 = byteArray;
            byte[] array3 = new byte[3];
            array3[0] = 3;
            byteArray2.Add(array3);
            byteArray.Add((byte)num2);
            byteArray.Add(new byte[]
            {
                2,
                240,
                128,
                50,
                1,
                0,
                0
            });
            byteArray.Add(ByteArrayLib.GetByteArrayFromUShort((ushort)(num - 1), DataFormat.ABCD));
            byteArray.Add(new byte[]
            {
                0,
                14
            });
            byteArray.Add(ByteArrayLib.GetByteArrayFromUShort((ushort)(num + 4), DataFormat.ABCD));
            byteArray.Add(new byte[]
            {
                5,
                1,
                18,
                10,
                16,
                1
            });
            byteArray.Add(ByteArrayLib.GetByteArrayFromUShort((ushort)num, DataFormat.ABCD));
            byteArray.Add(ByteArrayLib.GetByteArrayFromUShort((ushort)DB, DataFormat.ABCD));
            byteArray.Add((byte)StoreType);
            byteArray.Add(0);
            byteArray.Add(ByteArrayLib.GetByteArrayFromUShort((ushort)(StartByteAdr * 8 + bitAddress), DataFormat.ABCD));
            byteArray.Add(new byte[]
            {
                0,
                3
            });
            byteArray.Add(ByteArrayLib.GetByteArrayFromUShort((ushort)num, DataFormat.ABCD));
            byteArray.Add(array2);
            array = this.SendAndReceive(byteArray.array);
            if (array == null)
                throw new Exception();
            if (array[21] != 255)
                throw new Exception();
            return true;
        }

        private bool Write(StoreType StoreType, int DB, int StartByteAdr, object evalue, int bitAdr = -1)
        {
            bool result;
            if (bitAdr == -1)
            {
                string name = evalue.GetType().Name;
                if (name != null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(13)
                    {
                        {
                            "Byte",
                            0
                        },
                        {
                            "Int16",
                            1
                        },
                        {
                            "UInt16",
                            2
                        },
                        {
                            "Int32",
                            3
                        },
                        {
                            "UInt32",
                            4
                        },
                        {
                            "Single",
                            5
                        },
                        {
                            "Byte[]",
                            6
                        },
                        {
                            "Int16[]",
                            7
                        },
                        {
                            "UInt16[]",
                            8
                        },
                        {
                            "Int32[]",
                            9
                        },
                        {
                            "UInt32[]",
                            10
                        },
                        {
                            "Single[]",
                            11
                        },
                        {
                            "String",
                            12
                        },
                        {
                            "Int64",
                            13
                        },
                        {
                            "UInt64",
                            14
                        },
                        {
                            "Double",
                            15
                        }
                    };
                    int num;
                    if (dictionary.TryGetValue(name, out num))
                    {
                        byte[] value;
                        switch (num)
                        {
                            case 0:
                                value = ByteArrayLib.GetByteArrayFromByte((byte)evalue);
                                break;
                            case 1:
                                value = ByteArrayLib.GetByteArrayFromShort((short)evalue, DataFormat.ABCD);
                                break;
                            case 2:
                                value = ByteArrayLib.GetByteArrayFromUShort((ushort)evalue, DataFormat.ABCD);
                                break;
                            case 3:
                                value = ByteArrayLib.GetByteArrayFromInt((int)evalue, DataFormat.ABCD);
                                break;
                            case 4:
                                value = ByteArrayLib.GetByteArrayFromUInt((uint)evalue, DataFormat.ABCD);
                                break;
                            case 5:
                                value = ByteArrayLib.GetByteArrayFromFloat((float)evalue, DataFormat.ABCD);
                                break;
                            case 6:
                                value = (byte[])evalue;
                                break;
                            case 7:
                                value = ByteArrayLib.GetByteArrayFromShortArray((short[])evalue, DataFormat.ABCD);
                                break;
                            case 8:
                                value = ByteArrayLib.GetByteArrayFromUShortArray((ushort[])evalue, DataFormat.ABCD);
                                break;
                            case 9:
                                value = ByteArrayLib.GetByteArrayFromIntArray((int[])evalue, DataFormat.ABCD);
                                break;
                            case 10:
                                value = ByteArrayLib.GetByteArrayFromUIntArray((uint[])evalue, DataFormat.ABCD);
                                break;
                            case 11:
                                value = ByteArrayLib.GetByteArrayFromFloatArray((float[])evalue, DataFormat.ABCD);
                                break;
                            case 12:
                                value = ByteArrayLib.GetByteArrayFromString((string)evalue, Encoding.GetEncoding("GBK"));
                                break;
                            case 13:
                                value = ByteArrayLib.GetByteArrayFromLong((long)evalue, DataFormat.ABCD);
                                break;
                            case 14:
                                value = ByteArrayLib.GetByteArrayFromULong((ulong)evalue, DataFormat.ABCD);
                                break;
                            case 15:
                                value = ByteArrayLib.GetByteArrayFromDouble((double)evalue, DataFormat.ABCD);
                                break;
                            default:
                                goto IL_285;
                        }
                        return this.WriteBytes(StoreType, DB, StartByteAdr, value);
                    }
                }
                IL_285:
                result = false;
            }
            else if (evalue is bool)
            {
                result = this.WriteBit(StoreType, DB, StartByteAdr, bitAdr, (bool)evalue);
            }
            else
            {
                int num2 = 0;
                bool flag;
                if (evalue is int)
                {
                    num2 = (int)evalue;
                    flag = true;
                }
                else
                {
                    flag = false;
                }
                result = (flag && (bitAdr >= 0 && bitAdr <= 7) && this.WriteBit(StoreType, DB, StartByteAdr, bitAdr, num2 == 1));
            }
            return result;
        }

        public bool Write(string variable, object value)
        {
            string text = variable.ToUpper();
            text = text.Replace(" ", "");
            bool flag;
            bool result;
            try
            {
                string text2 = text.Substring(0, 2);
                string text3 = text2;
                string text4 = text3;
                uint num = PrivateImplementationDetails.ComputeStringHash(text4);
                if (num <= 1458105184U)
                {
                    if (num <= 970965853U)
                    {
                        if (num != 919500163U)
                        {
                            if (num == 970965853U)
                            {
                                if (text4 == "MW")
                                {
                                    if (value is short)
                                    {
                                        object obj = Convert.ChangeType(value, typeof(short));
                                        flag = this.Write(StoreType.Marker, 0, int.Parse(text.Substring(2)), (short)obj, -1);
                                    }
                                    else
                                    {
                                        object obj2 = Convert.ChangeType(value, typeof(ushort));
                                        flag = this.Write(StoreType.Marker, 0, int.Parse(text.Substring(2)), (ushort)obj2, -1);
                                    }
                                    return flag;
                                }
                            }
                        }
                        else if (text4 == "DB")
                        {
                            string[] array = text.Split(new char[]
                            {
                                '.'
                            });
                            if (array.Length < 2)
                            {
                                throw new Exception();
                            }
                            int int_ = int.Parse(array[0].Substring(2));
                            string text5 = array[1].Substring(0, 3);
                            string text6 = array[1].Substring(3);
                            int num2;
                            if (!text6.Contains('|'))
                            {
                                num2 = int.Parse(text6);
                            }
                            else
                            {
                                string[] array2 = text6.Split(new char[]
                                {
                                    '|'
                                });
                                num2 = int.Parse(array2[0]);
                            }
                            text2 = text5;
                            if (text2 != null)
                            {
                                if (text2 == "DBB")
                                {
                                    object obj3 = Convert.ChangeType(value, typeof(byte));
                                    flag = (result = this.Write(StoreType.DataBlock, int_, num2, (byte)obj3, -1));
                                    return result;
                                }
                                if (text2 == "DBW")
                                {
                                    if (value is short)
                                    {
                                        object obj4 = Convert.ChangeType(value, typeof(short));
                                        flag = this.Write(StoreType.DataBlock, int_, num2, (short)obj4, -1);
                                    }
                                    else
                                    {
                                        object obj5 = Convert.ChangeType(value, typeof(ushort));
                                        flag = this.Write(StoreType.DataBlock, int_, num2, (ushort)obj5, -1);
                                    }
                                    return flag;
                                }
                                if (text2 == "DBD")
                                {
                                    if (value is int)
                                    {
                                        object obj6 = Convert.ChangeType(value, typeof(int));
                                        flag = this.Write(StoreType.DataBlock, int_, num2, (int)obj6, -1);
                                    }
                                    else if (value is uint)
                                    {
                                        object obj7 = Convert.ChangeType(value, typeof(uint));
                                        flag = this.Write(StoreType.DataBlock, int_, num2, (uint)obj7, -1);
                                    }
                                    else
                                    {
                                        object obj8 = Convert.ChangeType(value, typeof(float));
                                        flag = this.Write(StoreType.DataBlock, int_, num2, (float)obj8, -1);
                                    }
                                    return flag;
                                }
                                if (text2 == "DBR")
                                {
                                    if (value is long)
                                    {
                                        object obj9 = Convert.ChangeType(value, typeof(long));
                                        flag = this.Write(StoreType.DataBlock, int_, num2, (long)obj9, -1);
                                    }
                                    else if (value is ulong)
                                    {
                                        object obj10 = Convert.ChangeType(value, typeof(ulong));
                                        flag = this.Write(StoreType.DataBlock, int_, num2, (ulong)obj10, -1);
                                    }
                                    else
                                    {
                                        object obj11 = Convert.ChangeType(value, typeof(double));
                                        flag = this.Write(StoreType.DataBlock, int_, num2, (double)obj11, -1);
                                    }
                                    return flag;
                                }
                                if (text2 == "DBS")
                                {
                                    if (!text6.Contains('|'))
                                    {
                                        byte[] byteArrayFromSiemensString = ByteArrayLib.GetByteArrayFromSiemensString((string)value);
                                        byteArrayFromSiemensString[0] = byte.Parse(array[2]);
                                        flag = (result = this.Write(StoreType.DataBlock, int_, num2, byteArrayFromSiemensString, -1));
                                        return result;
                                    }
                                    byte[] byteArrayFromString = ByteArrayLib.GetByteArrayFromString((string)value, Encoding.GetEncoding("GBK"));
                                    flag = (result = this.Write(StoreType.DataBlock, int_, num2, byteArrayFromString, -1));
                                    return result;
                                }
                                else if (text2 == "DBX")
                                {
                                    int SleepTime = num2;
                                    int WaitTimes = int.Parse(array[2]);
                                    flag = (result = this.Write(StoreType.DataBlock, int_, SleepTime, value, WaitTimes));
                                    return result;
                                }
                            }
                            return false;
                        }
                    }
                    else if (num != 1155519662U)
                    {
                        if (num != 1189074900U)
                        {
                            if (num == 1458105184U)
                            {
                                if (text4 == "ID")
                                {
                                    object obj12 = Convert.ChangeType(value, typeof(uint));
                                    flag = (result = this.Write(StoreType.Input, 0, int.Parse(text.Substring(2)), (uint)obj12, -1));
                                    return result;
                                }
                            }
                        }
                        else if (text4 == "MD")
                        {
                            if (value is int)
                            {
                                object obj13 = Convert.ChangeType(value, typeof(int));
                                flag = this.Write(StoreType.Marker, 0, int.Parse(text.Substring(2)), (int)obj13, -1);
                            }
                            else if (value is uint)
                            {
                                object obj14 = Convert.ChangeType(value, typeof(uint));
                                flag = this.Write(StoreType.Marker, 0, int.Parse(text.Substring(2)), (uint)obj14, -1);
                            }
                            else
                            {
                                object obj15 = Convert.ChangeType(value, typeof(float));
                                flag = this.Write(StoreType.Marker, 0, int.Parse(text.Substring(2)), (float)obj15, -1);
                            }
                            return flag;
                        }
                    }
                    else if (text4 == "MB")
                    {
                        object obj16 = Convert.ChangeType(value, typeof(byte));
                        flag = (result = this.Write(StoreType.Marker, 0, int.Parse(text.Substring(2)), (byte)obj16, -1));
                        return result;
                    }
                }
                else if (num <= 1776879945U)
                {
                    if (num != 1558770898U)
                    {
                        if (num == 1776879945U)
                        {
                            if (text4 == "IW")
                            {
                                object obj17 = Convert.ChangeType(value, typeof(ushort));
                                flag = (result = this.Write(StoreType.Input, 0, int.Parse(text.Substring(2)), (ushort)obj17, -1));
                                return result;
                            }
                        }
                    }
                    else if (text4 == "IB")
                    {
                        object obj18 = Convert.ChangeType(value, typeof(byte));
                        flag = (result = this.Write(StoreType.Input, 0, int.Parse(text.Substring(2)), (byte)obj18, -1));
                        return result;
                    }
                }
                else if (num != 2046601777U)
                {
                    if (num != 2264710824U)
                    {
                        if (num == 2365376538U)
                        {
                            if (text4 == "QB")
                            {
                                object obj19 = Convert.ChangeType(value, typeof(byte));
                                flag = (result = this.Write(StoreType.Output, 0, int.Parse(text.Substring(2)), (byte)obj19, -1));
                                return result;
                            }
                        }
                    }
                    else if (text4 == "QD")
                    {
                        object obj20 = Convert.ChangeType(value, typeof(uint));
                        flag = (result = this.Write(StoreType.Output, 0, int.Parse(text.Substring(2)), (uint)obj20, -1));
                        return result;
                    }
                }
                else if (text4 == "QW")
                {
                    object obj21 = Convert.ChangeType(value, typeof(ushort));
                    flag = (result = this.Write(StoreType.Output, 0, int.Parse(text.Substring(2)), (ushort)obj21, -1));
                    return result;
                }
                text2 = text.Substring(0, 1);
                if (text2 != null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(8)
                    {
                        {
                            "E",
                            0
                        },
                        {
                            "I",
                            1
                        },
                        {
                            "A",
                            2
                        },
                        {
                            "Q",
                            3
                        },
                        {
                            "M",
                            4
                        },
                        {
                            "T",
                            5
                        },
                        {
                            "Z",
                            6
                        },
                        {
                            "C",
                            7
                        }
                    };
                    int num3;
                    if (dictionary.TryGetValue(text2, out num3))
                    {
                        StoreType storeType_;
                        switch (num3)
                        {
                            case 0:
                            case 1:
                                storeType_ = StoreType.Input;
                                break;
                            case 2:
                            case 3:
                                storeType_ = StoreType.Output;
                                break;
                            case 4:
                                storeType_ = StoreType.Marker;
                                break;
                            case 5:
                                flag = (result = this.Write(StoreType.Timer, 0, int.Parse(text.Substring(1)), (double)value, -1));
                                return result;
                            case 6:
                            case 7:
                                flag = (result = this.Write(StoreType.Counter, 0, int.Parse(text.Substring(1)), (short)value, -1));
                                return result;
                            default:
                                goto IL_97B;
                        }
                        string text7 = text.Substring(1);
                        if (text7.IndexOf(".") == -1)
                        {
                            throw new Exception();
                        }
                        int int_4 = int.Parse(text7.Substring(0, text7.IndexOf(".")));
                        int int_5 = int.Parse(text7.Substring(text7.IndexOf(".") + 1));
                        flag = (result = this.Write(storeType_, 0, int_4, value, int_5));
                        return result;
                    }
                }
                IL_97B:
                throw new Exception();
            }
            catch
            {
                flag = false;
            }
            result = flag;
            return result;
        }

        public CalResult Write(string variable, string value, DataType dataType)
        {
            CalResult xktResult = new CalResult();
            try
            {
                switch (dataType)
                {
                    case DataType.Bool:
                        xktResult.IsSuccess = this.Write(variable, value == "1" || value.ToLower() == "true");
                        break;
                    case DataType.Byte:
                        xktResult.IsSuccess = this.Write(variable, byte.Parse(value));
                        break;
                    case DataType.Short:
                        xktResult.IsSuccess = this.Write(variable, short.Parse(value));
                        break;
                    case DataType.UShort:
                        xktResult.IsSuccess = this.Write(variable, ushort.Parse(value));
                        break;
                    case DataType.Int:
                        xktResult.IsSuccess = this.Write(variable, int.Parse(value));
                        break;
                    case DataType.UInt:
                        xktResult.IsSuccess = this.Write(variable, uint.Parse(value));
                        break;
                    case DataType.Float:
                        xktResult.IsSuccess = this.Write(variable, float.Parse(value));
                        break;
                    case DataType.Double:
                        xktResult.IsSuccess = this.Write(variable, double.Parse(value));
                        break;
                    case DataType.Long:
                        xktResult.IsSuccess = this.Write(variable, long.Parse(value));
                        break;
                    case DataType.ULong:
                        xktResult.IsSuccess = this.Write(variable, ulong.Parse(value));
                        break;
                    case DataType.String:
                        xktResult.IsSuccess = this.Write(variable, value);
                        break;
                    case DataType.ByteArray:
                        xktResult.IsSuccess = this.Write(variable, ByteArrayLib.GetByteArrayFromHexString(value, ' '));
                        break;
                    case DataType.HexString:
                        xktResult.IsSuccess = false;
                        xktResult.Message = "不支持的数据类型";
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

        public bool CoolStart()
        {
            byte[] array = this.SendAndReceive(this.S7_COLD_START);
            bool result;
            if (array != null)
            {
                if (array.Length >= 20)
                {
                    result = (array[19] == 40);
                }
                else
                {
                    result = (array.Length >= 21 && array[20] == 2);
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        public bool WarmStart()
        {
            byte[] array = this.SendAndReceive(this.S7_HOT_START);
            bool result;
            if (array != null)
            {
                if (array.Length >= 20)
                {
                    result = (array[19] == 40);
                }
                else
                {
                    result = (array.Length >= 21 && array[20] == 2);
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        public bool StopCPU()
        {
            byte[] array = this.SendAndReceive(this.S7_STOP);
            bool result;
            if (array != null)
            {
                if (array.Length == 19)
                {
                    result = true;
                }
                else if (array.Length >= 20)
                {
                    result = (array[19] == 41);
                }
                else
                {
                    result = (array.Length >= 21 && array[20] == 7);
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        private byte[] SendAndReceive(byte[] SendByte)
        {
            byte[] result;
            this.InteractiveLock_Child.Enter();
            try
            {
                IMessage message = this.GetNewNetMessage();
                if (message != null)
                    message.SendData = SendByte;
                this.tcpclient.Send(SendByte, SendByte.Length, SocketFlags.None);
                Thread.Sleep(1);
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
                this.InteractiveLock_Child.Leave();
            }
            return result;
        }

        private byte[] SendAndReceiveWithoutLock(byte[] SendByte)
        {
            byte[] result;
            try
            {
                IMessage message = this.GetNewNetMessage();
                if (message != null)
                {
                    message.SendData = SendByte;
                }
                this.tcpclient.Send(SendByte, SendByte.Length, SocketFlags.None);
                Thread.Sleep(1);
                CalResult<byte[]> xktResult = base.ReceiveByMessage(this.tcpclient, this.ReceiveTimeOut, message, null);
                if (!xktResult.IsSuccess)
                {
                    result = null;
                }
                else if (message != null && !message.CheckHeadDataLegal(base.Token.ToByteArray()))
                {
                    result = null;
                }
                else
                {
                    result = xktResult.Content;
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        private IMessage GetNewNetMessage()
        {
            return new S7Message();
        }

        public bool WriteMultipleVars(List<SiemensVar> SiemensVar)
        {
            bool result;
            if (SiemensVar.Count >= 20 || SiemensVar.Count <= 0)
                result = false;
            else
            {
                List<SiemensGroup> list = new List<SiemensGroup>();
                foreach (SiemensVar siemensVar_ in SiemensVar)
                {
                    SiemensGroup siemensGroup = this.ConvertVarToGroup(siemensVar_, false);
                    if (siemensGroup == null)
                        return false;
                    list.Add(siemensGroup);
                }
                result = this.WriteMultipleVars(list);
            }
            return result;
        }

        public List<SiemensVar> ReadMultipleVars(List<SiemensVar> SiemensVar)
        {
            List<SiemensVar> result;
            if (SiemensVar.Count >= 20 || SiemensVar.Count <= 0)
                result = null;
            else
            {
                List<SiemensGroup> list = new List<SiemensGroup>();
                foreach (SiemensVar siemensVar_ in SiemensVar)
                {
                    SiemensGroup siemensGroup = this.ConvertVarToGroup(siemensVar_, true);
                    if (siemensGroup == null)
                        return null;
                    list.Add(siemensGroup);
                }
                list = this.ReadMultipleVars(list);
                if (list == null)
                    result = null;
                else
                {
                    foreach (SiemensVar siemensVar in SiemensVar)
                    {
                        int index = SiemensVar.IndexOf(siemensVar);
                        if (list[index].Value == null)
                            return null;
                        if (list[index].Value.Length != this.GetReadByteLength(siemensVar.VarAddress, siemensVar.VarType))
                            return null;
                        switch (siemensVar.VarType)
                        {
                            case DataType.Bool:
                                {
                                    int offset = Convert.ToInt32(siemensVar.VarAddress.Substring(siemensVar.VarAddress.LastIndexOf(".") + 1));
                                    siemensVar.VarValue = BitLib.GetBitFromByte(list[index].Value[0], offset);
                                    break;
                                }
                            case DataType.Byte:
                                siemensVar.VarValue = list[index].Value[0];
                                break;
                            case DataType.Short:
                                siemensVar.VarValue = ShortLib.GetShortFromByteArray(list[index].Value, 0, DataFormat.ABCD);
                                break;
                            case DataType.UShort:
                                siemensVar.VarValue = UShortLib.GetUShortFromByteArray(list[index].Value, 0, DataFormat.ABCD);
                                break;
                            case DataType.Int:
                                siemensVar.VarValue = IntLib.GetIntFromByteArray(list[index].Value, 0, DataFormat.ABCD);
                                break;
                            case DataType.UInt:
                                siemensVar.VarValue = UIntLib.GetUIntFromByteArray(list[index].Value, 0, DataFormat.ABCD);
                                break;
                            case DataType.Float:
                                siemensVar.VarValue = FloatLib.GetFloatFromByteArray(list[index].Value, 0, DataFormat.ABCD);
                                break;
                            case DataType.Double:
                                siemensVar.VarValue = DoubleLib.GetDoubleFromByteArray(list[index].Value, 0, DataFormat.ABCD);
                                break;
                            case DataType.Long:
                                siemensVar.VarValue = LongLib.GetLongFromByteArray(list[index].Value, 0, DataFormat.ABCD);
                                break;
                            case DataType.ULong:
                                siemensVar.VarValue = ULongLib.GetULongFromByteArray(list[index].Value, 0, DataFormat.ABCD);
                                break;
                            case DataType.String:
                                if (!siemensVar.VarAddress.Contains('|'))
                                {
                                    byte[] value = list[index].Value;
                                    int num = (int)value[1];
                                    if (num > 0)
                                        siemensVar.VarValue = Encoding.GetEncoding("GBK").GetString(ByteArrayLib.GetByteArray(value, 2, num));
                                    else
                                        siemensVar.VarValue = "empty";
                                }
                                else
                                    siemensVar.VarValue = StringLib.GetStringFromByteArray(list[index].Value, 0, list[index].Value.Length, Encoding.GetEncoding("GBK"));
                                break;
                        }
                    }
                    result = SiemensVar;
                }
            }
            return result;
        }

        private SiemensGroup ConvertVarToGroup(SiemensVar var, bool IsRead = true)
        {
            SiemensGroup siemensGroup = new SiemensGroup();
            try
            {
                if (!IsRead)
                {
                    switch (var.VarType)
                    {
                        case DataType.Bool:
                            return null;
                        case DataType.Byte:
                            siemensGroup.Value = ByteArrayLib.GetByteArrayFromByte(Convert.ToByte(var.VarValue));
                            break;
                        case DataType.Short:
                            siemensGroup.Value = ByteArrayLib.GetByteArrayFromShort(Convert.ToInt16(var.VarValue), DataFormat.ABCD);
                            break;
                        case DataType.UShort:
                            siemensGroup.Value = ByteArrayLib.GetByteArrayFromUShort(Convert.ToUInt16(var.VarValue), DataFormat.ABCD);
                            break;
                        case DataType.Int:
                            siemensGroup.Value = ByteArrayLib.GetByteArrayFromInt(Convert.ToInt32(var.VarValue), DataFormat.ABCD);
                            break;
                        case DataType.UInt:
                            siemensGroup.Value = ByteArrayLib.GetByteArrayFromUInt(Convert.ToUInt32(var.VarValue), DataFormat.ABCD);
                            break;
                        case DataType.Float:
                            siemensGroup.Value = ByteArrayLib.GetByteArrayFromFloat(Convert.ToSingle(var.VarValue), DataFormat.ABCD);
                            break;
                        case DataType.Double:
                            siemensGroup.Value = ByteArrayLib.GetByteArrayFromDouble(Convert.ToDouble(var.VarValue), DataFormat.ABCD);
                            break;
                        case DataType.Long:
                            siemensGroup.Value = ByteArrayLib.GetByteArrayFromLong(Convert.ToInt64(var.VarValue), DataFormat.ABCD);
                            break;
                        case DataType.ULong:
                            siemensGroup.Value = ByteArrayLib.GetByteArrayFromULong(Convert.ToUInt64(var.VarValue), DataFormat.ABCD);
                            break;
                        case DataType.String:
                            siemensGroup.Value = ByteArrayLib.GetByteArrayFromSiemensString(Convert.ToString(var.VarValue));
                            break;
                        case DataType.ByteArray:
                            siemensGroup.Value = (byte[])var.VarValue;
                            break;
                    }
                }
                siemensGroup.Count = this.GetReadByteLength(var.VarAddress, var.VarType);
                string text = var.VarAddress.Replace(" ", "").ToUpper();
                string text2 = text.Substring(0, 2);
                string text3 = text2;
                string text4 = text3;
                uint num = PrivateImplementationDetails.ComputeStringHash(text4);
                if (num <= 970965853U)
                {
                    if (num <= 701244021U)
                    {
                        if (num <= 382469260U)
                        {
                            if (num != 348914022U)
                            {
                                if (num != 382469260U)
                                {
                                    goto IL_49A;
                                }
                                if (!(text4 == "ED"))
                                {
                                    goto IL_49A;
                                }
                                goto IL_43F;
                            }
                            else
                            {
                                if (!(text4 == "EB"))
                                {
                                    goto IL_49A;
                                }
                                goto IL_43F;
                            }
                        }
                        else if (num != 651499544U)
                        {
                            if (num != 701244021U)
                            {
                                goto IL_49A;
                            }
                            if (!(text4 == "EW"))
                            {
                                goto IL_49A;
                            }
                            goto IL_43F;
                        }
                        else
                        {
                            if (!(text4 == "AD"))
                            {
                                goto IL_49A;
                            }
                            goto IL_6C5;
                        }
                    }
                    else if (num <= 919500163U)
                    {
                        if (num != 752165258U)
                        {
                            if (num != 919500163U)
                            {
                                goto IL_49A;
                            }
                            if (!(text4 == "DB"))
                            {
                                goto IL_49A;
                            }
                            siemensGroup.StoreType = StoreType.DataBlock;
                            string[] array = text.Split(new char[]
                            {
                                '.'
                            });
                            if (array.Length < 2)
                            {
                                return null;
                            }
                            siemensGroup.DB = Convert.ToInt32(array[0].Substring(2));
                            siemensGroup.StartByteAdr = Convert.ToInt32(array[1].Substring(3));
                            goto IL_6E3;
                        }
                        else
                        {
                            if (!(text4 == "AB"))
                            {
                                goto IL_49A;
                            }
                            goto IL_6C5;
                        }
                    }
                    else if (num != 970274305U)
                    {
                        if (num != 970965853U)
                        {
                            goto IL_49A;
                        }
                        if (!(text4 == "MW"))
                        {
                            goto IL_49A;
                        }
                    }
                    else
                    {
                        if (!(text4 == "AW"))
                        {
                            goto IL_49A;
                        }
                        goto IL_6C5;
                    }
                }
                else if (num <= 1558770898U)
                {
                    if (num <= 1189074900U)
                    {
                        if (num != 1155519662U)
                        {
                            if (num != 1189074900U)
                            {
                                goto IL_49A;
                            }
                            if (!(text4 == "MD"))
                            {
                                goto IL_49A;
                            }
                        }
                        else if (!(text4 == "MB"))
                        {
                            goto IL_49A;
                        }
                    }
                    else if (num != 1458105184U)
                    {
                        if (num != 1558770898U)
                        {
                            goto IL_49A;
                        }
                        if (!(text4 == "IB"))
                        {
                            goto IL_49A;
                        }
                        goto IL_43F;
                    }
                    else
                    {
                        if (!(text4 == "ID"))
                        {
                            goto IL_49A;
                        }
                        goto IL_43F;
                    }
                }
                else if (num <= 2046601777U)
                {
                    if (num != 1776879945U)
                    {
                        if (num != 2046601777U)
                        {
                            goto IL_49A;
                        }
                        if (!(text4 == "QW"))
                        {
                            goto IL_49A;
                        }
                        goto IL_6C5;
                    }
                    else
                    {
                        if (text4 == "IW")
                        {
                            goto IL_43F;
                        }
                        goto IL_49A;
                    }
                }
                else if (num != 2264710824U)
                {
                    if (num != 2365376538U)
                    {
                        goto IL_49A;
                    }
                    if (!(text4 == "QB"))
                    {
                        goto IL_49A;
                    }
                    goto IL_6C5;
                }
                else
                {
                    if (!(text4 == "QD"))
                    {
                        goto IL_49A;
                    }
                    goto IL_6C5;
                }
                siemensGroup.StoreType = StoreType.Marker;
                siemensGroup.StartByteAdr = Convert.ToInt32(text.Substring(2));
                goto IL_6E3;
                IL_43F:
                siemensGroup.StoreType = StoreType.Input;
                siemensGroup.StartByteAdr = Convert.ToInt32(text.Substring(2));
                goto IL_6E3;
                IL_49A:
                string text5 = text.Substring(0, 1);
                string text6 = text5;
                string text7 = text6;
                num = PrivateImplementationDetails.ComputeStringHash(text7);
                if (num <= 3322673650U)
                {
                    if (num != 3222007936U)
                    {
                        if (num != 3289118412U)
                        {
                            if (num != 3322673650U)
                            {
                                goto IL_6A4;
                            }
                            if (text7 == "C")
                            {
                                siemensGroup.StoreType = StoreType.Counter;
                                siemensGroup.StartByteAdr = Convert.ToInt32(text.Substring(1));
                                goto IL_6E3;
                            }
                            goto IL_6A4;
                        }
                        else
                        {
                            if (!(text7 == "A"))
                            {
                                goto IL_6A4;
                            }
                            goto IL_646;
                        }
                    }
                    else if (!(text7 == "E"))
                    {
                        goto IL_6A4;
                    }
                }
                else if (num <= 3423339364U)
                {
                    if (num != 3356228888U)
                    {
                        if (num != 3423339364U)
                        {
                            goto IL_6A4;
                        }
                        if (!(text7 == "I"))
                        {
                            goto IL_6A4;
                        }
                    }
                    else
                    {
                        if (!(text7 == "M"))
                        {
                            goto IL_6A4;
                        }
                        siemensGroup.StoreType = StoreType.Marker;
                        if (text.IndexOf(".") == -1)
                        {
                            return null;
                        }
                        siemensGroup.StartByteAdr = Convert.ToInt32(text.Substring(1).Substring(0, text.Substring(1).IndexOf(".")));
                        goto IL_6E3;
                    }
                }
                else if (num != 3507227459U)
                {
                    if (num != 3557560316U)
                    {
                        goto IL_6A4;
                    }
                    if (text7 == "Q")
                    {
                        goto IL_646;
                    }
                    goto IL_6A4;
                }
                else
                {
                    if (!(text7 == "T"))
                    {
                        goto IL_6A4;
                    }
                    siemensGroup.StoreType = StoreType.Timer;
                    siemensGroup.StartByteAdr = Convert.ToInt32(text.Substring(1));
                    goto IL_6E3;
                }
                siemensGroup.StoreType = StoreType.Input;
                if (text.IndexOf(".") == -1)
                {
                    return null;
                }
                siemensGroup.StartByteAdr = Convert.ToInt32(text.Substring(1).Substring(0, text.Substring(1).IndexOf(".")));
                goto IL_6E3;
                IL_646:
                siemensGroup.StoreType = StoreType.Output;
                if (text.IndexOf(".") == -1)
                {
                    return null;
                }
                siemensGroup.StartByteAdr = Convert.ToInt32(text.Substring(1).Substring(0, text.Substring(1).IndexOf(".")));
                goto IL_6E3;
                IL_6A4:
                return null;
                IL_6C5:
                siemensGroup.StoreType = StoreType.Output;
                siemensGroup.StartByteAdr = Convert.ToInt32(text.Substring(2));
                IL_6E3:;
            }
            catch (Exception)
            {
                return null;
            }
            return siemensGroup;
        }

        private int GetReadByteLength(string address, DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Bool:
                case DataType.Byte:
                    return 1;
                case DataType.Short:
                case DataType.UShort:
                    return 2;
                case DataType.Int:
                case DataType.UInt:
                case DataType.Float:
                    return 4;
                case DataType.String:
                    if (address.Contains('|'))
                    {
                        string[] array = address.Split(new char[]
                        {
                        '|'
                        });
                        if (array.Length != 2)
                        {
                            return -1;
                        }
                        int result;
                        if (int.TryParse(array[1], out result))
                        {
                            return result;
                        }
                        return -1;
                    }
                    else
                    {
                        if (!address.Contains('.'))
                        {
                            return -1;
                        }
                        string[] array2 = address.Split(new char[]
                        {
                        '.'
                        });
                        if (array2.Length != 3)
                        {
                            return -1;
                        }
                        int num;
                        if (int.TryParse(array2[2], out num))
                        {
                            return num + 2;
                        }
                        return -1;
                    }
            }
            return -1;
        }

        public List<SiemensGroup> ReadMultipleVars(List<SiemensGroup> SiemensGroup)
        {
            int num = SiemensGroup.Sum((SiemensGroup dataItem) => dataItem.Count);
            List<SiemensGroup> result;
            if (num > (int)(this.MaxPDUSize - 18))
            {
                if (SiemensGroup.Count != 1)
                    result = null;
                else
                {
                    SiemensGroup[0].Value = this.ReadBytes(SiemensGroup[0].StoreType, SiemensGroup[0].DB, SiemensGroup[0].StartByteAdr, SiemensGroup[0].Count);
                    result = SiemensGroup;
                }
            }
            else if (SiemensGroup.Count >= ((this.MaxPDUSize == 240) ? 20 : 80))
                result = null;
            else
            {
                int num2 = 19 + SiemensGroup.Count * 12;
                ByteArray byteArray = new ByteArray();
                byteArray.Add(this.ReadHeaderPackage(SiemensGroup.Count).array);
                foreach (SiemensGroup siemensGroup in SiemensGroup)
                    byteArray.Add(this.CreateReadDataRequestPackage(siemensGroup.StoreType, siemensGroup.DB, siemensGroup.StartByteAdr, siemensGroup.Count));
                byte[] array = this.SendAndReceive(byteArray.array);
                if (array == null)
                    result = null;
                else if (this.ParseDataIntoDataItems(array, SiemensGroup))
                    result = SiemensGroup;
                else
                    result = null;
            }
            return result;
        }

        private bool ParseDataIntoDataItems(byte[] s7data, List<SiemensGroup> dataItems)
        {
            bool result;
            if (s7data.Length <= 21)
            {
                result = false;
            }
            else
            {
                int num = 21;
                foreach (SiemensGroup siemensGroup in dataItems)
                {
                    if (s7data[num] != 255)
                        return false;
                    num += 4;
                    int count = siemensGroup.Count;
                    siemensGroup.Value = s7data.Skip(num).Take(count).ToArray<byte>();
                    num += count;
                    if (siemensGroup.Count % 2 != 0)
                        num++;
                }
                result = true;
            }
            return result;
        }

        private ByteArray CreateReadDataRequestPackage(StoreType dataType, int db, int startByteAdr, int count = 1)
        {
            ByteArray byteArray = new ByteArray();
            byteArray.Add(new byte[]
            {
                18,
                10,
                16
            });
            if (dataType - StoreType.Counter > 1)
            {
                byteArray.Add(2);
            }
            else
            {
                byteArray.Add((byte)dataType);
            }
            byteArray.Add(ByteArrayLib.GetByteArrayFromUShort((ushort)count, DataFormat.ABCD));
            byteArray.Add(ByteArrayLib.GetByteArrayFromUShort((ushort)db, DataFormat.ABCD));
            byteArray.Add((byte)dataType);
            int num = (int)((long)(startByteAdr * 8) / 65535L);
            byteArray.Add((byte)num);
            if (dataType - StoreType.Counter > 1)
            {
                byteArray.Add(ByteArrayLib.GetByteArrayFromUShort((ushort)(startByteAdr * 8), DataFormat.ABCD));
            }
            else
            {
                byteArray.Add(ByteArrayLib.GetByteArrayFromUShort((ushort)startByteAdr, DataFormat.ABCD));
            }
            return byteArray;
        }

        private ByteArray ReadHeaderPackage(int amount = 1)
        {
            ByteArray byteArray = new ByteArray();
            ByteArray byteArray2 = byteArray;
            byte[] array = new byte[2];
            array[0] = 3;
            byteArray2.Add(array);
            byteArray.Add(ByteArrayLib.GetByteArrayFromShort((short)(19 + 12 * amount), DataFormat.ABCD));
            byteArray.Add(new byte[]
            {
                2,
                240,
                128,
                50,
                1,
                0,
                0,
                0,
                0
            });
            byteArray.Add(ByteArrayLib.GetByteArrayFromUShort((ushort)(2 + amount * 12), DataFormat.ABCD));
            byteArray.Add(new byte[]
            {
                0,
                0,
                4
            });
            byteArray.Add((byte)amount);
            return byteArray;
        }

        public bool WriteMultipleVars(List<SiemensGroup> SiemensGroup)
        {
            ByteArray byteArray = new ByteArray();
            this.CreateRequest(byteArray, SiemensGroup);
            byte[] array = this.SendAndReceive(byteArray.array);
            return array != null && this.ParseResponse(array, array.Length, SiemensGroup);
        }

        private int CreateRequest(ByteArray message, List<SiemensGroup> dataItems)
        {
            message.Add(Header.Template);
            message[18] = (byte)dataItems.Count;
            int num = dataItems.Count * Parameter.Template.Length;
            this.SetWordAt(message, 13, (ushort)(2 + num));
            int num2 = Header.Template.Length;
            ByteArray byteArray = new ByteArray();
            foreach (SiemensGroup siemensGroup in dataItems)
            {
                message.Add(Parameter.Template);
                byte[] value = siemensGroup.Value;
                message[num2 + 3] = 2;
                this.SetWordAt(message, num2 + 4, (ushort)value.Length);
                this.SetWordAt(message, num2 + 6, (ushort)siemensGroup.DB);
                message[num2 + 8] = (byte)siemensGroup.StoreType;
                byteArray.Add(0);
                this.SetAddressAt(message, num2 + 9, siemensGroup.StartByteAdr, 0);
                int num3 = value.Length;
                byteArray.Add(4);
                byteArray.Add((ushort)(num3 << 3));
                byteArray.Add(value);
                if ((num3 & 1) == 1)
                    byteArray.Add(0);
                num2 += Parameter.Template.Length;
            }
            message.Add(byteArray.array);
            this.SetWordAt(message, 2, (ushort)message.Length);
            this.SetWordAt(message, 15, (ushort)(message.Length - num2));
            return message.Length;
        }

        private void SetAddressAt(ByteArray buffer, int index, int startByte, byte bitNumber)
        {
            int num = startByte * 8 + (int)bitNumber;
            buffer[index + 2] = (byte)num;
            num >>= 8;
            buffer[index + 1] = (byte)num;
            num >>= 8;
            buffer[index] = (byte)num;
        }

        private void SetWordAt(ByteArray buffer, int index, ushort value)
        {
            buffer[index] = (byte)(value >> 8);
            buffer[index + 1] = (byte)value;
        }

        private bool ParseResponse(byte[] message, int length, List<SiemensGroup> dataItems)
        {
            bool result;
            if (length < 12)
                result = false;
            else
            {
                ushort num = this.GetWordAt(message, 10);
                if (num > 0)
                    result = false;
                else if (length < 14 + dataItems.Count)
                    result = false;
                else
                {
                    IList<byte> list = new ArraySegment<byte>(message, 21, dataItems.Count);
                    for (int i = 0; i < dataItems.Count; i++)
                    {
                        byte b = list[i];
                        if (b != 255)
                            return false;
                    }
                    result = true;
                }
            }
            return result;
        }

        private ushort GetWordAt(IList<byte> buf, int index)
        {
            return (ushort)(((int)buf[index] << 8) + (int)buf[index]);
        }

        public SiemensS7()
        {
            this.MaxPDUSize = 240;
            this.ConnectTimeOut = 2000;
            this.ReceiveTimeOut = 2000;
            this.SleepTime = 5;
            this.InteractiveLock_Child = new SimpleHybirdLock();
            this.WaitTimes = 10;
            this.S7_STOP = new byte[]
            {
                3,
                0,
                0,
                33,
                2,
                240,
                128,
                50,
                1,
                0,
                0,
                14,
                0,
                0,
                16,
                0,
                0,
                41,
                0,
                0,
                0,
                0,
                0,
                9,
                80,
                95,
                80,
                82,
                79,
                71,
                82,
                65,
                77
            };
            this.S7_HOT_START = new byte[]
            {
                3,
                0,
                0,
                37,
                2,
                240,
                128,
                50,
                1,
                0,
                0,
                12,
                0,
                0,
                20,
                0,
                0,
                40,
                0,
                0,
                0,
                0,
                0,
                0,
                253,
                0,
                0,
                9,
                80,
                95,
                80,
                82,
                79,
                71,
                82,
                65,
                77
            };
            this.S7_COLD_START = new byte[]
            {
                3,
                0,
                0,
                39,
                2,
                240,
                128,
                50,
                1,
                0,
                0,
                15,
                0,
                0,
                22,
                0,
                0,
                40,
                0,
                0,
                0,
                0,
                0,
                0,
                253,
                0,
                2,
                67,
                32,
                9,
                80,
                95,
                80,
                82,
                79,
                71,
                82,
                65,
                77
            };
        }

        private SimpleHybirdLock InteractiveLock_Child;

        private int SleepTime { get; set; }

        private int WaitTimes { get; set; }

        private byte[] S7_STOP;

        private byte[] S7_HOT_START;

        private byte[] S7_COLD_START;
    }
}
