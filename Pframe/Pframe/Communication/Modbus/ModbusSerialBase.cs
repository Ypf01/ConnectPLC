using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Pframe.Base;
using Pframe.Common;
using Pframe.DataConvert;
using Pframe.Modbus.Interface;

namespace Pframe.Modbus
{
	public class ModbusSerialBase : SerialDeviceBase, IModbusMaster
	{
		public byte SlaveAddress { get; set; }
        
		public virtual byte[] ReadOutputStatus(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
			return null;
		}
		public virtual byte[] ReadInputStatus(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
			return null;
		}      
		public virtual byte[] ReadKeepReg(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
			return null;
		}      
		public virtual byte[] ReadInputReg(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
			return null;
		}
        
		public virtual bool ForceCoil(byte slaveAddress, ushort coilAddress, bool value)
		{
			return false;
		}
        
		public virtual bool PreSetSingleReg(byte slaveAddress, ushort registerAddress, byte[] value)
		{
			return false;
		}
        
		public virtual bool ForceMultiCoil(byte slaveAddress, ushort startAddress, bool[] data)
		{
			return false;
		}
		public virtual bool PreSetMultiReg(byte slaveAddress, ushort startAddress, byte[] data)
		{
			return false;
		}
        
		public bool PreSetBoolReg(byte iDevAdd, string iAddress, bool SetValue)
		{
			bool result;
			if (!iAddress.Contains('.'))
			{
				result = false;
			}
			else
			{
				string[] array = iAddress.Split(new char[]
				{
					'.'
				});
				if (array.Length != 2)
				{
					result = false;
				}
				else
				{
					ushort startAddress = ushort.Parse(array[0]);
					int num = int.Parse(array[1]);
					if (num >= 16 || num < 0)
					{
						result = false;
					}
					else
					{
						byte[] array2 = this.ReadKeepReg(iDevAdd, startAddress, 1);
						result = (array2 != null && array2.Length == 2 && this.PreSetSingleReg(iDevAdd, startAddress, ShortLib.SetbitValueFrom2ByteArray(array2, num, SetValue, base.DataFormat)));
					}
				}
			}
			return result;
		}
        
		public bool PreSetSingleReg(byte slaveAddress, ushort startAddress, short value)
		{
			return this.PreSetSingleReg(slaveAddress, startAddress, ByteArrayLib.GetByteArrayFromShort(value, DataFormat.ABCD));
		}
        
		public bool PreSetSingleReg(byte slaveAddress, ushort startAddress, ushort value)
		{
			return this.PreSetSingleReg(slaveAddress, startAddress, ByteArrayLib.GetByteArrayFromUShort(value, DataFormat.ABCD));
		}
        
		public bool PreSetMultiReg(byte slaveAddress, ushort startAddress, short value)
		{
			return this.PreSetMultiReg(slaveAddress, startAddress, ByteArrayLib.GetByteArrayFromShort(value, DataFormat.ABCD));
		}
        
		public bool PreSetMultiReg(byte slaveAddress, ushort startAddress, ushort value)
		{
			return this.PreSetMultiReg(slaveAddress, startAddress, ByteArrayLib.GetByteArrayFromUShort(value, DataFormat.ABCD));
		}
        
