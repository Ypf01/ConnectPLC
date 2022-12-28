using System;
using System.Text;
using Pframe.Base;
using Pframe.Common;
using Pframe.DataConvert;

namespace Pframe.PLC.Melsec
{
    /// <summary>
    ///  FX3U编程口
    /// </summary>
	public class MelsecFxSerial : SerialDeviceBase
    {
        public MelsecFxSerial(DataFormat dataFormat = DataFormat.DCBA)
        {
            base.DataFormat = dataFormat;
        }

        public override CalResult<bool[]> ReadBoolArray(string address, ushort length)
        {
            CalResult<byte[], int> xktResult = this.GetReadCommandForBool(address, length);
            CalResult<bool[]> result;
            if (!xktResult.IsSuccess)
            {
                result = null;
            }
            else
            {
                byte[] byte_ = null;
                if (base.SendAndReceive(xktResult.Content1, ref byte_, 0) && this.CheckResponseForRead(byte_))
                {
                    result = CalResult.CreateSuccessResult<bool[]>(this.AnlysisDataForBool(byte_, xktResult.Content2, (int)length));
                }
                else
                {
                    result = CalResult.CreateFailedResult<bool[]>(new CalResult());
                }
            }
            return result;
        }

        public override CalResult<byte[]> ReadByteArray(string address, ushort length)
        {
            CalResult<byte[]> xktResult = this.GetReadCommandForWord(address, length);
            CalResult<byte[]> result;
            if (!xktResult.IsSuccess)
            {
                result = null;
            }
            else
            {
                byte[] byte_ = null;
                if (base.SendAndReceive(xktResult.Content, ref byte_, 0) && this.CheckResponseForRead(byte_))
                    result = CalResult.CreateSuccessResult<byte[]>(this.AnlysisData(byte_));
                else
                    result = CalResult.CreateFailedResult<byte[]>(new CalResult());
            }
            return result;
        }

        public override CalResult WriteByteArray(string address, byte[] value)
        {
            CalResult<byte[]> xktResult = this.GetWriteCommandForWord(address, value);
            CalResult result;
            if (!xktResult.IsSuccess)
            {
                result = CalResult.CreateFailedResult();
            }
            else
            {
                byte[] array = null;
                if (base.SendAndReceive(xktResult.Content, ref array, 0))
                    result = ((array[0] == 6) ? CalResult.CreateSuccessResult() : CalResult.CreateFailedResult());
                else
                    result = CalResult.CreateFailedResult();
            }
            return result;
        }

        public override CalResult WriteBoolArray(string address, bool[] values)
        {
            CalResult<byte[]> xktResult = this.GetWriteCommandForBoolArray(address, values);
            CalResult result;
            if (!xktResult.IsSuccess)
            {
                result = CalResult.CreateFailedResult();
            }
            else
            {
                byte[] array = null;
                if (base.SendAndReceive(xktResult.Content, ref array, 0))
                    result = ((array[0] == 6) ? CalResult.CreateSuccessResult() : CalResult.CreateFailedResult());
                else
                    result = CalResult.CreateFailedResult();
            }
            return result;
        }

        private CalResult<byte[]> GetReadCommandForWord(string address, ushort length)
        {
            CalResult<ushort> xktResult = this.AnalysisAddressForWord(address);
            CalResult<byte[]> result;
            if (!xktResult.IsSuccess)
                result = CalResult.CreateFailedResult<byte[]>(xktResult);
            else
            {
                ushort content = xktResult.Content;
                ByteArray byteArray = new ByteArray();
                byteArray.Add(2);
                byteArray.Add(48);
                byteArray.Add(Encoding.ASCII.GetBytes(content.ToString("X4")));
                byteArray.Add(Encoding.ASCII.GetBytes(((byte)(length * 2)).ToString("X2")));
                byteArray.Add(3);
                byteArray.Add(this.CalculateSUM(byteArray.array));
                result = CalResult.CreateSuccessResult<byte[]>(byteArray.array);
            }
            return result;
        }

