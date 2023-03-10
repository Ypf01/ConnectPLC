using System;
using System.Linq;
using System.Text;
using Pframe.Common;
using Pframe.DataConvert;
using Pframe.Modbus;

namespace Pframe.PLC.Xinje
{
	public class XinjeXCModbus : ModbusRtu
	{
		public bool Readbool(string address, ref bool value, byte SlaveID = 1)
		{
			CalResult<byte[]> xktResult = this.ReadBytes(address, 1, SlaveID);
			if (xktResult.IsSuccess)
			{
				value = (xktResult.Content[0] == 1);
			}
			return xktResult.IsSuccess;
		}
        
		public CalResult<byte[]> ReadBytes(string address, ushort length, byte SlaveID = 1)
		{
			CalResult<byte[]> xktResult = new CalResult<byte[]>();
			CalResult<ushort, int> xktResult2 = this.AnlysisAddress(address);
			CalResult<byte[]> result;
			if (xktResult2.IsSuccess)
			{
				if (xktResult2.Content2 == 0)
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
				else if (xktResult2.Content2 == 4)
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
				else
				{
					result = CalResult.CreateFailedResult<byte[]>(new CalResult());
				}
			}
			else
			{
				result = CalResult.CreateFailedResult<byte[]>(xktResult2);
			}
			return result;
		}
        
		public bool Readshort(string address, ref short value, byte SlaveID = 1)
		{
			CalResult<byte[]> xktResult = this.ReadBytes(address, 1, SlaveID);
			if (xktResult.IsSuccess)
			{
				value = ShortLib.GetShortFromByteArray(xktResult.Content, 0, DataFormat.ABCD);
			}
			return xktResult.IsSuccess;
		}
        
		public bool Readushort(string address, ref ushort value, byte SlaveID = 1)
		{
			CalResult<byte[]> xktResult = this.ReadBytes(address, 1, SlaveID);
			if (xktResult.IsSuccess)
			{
				value = UShortLib.GetUShortFromByteArray(xktResult.Content, 0, DataFormat.ABCD);
			}
			return xktResult.IsSuccess;
		}
        
		public bool Readint(string address, ref int value, byte SlaveID = 1)
		{
			CalResult<byte[]> xktResult = this.ReadBytes(address, 2, SlaveID);
			if (xktResult.IsSuccess)
			{
				value = IntLib.GetIntFromByteArray(xktResult.Content, 0, DataFormat.ABCD);
			}
			return xktResult.IsSuccess;
		}
        
		public bool Readuint(string address, ref uint value, byte SlaveID = 1)
		{
			CalResult<byte[]> xktResult = this.ReadBytes(address, 2, SlaveID);
			if (xktResult.IsSuccess)
			{
				value = UIntLib.GetUIntFromByteArray(xktResult.Content, 0, DataFormat.ABCD);
			}
			return xktResult.IsSuccess;
		}
        
		public bool Readfloat(string address, ref float value, byte SlaveID = 1)
		{
			CalResult<byte[]> xktResult = this.ReadBytes(address, 2, SlaveID);
			if (xktResult.IsSuccess)
			{
				value = FloatLib.GetFloatFromByteArray(xktResult.Content, 0, DataFormat.ABCD);
			}
			return xktResult.IsSuccess;
		}
        
		public bool Readlong(string address, ref long value, byte SlaveID = 1)
		{
			CalResult<byte[]> xktResult = this.ReadBytes(address, 4, SlaveID);
			if (xktResult.IsSuccess)
			{
				value = LongLib.GetLongFromByteArray(xktResult.Content, 0, DataFormat.ABCD);
			}
			return xktResult.IsSuccess;
		}
        
		public bool Readulong(string address, ref ulong value, byte SlaveID = 1)
		{
			CalResult<byte[]> xktResult = this.ReadBytes(address, 4, SlaveID);
			if (xktResult.IsSuccess)
			{
				value = ULongLib.GetULongFromByteArray(xktResult.Content, 0, DataFormat.ABCD);
			}
			return xktResult.IsSuccess;
		}
        
		public bool Readdouble(string address, ref double value, byte SlaveID = 1)
		{
			CalResult<byte[]> xktResult = this.ReadBytes(address, 4, SlaveID);
			if (xktResult.IsSuccess)
			{
				value = DoubleLib.GetDoubleFromByteArray(xktResult.Content, 0, DataFormat.ABCD);
			}
			return xktResult.IsSuccess;
		}
        
