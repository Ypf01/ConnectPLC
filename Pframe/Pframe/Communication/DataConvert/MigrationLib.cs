using System;
using Pframe.Common;

namespace Pframe.DataConvert
{
    public class MigrationLib
    {
        private static string ScopeCheck(DataType dataType_0)
        {
            string result = string.Empty;
            switch (dataType_0)
            {
                case DataType.Byte:
                    result = "设置范围：" + MigrationLib.ByteMin + "-" + MigrationLib.ByteMax;
                    break;
                case DataType.Short:
                    result = "设置范围：" + MigrationLib.ShortMin + "-" + MigrationLib.ShortMax;
                    break;
                case DataType.UShort:
                    result = "设置范围：" + MigrationLib.UShortMin + "-" + MigrationLib.UShortMax;
                    break;
                case DataType.Int:
                    result = "设置范围：" + MigrationLib.IntMin + "-" + MigrationLib.IntMax;
                    break;
                case DataType.UInt:
                    result = "设置范围：" + MigrationLib.UIntMin + "-" + MigrationLib.UIntMax;
                    break;
                default:
                    result = "设置范围超限";
                    break;
            }
            return result;
        }

        public static object GetMigrationValue(object value, string scale, string offset)
        {
            object result;
            if (scale == "1" && offset == "0")
                result = value;
            else
            {
                string name = value.GetType().Name;
                string text = name.ToLower();
                string text2 = text;
                uint num = PrivateImplementationDetails.ComputeStringHash(text2);
                if (num <= 1683620383U)
                {
                    if (num <= 132346577U)
                    {
                        if (num != 64103268U)
                        {
                            if (num != 132346577U)
                            {
                                goto IL_18E;
                            }
                            if (!(text2 == "int16"))
                            {
                                goto IL_18E;
                            }
                            goto IL_192;
                        }
                        else if (!(text2 == "int64"))
                        {
                            goto IL_18E;
                        }
                    }
                    else if (num != 848563180U)
                    {
                        if (num != 1683620383U)
                        {
                            goto IL_18E;
                        }
                        if (!(text2 == "byte"))
                        {
                            goto IL_18E;
                        }
                        goto IL_192;
                    }
                    else
                    {
                        if (!(text2 == "uint32"))
                        {
                            goto IL_18E;
                        }
                        goto IL_192;
                    }
                }
                else if (num <= 2699759368U)
                {
                    if (num != 2133018345U)
                    {
                        if (num != 2699759368U)
                        {
                            goto IL_18E;
                        }
                        if (!(text2 == "double"))
                        {
                            goto IL_18E;
                        }
                    }
                    else
                    {
                        if (!(text2 == "single"))
                        {
                            goto IL_18E;
                        }
                        goto IL_192;
                    }
                }
                else if (num != 2928590578U)
                {
                    if (num != 2929723411U)
                    {
                        if (num != 4225688255U)
                        {
                            goto IL_18E;
                        }
                        if (!(text2 == "int32"))
                        {
                            goto IL_18E;
                        }
                        goto IL_192;
                    }
                    else if (!(text2 == "uint64"))
                    {
                        goto IL_18E;
                    }
                }
                else
                {
                    if (!(text2 == "uint16"))
                    {
                        goto IL_18E;
                    }
                    goto IL_192;
                }
                return Convert.ToDouble((Convert.ToDouble(value) * Convert.ToDouble(scale) + Convert.ToDouble(offset)).ToString("N4"));
                IL_18E:
                return value;
                IL_192:
                result = Convert.ToSingle((Convert.ToSingle(value) * Convert.ToSingle(scale) + Convert.ToSingle(offset)).ToString("N4"));
            }
            return result;
        }

