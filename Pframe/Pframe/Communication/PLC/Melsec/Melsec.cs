using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Pframe.Common;
using Pframe.DataConvert;

namespace Pframe.PLC.Melsec
{
    /// <summary>
    /// 三菱MC协议通信库
    /// </summary>
	public class Melsec
	{
		public DataFormat DataFormat { get; set; }
        
		public int ConnectTimeOut
		{
			set
			{
				switch (this.Protocol)
				{
				case MelsecProtocolType.MCBinary:
					this.mcbinary.ConnectTimeOut = value;
					break;
				case MelsecProtocolType.MCASCII:
					this.mcascii.ConnectTimeOut = value;
					break;
				case MelsecProtocolType.MCA1E:
					this.a1ENet.ConnectTimeOut = value;
					break;
				}
			}
		}
        
		public Melsec(MelsecProtocolType Protocol, bool IsFX5U = false, DataFormat dataformat = DataFormat.DCBA)
		{
			this.Protocol = Protocol;
			this.DataFormat = dataformat;
			switch (Protocol)
			{
			case MelsecProtocolType.MCBinary:
				this.mcbinary = new MelsecMCBinary(dataformat);
				this.mcbinary.IsFx5U = IsFX5U;
				break;
			case MelsecProtocolType.MCASCII:
				this.mcascii = new MelsecMCAscii(dataformat);
				this.mcascii.IsFx5U = IsFX5U;
				break;
			case MelsecProtocolType.MCA1E:
				this.a1ENet = new MelsecMCA1E(dataformat);
				break;
			}
		}
        
		public bool Connect(string Ip, int Port)
		{
			bool result;
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				result = this.mcbinary.Connect(Ip, Port);
				break;
			case MelsecProtocolType.MCASCII:
				result = this.mcascii.Connect(Ip, Port);
				break;
			case MelsecProtocolType.MCA1E:
				result = this.a1ENet.Connect(Ip, Port);
				break;
			default:
				result = false;
				break;
			}
			return result;
		}
        
