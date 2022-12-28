using System;
using System.Linq;
using System.Text;
using Pframe.Common;
using Pframe.DataConvert;
using Pframe.Modbus;

namespace Pframe.PLC.Inovance
{
    /// <summary>
    ///  汇川Modbus通信类
    /// </summary>
	public class InovanceModbus : ModbusRtu
    {

        public bool ReadBool(string address, ref bool value, byte SlaveID = 1)
        {
            CalResult<byte[]> xktResult = this.ReadBytes(address, 1, SlaveID);
            if (xktResult.IsSuccess)
            {
                value = (xktResult.Content[0] == 1);
            }
            return xktResult.IsSuccess;
        }
        private static object _LOCK = new object();
        public CalResult<byte[]> ReadBytes(string address, ushort length, byte SlaveID = 1)
        {
            lock (_LOCK)
            {
                CalResult<byte[]> xktResult = new CalResult<byte[]>();
                CalResult<ushort, bool> xktResult2 = this.AnlysisAddress(address);
                CalResult<byte[]> result;
                if (xktResult2.IsSuccess)
                {
                    if (xktResult2.Content2)
                    {
                        byte[] array = this.ReadOutputStatus(SlaveID, xktResult2.Content1, length);
                        if (array != null)
                        {
                            xktResult.IsSuccess = true;
                            xktResult.Content = array;
                            result = xktResult;
                        }
                        else
                        {
                            result = CalResult.CreateFailedResult<byte[]>(new CalResult());
                        }
                    }
                    else
                    {
                        byte[] array2 = this.ReadKeepReg(SlaveID, xktResult2.Content1, length);
                        if (array2 != null)
                        {
                            xktResult.IsSuccess = true;
                            xktResult.Content = array2;
                            result = xktResult;
                        }
                        else
                        {
                            result = CalResult.CreateFailedResult<byte[]>(new CalResult());
                        }
                    }
                }
                else
                {
                    result = CalResult.CreateFailedResult<byte[]>(xktResult2);
                }
                return result;
            }
        }

        public bool ReadShort(string address, ref short value, byte SlaveID = 1)
        {
            CalResult<byte[]> xktResult = this.ReadBytes(address, 1, SlaveID);
            if (xktResult.IsSuccess)
            {
                value = ShortLib.GetShortFromByteArray(xktResult.Content, 0, DataFormat);
            }
            return xktResult.IsSuccess;
        }

        public bool ReadUshort(string address, ref ushort value, byte SlaveID = 1)
        {
            CalResult<byte[]> xktResult = this.ReadBytes(address, 1, SlaveID);
            if (xktResult.IsSuccess)
            {
                value = UShortLib.GetUShortFromByteArray(xktResult.Content, 0, DataFormat);
            }
            return xktResult.IsSuccess;
        }

        public bool ReadInt(string address, ref int value, byte SlaveID = 1)
        {
            CalResult<byte[]> xktResult = this.ReadBytes(address, 2, SlaveID);
            if (xktResult.IsSuccess)
            {
                value = IntLib.GetIntFromByteArray(xktResult.Content, 0, DataFormat);
            }
            return xktResult.IsSuccess;
        }

        public bool ReadUInt(string address, ref uint value, byte SlaveID = 1)
        {
            CalResult<byte[]> xktResult = this.ReadBytes(address, 2, SlaveID);
            if (xktResult.IsSuccess)
            {
                value = UIntLib.GetUIntFromByteArray(xktResult.Content, 0, DataFormat);
            }
            return xktResult.IsSuccess;
        }

        public bool ReadFloat(string address, ref float value, byte SlaveID = 1)
        {
            CalResult<byte[]> xktResult = this.ReadBytes(address, 2, SlaveID);
            if (xktResult.IsSuccess)
            {
                value = FloatLib.GetFloatFromByteArray(xktResult.Content, 0, DataFormat);
            }
            return xktResult.IsSuccess;
        }

        public bool ReadLong(string address, ref long value, byte SlaveID = 1)
        {
            CalResult<byte[]> xktResult = this.ReadBytes(address, 4, SlaveID);
            if (xktResult.IsSuccess)
            {
                value = LongLib.GetLongFromByteArray(xktResult.Content, 0, DataFormat);
            }
            return xktResult.IsSuccess;
        }

        public bool ReadUlong(string address, ref ulong value, byte SlaveID = 1)
        {
            CalResult<byte[]> xktResult = this.ReadBytes(address, 4, SlaveID);
            if (xktResult.IsSuccess)
            {
                value = ULongLib.GetULongFromByteArray(xktResult.Content, 0, DataFormat);
            }
            return xktResult.IsSuccess;
        }

