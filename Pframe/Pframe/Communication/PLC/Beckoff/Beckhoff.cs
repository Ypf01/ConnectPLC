using System;
using System.IO;
using System.Linq;
using System.Text;
using TwinCAT.Ads;
using Pframe.Common;
using Pframe.DataConvert;

namespace Pframe.PLC.Beckoff
{
    /// <summary>
    /// 倍福
    /// </summary>
	public class Beckhoff
	{
		public bool Connect(string NetID, int Port)
		{
			try
			{
				this.tcAdsClient_0.Connect(NetID, Port);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}
        
		public void DisConnect()
		{
			if (this.tcAdsClient_0 != null)
			{
				this.tcAdsClient_0.Dispose();
			}
		}
        
		public BinaryReader ReadStruct(string Address, int length)
		{
			int num = 0;
			BinaryReader result;
			try
			{
				num = this.tcAdsClient_0.CreateVariableHandle(Address);
				AdsStream adsStream = new AdsStream(length);
				BinaryReader binaryReader = new BinaryReader(adsStream);
				adsStream.Position = 0L;
				this.tcAdsClient_0.Read(num, adsStream);
				result = binaryReader;
			}
			catch (Exception)
			{
				result = null;
			}
			finally
			{
				this.tcAdsClient_0.DeleteVariableHandle(num);
			}
			return result;
		}
        
		public T[] ReadArray<T>(string Address, int length)
		{
			int num = 0;
			T[] result;
			try
			{
				num = this.tcAdsClient_0.CreateVariableHandle(Address);
				result = (T[])this.tcAdsClient_0.ReadAny(num, typeof(T[]), new int[]
				{
					length
				});
			}
			catch (Exception)
			{
				result = null;
			}
			finally
			{
				this.tcAdsClient_0.DeleteVariableHandle(num);
			}
			return result;
		}
        
		public CalResult<object> Read(string address, ComplexDataType vartype)
		{
			int num = 0;
			CalResult<object> xktResult = new CalResult<object>
			{
				IsSuccess = true
			};
			CalResult<object> result;
			try
			{
				switch (vartype)
				{
				case ComplexDataType.Bool:
					num = this.tcAdsClient_0.CreateVariableHandle(address);
					xktResult.Content = (bool)this.tcAdsClient_0.ReadAny(num, typeof(bool));
					return xktResult;
				case ComplexDataType.Byte:
					num = this.tcAdsClient_0.CreateVariableHandle(address);
					xktResult.Content = (byte)this.tcAdsClient_0.ReadAny(num, typeof(byte));
					return xktResult;
				case ComplexDataType.Short:
					num = this.tcAdsClient_0.CreateVariableHandle(address);
					xktResult.Content = (short)this.tcAdsClient_0.ReadAny(num, typeof(short));
					return xktResult;
				case ComplexDataType.UShort:
					num = this.tcAdsClient_0.CreateVariableHandle(address);
					xktResult.Content = (ushort)this.tcAdsClient_0.ReadAny(num, typeof(ushort));
					return xktResult;
				case ComplexDataType.Int:
					num = this.tcAdsClient_0.CreateVariableHandle(address);
					xktResult.Content = (int)this.tcAdsClient_0.ReadAny(num, typeof(int));
					return xktResult;
				case ComplexDataType.UInt:
					num = this.tcAdsClient_0.CreateVariableHandle(address);
					xktResult.Content = (uint)this.tcAdsClient_0.ReadAny(num, typeof(uint));
					return xktResult;
				case ComplexDataType.Float:
					num = this.tcAdsClient_0.CreateVariableHandle(address);
					xktResult.Content = (float)this.tcAdsClient_0.ReadAny(num, typeof(float));
					return xktResult;
				case ComplexDataType.Double:
					num = this.tcAdsClient_0.CreateVariableHandle(address);
					xktResult.Content = (double)this.tcAdsClient_0.ReadAny(num, typeof(double));
					return xktResult;
				case ComplexDataType.Long:
					num = this.tcAdsClient_0.CreateVariableHandle(address);
					xktResult.Content = (long)this.tcAdsClient_0.ReadAny(num, typeof(long));
					return xktResult;
				case ComplexDataType.ULong:
					num = this.tcAdsClient_0.CreateVariableHandle(address);
					xktResult.Content = (ulong)this.tcAdsClient_0.ReadAny(num, typeof(ulong));
					return xktResult;
				case ComplexDataType.String:
					if (address.Contains('|'))
					{
						string[] array = address.Split(new char[]
						{
							'|'
						});
						if (array.Length == 2)
						{
							int num2 = 0;
							if (int.TryParse(array[1], out num2))
							{
								num = this.tcAdsClient_0.CreateVariableHandle(array[0]);
								xktResult.Content = this.tcAdsClient_0.ReadAny(num, typeof(string), new int[]
								{
									num2
								}).ToString().Substring(1);
								return xktResult;
							}
						}
					}
					xktResult.IsSuccess = false;
					xktResult.Message = "字符串地址格式不正确";
					return xktResult;
				}
				xktResult.IsSuccess = false;
				xktResult.Message = "不支持数据类型";
				result = xktResult;
			}
			catch (Exception ex)
			{
				xktResult.IsSuccess = false;
				xktResult.Message = ex.Message;
				result = xktResult;
			}
			finally
			{
				this.tcAdsClient_0.DeleteVariableHandle(num);
			}
			return result;
		}
        
		public CalResult Write(string address, object value, ComplexDataType vartype)
		{
			CalResult xktResult = new CalResult();
			try
			{
				switch (vartype)
				{
				case ComplexDataType.Bool:
				{
					int num = this.tcAdsClient_0.CreateVariableHandle(address);
					this.tcAdsClient_0.WriteAny(num, value.ToString() == "1" || value.ToString().ToLower() == "true");
					this.tcAdsClient_0.DeleteVariableHandle(num);
					return CalResult.CreateSuccessResult();
				}
				case ComplexDataType.Byte:
				{
					int num = this.tcAdsClient_0.CreateVariableHandle(address);
					this.tcAdsClient_0.WriteAny(num, Convert.ToByte(value));
					this.tcAdsClient_0.DeleteVariableHandle(num);
					return CalResult.CreateSuccessResult();
				}
				case ComplexDataType.SByte:
				{
					int num = this.tcAdsClient_0.CreateVariableHandle(address);
					this.tcAdsClient_0.WriteAny(num, Convert.ToSByte(value));
					this.tcAdsClient_0.DeleteVariableHandle(num);
					return CalResult.CreateSuccessResult();
				}
				case ComplexDataType.Short:
				{
					int num = this.tcAdsClient_0.CreateVariableHandle(address);
					this.tcAdsClient_0.WriteAny(num, Convert.ToInt16(value));
					this.tcAdsClient_0.DeleteVariableHandle(num);
					return CalResult.CreateSuccessResult();
				}
				case ComplexDataType.UShort:
				{
					int num = this.tcAdsClient_0.CreateVariableHandle(address);
					this.tcAdsClient_0.WriteAny(num, Convert.ToUInt16(value));
					this.tcAdsClient_0.DeleteVariableHandle(num);
					return CalResult.CreateSuccessResult();
				}
				case ComplexDataType.Int:
				{
					int num = this.tcAdsClient_0.CreateVariableHandle(address);
					this.tcAdsClient_0.WriteAny(num, Convert.ToInt32(value));
					this.tcAdsClient_0.DeleteVariableHandle(num);
					return CalResult.CreateSuccessResult();
				}
				case ComplexDataType.UInt:
				{
					int num = this.tcAdsClient_0.CreateVariableHandle(address);
					this.tcAdsClient_0.WriteAny(num, Convert.ToUInt32(value));
					this.tcAdsClient_0.DeleteVariableHandle(num);
					return CalResult.CreateSuccessResult();
				}
				case ComplexDataType.Float:
				{
					int num = this.tcAdsClient_0.CreateVariableHandle(address);
					this.tcAdsClient_0.WriteAny(num, Convert.ToSingle(value));
					this.tcAdsClient_0.DeleteVariableHandle(num);
					return CalResult.CreateSuccessResult();
				}
				case ComplexDataType.Double:
				{
					int num = this.tcAdsClient_0.CreateVariableHandle(address);
					this.tcAdsClient_0.WriteAny(num, Convert.ToDouble(value));
					this.tcAdsClient_0.DeleteVariableHandle(num);
					return CalResult.CreateSuccessResult();
				}
				case ComplexDataType.String:
				{
					int num;
					if (address.Contains('|'))
					{
						string[] array = address.Split(new char[]
						{
							'|'
						});
						if (array.Length == 2)
						{
							int num2 = 0;
							if (int.TryParse(array[1], out num2))
							{
								num = this.tcAdsClient_0.CreateVariableHandle(array[0]);
								Encoding ascii = Encoding.ASCII;
								string @string = ascii.GetString(ByteArrayLib.CombineTwoByteArray(new byte[]
								{
									(byte)ascii.GetBytes(value.ToString()).Length
								}, ascii.GetBytes(value.ToString())));
								this.tcAdsClient_0.WriteAny(num, @string, new int[]
								{
									num2
								});
								this.tcAdsClient_0.DeleteVariableHandle(num);
								return CalResult.CreateSuccessResult();
							}
						}
						goto IL_39F;
					}
					num = this.tcAdsClient_0.CreateVariableHandle(address);
					this.tcAdsClient_0.WriteAny(num, value, new int[]
					{
						80
					});
					this.tcAdsClient_0.DeleteVariableHandle(num);
					return CalResult.CreateSuccessResult();
				}
				}
				return CalResult.CreateFailedResult();
			}
			catch (Exception ex)
			{
				xktResult.IsSuccess = false;
				xktResult.Message = ex.Message;
				return xktResult;
			}
			IL_39F:
			return CalResult.CreateFailedResult();
		}
        
		public Beckhoff()
		{
			this.tcAdsClient_0 = new TcAdsClient();
		}
        
		private TcAdsClient tcAdsClient_0;
	}
}
