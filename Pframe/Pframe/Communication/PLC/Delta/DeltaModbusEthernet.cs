using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Pframe.Common;
using Pframe.DataConvert;

namespace Pframe.PLC.Delta
{
    /// <summary>
    /// 台达网口通信库
    /// </summary>
	public class DeltaModbusEthernet
	{
		public DataFormat DataFormat { get; set; }
        
		public int WaitTimes { get; set; }
        
		public DeltaModbusEthernet(DeltaModbusEthernetType Protocol, int WaitTimes, DataFormat dataformat = DataFormat.CDAB)
		{
			this.DataFormat = DataFormat.CDAB;
			this.waitTimes = 5;
			this.Protocol = Protocol;
			this.DataFormat = dataformat;
			if (Protocol != DeltaModbusEthernetType.DeltaTCP)
			{
				if (Protocol == DeltaModbusEthernetType.DeltaUDP)
				{
					this.DataFormat = this.DataFormat;
				}
			}
			else
			{
				this.deltaTcp = new DeltaModbusTcp();
				this.deltaTcp.WaitTimes = WaitTimes;
				this.deltaTcp.DataFormat = this.DataFormat;
			}
		}
        
		public bool Connect(string IpAddress, int Port)
		{
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			bool result;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaUDP)
				{
					result = true;
				}
				else
				{
					this.deltaUdp = new DeltaModbusUdp(IpAddress, Port, this.DataFormat);
					result = true;
				}
			}
			else
			{
				result = this.deltaTcp.Connect(IpAddress, Port);
			}
			return result;
		}
        
		public void DisConnect()
		{
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaUDP)
				{
				}
			}
			else
			{
				this.deltaTcp.DisConnect();
			}
		}
        
		public bool ReadBool(string address, ref bool value)
		{
			bool[] array = this.ReadBool(address, 1);
			if (array != null)
			{
				value = array[0];
			}
			return false;
		}
        
		public bool[] ReadBool(string address, ushort length)
		{
			CalResult<bool[]> xktResult = CalResult.CreateFailedResult<bool[]>(new CalResult());
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					xktResult = this.deltaUdp.ReadBoolArray(address, length);
				}
			}
			else
			{
				xktResult = this.deltaTcp.ReadBoolArray(address, length);
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
			if (array != null)
			{
				value = array[0];
			}
			return false;
		}
        
		public byte[] ReadBytes(string address, ushort length)
		{
			CalResult<byte[]> xktResult = CalResult.CreateFailedResult<byte[]>(new CalResult());
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					xktResult = this.deltaUdp.ReadByteArray(address, length);
				}
			}
			else
			{
				xktResult = this.deltaTcp.ReadByteArray(address, length);
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
			if (array != null)
			{
				value = array[0];
			}
			return false;
		}
        
		public short[] ReadShort(string address, ushort length)
		{
			CalResult<byte[]> xktResult = CalResult.CreateFailedResult<byte[]>(new CalResult());
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					xktResult = this.deltaUdp.ReadByteArray(address, length);
				}
			}
			else
			{
				xktResult = this.deltaTcp.ReadByteArray(address, length);
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
        
		public bool Readushort(string address, ref ushort value)
		{
			ushort[] array = this.Readushorts(address, 1);
			if (array != null)
			{
				value = array[0];
			}
			return false;
		}

		public ushort[] Readushorts(string address, ushort length)
		{
			CalResult<byte[]> xktResult = CalResult.CreateFailedResult<byte[]>(new CalResult());
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					xktResult = this.deltaUdp.ReadByteArray(address, length);
				}
			}
			else
			{
				xktResult = this.deltaTcp.ReadByteArray(address, length);
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
			if (array != null)
			{
				value = array[0];
			}
			return false;
		}
        
		public int[] ReadInt(string address, ushort length)
		{
			CalResult<byte[]> xktResult = CalResult.CreateFailedResult<byte[]>(new CalResult());
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					xktResult = this.deltaUdp.ReadByteArray(address, (ushort)(length * 2));
				}
			}
			else
			{
				xktResult = this.deltaTcp.ReadByteArray(address, (ushort)(length * 2));
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
			if (array != null)
			{
				value = array[0];
			}
			return false;
		}
        
		public uint[] ReadUInt(string address, ushort length)
		{
			CalResult<byte[]> xktResult = CalResult.CreateFailedResult<byte[]>(new CalResult());
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					xktResult = this.deltaUdp.ReadByteArray(address, (ushort)(length * 2));
				}
			}
			else
			{
				xktResult = this.deltaTcp.ReadByteArray(address, (ushort)(length * 2));
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
			if (array != null)
			{
				value = array[0];
			}
			return false;
		}
        
		public float[] ReadFloat(string address, ushort length)
		{
			CalResult<byte[]> xktResult = CalResult.CreateFailedResult<byte[]>(new CalResult());
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					xktResult = this.deltaUdp.ReadByteArray(address, (ushort)(length * 2));
				}
			}
			else
			{
				xktResult = this.deltaTcp.ReadByteArray(address, (ushort)(length * 2));
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
			if (array != null)
			{
				value = array[0];
			}
			return false;
		}
        
		public long[] ReadLong(string address, ushort length)
		{
			CalResult<byte[]> xktResult = CalResult.CreateFailedResult<byte[]>(new CalResult());
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					xktResult = this.deltaUdp.ReadByteArray(address, (ushort)(length * 4));
				}
			}
			else
			{
				xktResult = this.deltaTcp.ReadByteArray(address, (ushort)(length * 4));
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
			if (array != null)
			{
				value = array[0];
			}
			return false;
		}
        
		public ulong[] ReadULongs(string address, ushort length)
		{
			CalResult<byte[]> xktResult = CalResult.CreateFailedResult<byte[]>(new CalResult());
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					xktResult = this.deltaUdp.ReadByteArray(address, (ushort)(length * 4));
				}
			}
			else
			{
				xktResult = this.deltaTcp.ReadByteArray(address, (ushort)(length * 4));
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
			if (array != null)
			{
				value = array[0];
			}
			return false;
		}
        
		public double[] ReadDouble(string address, ushort length)
		{
			CalResult<byte[]> xktResult = CalResult.CreateFailedResult<byte[]>(new CalResult());
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					xktResult = this.deltaUdp.ReadByteArray(address, (ushort)(length * 4));
				}
			}
			else
			{
				xktResult = this.deltaTcp.ReadByteArray(address, (ushort)(length * 4));
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
			CalResult<byte[]> xktResult = CalResult.CreateFailedResult<byte[]>(new CalResult());
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					xktResult = this.deltaUdp.ReadByteArray(address, (ushort)length);
				}
			}
			else
			{
				xktResult = this.deltaTcp.ReadByteArray(address, (ushort)length);
			}
			if (xktResult.IsSuccess)
			{
				value = StringLib.GetStringFromByteArray(xktResult.Content, 0, xktResult.Content.Length * 2, Encoding.ASCII);
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
						if (this.Readushort(array[0], ref value2))
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
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					result = this.deltaUdp.WriteBoolArray(address, value).IsSuccess;
				}
			}
			else
			{
				result = this.deltaTcp.WriteBoolArray(address, value).IsSuccess;
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
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					result = this.deltaUdp.WriteByteArray(address, value).IsSuccess;
				}
			}
			else
			{
				result = this.deltaTcp.WriteByteArray(address, value).IsSuccess;
			}
			return result;
		}
        
		public bool Write(string address, short[] values)
		{
			bool result = false;
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					result = this.deltaUdp.WriteByteArray(address, ByteArrayLib.GetByteArrayFromShortArray(values, this.DataFormat)).IsSuccess;
				}
			}
			else
			{
				result = this.deltaTcp.WriteByteArray(address, ByteArrayLib.GetByteArrayFromShortArray(values, this.DataFormat)).IsSuccess;
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
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					result = this.deltaUdp.WriteByteArray(address, ByteArrayLib.GetByteArrayFromUShortArray(values, this.DataFormat)).IsSuccess;
				}
			}
			else
			{
				result = this.deltaTcp.WriteByteArray(address, ByteArrayLib.GetByteArrayFromUShortArray(values, this.DataFormat)).IsSuccess;
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
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					result = this.deltaUdp.WriteByteArray(address, ByteArrayLib.GetByteArrayFromIntArray(values, this.DataFormat)).IsSuccess;
				}
			}
			else
			{
				result = this.deltaTcp.WriteByteArray(address, ByteArrayLib.GetByteArrayFromIntArray(values, this.DataFormat)).IsSuccess;
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
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					result = this.deltaUdp.WriteByteArray(address, ByteArrayLib.GetByteArrayFromUIntArray(values, this.DataFormat)).IsSuccess;
				}
			}
			else
			{
				result = this.deltaTcp.WriteByteArray(address, ByteArrayLib.GetByteArrayFromUIntArray(values, this.DataFormat)).IsSuccess;
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
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					result = this.deltaUdp.WriteByteArray(address, ByteArrayLib.GetByteArrayFromFloatArray(values, this.DataFormat)).IsSuccess;
				}
			}
			else
			{
				result = this.deltaTcp.WriteByteArray(address, ByteArrayLib.GetByteArrayFromFloatArray(values, this.DataFormat)).IsSuccess;
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
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					result = this.deltaUdp.WriteByteArray(address, ByteArrayLib.GetByteArrayFromLongArray(values, this.DataFormat)).IsSuccess;
				}
			}
			else
			{
				result = this.deltaTcp.WriteByteArray(address, ByteArrayLib.GetByteArrayFromLongArray(values, this.DataFormat)).IsSuccess;
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
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					result = this.deltaUdp.WriteByteArray(address, ByteArrayLib.GetByteArrayFromULongArray(values, this.DataFormat)).IsSuccess;
				}
			}
			else
			{
				result = this.deltaTcp.WriteByteArray(address, ByteArrayLib.GetByteArrayFromULongArray(values, this.DataFormat)).IsSuccess;
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
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					result = this.deltaUdp.WriteByteArray(address, ByteArrayLib.GetByteArrayFromDoubleArray(values, this.DataFormat)).IsSuccess;
				}
			}
			else
			{
				result = this.deltaTcp.WriteByteArray(address, ByteArrayLib.GetByteArrayFromDoubleArray(values, this.DataFormat)).IsSuccess;
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
			DeltaModbusEthernetType deltaModbusEthernetType = this.Protocol;
			DeltaModbusEthernetType deltaModbusEthernetType2 = deltaModbusEthernetType;
			if (deltaModbusEthernetType2 != DeltaModbusEthernetType.DeltaTCP)
			{
				if (deltaModbusEthernetType2 == DeltaModbusEthernetType.DeltaUDP)
				{
					result = this.deltaUdp.WriteByteArray(address, ByteArrayLib.GetByteArrayFromString(value, encoding)).IsSuccess;
				}
			}
			else
			{
				result = this.deltaTcp.WriteByteArray(address, ByteArrayLib.GetByteArrayFromString(value, encoding)).IsSuccess;
			}
			return result;
		}
        
		public bool Write(string address, string value)
		{
			return this.Write(address, value, Encoding.ASCII);
		}
        
		private DeltaModbusEthernetType Protocol;
        
		private DeltaModbusTcp deltaTcp;
        
		private DeltaModbusUdp deltaUdp;
              
		public int waitTimes;
        
	}
}