		public void DisConnect()
		{
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				this.mcbinary.DisConnect();
				break;
			case MelsecProtocolType.MCASCII:
				this.mcascii.DisConnect();
				break;
			case MelsecProtocolType.MCA1E:
				this.a1ENet.DisConnect();
				break;
			}
		}
        
		public bool ReadBool(string address, ref bool value)
		{
            if (address.Contains("."))
            {
                string[] _adress = address.Split('.');
                byte[] result1 = this.ReadBytes(_adress[0], 1);
                if (result1 == null)
                    return false;
                value = BitLib.GetBitFrom2ByteArray(result1, 0, Convert.ToInt16(_adress[1]));
                return true;
            }
            else
            {
                bool[] array = this.ReadBool(address, 1);
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
		}
        
		public bool[] ReadBool(string address, ushort length)
		{
			CalResult<bool[]> xktResult = CalResult.CreateFailedResult<bool[]>(new CalResult());
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				xktResult = this.mcbinary.ReadBoolArray(address, length);
				break;
			case MelsecProtocolType.MCASCII:
				xktResult = this.mcascii.ReadBoolArray(address, length);
				break;
			case MelsecProtocolType.MCA1E:
				xktResult = this.a1ENet.ReadBoolArray(address, length);
				break;
			}
			bool[] result;
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
			{
				result = false;
			}
			return result;
		}
        
		public byte[] ReadBytes(string address, ushort length)
		{
			CalResult<byte[]> xktResult = CalResult.CreateFailedResult<byte[]>(new CalResult());
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				xktResult = this.mcbinary.ReadByteArray(address, length);
				break;
			case MelsecProtocolType.MCASCII:
				xktResult = this.mcascii.ReadByteArray(address, length);
				break;
			case MelsecProtocolType.MCA1E:
				xktResult = this.a1ENet.ReadByteArray(address, length);
				break;
			}
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
        
		public short[] ReadShort(string address, ushort length)
		{
			CalResult<byte[]> xktResult = CalResult.CreateFailedResult<byte[]>(new CalResult());
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				xktResult = this.mcbinary.ReadByteArray(address, length);
				break;
			case MelsecProtocolType.MCASCII:
				xktResult = this.mcascii.ReadByteArray(address, length);
				break;
			case MelsecProtocolType.MCA1E:
				xktResult = this.a1ENet.ReadByteArray(address, length);
				break;
			}
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
        
		public bool ReadUShort(string address, ref ushort value)
		{
			ushort[] array = this.ReadUShorts(address, 1);
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
        
		public ushort[] ReadUShorts(string address, ushort length)
		{
			CalResult<byte[]> xktResult = CalResult.CreateFailedResult<byte[]>(new CalResult());
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				xktResult = this.mcbinary.ReadByteArray(address, length);
				break;
			case MelsecProtocolType.MCASCII:
				xktResult = this.mcascii.ReadByteArray(address, length);
				break;
			case MelsecProtocolType.MCA1E:
				xktResult = this.a1ENet.ReadByteArray(address, length);
				break;
			}
			ushort[] result;
			if (xktResult.IsSuccess)
			{
				result = UShortLib.GetUShortArrayFromByteArray(xktResult.Content, this.DataFormat);
			}
			else
			{
				result = null;
			}
			return result;
		}
        
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
        
		public int[] ReadInt(string address, ushort length)
		{
			CalResult<byte[]> xktResult = CalResult.CreateFailedResult<byte[]>(new CalResult());
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				xktResult = this.mcbinary.ReadByteArray(address, (ushort)(length * 2));
				break;
			case MelsecProtocolType.MCASCII:
				xktResult = this.mcascii.ReadByteArray(address, (ushort)(length * 2));
				break;
			case MelsecProtocolType.MCA1E:
				xktResult = this.a1ENet.ReadByteArray(address, (ushort)(length * 2));
				break;
			}
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
        
		public uint[] ReadUInt(string address, ushort length)
		{
			CalResult<byte[]> xktResult = CalResult.CreateFailedResult<byte[]>(new CalResult());
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				xktResult = this.mcbinary.ReadByteArray(address, (ushort)(length * 2));
				break;
			case MelsecProtocolType.MCASCII:
				xktResult = this.mcascii.ReadByteArray(address, (ushort)(length * 2));
				break;
			case MelsecProtocolType.MCA1E:
				xktResult = this.a1ENet.ReadByteArray(address, (ushort)(length * 2));
				break;
			}
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
        
		public float[] ReadFloat(string address, ushort length)
		{
			CalResult<byte[]> xktResult = CalResult.CreateFailedResult<byte[]>(new CalResult());
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				xktResult = this.mcbinary.ReadByteArray(address, (ushort)(length * 2));
				break;
			case MelsecProtocolType.MCASCII:
				xktResult = this.mcascii.ReadByteArray(address, (ushort)(length * 2));
				break;
			case MelsecProtocolType.MCA1E:
				xktResult = this.a1ENet.ReadByteArray(address, (ushort)(length * 2));
				break;
			}
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
        
		public long[] ReadLong(string address, ushort length)
		{
			CalResult<byte[]> xktResult = CalResult.CreateFailedResult<byte[]>(new CalResult());
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				xktResult = this.mcbinary.ReadByteArray(address, (ushort)(length * 4));
				break;
			case MelsecProtocolType.MCASCII:
				xktResult = this.mcascii.ReadByteArray(address, (ushort)(length * 4));
				break;
			case MelsecProtocolType.MCA1E:
				xktResult = this.a1ENet.ReadByteArray(address, (ushort)(length * 4));
				break;
			}
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
        
		public bool ReadULong(string address, ref ulong value)
		{
			ulong[] array = this.ReadULongs(address, 1);
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
        
		public ulong[] ReadULongs(string address, ushort length)
		{
			CalResult<byte[]> xktResult = CalResult.CreateFailedResult<byte[]>(new CalResult());
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				xktResult = this.mcbinary.ReadByteArray(address, (ushort)(length * 4));
				break;
			case MelsecProtocolType.MCASCII:
				xktResult = this.mcascii.ReadByteArray(address, (ushort)(length * 4));
				break;
			case MelsecProtocolType.MCA1E:
				xktResult = this.a1ENet.ReadByteArray(address, (ushort)(length * 4));
				break;
			}
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
        
		public double[] ReadDouble(string address, ushort length)
		{
			CalResult<byte[]> xktResult = CalResult.CreateFailedResult<byte[]>(new CalResult());
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				xktResult = this.mcbinary.ReadByteArray(address, (ushort)(length * 4));
				break;
			case MelsecProtocolType.MCASCII:
				xktResult = this.mcascii.ReadByteArray(address, (ushort)(length * 4));
				break;
			case MelsecProtocolType.MCA1E:
				xktResult = this.a1ENet.ReadByteArray(address, (ushort)(length * 4));
				break;
			}
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
        
		public bool ReadString(string address, int length, ref string value)
		{
			CalResult<byte[]> xktResult = null;
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				xktResult = this.mcbinary.ReadByteArray(address, (ushort)length);
				break;
			case MelsecProtocolType.MCASCII:
				xktResult = this.mcascii.ReadByteArray(address, (ushort)length);
				break;
			case MelsecProtocolType.MCA1E:
				xktResult = this.a1ENet.ReadByteArray(address, (ushort)length);
				break;
			}
			if (xktResult.IsSuccess)
			{
				value = StringLib.GetStringFromByteArray(xktResult.Content, 0, xktResult.Content.Length, Encoding.ASCII);
			}
			return xktResult.IsSuccess;
		}
        
		public CalResult Write(string address, object value, DataType dataType)
		{
			CalResult xktResult = new CalResult();
			try
			{
				switch (dataType)
				{
				case DataType.Bool:
					if (address.Contains('.'))
						xktResult.IsSuccess = this.Write(address, CommonMethods.IsBoolean(value.ToString()), true);
					else
						xktResult.IsSuccess = this.Write(address, CommonMethods.IsBoolean(value.ToString()));
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
			bool result = false;
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				result = this.mcbinary.WriteBoolArray(address, value).IsSuccess;
				break;
			case MelsecProtocolType.MCASCII:
				result = this.mcascii.WriteBoolArray(address, value).IsSuccess;
				break;
			case MelsecProtocolType.MCA1E:
				result = this.a1ENet.WriteBoolArray(address, value).IsSuccess;
				break;
			}
			return result;
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
			bool result = false;
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				result = this.mcbinary.WriteByteArray(address, value).IsSuccess;
				break;
			case MelsecProtocolType.MCASCII:
				result = this.mcascii.WriteByteArray(address, value).IsSuccess;
				break;
			case MelsecProtocolType.MCA1E:
				result = this.a1ENet.WriteByteArray(address, value).IsSuccess;
				break;
			}
			return result;
		}
        
		public bool Write(string address, short[] values)
		{
			bool result = false;
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				result = this.mcbinary.WriteByteArray(address, ByteArrayLib.GetByteArrayFromShortArray(values, this.DataFormat)).IsSuccess;
				break;
			case MelsecProtocolType.MCASCII:
				result = this.mcascii.WriteByteArray(address, ByteArrayLib.GetByteArrayFromShortArray(values, this.DataFormat)).IsSuccess;
				break;
			case MelsecProtocolType.MCA1E:
				result = this.a1ENet.WriteByteArray(address, ByteArrayLib.GetByteArrayFromShortArray(values, this.DataFormat)).IsSuccess;
				break;
			}
			return result;
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
			bool result = false;
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				result = this.mcbinary.WriteByteArray(address, ByteArrayLib.GetByteArrayFromUShortArray(values, this.DataFormat)).IsSuccess;
				break;
			case MelsecProtocolType.MCASCII:
				result = this.mcascii.WriteByteArray(address, ByteArrayLib.GetByteArrayFromUShortArray(values, this.DataFormat)).IsSuccess;
				break;
			case MelsecProtocolType.MCA1E:
				result = this.a1ENet.WriteByteArray(address, ByteArrayLib.GetByteArrayFromUShortArray(values, this.DataFormat)).IsSuccess;
				break;
			}
			return result;
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
			bool result = false;
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				result = this.mcbinary.WriteByteArray(address, ByteArrayLib.GetByteArrayFromIntArray(values, this.DataFormat)).IsSuccess;
				break;
			case MelsecProtocolType.MCASCII:
				result = this.mcascii.WriteByteArray(address, ByteArrayLib.GetByteArrayFromIntArray(values, this.DataFormat)).IsSuccess;
				break;
			case MelsecProtocolType.MCA1E:
				result = this.a1ENet.WriteByteArray(address, ByteArrayLib.GetByteArrayFromIntArray(values, this.DataFormat)).IsSuccess;
				break;
			}
			return result;
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
			bool result = false;
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				result = this.mcbinary.WriteByteArray(address, ByteArrayLib.GetByteArrayFromUIntArray(values, this.DataFormat)).IsSuccess;
				break;
			case MelsecProtocolType.MCASCII:
				result = this.mcascii.WriteByteArray(address, ByteArrayLib.GetByteArrayFromUIntArray(values, this.DataFormat)).IsSuccess;
				break;
			case MelsecProtocolType.MCA1E:
				result = this.a1ENet.WriteByteArray(address, ByteArrayLib.GetByteArrayFromUIntArray(values, this.DataFormat)).IsSuccess;
				break;
			}
			return result;
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
			bool result = false;
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				result = this.mcbinary.WriteByteArray(address, ByteArrayLib.GetByteArrayFromFloatArray(values, this.DataFormat)).IsSuccess;
				break;
			case MelsecProtocolType.MCASCII:
				result = this.mcascii.WriteByteArray(address, ByteArrayLib.GetByteArrayFromFloatArray(values, this.DataFormat)).IsSuccess;
				break;
			case MelsecProtocolType.MCA1E:
				result = this.a1ENet.WriteByteArray(address, ByteArrayLib.GetByteArrayFromFloatArray(values, this.DataFormat)).IsSuccess;
				break;
			}
			return result;
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
			bool result = false;
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				result = this.mcbinary.WriteByteArray(address, ByteArrayLib.GetByteArrayFromLongArray(values, this.DataFormat)).IsSuccess;
				break;
			case MelsecProtocolType.MCASCII:
				result = this.mcascii.WriteByteArray(address, ByteArrayLib.GetByteArrayFromLongArray(values, this.DataFormat)).IsSuccess;
				break;
			case MelsecProtocolType.MCA1E:
				result = this.a1ENet.WriteByteArray(address, ByteArrayLib.GetByteArrayFromLongArray(values, this.DataFormat)).IsSuccess;
				break;
			}
			return result;
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
			bool result = false;
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				result = this.mcbinary.WriteByteArray(address, ByteArrayLib.GetByteArrayFromULongArray(values, this.DataFormat)).IsSuccess;
				break;
			case MelsecProtocolType.MCASCII:
				result = this.mcascii.WriteByteArray(address, ByteArrayLib.GetByteArrayFromULongArray(values, this.DataFormat)).IsSuccess;
				break;
			case MelsecProtocolType.MCA1E:
				result = this.a1ENet.WriteByteArray(address, ByteArrayLib.GetByteArrayFromULongArray(values, this.DataFormat)).IsSuccess;
				break;
			}
			return result;
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
			bool result = false;
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				result = this.mcbinary.WriteByteArray(address, ByteArrayLib.GetByteArrayFromDoubleArray(values, this.DataFormat)).IsSuccess;
				break;
			case MelsecProtocolType.MCASCII:
				result = this.mcascii.WriteByteArray(address, ByteArrayLib.GetByteArrayFromDoubleArray(values, this.DataFormat)).IsSuccess;
				break;
			case MelsecProtocolType.MCA1E:
				result = this.a1ENet.WriteByteArray(address, ByteArrayLib.GetByteArrayFromDoubleArray(values, this.DataFormat)).IsSuccess;
				break;
			}
			return result;
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
			bool result = false;
			switch (this.Protocol)
			{
			case MelsecProtocolType.MCBinary:
				result = this.mcbinary.WriteByteArray(address, ByteArrayLib.GetByteArrayFromString(value, encoding)).IsSuccess;
				break;
			case MelsecProtocolType.MCASCII:
				result = this.mcascii.WriteByteArray(address, ByteArrayLib.GetByteArrayFromString(value, encoding)).IsSuccess;
				break;
			case MelsecProtocolType.MCA1E:
				result = this.a1ENet.WriteByteArray(address, ByteArrayLib.GetByteArrayFromString(value, encoding)).IsSuccess;
				break;
			}
			return result;
		}
        
		public bool Write(string address, string value)
		{
			return this.Write(address, value, Encoding.ASCII);
		}
        
		private MelsecProtocolType Protocol;
        
		private MelsecMCAscii mcascii;
        
		private MelsecMCBinary mcbinary;
        
		private MelsecMCA1E a1ENet;
        
	}
}
