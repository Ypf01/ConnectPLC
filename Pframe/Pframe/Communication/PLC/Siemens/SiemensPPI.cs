using System;
using System.Linq;
using System.Text;
using Pframe.Base;
using Pframe.Common;
using Pframe.DataConvert;

namespace Pframe.PLC.Siemens
{
    /// <summary>
    /// 西门子S7-200
    /// </summary>
	public class SiemensPPI : SerialDeviceBase
	{
		public SiemensPPI(DataFormat dataFormat = DataFormat.DCBA)
		{
			this.station = 2;
			this.executeConfirm = new byte[]
			{
				16,
				2,
				0,
				92,
				94,
				22
			};
			base.DataFormat = dataFormat;
		}
        
		public byte Station
		{
			get
			{
				return this.station;
			}
			set
			{
				this.station = value;
				this.executeConfirm[1] = value;
				int num = 0;
				for (int i = 1; i < 4; i++)
				{
					num += (int)this.executeConfirm[i];
				}
				this.executeConfirm[4] = (byte)num;
			}
		}
        
		public object Read(string variable, DataType VarType)
		{
			string text = variable.ToUpper();
			text = text.Replace(" ", "");
			object result;
			if (text.Contains('.'))
			{
				string[] array = variable.Split(new char[]
				{
					'.'
				});
				if (array.Length == 2)
				{
					byte[] array2 = base.ReadBytes(array[0], 1);
					if (array2 != null && array2.Length == 1)
					{
						int offset = 0;
						if (int.TryParse(array[1], out offset))
						{
							result = BitLib.GetBitFromByte(array2[0], offset);
						}
						else
						{
							result = null;
						}
					}
					else
					{
						result = null;
					}
				}
				else
				{
					result = null;
				}
			}
			else if (text.Length > 2)
			{
				string address = string.Empty;
				ushort length = 0;
				if (text.Contains('|'))
				{
					address = text.Split(new char[]
					{
						'|'
					})[0].ToArray<char>().Skip(1).ToString();
					length = ushort.Parse(text.Split(new char[]
					{
						'|'
					})[1]);
				}
				else
				{
					address = text.ToArray<char>().Skip(1).ToString();
				}
				string text2 = variable.Substring(1, 1);
				byte[] array3 = null;
				string text3 = text2;
				string a = text3;
				if (!(a == "B"))
				{
					if (!(a == "W"))
					{
						if (!(a == "D"))
						{
							if (!(a == "R"))
							{
								if (a == "S")
								{
									array3 = base.ReadBytes(address, length);
								}
							}
							else
							{
								array3 = base.ReadBytes(address, 8);
							}
						}
						else
						{
							array3 = base.ReadBytes(address, 4);
						}
					}
					else
					{
						array3 = base.ReadBytes(address, 2);
					}
				}
				else
				{
					array3 = base.ReadBytes(address, 1);
				}
				if (array3 != null)
				{
					switch (VarType)
					{
					case DataType.Byte:
						result = Convert.ToByte(array3[0]);
						break;
					case DataType.Short:
						result = ShortLib.GetShortFromByteArray(array3, 0, DataFormat.DCBA);
						break;
					case DataType.UShort:
						result = UShortLib.GetUShortFromByteArray(array3, 0, DataFormat.DCBA);
						break;
					case DataType.Int:
						result = IntLib.GetIntFromByteArray(array3, 0, DataFormat.DCBA);
						break;
					case DataType.UInt:
						result = UIntLib.GetUIntFromByteArray(array3, 0, DataFormat.DCBA);
						break;
					case DataType.Float:
						result = FloatLib.GetFloatFromByteArray(array3, 0, DataFormat.DCBA);
						break;
					case DataType.Double:
						result = DoubleLib.GetDoubleFromByteArray(array3, 0, DataFormat.DCBA);
						break;
					case DataType.Long:
						result = LongLib.GetLongFromByteArray(array3, 0, DataFormat.DCBA);
						break;
					case DataType.ULong:
						result = ULongLib.GetULongFromByteArray(array3, 0, DataFormat.DCBA);
						break;
					case DataType.String:
						result = StringLib.GetStringFromByteArray(array3, 0, array3.Length, Encoding.ASCII);
						break;
					default:
						result = null;
						break;
					}
				}
				else
				{
					result = null;
				}
			}
			else
			{
				result = null;
			}
			return result;
		}
        
