using System;
using Pframe.Common;
using Pframe.Modbus;

namespace Pframe.PLC.Delta
{
    /// <summary>
    /// DeltaModbusUdp通信库
    /// </summary>
	public class DeltaModbusUdp : ModbusUdp
	{
		public DeltaModbusUdp(string ipAddress, int port, DataFormat dataFormat = DataFormat.ABCD): base(ipAddress, port, DataFormat.ABCD)
        {
			base.IpAddress = ipAddress;
			base.Port = port;
			base.DataFormat = DataFormat.ABCD;
		}
        
		public override CalResult<byte[]> ReadByteArray(string address, ushort length)
		{
			CalResult<byte[]> xktResult = new CalResult<byte[]>();
			CalResult<ushort, int> xktResult2 = this.AnlysisAddress(address);
			CalResult<byte[]> result;
			if (xktResult2.IsSuccess)
			{
				if (xktResult2.Content2 == 0)
				{
					byte[] array = base.ReadOutputStatus(xktResult2.Content1, length);
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
				else if (xktResult2.Content2 == 1)
				{
					byte[] array2 = base.ReadInputStatus(xktResult2.Content1, length);
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
				else if (xktResult2.Content2 == 4)
				{
					byte[] array3 = base.ReadKeepReg(xktResult2.Content1, length);
					if (array3 != null)
					{
						xktResult.IsSuccess = true;
						xktResult.Content = array3;
						result = xktResult;
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
			}
			else
			{
				result = CalResult.CreateFailedResult<byte[]>(xktResult2);
			}
			return result;
		}
        
		public new CalResult WriteByteArray(string address, byte[] value)
		{
			CalResult<ushort, int> xktResult = this.AnlysisAddress(address);
			CalResult result;
			if (xktResult.IsSuccess)
			{
				if (xktResult.Content2 != 4)
				{
					result = CalResult.CreateFailedResult();
				}
				else if (base.PreSetMultiReg(xktResult.Content1, value))
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
        
		public override CalResult WriteBoolArray(string address, bool[] values)
		{
			CalResult<ushort, int> xktResult = this.AnlysisAddress(address);
			CalResult result;
			if (xktResult.IsSuccess)
			{
				if (xktResult.Content2 != 0)
				{
					result = CalResult.CreateFailedResult();
				}
				else if (base.ForceMultiCoil(xktResult.Content1, values))
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
        
		private CalResult<ushort, int> AnlysisAddress(string address)
		{
			CalResult<ushort, int> xktResult = new CalResult<ushort, int>();
			int num = 0;
			string text = address.Substring(0, 1).ToLower();
			string text2 = text;
			uint num2 = PrivateImplementationDetails.ComputeStringHash(text2);
			if (num2 <= 3893112696U)
			{
				if (num2 != 3775669363U)
				{
					if (num2 != 3859557458U)
					{
						if (num2 == 3893112696U)
						{
							if (text2 == "m")
							{
								if (!this.GetStartAddress(address.Substring(1), DeltaModbusDataType.M.FromBase, ref num))
								{
									xktResult.Message = "地址格式不正确";
									return xktResult;
								}
								if (num > 4095)
								{
									xktResult.Message = "M区不能超过" + 4095.ToString();
									return xktResult;
								}
								xktResult.Content1 = (ushort)(2048 + num);
								xktResult.Content2 = 0;
							}
						}
					}
					else if (text2 == "c")
					{
						if (address.StartsWith("cr") || address.StartsWith("CR"))
						{
							if (!this.GetStartAddress(address.Substring(2), DeltaModbusDataType.CR.FromBase, ref num))
							{
								xktResult.Message = "地址格式不正确";
								return xktResult;
							}
							if (num > 199)
							{
								xktResult.Message = "CR区不能超过" + 199.ToString();
								return xktResult;
							}
							xktResult.Content1 = (ushort)(3584 + num);
							xktResult.Content2 = 4;
						}
						else
						{
							if (!this.GetStartAddress(address.Substring(1), DeltaModbusDataType.C.FromBase, ref num))
							{
								xktResult.Message = "地址格式不正确";
								return xktResult;
							}
							if (num > 199)
							{
								xktResult.Message = "C区不能超过" + 199.ToString();
								return xktResult;
							}
							xktResult.Content1 = (ushort)(3584 + num);
							xktResult.Content2 = 0;
						}
					}
				}
				else if (text2 == "d")
				{
					if (!this.GetStartAddress(address.Substring(1), DeltaModbusDataType.D.FromBase, ref num))
					{
						xktResult.Message = "地址格式不正确";
						return xktResult;
					}
					if (num >= 4096)
					{
						if (num > 9999)
						{
							xktResult.Message = "D区不能超过" + 9999.ToString();
							return xktResult;
						}
						xktResult.Content1 = (ushort)(32768 + num);
						xktResult.Content2 = 4;
					}
					else
					{
						xktResult.Content1 = (ushort)(4096 + num);
						xktResult.Content2 = 4;
					}
				}
			}
			else if (num2 <= 4127999362U)
			{
				if (num2 != 4044111267U)
				{
					if (num2 == 4127999362U)
					{
						if (text2 == "s")
						{
							if (!this.GetStartAddress(address.Substring(1), DeltaModbusDataType.S.FromBase, ref num))
							{
								xktResult.Message = "地址格式不正确";
								return xktResult;
							}
							if (num > 1023)
							{
								xktResult.Message = "S区不能超过" + 1023.ToString();
								return xktResult;
							}
							xktResult.Content1 = (ushort)(0 + num);
							xktResult.Content2 = 0;
						}
					}
				}
				else if (text2 == "t")
				{
					if (address.StartsWith("tr") || address.StartsWith("TR"))
					{
						if (!this.GetStartAddress(address.Substring(2), DeltaModbusDataType.TR.FromBase, ref num))
						{
							xktResult.Message = "地址格式不正确";
							return xktResult;
						}
						if (num > 255)
						{
							xktResult.Message = "TR区不能超过" + 255.ToString();
							return xktResult;
						}
						xktResult.Content1 = (ushort)(1536 + num);
						xktResult.Content2 = 4;
					}
					else
					{
						if (!this.GetStartAddress(address.Substring(1), DeltaModbusDataType.T.FromBase, ref num))
						{
							xktResult.Message = "地址格式不正确";
							return xktResult;
						}
						if (num > 255)
						{
							xktResult.Message = "T区不能超过" + 255.ToString();
							return xktResult;
						}
						xktResult.Content1 = (ushort)(1536 + num);
						xktResult.Content2 = 0;
					}
				}
			}
			else if (num2 != 4228665076U)
			{
				if (num2 == 4245442695U)
				{
					if (text2 == "x")
					{
						if (!this.GetStartAddress(address.Substring(1), DeltaModbusDataType.X.FromBase, ref num))
						{
							xktResult.Message = "地址格式不正确";
							return xktResult;
						}
						if (num > 255)
						{
							xktResult.Message = "X区不能超过" + 255.ToString();
							return xktResult;
						}
						xktResult.Content1 = (ushort)(1024 + num);
						xktResult.Content2 = 1;
					}
				}
			}
			else if (text2 == "y")
			{
				if (!this.GetStartAddress(address.Substring(1), DeltaModbusDataType.Y.FromBase, ref num))
				{
					xktResult.Message = "地址格式不正确";
					return xktResult;
				}
				if (num > 255)
				{
					xktResult.Message = "Y区不能超过" + 255.ToString();
					return xktResult;
				}
				xktResult.Content1 = (ushort)(1280 + num);
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
