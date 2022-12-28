using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using Pframe.Common;

namespace Pframe.PLC.Delta
{
    /// <summary>
    /// 台达串口通信库
    /// </summary>
	public class DeltaModbusSerial
	{
		public DataFormat DataFormat { get; set; }
        
		public int SleepTime
		{
			set
			{
				DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
				DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
				if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
				{
					if (deltaModbusSerialType2 == DeltaModbusSerialType.AscII)
					{
						if (this.deltaAscii != null)
						{
							this.deltaAscii.SleepTime = value;
						}
					}
				}
				else if (this.deltaRtu != null)
				{
					this.deltaRtu.SleepTime = value;
				}
			}
		}
        
		public DeltaModbusSerial(DeltaModbusSerialType Protocol, int SleepTime, DataFormat dataformat = DataFormat.CDAB)
		{
			this.DataFormat = DataFormat.CDAB;
			this.Protocol = Protocol;
			this.DataFormat = dataformat;
			if (Protocol != DeltaModbusSerialType.DeltaRTU)
			{
				if (Protocol == DeltaModbusSerialType.AscII)
				{
					this.deltaAscii = new DeltaModbusAscii();
					this.deltaAscii.SleepTime = SleepTime;
					this.deltaAscii.DataFormat = this.DataFormat;
				}
			}
			else
			{
				this.deltaRtu = new DeltaModbusRtu();
				this.deltaRtu.SleepTime = SleepTime;
				this.deltaRtu.DataFormat = this.DataFormat;
			}
		}
        