        private CalResult<ushort> AnalysisAddressForWord(string address)
        {
            CalResult<FXStoreType, ushort> xktResult = this.AnalysisAddress(address);
            CalResult<ushort> result;
            if (!xktResult.IsSuccess)
            {
                result = CalResult.CreateFailedResult<ushort>(xktResult);
            }
            else
            {
                ushort num = xktResult.Content2;
                if (xktResult.Content1 == FXStoreType.D)
                {
                    if (xktResult.Content2 >= 8000)
                        num = (ushort)((num - 8000) * 2 + 3584);
                    else
                        num = (ushort)(num * 2 + 4096);
                }
                else if (xktResult.Content1 == FXStoreType.C)
                {
                    if (num >= 200)
                        num = (ushort)((num - 200) * 4 + 3072);
                    else
                        num = (ushort)(num * 2 + 2560);
                }
                else
                {
                    if (xktResult.Content1 != FXStoreType.T)
                        return new CalResult<ushort>("当前的类型不支持字读写");
                    num = (ushort)(num * 2 + 2048);
                }
                result = CalResult.CreateSuccessResult<ushort>(num);
            }
            return result;
        }

        private CalResult<FXStoreType, ushort> AnalysisAddress(string address)
        {
            CalResult<FXStoreType, ushort> xktResult = new CalResult<FXStoreType, ushort>();
            try
            {
                string text = address[0].ToString().ToLower();
                string text2 = text;
                uint num = PrivateImplementationDetails.ComputeStringHash(text2);
                if (num <= 3893112696U)
                {
                    if (num != 3775669363U)
                    {
                        if (num != 3859557458U)
                        {
                            if (num == 3893112696U)
                            {
                                if (text2 == "m")
                                {
                                    xktResult.Content1 = FXStoreType.M;
                                    xktResult.Content2 = Convert.ToUInt16(address.Substring(1), FXStoreType.M.FromBase);
                                    goto IL_22C;
                                }
                            }
                        }
                        else if (text2 == "c")
                        {
                            xktResult.Content1 = FXStoreType.C;
                            xktResult.Content2 = Convert.ToUInt16(address.Substring(1), FXStoreType.C.FromBase);
                            goto IL_22C;
                        }
                    }
                    else if (text2 == "d")
                    {
                        xktResult.Content1 = FXStoreType.D;
                        xktResult.Content2 = Convert.ToUInt16(address.Substring(1), FXStoreType.D.FromBase);
                        goto IL_22C;
                    }
                }
                else if (num <= 4127999362U)
                {
                    if (num != 4044111267U)
                    {
                        if (num == 4127999362U)
                        {
                            if (text2 == "s")
                            {
                                xktResult.Content1 = FXStoreType.S;
                                xktResult.Content2 = Convert.ToUInt16(address.Substring(1), FXStoreType.S.FromBase);
                                goto IL_22C;
                            }
                        }
                    }
                    else if (text2 == "t")
                    {
                        xktResult.Content1 = FXStoreType.T;
                        xktResult.Content2 = Convert.ToUInt16(address.Substring(1), FXStoreType.T.FromBase);
                        goto IL_22C;
                    }
                }
                else if (num != 4228665076U)
                {
                    if (num == 4245442695U)
                    {
                        if (text2 == "x")
                        {
                            xktResult.Content1 = FXStoreType.X;
                            xktResult.Content2 = Convert.ToUInt16(address.Substring(1), FXStoreType.X.FromBase);
                            goto IL_22C;
                        }
                    }
                }
                else if (text2 == "y")
                {
                    xktResult.Content1 = FXStoreType.Y;
                    xktResult.Content2 = Convert.ToUInt16(address.Substring(1), FXStoreType.Y.FromBase);
                    goto IL_22C;
                }
                throw new Exception("不支持的存储区");
                IL_22C:;
            }
            catch (Exception ex)
            {
                xktResult.IsSuccess = false;
                xktResult.Message = ex.Message;
                return xktResult;
            }
            xktResult.IsSuccess = true;
            return xktResult;
        }