		public override CalResult<byte[]> ReadByteArray(string address, ushort length)
		{
			this.InteractiveLock.Enter();
			CalResult<byte[]> xktResult = this.BuildReadCommand(this.station, address, length, false);
			CalResult<byte[]> result;
			if (!xktResult.IsSuccess)
			{
				result = CalResult.CreateFailedResult<byte[]>(new CalResult());
			}
			else
			{
				byte[] array = null;
				if (base.SendAndReceive(xktResult.Content, ref array, 0))
				{
					if (array[0] == 229)
					{
						if (base.SendAndReceive(this.executeConfirm, ref array, 0))
						{
							if (array.Length < 21)
							{
								this.InteractiveLock.Leave();
								result = null;
							}
							else if (array[17] != 0 || array[18] > 0)
							{
								this.InteractiveLock.Leave();
								result = null;
							}
							else if (array[21] != 255 || array[22] != 4)
							{
								this.InteractiveLock.Leave();
								result = null;
							}
							else
							{
								byte[] array2 = new byte[(int)length];
								Array.Copy(array, 25, array2, 0, (int)length);
								this.InteractiveLock.Leave();
								result = CalResult.CreateSuccessResult<byte[]>(array2);
							}
						}
						else
						{
							this.InteractiveLock.Leave();
							result = CalResult.CreateFailedResult<byte[]>(new CalResult());
						}
					}
					else
					{
						this.InteractiveLock.Leave();
						result = CalResult.CreateFailedResult<byte[]>(new CalResult());
					}
				}
				else
				{
					this.InteractiveLock.Leave();
					result = CalResult.CreateFailedResult<byte[]>(new CalResult());
				}
			}
			return result;
		}
        
		public override CalResult<bool[]> ReadBoolArray(string address, ushort length)
		{
			this.InteractiveLock.Enter();
			CalResult<byte[]> xktResult = this.BuildReadCommand(this.station, address, length, true);
			CalResult<bool[]> result;
			if (!xktResult.IsSuccess)
			{
				result = CalResult.CreateFailedResult<bool[]>(new CalResult());
			}
			else
			{
				byte[] array = null;
				if (base.SendAndReceive(xktResult.Content, ref array, 0))
				{
					if (array[0] == 229)
					{
						if (base.SendAndReceive(this.executeConfirm, ref array, 0))
						{
							if (array.Length < 21)
							{
								result = null;
							}
							else if (array[17] != 0 || array[18] > 0)
							{
								result = null;
							}
							else if (array[21] != 255 || array[22] != 3)
							{
								result = null;
							}
							else
							{
								byte[] array2 = new byte[(int)length];
								Array.Copy(array, 25, array2, 0, array2.Length);
								this.InteractiveLock.Leave();
								result = CalResult.CreateSuccessResult<bool[]>(BitLib.ByteToBoolArray(array2, (int)length));
							}
						}
						else
						{
							this.InteractiveLock.Leave();
							result = CalResult.CreateFailedResult<bool[]>(new CalResult());
						}
					}
					else
					{
						this.InteractiveLock.Leave();
						result = CalResult.CreateFailedResult<bool[]>(new CalResult());
					}
				}
				else
				{
					this.InteractiveLock.Leave();
					result = CalResult.CreateFailedResult<bool[]>(new CalResult());
				}
			}
			return result;
		}
        