		public bool Connect(string iPortName, int iBaudRate, int iDataBits, Parity iParity, StopBits iStopBits)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			bool result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				result = (deltaModbusSerialType2 == DeltaModbusSerialType.AscII && this.deltaAscii.Connect(iPortName, iBaudRate, iDataBits, iParity, iStopBits));
			}
			else
			{
				result = this.deltaRtu.Connect(iPortName, iBaudRate, iDataBits, iParity, iStopBits);
			}
			return result;
		}
        
		public void DisConnect()
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 == DeltaModbusSerialType.AscII)
				{
					this.deltaAscii.DisConnect();
				}
			}
			else
			{
				this.deltaRtu.DisConnect();
			}
		}
        
		public bool Readbool(string address, ref bool value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			bool result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				result = (deltaModbusSerialType2 == DeltaModbusSerialType.AscII && this.deltaAscii.Readbool(address, ref value, SlaveID));
			}
			else
			{
				result = this.deltaRtu.Readbool(address, ref value, SlaveID);
			}
			return result;
		}
        
		public CalResult<byte[]> ReadBytes(string address, ushort length, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult<byte[]> result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult<byte[]>(new CalResult());
				}
				else
				{
					result = this.deltaAscii.ReadBytes(address, length, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.ReadBytes(address, length, SlaveID);
			}
			return result;
		}
        
		public bool Readshort(string address, ref short value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			bool result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				result = (deltaModbusSerialType2 == DeltaModbusSerialType.AscII && this.deltaAscii.Readshort(address, ref value, SlaveID));
			}
			else
			{
				result = this.deltaRtu.Readshort(address, ref value, SlaveID);
			}
			return result;
		}
        
		public bool Readushort(string address, ref ushort value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			bool result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				result = (deltaModbusSerialType2 == DeltaModbusSerialType.AscII && this.deltaAscii.Readushort(address, ref value, SlaveID));
			}
			else
			{
				result = this.deltaRtu.Readushort(address, ref value, SlaveID);
			}
			return result;
		}
        
		public bool Readint(string address, ref int value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			bool result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				result = (deltaModbusSerialType2 == DeltaModbusSerialType.AscII && this.deltaAscii.Readint(address, ref value, SlaveID));
			}
			else
			{
				result = this.deltaRtu.Readint(address, ref value, SlaveID);
			}
			return result;
		}
        
		public bool Readuint(string address, ref uint value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			bool result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				result = (deltaModbusSerialType2 == DeltaModbusSerialType.AscII && this.deltaAscii.Readuint(address, ref value, SlaveID));
			}
			else
			{
				result = this.deltaRtu.Readuint(address, ref value, SlaveID);
			}
			return result;
		}
        
		public bool Readfloat(string address, ref float value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			bool result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				result = (deltaModbusSerialType2 == DeltaModbusSerialType.AscII && this.deltaAscii.Readfloat(address, ref value, SlaveID));
			}
			else
			{
				result = this.deltaRtu.Readfloat(address, ref value, SlaveID);
			}
			return result;
		}
        
		public bool Readlong(string address, ref long value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			bool result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				result = (deltaModbusSerialType2 == DeltaModbusSerialType.AscII && this.deltaAscii.Readlong(address, ref value, SlaveID));
			}
			else
			{
				result = this.deltaRtu.Readlong(address, ref value, SlaveID);
			}
			return result;
		}
        
		public bool Readulong(string address, ref ulong value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			bool result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				result = (deltaModbusSerialType2 == DeltaModbusSerialType.AscII && this.deltaAscii.Readulong(address, ref value, SlaveID));
			}
			else
			{
				result = this.deltaRtu.Readulong(address, ref value, SlaveID);
			}
			return result;
		}
        
		public bool Readdouble(string address, ref double value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			bool result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				result = (deltaModbusSerialType2 == DeltaModbusSerialType.AscII && this.deltaAscii.Readdouble(address, ref value, SlaveID));
			}
			else
			{
				result = this.deltaRtu.Readdouble(address, ref value, SlaveID);
			}
			return result;
		}
        
		public bool Readstring(string address, ushort length, ref string value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			bool result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				result = (deltaModbusSerialType2 == DeltaModbusSerialType.AscII && this.deltaAscii.Readstring(address, length, ref value, SlaveID));
			}
			else
			{
				result = this.deltaRtu.Readstring(address, length, ref value, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, string value, DataType vartype, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, value, vartype, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, value, vartype, SlaveID);
			}
			return result;
		}
        
		public CalResult WriteBoolReg(string address, bool value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.WriteBoolReg(address, value, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.WriteBoolReg(address, value, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, byte[] value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, value, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, value, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, bool value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, value, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, value, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, bool[] values, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, values, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, values, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, short[] values, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, values, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, values, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, short value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, value, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, value, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, ushort[] values, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, values, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, values, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, ushort value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, value, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, value, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, int[] values, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, values, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, values, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, int value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, value, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, value, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, uint[] values, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, values, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, values, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, uint value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, value, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, value, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, float[] values, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, values, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, values, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, float value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, value, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, value, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, long[] values, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, values, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, values, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, long value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, value, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, value, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, ulong[] values, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, values, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, values, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, ulong value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, value, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, value, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, double[] values, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, values, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, values, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, double value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, value, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, value, SlaveID);
			}
			return result;
		}
        
		public CalResult Write(string address, string value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, value, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, value, SlaveID);
			}
			return result;
		}
        
		public CalResult WriteUnicodeString(string address, string value, byte SlaveID = 1)
		{
			DeltaModbusSerialType deltaModbusSerialType = this.Protocol;
			DeltaModbusSerialType deltaModbusSerialType2 = deltaModbusSerialType;
			CalResult result;
			if (deltaModbusSerialType2 != DeltaModbusSerialType.DeltaRTU)
			{
				if (deltaModbusSerialType2 != DeltaModbusSerialType.AscII)
				{
					result = CalResult.CreateFailedResult();
				}
				else
				{
					result = this.deltaAscii.Write(address, value, SlaveID);
				}
			}
			else
			{
				result = this.deltaRtu.Write(address, value, SlaveID);
			}
			return result;
		}
        
		private DeltaModbusSerialType Protocol;
        
		private DeltaModbusRtu deltaRtu;
        
		private DeltaModbusAscii deltaAscii;
        
	}
}