		public bool Readstring(string address, ushort length, ref string value, byte SlaveID = 1)
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
					if (this.Readushort(array[0], ref value2, SlaveID))
					{
						int bit = Convert.ToInt32(array[1]);
						ushort value3 = UShortLib.SetbitValueFromUShort(value2, bit, value, DataFormat.ABCD);
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
			CalResult<ushort, int> xktResult = this.AnlysisAddress(address);
			CalResult result;
			if (xktResult.IsSuccess)
			{
				if (xktResult.Content2 != 4)
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
			CalResult<ushort, int> xktResult = this.AnlysisAddress(address);
			CalResult result;
			if (xktResult.IsSuccess)
			{
				if (xktResult.Content2 != 0)
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
			return this.Write(address, ByteArrayLib.GetByteArrayFromShortArray(values, DataFormat.ABCD), SlaveID);
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
			return this.Write(address, ByteArrayLib.GetByteArrayFromUShortArray(values, DataFormat.ABCD), SlaveID);
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
			return this.Write(address, ByteArrayLib.GetByteArrayFromIntArray(values, DataFormat.ABCD), SlaveID);
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
			return this.Write(address, ByteArrayLib.GetByteArrayFromUIntArray(values, DataFormat.ABCD), SlaveID);
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
			return this.Write(address, ByteArrayLib.GetByteArrayFromFloatArray(values, DataFormat.ABCD), SlaveID);
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
			return this.Write(address, ByteArrayLib.GetByteArrayFromLongArray(values, DataFormat.ABCD), SlaveID);
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
			return this.Write(address, ByteArrayLib.GetByteArrayFromULongArray(values, DataFormat.ABCD), SlaveID);
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
			return this.Write(address, ByteArrayLib.GetByteArrayFromDoubleArray(values, DataFormat.ABCD), SlaveID);
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
        
		private CalResult<ushort, int> AnlysisAddress(string address)
		{
			CalResult<ushort, int> xktResult = new CalResult<ushort, int>();
			int num = 0;
			string text = address.Substring(0, 1).ToLower();
			string text2 = text;
			uint num2 = PrivateImplementationDetails.ComputeStringHash(text2);
			if (num2 <= 3859557458U)
			{
				if (num2 <= 3775669363U)
				{
					if (num2 != 3758891744U)
					{
						if (num2 == 3775669363U)
						{
							if (text2 == "d")
							{
								if (!this.GetStartAddress(address.Substring(1), XinjeXCDataType.D.FromBase, ref num))
								{
									xktResult.Message = "地址格式不正确";
									return xktResult;
								}
								if (num >= 8000)
								{
									if (num > 8511)
									{
										xktResult.Message = "D区不能超过" + 8511.ToString();
										return xktResult;
									}
									xktResult.Content1 = (ushort)(8384 + num);
									xktResult.Content2 = 4;
								}
								else
								{
									xktResult.Content1 = (ushort)(0 + num);
									xktResult.Content2 = 4;
								}
							}
						}
					}
					else if (text2 == "e")
					{
						if (!this.GetStartAddress(address.Substring(2), XinjeXCDataType.ED.FromBase, ref num))
						{
							xktResult.Message = "地址格式不正确";
							return xktResult;
						}
						if (num > 36863)
						{
							xktResult.Message = "ED区不能超过" + 36863.ToString();
							return xktResult;
						}
						xktResult.Content1 = (ushort)(28672 + num);
						xktResult.Content2 = 4;
					}
				}
				else if (num2 != 3809224601U)
				{
					if (num2 == 3859557458U)
					{
						if (text2 == "c")
						{
							if (address.StartsWith("cd") || address.StartsWith("CD"))
							{
								if (!this.GetStartAddress(address.Substring(2), XinjeXCDataType.CD.FromBase, ref num))
								{
									xktResult.Message = "地址格式不正确";
									return xktResult;
								}
								if (num > 634)
								{
									xktResult.Message = "CD区不能超过" + 634.ToString();
									return xktResult;
								}
								xktResult.Content1 = (ushort)(14436 + num);
								xktResult.Content2 = 4;
							}
							else
							{
								if (!this.GetStartAddress(address.Substring(1), XinjeXCDataType.C.FromBase, ref num))
								{
									xktResult.Message = "地址格式不正确";
									return xktResult;
								}
								if (num > 634)
								{
									xktResult.Message = "C区不能超过" + 634.ToString();
									return xktResult;
								}
								xktResult.Content1 = (ushort)(27648 + num);
								xktResult.Content2 = 0;
							}
						}
					}
				}
				else if (text2 == "f")
				{
					if (!this.GetStartAddress(address.Substring(2), XinjeXCDataType.FD.FromBase, ref num))
					{
						xktResult.Message = "地址格式不正确";
						return xktResult;
					}
					if (num >= 8000)
					{
						if (num > 5000)
						{
							xktResult.Message = "FD区不能超过" + 5000.ToString();
							return xktResult;
						}
						xktResult.Content1 = (ushort)(18624 + num);
						xktResult.Content2 = 4;
					}
					else
					{
						if (num > 5000)
						{
							xktResult.Message = "FD1区不能超过" + 5000.ToString();
							return xktResult;
						}
						xktResult.Content1 = (ushort)(18432 + num);
						xktResult.Content2 = 4;
					}
				}
			}
			else if (num2 <= 4044111267U)
			{
				if (num2 != 3893112696U)
				{
					if (num2 == 4044111267U)
					{
						if (text2 == "t")
						{
							if (address.StartsWith("td") || address.StartsWith("TD"))
							{
								if (!this.GetStartAddress(address.Substring(2), XinjeXCDataType.TD.FromBase, ref num))
								{
									xktResult.Message = "地址格式不正确";
									return xktResult;
								}
								if (num > 618)
								{
									xktResult.Message = "TD区不能超过" + 618.ToString();
									return xktResult;
								}
								xktResult.Content1 = (ushort)(12288 + num);
								xktResult.Content2 = 4;
							}
							else
							{
								if (!this.GetStartAddress(address.Substring(1), XinjeXCDataType.T.FromBase, ref num))
								{
									xktResult.Message = "地址格式不正确";
									return xktResult;
								}
								if (num > 618)
								{
									xktResult.Message = "T区不能超过" + 618.ToString();
									return xktResult;
								}
								xktResult.Content1 = (ushort)(25600 + num);
								xktResult.Content2 = 0;
							}
						}
					}
				}
				else if (text2 == "m")
				{
					if (!this.GetStartAddress(address.Substring(1), XinjeXCDataType.M.FromBase, ref num))
					{
						xktResult.Message = "地址格式不正确";
						return xktResult;
					}
					if (num >= 8000)
					{
						if (num > 8511)
						{
							xktResult.Message = "M区不能超过" + 8511.ToString();
							return xktResult;
						}
						xktResult.Content1 = (ushort)(16576 + num);
						xktResult.Content2 = 0;
					}
					else
					{
						xktResult.Content1 = (ushort)(0 + num);
						xktResult.Content2 = 0;
					}
				}
			}
			else if (num2 != 4127999362U)
			{
				if (num2 != 4228665076U)
				{
					if (num2 == 4245442695U)
					{
						if (text2 == "x")
						{
							if (!this.GetStartAddress(address.Substring(1), XinjeXCDataType.X.FromBase, ref num))
							{
								xktResult.Message = "地址格式不正确";
								return xktResult;
							}
							if (num > 1037)
							{
								xktResult.Message = "X区不能超过" + 1037.ToString();
								return xktResult;
							}
							xktResult.Content1 = (ushort)(16384 + num);
							xktResult.Content2 = 0;
						}
					}
				}
				else if (text2 == "y")
				{
					if (!this.GetStartAddress(address.Substring(1), XinjeXCDataType.Y.FromBase, ref num))
					{
						xktResult.Message = "地址格式不正确";
						return xktResult;
					}
					if (num > 1037)
					{
						xktResult.Message = "Y区不能超过" + 1037.ToString();
						return xktResult;
					}
					xktResult.Content1 = (ushort)(18432 + num);
					xktResult.Content2 = 0;
				}
			}
			else if (text2 == "s")
			{
				if (!this.GetStartAddress(address.Substring(1), XinjeXCDataType.S.FromBase, ref num))
				{
					xktResult.Message = "地址格式不正确";
					return xktResult;
				}
				if (num > 1023)
				{
					xktResult.Message = "S区不能超过" + 1023.ToString();
					return xktResult;
				}
				xktResult.Content1 = (ushort)(20480 + num);
				xktResult.Content2 = 0;
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