		public bool Write(string variable, object value)
		{
			string text = variable.ToUpper();
			text = text.Replace(" ", "");
			bool result;
			if (text.Contains('.'))
			{
				string[] array = variable.Split(new char[]
				{
					'.'
				});
				result = (array.Length == 2 && this.Write(text, new bool[]
				{
					value.ToString() == "1" || value.ToString().ToLower() == "true"
				}));
			}
			else if (text.Length > 2)
			{
				string variable2 = string.Empty;
				if (text.Contains('|'))
				{
					string[] array2 = variable.Split(new char[]
					{
						'|'
					});
					if (array2.Length != 2)
					{
						return false;
					}
					variable2 = array2[0].Substring(0, 1) + array2[0].Substring(2);
					ushort.Parse(array2[1]);
				}
				else
				{
					variable2 = text.Substring(0, 1) + text.Substring(2);
				}
				string text2 = text.Substring(1, 1);
				string text3 = text2;
				string a = text3;
				if (!(a == "B"))
				{
					if (!(a == "W"))
					{
						if (!(a == "D"))
						{
							if (!(a == "R"))
							{
								result = (a == "S" && this.Write(variable2, ByteArrayLib.GetByteArrayFromString(value.ToString(), Encoding.GetEncoding("GBK"))));
							}
							else if (value is long)
							{
								result = this.Write(variable2, ByteArrayLib.GetByteArrayFromLong((long)value, DataFormat.ABCD));
							}
							else if (value is ulong)
							{
								result = this.Write(variable2, ByteArrayLib.GetByteArrayFromULong((ulong)value, DataFormat.ABCD));
							}
							else
							{
								result = this.Write(variable2, ByteArrayLib.GetByteArrayFromDouble((double)value, DataFormat.ABCD));
							}
						}
						else if (value is int)
						{
							result = this.Write(variable2, ByteArrayLib.GetByteArrayFromInt((int)value, DataFormat.ABCD));
						}
						else if (value is uint)
						{
							result = this.Write(variable2, ByteArrayLib.GetByteArrayFromUInt((uint)value, DataFormat.ABCD));
						}
						else
						{
							result = this.Write(variable2, ByteArrayLib.GetByteArrayFromFloat((float)value, DataFormat.ABCD));
						}
					}
					else if (value is short)
					{
						result = this.Write(variable2, ByteArrayLib.GetByteArrayFromShort((short)value, DataFormat.ABCD));
					}
					else
					{
						result = this.Write(variable2, ByteArrayLib.GetByteArrayFromUShort((ushort)value, DataFormat.ABCD));
					}
				}
				else
				{
					result = this.Write(variable2, new byte[]
					{
						(byte)value
					});
				}
			}
			else
			{
				result = false;
			}
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
					xktResult.IsSuccess = this.Write(variable, byte.Parse(value));
					break;
				case DataType.ByteArray:
					xktResult.IsSuccess = this.Write(variable, value);
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
        
		private CalResult<byte[]> BuildReadCommand(byte station, string address, ushort length, bool isBit)
		{
			CalResult<byte, int, ushort> xktResult = this.AnalysisAddress(address);
			CalResult<byte[]> result;
			if (!xktResult.IsSuccess)
				result = CalResult.CreateFailedResult<byte[]>(xktResult);
			else
			{
				byte[] array = new byte[33];
				array[0] = 104;
				array[1] = BitConverter.GetBytes(array.Length - 6)[0];
				array[2] = BitConverter.GetBytes(array.Length - 6)[0];
				array[3] = 104;
				array[4] = station;
				array[5] = 0;
				array[6] = 108;
				array[7] = 50;
				array[8] = 1;
				array[9] = 0;
				array[10] = 0;
				array[11] = 0;
				array[12] = 0;
				array[13] = 0;
				array[14] = 14;
				array[15] = 0;
				array[16] = 0;
				array[17] = 4;
				array[18] = 1;
				array[19] = 18;
				array[20] = 10;
				array[21] = 16;
				array[22] = (byte)(isBit ? 1 : 2);
				array[23] = 0;
				array[24] = BitConverter.GetBytes(length)[0];
				array[25] = BitConverter.GetBytes(length)[1];
				array[26] = (byte)xktResult.Content3;
				array[27] = xktResult.Content1;
				array[28] = BitConverter.GetBytes(xktResult.Content2)[2];
				array[29] = BitConverter.GetBytes(xktResult.Content2)[1];
				array[30] = BitConverter.GetBytes(xktResult.Content2)[0];
				int num = 0;
				for (int i = 4; i < 31; i++)
				{
					num += (int)array[i];
				}
				array[31] = BitConverter.GetBytes(num)[0];
				array[32] = 22;
				result = CalResult.CreateSuccessResult<byte[]>(array);
			}
			return result;
		}
        
		private CalResult<byte, int, ushort> AnalysisAddress(string address)
		{
			CalResult<byte, int, ushort> xktResult = new CalResult<byte, int, ushort>();
			try
			{
				xktResult.Content3 = 0;
				if (address.Substring(0, 2) == "AI")
				{
					xktResult.Content1 = 6;
					xktResult.Content2 = this.CalculateAddressStarted(address.Substring(2), false);
				}
				else if (address.Substring(0, 2) == "AQ")
				{
					xktResult.Content1 = 7;
					xktResult.Content2 = this.CalculateAddressStarted(address.Substring(2), false);
				}
				else if (address[0] == 'T')
				{
					xktResult.Content1 = 31;
					xktResult.Content2 = this.CalculateAddressStarted(address.Substring(1), false);
				}
				else if (address[0] == 'C')
				{
					xktResult.Content1 = 30;
					xktResult.Content2 = this.CalculateAddressStarted(address.Substring(1), false);
				}
				else if (address.Substring(0, 2) == "SM")
				{
					xktResult.Content1 = 5;
					xktResult.Content2 = this.CalculateAddressStarted(address.Substring(2), false);
				}
				else if (address[0] == 'S')
				{
					xktResult.Content1 = 4;
					xktResult.Content2 = this.CalculateAddressStarted(address.Substring(1), false);
				}
				else if (address[0] == 'I')
				{
					xktResult.Content1 = 129;
					xktResult.Content2 = this.CalculateAddressStarted(address.Substring(1), false);
				}
				else if (address[0] == 'Q')
				{
					xktResult.Content1 = 130;
					xktResult.Content2 = this.CalculateAddressStarted(address.Substring(1), false);
				}
				else if (address[0] == 'M')
				{
					xktResult.Content1 = 131;
					xktResult.Content2 = this.CalculateAddressStarted(address.Substring(1), false);
				}
				else if (address[0] == 'D' || address.Substring(0, 2) == "DB")
				{
					xktResult.Content1 = 132;
					string[] array = address.Split(new char[]
					{
						'.'
					});
					if (address[1] == 'B')
					{
						xktResult.Content3 = Convert.ToUInt16(array[0].Substring(2));
					}
					else
					{
						xktResult.Content3 = Convert.ToUInt16(array[0].Substring(1));
					}
					xktResult.Content2 = this.CalculateAddressStarted(address.Substring(address.IndexOf('.') + 1), false);
				}
				else
				{
					if (address[0] != 'V')
					{
						xktResult.Message = "输入的类型不支持，请重新输入";
						xktResult.Content1 = 0;
						xktResult.Content2 = 0;
						xktResult.Content3 = 0;
						return xktResult;
					}
					xktResult.Content1 = 132;
					xktResult.Content3 = 1;
					xktResult.Content2 = this.CalculateAddressStarted(address.Substring(1), false);
				}
			}
			catch (Exception ex)
			{
				xktResult.Message = ex.Message;
				return xktResult;
			}
			xktResult.IsSuccess = true;
			return xktResult;
		}
        
		private int CalculateAddressStarted(string address, bool isCT = false)
		{
			int result;
			if (address.IndexOf('.') < 0)
                result = isCT ? Convert.ToInt32(address) : Convert.ToInt32(address) * 8;
            else
			{
				string[] array = address.Split(new char[]
				{
					'.'
				});
				result = Convert.ToInt32(array[0]) * 8 + Convert.ToInt32(array[1]);
			}
			return result;
		}
        
		public override CalResult WriteByteArray(string address, byte[] value)
		{
			this.InteractiveLock.Enter();
			CalResult<byte[]> xktResult = this.BuildWriteCommand(this.station, address, value);
			CalResult result;
			if (!xktResult.IsSuccess)
			{
				result = CalResult.CreateFailedResult();
			}
			else
			{
				byte[] array = null;
				if (base.SendAndReceive(xktResult.Content, ref array, 0))
				{
					if (array[0] == 229)
					{
						if (base.SendAndReceive(this.executeConfirm, ref array, 0))
						{
							if (array.Length < 21)
							{
								this.InteractiveLock.Leave();
								result = CalResult.CreateFailedResult();
							}
							else if (array[17] != 0 || array[18] > 0)
							{
								this.InteractiveLock.Leave();
								result = CalResult.CreateFailedResult();
							}
							else if (array[21] != 255)
							{
								this.InteractiveLock.Leave();
								result = CalResult.CreateFailedResult();
							}
							else
							{
								this.InteractiveLock.Leave();
								result = CalResult.CreateSuccessResult();
							}
						}
						else
						{
							this.InteractiveLock.Leave();
							result = CalResult.CreateFailedResult();
						}
					}
					else
					{
						this.InteractiveLock.Leave();
						result = CalResult.CreateFailedResult();
					}
				}
				else
				{
					this.InteractiveLock.Leave();
					result = CalResult.CreateFailedResult();
				}
			}
			return result;
		}
        
		public override CalResult WriteBoolArray(string address, bool[] value)
		{
			this.InteractiveLock.Enter();
			CalResult<byte[]> xktResult = this.BuildWriteCommand(this.station, address, value);
			CalResult result;
			if (!xktResult.IsSuccess)
				result = CalResult.CreateFailedResult();
			else
			{
				byte[] array = null;
				if (base.SendAndReceive(xktResult.Content, ref array, 0))
				{
					if (array[0] == 229)
					{
						if (base.SendAndReceive(this.executeConfirm, ref array, 0))
						{
							if (array.Length < 21)
							{
								this.InteractiveLock.Leave();
								result = CalResult.CreateFailedResult();
							}
							else if (array[17] != 0 || array[18] > 0)
							{
								this.InteractiveLock.Leave();
								result = CalResult.CreateFailedResult();
							}
							else if (array[21] != 255)
							{
								this.InteractiveLock.Leave();
								result = CalResult.CreateFailedResult();
							}
							else
							{
								this.InteractiveLock.Leave();
								result = CalResult.CreateSuccessResult();
							}
						}
						else
						{
							this.InteractiveLock.Leave();
							result = CalResult.CreateFailedResult();
						}
					}
					else
					{
						this.InteractiveLock.Leave();
						result = CalResult.CreateFailedResult();
					}
				}
				else
				{
					this.InteractiveLock.Leave();
					result = CalResult.CreateFailedResult();
				}
			}
			return result;
		}
        
		private CalResult<byte[]> BuildWriteCommand(byte station, string address, byte[] values)
		{
			CalResult<byte, int, ushort> xktResult = this.AnalysisAddress(address);
			CalResult<byte[]> result;
			if (!xktResult.IsSuccess)
				result = CalResult.CreateFailedResult<byte[]>(xktResult);
			else
			{
				int num = values.Length;
				byte[] array = new byte[37 + values.Length];
				array[0] = 104;
				array[1] = BitConverter.GetBytes(array.Length - 6)[0];
				array[2] = BitConverter.GetBytes(array.Length - 6)[0];
				array[3] = 104;
				array[4] = station;
				array[5] = 0;
				array[6] = 124;
				array[7] = 50;
				array[8] = 1;
				array[9] = 0;
				array[10] = 0;
				array[11] = 0;
				array[12] = 0;
				array[13] = 0;
				array[14] = 14;
				array[15] = 0;
				array[16] = (byte)(values.Length + 4);
				array[17] = 5;
				array[18] = 1;
				array[19] = 18;
				array[20] = 10;
				array[21] = 16;
				array[22] = 2;
				array[23] = 0;
				array[24] = BitConverter.GetBytes(num)[0];
				array[25] = BitConverter.GetBytes(num)[1];
				array[26] = (byte)xktResult.Content3;
				array[27] = xktResult.Content1;
				array[28] = BitConverter.GetBytes(xktResult.Content2)[2];
				array[29] = BitConverter.GetBytes(xktResult.Content2)[1];
				array[30] = BitConverter.GetBytes(xktResult.Content2)[0];
				array[31] = 0;
				array[32] = 4;
				array[33] = BitConverter.GetBytes(num * 8)[1];
				array[34] = BitConverter.GetBytes(num * 8)[0];
                values.CopyTo(array, 35);
				int num2 = 0;
				for (int i = 4; i < array.Length - 2; i++)
				{
					num2 += (int)array[i];
				}
				array[array.Length - 2] = BitConverter.GetBytes(num2)[0];
				array[array.Length - 1] = 22;
				result = CalResult.CreateSuccessResult<byte[]>(array);
			}
			return result;
		}
        
		private CalResult<byte[]> BuildWriteCommand(byte station, string address, bool[] values)
		{
			CalResult<byte, int, ushort> xktResult = this.AnalysisAddress(address);
			CalResult<byte[]> result;
			if (!xktResult.IsSuccess)
				result = CalResult.CreateFailedResult<byte[]>(xktResult);
			else
			{
				byte[] byteArrayFromBoolArray = ByteArrayLib.GetByteArrayFromBoolArray(values);
				byte[] array = new byte[37 + byteArrayFromBoolArray.Length];
				array[0] = 104;
				array[1] = BitConverter.GetBytes(array.Length - 6)[0];
				array[2] = BitConverter.GetBytes(array.Length - 6)[0];
				array[3] = 104;
				array[4] = station;
				array[5] = 0;
				array[6] = 124;
				array[7] = 50;
				array[8] = 1;
				array[9] = 0;
				array[10] = 0;
				array[11] = 0;
				array[12] = 0;
				array[13] = 0;
				array[14] = 14;
				array[15] = 0;
				array[16] = 5;
				array[17] = 5;
				array[18] = 1;
				array[19] = 18;
				array[20] = 10;
				array[21] = 16;
				array[22] = 1;
				array[23] = 0;
				array[24] = BitConverter.GetBytes(values.Length)[0];
				array[25] = BitConverter.GetBytes(values.Length)[1];
				array[26] = (byte)xktResult.Content3;
				array[27] = xktResult.Content1;
				array[28] = BitConverter.GetBytes(xktResult.Content2)[2];
				array[29] = BitConverter.GetBytes(xktResult.Content2)[1];
				array[30] = BitConverter.GetBytes(xktResult.Content2)[0];
				array[31] = 0;
				array[32] = 3;
				array[33] = BitConverter.GetBytes(values.Length)[1];
				array[34] = BitConverter.GetBytes(values.Length)[0];
				byteArrayFromBoolArray.CopyTo(array, 35);
				int num = 0;
				for (int i = 4; i < array.Length - 2; i++)
					num += (int)array[i];
				array[array.Length - 2] = BitConverter.GetBytes(num)[0];
				array[array.Length - 1] = 22;
				result = CalResult.CreateSuccessResult<byte[]>(array);
			}
			return result;
		}
        
		public bool Start()
		{
			byte[] array = new byte[]
			{
				104,
				33,
				33,
				104,
				0,
				0,
				108,
				50,
				1,
				0,
				0,
				0,
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
				77,
				170,
				22
			};
			array[4] = this.station;
			byte[] sendByte = array;
			byte[] array2 = null;
			return base.SendAndReceive(sendByte, ref array2, 0) && array2[0] == 229 && base.SendAndReceive(this.executeConfirm, ref array2, 0);
		}
        
		public bool Stop()
		{
			byte[] array = new byte[]
			{
				104,
				29,
				29,
				104,
				0,
				0,
				108,
				50,
				1,
				0,
				0,
				0,
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
				77,
				170,
				22
			};
			array[4] = this.station;
			byte[] sendByte = array;
			byte[] array2 = null;
			return base.SendAndReceive(sendByte, ref array2, 0) && array2[0] == 229 && base.SendAndReceive(this.executeConfirm, ref array2, 0);
		}
        
		private byte station;
        
		private byte[] executeConfirm;
	}
}