        private CalResult<byte[], int> GetReadCommandForBool(string address, ushort length)
        {
            CalResult<ushort, ushort, ushort> xktResult = this.AnalysisAddressForBool(address);
            CalResult<byte[], int> result;
            if (!xktResult.IsSuccess)
            {
                result = CalResult.CreateFailedResult<byte[], int>(xktResult);
            }
            else
            {
                ushort num = (ushort)((length % 8 == 0) ? (length / 8) : (length / 8 + 1));
                ushort content = xktResult.Content1;
                ByteArray byteArray = new ByteArray();
                byteArray.Add(2);
                byteArray.Add(48);
                byteArray.Add(Encoding.ASCII.GetBytes(content.ToString("X4")));
                byteArray.Add(Encoding.ASCII.GetBytes(num.ToString("X2")));
                byteArray.Add(3);
                byteArray.Add(this.CalculateSUM(byteArray.array));
                result = CalResult.CreateSuccessResult<byte[], int>(byteArray.array, (int)xktResult.Content3);
            }
            return result;
        }

        private CalResult<ushort, ushort, ushort> AnalysisAddressForBool(string address)
        {
            CalResult<FXStoreType, ushort> xktResult = this.AnalysisAddress(address);
            CalResult<ushort, ushort, ushort> result;
            if (!xktResult.IsSuccess)
            {
                result = CalResult.CreateFailedResult<ushort, ushort, ushort>(xktResult);
            }
            else
            {
                ushort num = xktResult.Content2;
                if (xktResult.Content1 == FXStoreType.M)
                {
                    if (num >= 8000)
                    {
                        num = (ushort)((num - 8000) / 8 + 480);
                    }
                    else
                    {
                        num = (ushort)(num / 8 + 256);
                    }
                }
                else if (xktResult.Content1 == FXStoreType.X)
                {
                    num = (ushort)(num / 8 + 128);
                }
                else if (xktResult.Content1 == FXStoreType.Y)
                {
                    num = (ushort)(num / 8 + 160);
                }
                else if (xktResult.Content1 == FXStoreType.S)
                {
                    num /= 8;
                }
                else if (xktResult.Content1 == FXStoreType.C)
                {
                    num = (ushort)(num * 2 + 448);
                }
                else
                {
                    if (xktResult.Content1 != FXStoreType.T)
                    {
                        return new CalResult<ushort, ushort, ushort>("当前的类型不支持字读写");
                    }
                    num = (ushort)(num * 2 + 192);
                }
                result = CalResult.CreateSuccessResult<ushort, ushort, ushort>(num, xktResult.Content2, (ushort)(xktResult.Content2 % 8));
            }
            return result;
        }

        private CalResult<byte[]> GetWriteCommandForWord(string address, byte[] value)
        {
            CalResult<ushort> xktResult = this.AnalysisAddressForWord(address);
            CalResult<byte[]> result;
            if (!xktResult.IsSuccess)
            {
                result = CalResult.CreateFailedResult<byte[]>(xktResult);
            }
            else
            {
                ushort num = (ushort)value.Length;
                if (value != null)
                {
                    value = ByteArrayLib.GetAsciiBytesFromByteArray(value);
                }
                ushort content = xktResult.Content;
                ByteArray byteArray = new ByteArray();
                byteArray.Add(2);
                byteArray.Add(49);
                byteArray.Add(Encoding.ASCII.GetBytes(content.ToString("X4")));
                byteArray.Add(Encoding.ASCII.GetBytes(((byte)num).ToString("X2")));
                byteArray.Add(value);
                byteArray.Add(3);
                byteArray.Add(this.CalculateSUM(byteArray.array));
                result = CalResult.CreateSuccessResult<byte[]>(byteArray.array);
            }
            return result;
        }

        private CalResult<byte[]> GetWriteCommandForBool(string address, bool value)
        {
            CalResult<ushort> xktResult = this.AnalysisAddressForForceBool(address);
            CalResult<byte[]> result;
            if (!xktResult.IsSuccess)
            {
                result = CalResult.CreateFailedResult<byte[]>(xktResult);
            }
            else
            {
                ushort content = xktResult.Content;
                ByteArray byteArray = new ByteArray();
                byteArray.Add(2);
                byteArray.Add((byte)(value ? 55 : 56));
                byte[] bytes = Encoding.ASCII.GetBytes(content.ToString("X4"));
                byteArray.Add(bytes[2]);
                byteArray.Add(bytes[3]);
                byteArray.Add(bytes[0]);
                byteArray.Add(bytes[1]);
                byteArray.Add(3);
                byteArray.Add(this.CalculateSUM(byteArray.array));
                result = CalResult.CreateSuccessResult<byte[]>(byteArray.array);
            }
            return result;
        }