        public static CalResult<string> SetMigrationValue(string set, DataType type, string scale, string offset)
        {
            CalResult<string> xktResult = new CalResult<string>();
            if (scale == "1" && offset == "0")
            {
                try
                {
                    switch (type)
                    {
                        case DataType.Byte:
                            xktResult.Content = Convert.ToByte(set).ToString();
                            break;
                        case DataType.Short:
                            xktResult.Content = Convert.ToInt16(set).ToString();
                            break;
                        case DataType.UShort:
                            xktResult.Content = Convert.ToUInt16(set).ToString();
                            break;
                        case DataType.Int:
                            xktResult.Content = Convert.ToInt32(set).ToString();
                            break;
                        case DataType.UInt:
                            xktResult.Content = Convert.ToUInt32(set).ToString();
                            break;
                        case DataType.Float:
                            xktResult.Content = Convert.ToSingle(set).ToString();
                            break;
                        case DataType.Double:
                            xktResult.Content = Convert.ToDouble(set).ToString();
                            break;
                        case DataType.Long:
                            xktResult.Content = Convert.ToInt64(set).ToString();
                            break;
                        case DataType.ULong:
                            xktResult.Content = Convert.ToUInt64(set).ToString();
                            break;
                        default:
                            xktResult.Content = set;
                            break;
                    }
                    xktResult.IsSuccess = true;
                    return xktResult;
                }
                catch (Exception)
                {
                    xktResult.IsSuccess = false;
                    xktResult.Message = "转换出错，" + MigrationLib.ScopeCheck(type);
                    return xktResult;
                }
            }
            CalResult<string> result;
            try
            {
                switch (type)
                {
                    case DataType.Byte:
                        xktResult.Content = Convert.ToByte((Convert.ToSingle(set) - Convert.ToSingle(offset)) / Convert.ToSingle(scale)).ToString();
                        break;
                    case DataType.Short:
                        xktResult.Content = Convert.ToInt16((Convert.ToSingle(set) - Convert.ToSingle(offset)) / Convert.ToSingle(scale)).ToString();
                        break;
                    case DataType.UShort:
                        xktResult.Content = Convert.ToUInt16((Convert.ToSingle(set) - Convert.ToSingle(offset)) / Convert.ToSingle(scale)).ToString();
                        break;
                    case DataType.Int:
                        xktResult.Content = Convert.ToInt32((Convert.ToSingle(set) - Convert.ToSingle(offset)) / Convert.ToSingle(scale)).ToString();
                        break;
                    case DataType.UInt:
                        xktResult.Content = Convert.ToUInt32((Convert.ToSingle(set) - Convert.ToSingle(offset)) / Convert.ToSingle(scale)).ToString();
                        break;
                    case DataType.Float:
                        xktResult.Content = Convert.ToSingle((Convert.ToSingle(set) - Convert.ToSingle(offset)) / Convert.ToSingle(scale)).ToString();
                        break;
                    case DataType.Double:
                        xktResult.Content = Convert.ToDouble((Convert.ToDouble(set) - Convert.ToDouble(offset)) / Convert.ToDouble(scale)).ToString();
                        break;
                    case DataType.Long:
                        xktResult.Content = Convert.ToInt64((Convert.ToDouble(set) - Convert.ToDouble(offset)) / Convert.ToDouble(scale)).ToString();
                        break;
                    case DataType.ULong:
                        xktResult.Content = Convert.ToUInt64((Convert.ToDouble(set) - Convert.ToDouble(offset)) / Convert.ToDouble(scale)).ToString();
                        break;
                    default:
                        xktResult.Content = set;
                        break;
                }
                xktResult.IsSuccess = true;
                result = xktResult;
            }
            catch (Exception)
            {
                xktResult.IsSuccess = false;
                xktResult.Message = "转换出错，" + MigrationLib.ScopeCheck(type);
                result = xktResult;
            }
            return result;
        }

