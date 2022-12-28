using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Pframe.Common;
using Pframe.DataConvert;
using Pframe.Tools;

namespace Pframe.Base
{
	public class SerialDeviceBase
	{
		public int ReadTimeOut { get; set; }

		public int WriteTimeOut { get; set; }

		public bool DtrEnable
		{
			get
			{
				return this.dtrEnable;
			}
			set
			{
				this.MyCom.DtrEnable = value;
				this.dtrEnable = value;
			}
		}

		public bool RtsEnable
		{
			get
			{
				return this.rtsEnable;
			}
			set
			{
				this.MyCom.RtsEnable = value;
				this.rtsEnable = value;
			}
		}

		public int SleepTime { get; set; }

		public int ReceiveTimeOut { get; set; }

		public DataFormat DataFormat { get; set; }

		public bool Connect(string iPortName, int iBaudRate = 9600, int iDataBits = 8, Parity iParity = Parity.None, StopBits iStopBits = StopBits.One)
		{
			this.MyCom = new SerialPort(iPortName, iBaudRate, iParity, iDataBits, iStopBits);
			if (this.MyCom.IsOpen)
				this.MyCom.Close();
			this.MyCom.ReadTimeout = this.ReadTimeOut;
			this.MyCom.WriteTimeout = this.WriteTimeOut;
			try
			{
				this.MyCom.Open();
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		public void DisConnect()
		{
			if (this.MyCom.IsOpen)
			{
				this.MyCom.Close();
			}
		}

		public bool SendAndReceive(byte[] sendByte, ref byte[] response, int count = 0)
		{
			bool result;

				this.InteractiveLock.Enter();
				try
				{
					this.MyCom.DiscardInBuffer();
					this.MyCom.Write(sendByte, 0, sendByte.Length);
					byte[] array = new byte[1024];
					MemoryStream memoryStream = new MemoryStream();
					if (count == 0)
					{
						DateTime now = DateTime.Now;
						while (true)
						{
							Thread.Sleep(this.SleepTime);
							if (this.MyCom.BytesToRead >= 1)
							{
								int count2 = this.MyCom.Read(array, 0, array.Length);
								memoryStream.Write(array, 0, count2);
							}
							else
							{
								if ((DateTime.Now - now).TotalMilliseconds > (double)this.ReceiveTimeOut)
								{
									goto IL_CA;
								}
								if (memoryStream.Length > 0L)
								{
									break;
								}
							}
						}
						goto IL_E7;
						IL_CA:
						memoryStream.Dispose();
						return false;
					}
					array = this.method_0(count);
					memoryStream.Write(array, 0, array.Length);
					IL_E7:
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

		private byte[] method_0(int int_4)
		{
			byte[] array = new byte[int_4];
			for (int num = 0; num != int_4; num += this.MyCom.Read(array, num, int_4 - num))
			{
			}
			return array;
		}

		public virtual CalResult<byte[]> ReadByteArray(string address, ushort length)
		{
			return CalResult.CreateFailedResult<byte[]>(new CalResult());
		}

		public virtual CalResult<bool[]> ReadBoolArray(string address, ushort length)
		{
			return CalResult.CreateFailedResult<bool[]>(new CalResult());
		}

		public virtual CalResult WriteByteArray(string address, byte[] value)
		{
			return CalResult.CreateFailedResult();
		}

		public virtual CalResult WriteBoolArray(string address, bool[] value)
		{
			return CalResult.CreateFailedResult();
		}

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

		public bool[] ReadBool(string address, ushort length)
		{
			CalResult<bool[]> xktResult = this.ReadBoolArray(address, length);
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

		public bool method_1(string address, ref ushort value)
		{
			ushort[] array = this.method_2(address, 1);
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

		public ushort[] method_2(string address, ushort length)
		{
			CalResult<byte[]> xktResult = this.ReadByteArray(address, length);
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

		public bool method_3(string address, ref ulong value)
		{
			ulong[] array = this.method_4(address, 1);
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

		public ulong[] method_4(string address, ushort length)
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

		public bool ReadString(string address, int length, ref string value)
		{
			CalResult<byte[]> xktResult = this.ReadByteArray(address, (ushort)length);
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
						if (this.method_1(array[0], ref value2))
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

		public SerialDeviceBase()
		{
			this.ReadTimeOut = 2000;
			this.WriteTimeOut = 2000;
			this.SleepTime = 40;
			this.ReceiveTimeOut = 2000;
			this.InteractiveLock = new SimpleHybirdLock();
			this.DataFormat = DataFormat.CDAB;
		}

		public SerialPort MyCom;

		private bool dtrEnable;

		private bool rtsEnable;

		public SimpleHybirdLock InteractiveLock;

	}
}