		public bool PreSetMultiReg(byte slaveAddress, ushort startAddress, float value)
		{
			return this.PreSetMultiReg(slaveAddress, startAddress, ByteArrayLib.GetByteArrayFromFloat(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(byte slaveAddress, ushort startAddress, int value)
		{
			return this.PreSetMultiReg(slaveAddress, startAddress, ByteArrayLib.GetByteArrayFromInt(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(byte slaveAddress, ushort startAddress, uint value)
		{
			return this.PreSetMultiReg(slaveAddress, startAddress, ByteArrayLib.GetByteArrayFromUInt(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(byte slaveAddress, ushort startAddress, float[] value)
		{
			return this.PreSetMultiReg(slaveAddress, startAddress, ByteArrayLib.GetByteArrayFromFloatArray(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(byte slaveAddress, ushort startAddress, int[] value)
		{
			return this.PreSetMultiReg(slaveAddress, startAddress, ByteArrayLib.GetByteArrayFromIntArray(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(byte slaveAddress, ushort startAddress, uint[] value)
		{
			return this.PreSetMultiReg(slaveAddress, startAddress, ByteArrayLib.GetByteArrayFromUIntArray(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(byte slaveAddress, ushort startAddress, short[] value)
		{
			return this.PreSetMultiReg(slaveAddress, startAddress, ByteArrayLib.GetByteArrayFromShortArray(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(byte slaveAddress, ushort startAddress, ushort[] value)
		{
			return this.PreSetMultiReg(slaveAddress, startAddress, ByteArrayLib.GetByteArrayFromUShortArray(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(byte slaveAddress, ushort startAddress, long value)
		{
			return this.PreSetMultiReg(slaveAddress, startAddress, ByteArrayLib.GetByteArrayFromLong(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(byte slaveAddress, ushort startAddress, ulong value)
		{
			return this.PreSetMultiReg(slaveAddress, startAddress, ByteArrayLib.GetByteArrayFromULong(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(byte slaveAddress, ushort startAddress, double value)
		{
			return this.PreSetMultiReg(slaveAddress, startAddress, ByteArrayLib.GetByteArrayFromDouble(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(byte slaveAddress, ushort startAddress, long[] value)
		{
			return this.PreSetMultiReg(slaveAddress, startAddress, ByteArrayLib.GetByteArrayFromLongArray(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(byte slaveAddress, ushort startAddress, ulong[] value)
		{
			return this.PreSetMultiReg(slaveAddress, startAddress, ByteArrayLib.GetByteArrayFromULongArray(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(byte slaveAddress, ushort startAddress, double[] value)
		{
			return this.PreSetMultiReg(slaveAddress, startAddress, ByteArrayLib.GetByteArrayFromDoubleArray(value, base.DataFormat));
		}
        
		public byte[] Read(ModbusArea area, byte devId, ushort startAddress, ushort length)
		{
			byte[] result;
			switch (area)
			{
			case ModbusArea.保持寄存器:
				result = this.ReadKeepReg(devId, startAddress, length);
				break;
			case ModbusArea.输入寄存器:
				result = this.ReadInputReg(devId, startAddress, length);
				break;
			case ModbusArea.输出线圈:
				result = this.ReadOutputStatus(devId, startAddress, length);
				break;
			case ModbusArea.输入线圈:
				result = this.ReadInputStatus(devId, startAddress, length);
				break;
			default:
				result = null;
				break;
			}
			return result;
		}
        
		public byte[] Read(ModbusArea area, ushort startAddress, ushort length)
		{
			return this.Read(area, this.SlaveAddress, startAddress, length);
		}
        
		public CalResult Write(byte devId, string address, string value, DataType dataType)
		{
			CalResult xktResult = new CalResult();
			try
			{
				switch (dataType)
				{
				case DataType.Bool:
				{
					ushort coilAddress;
					if (ushort.TryParse(address, out coilAddress))
					{
						xktResult.IsSuccess = this.ForceCoil(devId, coilAddress, value == "1" || value.ToLower() == "true");
					}
					else
					{
						xktResult.IsSuccess = this.PreSetBoolReg(devId, address, value == "1" || value.ToLower() == "true");
					}
					break;
				}
				case DataType.Byte:
					break;
				case DataType.Short:
					xktResult.IsSuccess = this.PreSetSingleReg(devId, ushort.Parse(address), Convert.ToInt16(value));
					break;
				case DataType.UShort:
					xktResult.IsSuccess = this.PreSetSingleReg(devId, ushort.Parse(address), Convert.ToUInt16(value));
					break;
				case DataType.Int:
					xktResult.IsSuccess = this.PreSetMultiReg(devId, ushort.Parse(address), Convert.ToInt32(value));
					break;
				case DataType.UInt:
					xktResult.IsSuccess = this.PreSetMultiReg(devId, ushort.Parse(address), Convert.ToUInt32(value));
					break;
				case DataType.Float:
					xktResult.IsSuccess = this.PreSetMultiReg(devId, ushort.Parse(address), Convert.ToSingle(value));
					break;
				case DataType.Double:
					xktResult.IsSuccess = this.PreSetMultiReg(devId, ushort.Parse(address), Convert.ToDouble(value));
					break;
				case DataType.Long:
					xktResult.IsSuccess = this.PreSetMultiReg(devId, ushort.Parse(address), Convert.ToInt64(value));
					break;
				case DataType.ULong:
					xktResult.IsSuccess = this.PreSetMultiReg(devId, ushort.Parse(address), Convert.ToUInt64(value));
					break;
				case DataType.String:
					xktResult.IsSuccess = this.PreSetMultiReg(devId, ushort.Parse(address.Substring(0, address.IndexOf('.'))), ByteArrayLib.GetByteArrayFromString(value, Encoding.ASCII));
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
        
		public CalResult Write(string address, string value, DataType dataType)
		{
			return this.Write(this.SlaveAddress, address, value, dataType);
		}
        
		public bool PreSetBoolReg(string iAddress, bool SetValue)
		{
			return this.PreSetBoolReg(this.SlaveAddress, iAddress, SetValue);
		}
        
		public byte[] ReadOutputStatus(ushort startAddress, ushort numberOfPoints)
		{
			return this.ReadOutputStatus(this.SlaveAddress, startAddress, numberOfPoints);
		}
        
		public byte[] ReadKeepReg(ushort startAddress, ushort numberOfPoints)
		{
			return this.ReadKeepReg(this.SlaveAddress, startAddress, numberOfPoints);
		}
        
		public byte[] ReadInputReg(ushort startAddress, ushort numberOfPoints)
		{
			return this.ReadInputReg(this.SlaveAddress, startAddress, numberOfPoints);
		}
        
		public byte[] ReadInputStatus(ushort startAddress, ushort numberOfPoints)
		{
			return this.ReadInputStatus(this.SlaveAddress, startAddress, numberOfPoints);
		}
        
		public bool ForceCoil(ushort coilAddress, bool value)
		{
			return this.ForceCoil(this.SlaveAddress, coilAddress, value);
		}
        
		public bool PreSetSingleReg(ushort registerAddress, byte[] value)
		{
			return this.PreSetSingleReg(this.SlaveAddress, registerAddress, value);
		}
        
		public bool ForceMultiCoil(ushort startAddress, bool[] data)
		{
			return this.ForceMultiCoil(this.SlaveAddress, startAddress, data);
		}
        
		public bool PreSetMultiReg(ushort startAddress, byte[] data)
		{
			return this.PreSetMultiReg(this.SlaveAddress, startAddress, data);
		}
        
		public bool PreSetSingleReg(ushort startAddress, short value)
		{
			return this.PreSetSingleReg(this.SlaveAddress, startAddress, ByteArrayLib.GetByteArrayFromShort(value, DataFormat.ABCD));
		}
        
		public bool PreSetSingleReg(ushort startAddress, ushort value)
		{
			return this.PreSetSingleReg(this.SlaveAddress, startAddress, ByteArrayLib.GetByteArrayFromUShort(value, DataFormat.ABCD));
		}
        
		public bool PreSetMultiReg(ushort startAddress, float value)
		{
			return this.PreSetMultiReg(this.SlaveAddress, startAddress, ByteArrayLib.GetByteArrayFromFloat(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(ushort startAddress, int value)
		{
			return this.PreSetMultiReg(this.SlaveAddress, startAddress, ByteArrayLib.GetByteArrayFromInt(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(ushort startAddress, uint value)
		{
			return this.PreSetMultiReg(this.SlaveAddress, startAddress, ByteArrayLib.GetByteArrayFromUInt(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(ushort startAddress, float[] value)
		{
			return this.PreSetMultiReg(this.SlaveAddress, startAddress, ByteArrayLib.GetByteArrayFromFloatArray(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(ushort startAddress, int[] value)
		{
			return this.PreSetMultiReg(this.SlaveAddress, startAddress, ByteArrayLib.GetByteArrayFromIntArray(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(ushort startAddress, uint[] value)
		{
			return this.PreSetMultiReg(this.SlaveAddress, startAddress, ByteArrayLib.GetByteArrayFromUIntArray(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(ushort startAddress, short[] value)
		{
			return this.PreSetMultiReg(this.SlaveAddress, startAddress, ByteArrayLib.GetByteArrayFromShortArray(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(ushort startAddress, ushort[] value)
		{
			return this.PreSetMultiReg(this.SlaveAddress, startAddress, ByteArrayLib.GetByteArrayFromUShortArray(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(ushort startAddress, long value)
		{
			return this.PreSetMultiReg(this.SlaveAddress, startAddress, ByteArrayLib.GetByteArrayFromLong(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(ushort startAddress, ulong value)
		{
			return this.PreSetMultiReg(this.SlaveAddress, startAddress, ByteArrayLib.GetByteArrayFromULong(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(ushort startAddress, double value)
		{
			return this.PreSetMultiReg(this.SlaveAddress, startAddress, ByteArrayLib.GetByteArrayFromDouble(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(ushort startAddress, long[] value)
		{
			return this.PreSetMultiReg(this.SlaveAddress, startAddress, ByteArrayLib.GetByteArrayFromLongArray(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(ushort startAddress, ulong[] value)
		{
			return this.PreSetMultiReg(this.SlaveAddress, startAddress, ByteArrayLib.GetByteArrayFromULongArray(value, base.DataFormat));
		}
        
		public bool PreSetMultiReg(ushort startAddress, double[] value)
		{
			return this.PreSetMultiReg(this.SlaveAddress, startAddress, ByteArrayLib.GetByteArrayFromDoubleArray(value, base.DataFormat));
		}
        
		public ModbusSerialBase()
		{
			this.SlaveAddress = 1;
		}
	}
}