        public static CalResult<string> SetMigrationValue(string set, ComplexDataType type, string scale, string offset)
        {
            CalResult<string> xktResult = new CalResult<string>();
            if (scale == "1" && offset == "0")
            {
                try
                {
                    switch (type)
                    {
                        case ComplexDataType.Byte:
                            xktResult.Content = Convert.ToByte(set).ToString();
                            goto IL_13E;
                        case ComplexDataType.Short:
                            xktResult.Content = Convert.ToInt16(set).ToString();
                            goto IL_13E;
                        case ComplexDataType.UShort:
                            xktResult.Content = Convert.ToUInt16(set).ToString();
                            goto IL_13E;
                        case ComplexDataType.Int:
                            xktResult.Content = Convert.ToInt32(set).ToString();
                            goto IL_13E;
                        case ComplexDataType.UInt:
                            xktResult.Content = Convert.ToUInt32(set).ToString();
                            goto IL_13E;
                        case ComplexDataType.Float:
                            xktResult.Content = Convert.ToSingle(set).ToString();
                            goto IL_13E;
                        case ComplexDataType.Double:
                            xktResult.Content = Convert.ToDouble(set).ToString();
                            goto IL_13E;
                        case ComplexDataType.Long:
                            xktResult.Content = Convert.ToInt64(set).ToString();
                            goto IL_13E;
                        case ComplexDataType.ULong:
                            xktResult.Content = Convert.ToUInt64(set).ToString();
                            goto IL_13E;
                    }
                    xktResult.Content = set;
                    IL_13E:
                    xktResult.IsSuccess = true;
                    return xktResult;
                }
                catch (Exception)
                {
                    xktResult.IsSuccess = false;
                    xktResult.Message = "转换出错";
                    return xktResult;
                }
            }
            CalResult<string> result;
            try
            {
                switch (type)
                {
                    case ComplexDataType.Byte:
                        xktResult.Content = Convert.ToByte((Convert.ToSingle(set) - Convert.ToSingle(offset)) / Convert.ToSingle(scale)).ToString();
                        goto IL_335;
                    case ComplexDataType.Short:
                        xktResult.Content = Convert.ToInt16((Convert.ToSingle(set) - Convert.ToSingle(offset)) / Convert.ToSingle(scale)).ToString();
                        goto IL_335;
                    case ComplexDataType.UShort:
                        xktResult.Content = Convert.ToUInt16((Convert.ToSingle(set) - Convert.ToSingle(offset)) / Convert.ToSingle(scale)).ToString();
                        goto IL_335;
                    case ComplexDataType.Int:
                        xktResult.Content = Convert.ToInt32((Convert.ToSingle(set) - Convert.ToSingle(offset)) / Convert.ToSingle(scale)).ToString();
                        goto IL_335;
                    case ComplexDataType.UInt:
                        xktResult.Content = Convert.ToUInt32((Convert.ToSingle(set) - Convert.ToSingle(offset)) / Convert.ToSingle(scale)).ToString();
                        goto IL_335;
                    case ComplexDataType.Float:
                        xktResult.Content = Convert.ToSingle((Convert.ToSingle(set) - Convert.ToSingle(offset)) / Convert.ToSingle(scale)).ToString();
                        goto IL_335;
                    case ComplexDataType.Double:
                        xktResult.Content = Convert.ToDouble((Convert.ToDouble(set) - Convert.ToDouble(offset)) / Convert.ToDouble(scale)).ToString();
                        goto IL_335;
                    case ComplexDataType.Long:
                        xktResult.Content = Convert.ToInt64((Convert.ToDouble(set) - Convert.ToDouble(offset)) / Convert.ToDouble(scale)).ToString();
                        goto IL_335;
                    case ComplexDataType.ULong:
                        xktResult.Content = Convert.ToUInt64((Convert.ToDouble(set) - Convert.ToDouble(offset)) / Convert.ToDouble(scale)).ToString();
                        goto IL_335;
                }
                xktResult.Content = set;
                IL_335:
                xktResult.IsSuccess = true;
                result = xktResult;
            }
            catch (Exception)
            {
                xktResult.IsSuccess = false;
                xktResult.Message = "转换出错";
                result = xktResult;
            }
            return result;
        }

        static MigrationLib()
        {
            MigrationLib.ByteMax = byte.MaxValue.ToString();
            MigrationLib.ByteMin = 0.ToString();
            MigrationLib.ShortMax = short.MaxValue.ToString();
            MigrationLib.ShortMin = short.MinValue.ToString();
            MigrationLib.UShortMax = ushort.MaxValue.ToString();
            MigrationLib.UShortMin = 0.ToString();
            MigrationLib.IntMax = int.MaxValue.ToString();
            MigrationLib.IntMin = int.MinValue.ToString();
            MigrationLib.UIntMax = uint.MaxValue.ToString();
            MigrationLib.UIntMin = 0U.ToString();
        }

        private static string ByteMax;

        private static string ByteMin;

        private static string ShortMax;

        private static string ShortMin;

        private static string UShortMax;

        private static string UShortMin;

        private static string IntMax;

        private static string IntMin;

        private static string UIntMax;

        private static string UIntMin;
    }
}