        private CalResult<ushort> AnalysisAddressForForceBool(string address)
        {
            CalResult<FXStoreType, ushort> xktResult = this.AnalysisAddress(address);
            CalResult<ushort> result;
            if (!xktResult.IsSuccess)
            {
                result = CalResult.CreateFailedResult<ushort>(xktResult);
            }
            else
            {
                ushort num = xktResult.Content2;
                if (xktResult.Content1 == FXStoreType.M)
                {
                    if (num >= 8000)
                    {
                        num = (ushort)(num - 8000 + 3840);
                    }
                    else
                    {
                        num += 2048;
                    }
                }
                else if (xktResult.Content1 == FXStoreType.S)
                    num = (ushort)(num + 0);
                else if (xktResult.Content1 == FXStoreType.X)
                    num += 1024;
                else
                {
                    if (xktResult.Content1 != FXStoreType.Y)
                    {
                        return new CalResult<ushort>("类型不支持读写");
                    }
                    num += 1280;
                }
                result = CalResult.CreateSuccessResult<ushort>(num);
            }
            return result;
        }

        private CalResult<byte[]> GetWriteCommandForBoolArray(string address, bool[] value)
        {
            CalResult<ushort, ushort, ushort> xktResult = this.AnalysisAddressForBool(address);
            CalResult<byte[]> result;
            if (!xktResult.IsSuccess)
            {
                result = CalResult.CreateFailedResult<byte[]>(xktResult);
            }
            else
            {
                ushort content = xktResult.Content1;
                ushort num = (ushort)((value.Length % 8 == 0) ? (value.Length / 8) : (value.Length / 8 + 1));
                ByteArray byteArray = new ByteArray();
                byteArray.Add(2);
                byteArray.Add(49);
                byteArray.Add(Encoding.ASCII.GetBytes(content.ToString("X4")));
                byteArray.Add(Encoding.ASCII.GetBytes(((byte)num).ToString("X2")));
                byteArray.Add(ByteArrayLib.GetAsciiBytesFromByteArray(ByteArrayLib.GetByteArrayFromBoolArray(value)));
                byteArray.Add(3);
                byteArray.Add(this.CalculateSUM(byteArray.array));
                result = CalResult.CreateSuccessResult<byte[]>(byteArray.array);
            }
            return result;
        }

        private bool CheckResponseForRead(byte[] response)
        {
            return response.Length != 0 && response[0] != 21 && response[0] == 2 && this.CheckSUM(response);
        }

        private bool[] AnlysisDataForBool(byte[] response, int start, int length)
        {
            bool[] result;
            try
            {
                result = BitLib.GetBitArray(BitLib.GetBitArrayFromByteArray(this.AnlysisData(response), false), start, length);
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        private byte[] AnlysisData(byte[] response)
        {
            byte[] result;
            try
            {
                byte[] array = new byte[(response.Length - 4) / 2];
                for (int i = 0; i < array.Length; i++)
                {
                    byte[] bytes = new byte[]
                    {
                        response[i * 2 + 1],
                        response[i * 2 + 2]
                    };
                    array[i] = Convert.ToByte(Encoding.ASCII.GetString(bytes), 16);
                }
                result = array;
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        private byte[] CalculateSUM(byte[] data)
        {
            int num = 0;
            for (int i = 1; i < data.Length; i++)
            {
                num += (int)data[i];
            }
            return Encoding.ASCII.GetBytes(((byte)num).ToString("X2"));
        }

        private bool CheckSUM(byte[] response)
        {
            bool result;
            if (response.Length <= 2)
            {
                result = false;
            }
            else
            {
                byte[] b = this.CalculateSUM(ByteArrayLib.GetByteArray(response, 0, response.Length - 2));
                result = ByteArrayLib.ByteArrayEquals(b, ByteArrayLib.GetByteArray(response, response.Length - 2, 2));
            }
            return result;
        }
    }
}