        public bool ReadDouble(string address, ref double value, byte SlaveID = 1)
        {
            CalResult<byte[]> xktResult = this.ReadBytes(address, 4, SlaveID);
            if (xktResult.IsSuccess)
            {
                value = DoubleLib.GetDoubleFromByteArray(xktResult.Content, 0, DataFormat);
            }
            return xktResult.IsSuccess;
        }

        public bool ReadString(string address, ushort length, ref string value, byte SlaveID = 1)
        {
            CalResult<byte[]> xktResult = this.ReadBytes(address, length, SlaveID);
            if (xktResult.IsSuccess)
            {
                value = StringLib.GetStringFromByteArray(xktResult.Content, 0, xktResult.Content.Length);
            }
            return xktResult.IsSuccess;
        }

        public CalResult Write(string address, string value, DataType vartype, byte SlaveID = 1)
        {
            CalResult xktResult = new CalResult();
            try
            {
                switch (vartype)
                {
                    case DataType.Bool:
                        if (address.Contains('.'))
                        {
                            xktResult.IsSuccess = this.WriteBoolReg(address, value.ToString() == "1" || value.ToString().ToLower() == "true", SlaveID).IsSuccess;
                        }
                        else
                        {
                            xktResult.IsSuccess = this.Write(address, value.ToString() == "1" || value.ToString().ToLower() == "true", SlaveID).IsSuccess;
                        }
                        break;
                    case DataType.Byte:
                        xktResult.IsSuccess = this.Write(address, new byte[]
                        {
                        Convert.ToByte(value)
                        }, SlaveID).IsSuccess;
                        break;
                    case DataType.Short:
                        xktResult.IsSuccess = this.Write(address, Convert.ToInt16(value), SlaveID).IsSuccess;
                        break;
                    case DataType.UShort:
                        xktResult.IsSuccess = this.Write(address, Convert.ToUInt16(value), SlaveID).IsSuccess;
                        break;
                    case DataType.Int:
                        xktResult.IsSuccess = this.Write(address, Convert.ToInt32(value), SlaveID).IsSuccess;
                        break;
                    case DataType.UInt:
                        xktResult.IsSuccess = this.Write(address, Convert.ToUInt32(value), SlaveID).IsSuccess;
                        break;
                    case DataType.Float:
                        xktResult.IsSuccess = this.Write(address, Convert.ToSingle(value), SlaveID).IsSuccess;
                        break;
                    case DataType.Double:
                        xktResult.IsSuccess = this.Write(address, Convert.ToDouble(value), SlaveID).IsSuccess;
                        break;
                    case DataType.Long:
                        xktResult.IsSuccess = this.Write(address, Convert.ToInt64(value), SlaveID).IsSuccess;
                        break;
                    case DataType.ULong:
                        xktResult.IsSuccess = this.Write(address, Convert.ToUInt64(value), SlaveID).IsSuccess;
                        break;
                    case DataType.String:
                        xktResult.IsSuccess = this.Write(address.Substring(0, address.IndexOf('.')), value.ToString(), SlaveID).IsSuccess;
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

        public CalResult WriteBoolReg(string address, bool value, byte SlaveID = 1)
        {
            CalResult result;
            if (address.Contains('.'))
            {
                string[] array = address.Split(new char[]
                {
                    '.'
                });
                if (array.Length == 2)
                {
                    ushort value2 = 0;
                    if (this.ReadUshort(array[0], ref value2, SlaveID))
                    {
                        int bit = Convert.ToInt32(array[1]);
                        ushort value3 = UShortLib.SetbitValueFromUShort(value2, bit, value, DataFormat);
                        result = this.Write(array[0], value3, 1);
                    }
                    else
                    {
                        result = CalResult.CreateFailedResult();
                    }
                }
                else
                {
                    result = CalResult.CreateFailedResult();
                }
            }
            else
            {
                result = CalResult.CreateFailedResult();
            }
            return result;
        }

        public CalResult Write(string address, byte[] value, byte SlaveID = 1)
        {
            CalResult<ushort, bool> xktResult = this.AnlysisAddress(address);
            CalResult result;
            if (xktResult.IsSuccess)
            {
                if (xktResult.Content2)
                {
                    result = CalResult.CreateFailedResult();
                }
                else if (this.PreSetMultiReg(SlaveID, xktResult.Content1, value))
                {
                    result = CalResult.CreateSuccessResult();
                }
                else
                {
                    result = CalResult.CreateFailedResult();
                }
            }
            else
            {
                result = xktResult;
            }
            return result;
        }

        public CalResult Write(string address, bool value, byte SlaveID = 1)
        {
            return this.Write(address, new bool[]
            {
                value
            }, SlaveID);
        }

        public CalResult Write(string address, bool[] values, byte SlaveID = 1)
        {
            CalResult<ushort, bool> xktResult = this.AnlysisAddress(address);
            CalResult result;
            if (xktResult.IsSuccess)
            {
                if (!xktResult.Content2)
                {
                    result = CalResult.CreateFailedResult();
                }
                else if (this.ForceMultiCoil(SlaveID, xktResult.Content1, values))
                {
                    result = CalResult.CreateSuccessResult();
                }
                else
                {
                    result = CalResult.CreateFailedResult();
                }
            }
            else
            {
                result = xktResult;
            }
            return result;
        }

        public CalResult Write(string address, short[] values, byte SlaveID = 1)
        {
            return this.Write(address, ByteArrayLib.GetByteArrayFromShortArray(values, DataFormat), SlaveID);
        }

        public CalResult Write(string address, short value, byte SlaveID = 1)
        {
            return this.Write(address, new short[]
            {
                value
            }, SlaveID);
        }

        public CalResult Write(string address, ushort[] values, byte SlaveID = 1)
        {
            return this.Write(address, ByteArrayLib.GetByteArrayFromUShortArray(values, DataFormat), SlaveID);
        }

        public CalResult Write(string address, ushort value, byte SlaveID = 1)
        {
            return this.Write(address, new ushort[]
            {
                value
            }, SlaveID);
        }

        public CalResult Write(string address, int[] values, byte SlaveID = 1)
        {
            return this.Write(address, ByteArrayLib.GetByteArrayFromIntArray(values, DataFormat), SlaveID);
        }

        public CalResult Write(string address, int value, byte SlaveID = 1)
        {
            return this.Write(address, new int[]
            {
                value
            }, SlaveID);
        }

        public CalResult Write(string address, uint[] values, byte SlaveID = 1)
        {
            return this.Write(address, ByteArrayLib.GetByteArrayFromUIntArray(values, DataFormat), SlaveID);
        }

        public CalResult Write(string address, uint value, byte SlaveID = 1)
        {
            return this.Write(address, new uint[]
            {
                value
            }, SlaveID);
        }

        public CalResult Write(string address, float[] values, byte SlaveID = 1)
        {
            return this.Write(address, ByteArrayLib.GetByteArrayFromFloatArray(values, DataFormat), SlaveID);
        }

        public CalResult Write(string address, float value, byte SlaveID = 1)
        {
            return this.Write(address, new float[]
            {
                value
            }, SlaveID);
        }

        public CalResult Write(string address, long[] values, byte SlaveID = 1)
        {
            return this.Write(address, ByteArrayLib.GetByteArrayFromLongArray(values, DataFormat), SlaveID);
        }

        public CalResult Write(string address, long value, byte SlaveID = 1)
        {
            return this.Write(address, new long[]
            {
                value
            }, SlaveID);
        }

        public CalResult Write(string address, ulong[] values, byte SlaveID = 1)
        {
            return this.Write(address, ByteArrayLib.GetByteArrayFromULongArray(values, DataFormat), SlaveID);
        }

        public CalResult Write(string address, ulong value, byte SlaveID = 1)
        {
            return this.Write(address, new ulong[]
            {
                value
            }, SlaveID);
        }

        public CalResult Write(string address, double[] values, byte SlaveID = 1)
        {
            return this.Write(address, ByteArrayLib.GetByteArrayFromDoubleArray(values, DataFormat), SlaveID);
        }

        public CalResult Write(string address, double value, byte SlaveID = 1)
        {
            return this.Write(address, new double[]
            {
                value
            }, SlaveID);
        }

        public CalResult Write(string address, string value, byte SlaveID = 1)
        {
            byte[] byteArrayFromString = ByteArrayLib.GetByteArrayFromString(value, Encoding.ASCII);
            return this.Write(address, byteArrayFromString, SlaveID);
        }

        public CalResult WriteUnicodeString(string address, string value, byte SlaveID = 1)
        {
            byte[] byteArrayFromString = ByteArrayLib.GetByteArrayFromString(value, Encoding.Unicode);
            return this.Write(address, byteArrayFromString, SlaveID);
        }

        public CalResult<ushort, bool> AnlysisAddress(string address)
        {
            CalResult<ushort, bool> xktResult = new CalResult<ushort, bool>();
            int num = 0;
            string text = address.Substring(0, 1).ToLower();
            string a = text;
            if (!(a == "m"))
            {
                if (!(a == "x"))
                {
                    if (!(a == "y"))
                    {
                        if (!(a == "s"))
                        {
                            if (!(a == "t"))
                            {
                                if (a == "c")
                                {
                                    if (address.StartsWith("cr") || address.StartsWith("CR"))
                                    {
                                        if (!this.GetStartAddress(address.Substring(2), InovanceDataType.CR.FromBase, ref num))
                                        {
                                            xktResult.Message = "地址格式不正确";
                                            return xktResult;
                                        }
                                        if (num > 62663)
                                        {
                                            xktResult.Message = "CR区不能超过" + 62663.ToString();
                                            return xktResult;
                                        }
                                        xktResult.Content1 = (ushort)(62464 + num);
                                        xktResult.Content2 = false;
                                    }
                                    else
                                    {
                                        if (!this.GetStartAddress(address.Substring(1), InovanceDataType.C.FromBase, ref num))
                                        {
                                            xktResult.Message = "地址格式不正确";
                                            return xktResult;
                                        }
                                        if (num > 62719)
                                        {
                                            xktResult.Message = "C区不能超过" + 62719.ToString();
                                            return xktResult;
                                        }
                                        xktResult.Content1 = (ushort)(62464 + num);
                                        xktResult.Content2 = true;
                                    }
                                }
                                else if (a == "d")
                                {
                                    if (!this.GetStartAddress(address.Substring(1), InovanceDataType.D.FromBase, ref num))
                                    {
                                        xktResult.Message = "地址格式不正确";
                                        return xktResult;
                                    }
                                    if (num > 8000)
                                    {
                                        xktResult.Message = "d区不能超过8000";
                                        return xktResult;
                                    }
                                    xktResult.Content1 = (ushort)(num);
                                    xktResult.Content2 = false;
                                }
                            }
                            else if (address.StartsWith("tr") || address.StartsWith("TR"))
                            {
                                if (!this.GetStartAddress(address.Substring(2), InovanceDataType.TR.FromBase, ref num))
                                {
                                    xktResult.Message = "地址格式不正确";
                                    return xktResult;
                                }
                                if (num > 61695)
                                {
                                    xktResult.Message = "TR区不能超过" + 61695.ToString();
                                    return xktResult;
                                }
                                xktResult.Content1 = (ushort)(61440 + num);
                                xktResult.Content2 = false;
                            }
                            else
                            {
                                if (!this.GetStartAddress(address.Substring(1), InovanceDataType.T.FromBase, ref num))
                                {
                                    xktResult.Message = "地址格式不正确";
                                    return xktResult;
                                }
                                if (num > 61695)
                                {
                                    xktResult.Message = "T区不能超过" + 61695.ToString();
                                    return xktResult;
                                }
                                xktResult.Content1 = (ushort)(61440 + num);
                                xktResult.Content2 = true;
                            }
                        }
                        else
                        {
                            if (!this.GetStartAddress(address.Substring(1), InovanceDataType.S.FromBase, ref num))
                            {
                                xktResult.Message = "地址格式不正确";
                                return xktResult;
                            }
                            if (num > 58343)
                            {
                                xktResult.Message = "S区不能超过" + 58343.ToString();
                                return xktResult;
                            }
                            xktResult.Content1 = (ushort)(57344 + num);
                            xktResult.Content2 = true;
                        }
                    }
                    else
                    {
                        if (!this.GetStartAddress(address.Substring(1), InovanceDataType.Y.FromBase, ref num))
                        {
                            xktResult.Message = "地址格式不正确";
                            return xktResult;
                        }
                        if (num > 64767)
                        {
                            xktResult.Message = "Y区不能超过" + 64767.ToString();
                            return xktResult;
                        }
                        xktResult.Content1 = (ushort)(64512 + num);
                        xktResult.Content2 = true;
                    }
                }
                else
                {
                    if (!this.GetStartAddress(address.Substring(1), InovanceDataType.X.FromBase, ref num))
                    {
                        xktResult.Message = "地址格式不正确";
                        return xktResult;
                    }
                    if (num > 63743)
                    {
                        xktResult.Message = "X区不能超过" + 63743.ToString();
                        return xktResult;
                    }
                    xktResult.Content1 = (ushort)(63488 + num);
                    xktResult.Content2 = true;
                }
            }
            else
            {
                if (!this.GetStartAddress(address.Substring(1), InovanceDataType.M.FromBase, ref num))
                {
                    xktResult.Message = "地址格式不正确";
                    return xktResult;
                }
                if (num >= 8000)
                {
                    if (num > 8256)
                    {
                        xktResult.Message = "M区不能超过" + 8256.ToString();
                        return xktResult;
                    }
                    xktResult.Content1 = (ushort)(0 + num);
                    xktResult.Content2 = true;
                }
                else
                {
                    xktResult.Content1 = (ushort)(0 + num);
                    xktResult.Content2 = true;
                }
            }
            xktResult.IsSuccess = true;
            return xktResult;
        }

        private bool GetStartAddress(string start, int fromBase, ref int result)
        {
            try
            {
                result = Convert.ToInt32(start, fromBase);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

    }
}
