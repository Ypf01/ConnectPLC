using System;
using System.Linq;
using System.Text;
using Pframe.Base;
using Pframe.Common;
using Pframe.DataConvert;

namespace Pframe.PLC.Melsec
{
    /// <summary>
    /// FX3U 485扩展模块
    /// </summary>
    public class MelsecFXLink : SerialDeviceBase
	{
		public MelsecFXLink(DataFormat dataFormat = DataFormat.DCBA)
		{
			this.station = 0;
			this.watiingTime = 0;
			this.sumCheck = true;
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
			}
		}
        
		public byte WaittingTime
		{
			get
			{
				return this.watiingTime;
			}
			set
			{
				if (this.watiingTime > 15)
				{
					this.watiingTime = 15;
				}
				else
				{
					this.watiingTime = value;
				}
			}
		}
        
		public bool SumCheck
		{
			get
			{
				return this.sumCheck;
			}
			set
			{
				this.sumCheck = value;
			}
		}
        
		public override CalResult<byte[]> ReadByteArray(string address, ushort length)
		{
			CalResult<byte[]> xktResult = this.BuildReadCommand(this.station, address, length, false, this.sumCheck, this.watiingTime);
			CalResult<byte[]> result;
			if (!xktResult.IsSuccess)
			{
				result = null;
			}
			else
			{
				byte[] array = null;
				if (base.SendAndReceive(xktResult.Content, ref array, 0) && array[0] == 2)
				{
					byte[] array2 = new byte[(int)(length * 2)];
					for (int i = 0; i < array2.Length / 2; i++)
					{
						ushort value = Convert.ToUInt16(Encoding.ASCII.GetString(array, i * 4 + 5, 4), 16);
						BitConverter.GetBytes(value).CopyTo(array2, i * 2);
					}
					result = CalResult.CreateSuccessResult<byte[]>(array2);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}
        
		public override CalResult<bool[]> ReadBoolArray(string address, ushort length)
		{
			CalResult<byte[]> xktResult = this.BuildReadCommand(this.station, address, length, true, this.sumCheck, this.watiingTime);
			CalResult<bool[]> result;
			if (!xktResult.IsSuccess)
			{
				result = null;
			}
			else
			{
				byte[] array = null;
				if (base.SendAndReceive(xktResult.Content, ref array, 0))
				{
					if (array[0] == 2)
					{
						byte[] array2 = new byte[(int)length];
						Array.Copy(array, 5, array2, 0, (int)length);
						result = CalResult.CreateSuccessResult<bool[]>((from m in array2
						select m == 49).ToArray<bool>());
					}
					else
						result = null;
				}
				else
					result = null;
			}
			return result;
		}
        
		public override CalResult WriteBoolArray(string address, bool[] values)
		{
			CalResult<byte[]> xktResult = this.BuildWriteBoolCommand(this.station, address, values, this.sumCheck, this.watiingTime);
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
					result = ((array[0] == 6) ? CalResult.CreateSuccessResult() : CalResult.CreateFailedResult());
				}
				else
				{
					result = CalResult.CreateFailedResult();
				}
			}
			return result;
		}
        
		public override CalResult WriteByteArray(string address, byte[] value)
		{
			CalResult<byte[]> xktResult = this.BuildWriteByteCommand(this.station, address, value, this.sumCheck, this.watiingTime);
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
					result = ((array[0] == 6) ? CalResult.CreateSuccessResult() : CalResult.CreateFailedResult());
				}
				else
				{
					result = CalResult.CreateFailedResult();
				}
			}
			return result;
		}
        
		public bool StartPLC()
		{
			CalResult<byte[]> xktResult = this.BuildStart(this.station, this.sumCheck, this.watiingTime);
			bool result;
			if (!xktResult.IsSuccess)
			{
				result = false;
			}
			else
			{
				byte[] array = null;
				result = (base.SendAndReceive(xktResult.Content, ref array, 0) && array[0] == 6);
			}
			return result;
		}
        
		public bool StopPLC()
		{
			CalResult<byte[]> xktResult = this.BuildStop(this.station, this.sumCheck, this.watiingTime);
			bool result;
			if (!xktResult.IsSuccess)
			{
				result = false;
			}
			else
			{
				byte[] array = null;
				result = (base.SendAndReceive(xktResult.Content, ref array, 0) && array[0] == 6);
			}
			return result;
		}
        
		private CalResult<byte[]> BuildStart(byte station, bool sumCheck = true, byte waitTime = 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(station.ToString("D2"));
			stringBuilder.Append("FF");
			stringBuilder.Append("RR");
			stringBuilder.Append(waitTime.ToString("X"));
			byte[] array;
			if (sumCheck)
			{
				array = Encoding.ASCII.GetBytes(this.CalculateAcc(stringBuilder.ToString()));
			}
			else
			{
				array = Encoding.ASCII.GetBytes(stringBuilder.ToString());
			}
			array = ByteArrayLib.CombineTwoByteArray(new byte[]
			{
				5
			}, array);
			return CalResult.CreateSuccessResult<byte[]>(array);
		}
        
		private CalResult<byte[]> BuildStop(byte station, bool sumCheck = true, byte waitTime = 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(station.ToString("D2"));
			stringBuilder.Append("FF");
			stringBuilder.Append("RS");
			stringBuilder.Append(waitTime.ToString("X"));
			byte[] array;
			if (sumCheck)
			{
				array = Encoding.ASCII.GetBytes(this.CalculateAcc(stringBuilder.ToString()));
			}
			else
			{
				array = Encoding.ASCII.GetBytes(stringBuilder.ToString());
			}
			array = ByteArrayLib.CombineTwoByteArray(new byte[]
			{
				5
			}, array);
			return CalResult.CreateSuccessResult<byte[]>(array);
		}
        
		private CalResult<byte[]> BuildWriteByteCommand(byte station, string address, byte[] value, bool sumCheck = true, byte waitTime = 0)
		{
			CalResult<string> xktResult = this.FxAnalysisAddress(address);
			CalResult<byte[]> result;
			if (!xktResult.IsSuccess)
			{
				result = CalResult.CreateFailedResult<byte[]>(xktResult);
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(station.ToString("D2"));
				stringBuilder.Append("FF");
				stringBuilder.Append("WW");
				stringBuilder.Append(waitTime.ToString("X"));
				stringBuilder.Append(xktResult.Content);
				stringBuilder.Append((value.Length / 2).ToString("D2"));
				byte[] array = new byte[value.Length * 2];
				for (int i = 0; i < value.Length / 2; i++)
				{
					Encoding.ASCII.GetBytes(BitConverter.ToUInt16(value, i * 2).ToString("X4")).CopyTo(array, 4 * i);
				}
				stringBuilder.Append(Encoding.ASCII.GetString(array));
				byte[] array2;
				if (sumCheck)
				{
					array2 = Encoding.ASCII.GetBytes(this.CalculateAcc(stringBuilder.ToString()));
				}
				else
				{
					array2 = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				}
				array2 = ByteArrayLib.CombineTwoByteArray(new byte[]
				{
					5
				}, array2);
				result = CalResult.CreateSuccessResult<byte[]>(array2);
			}
			return result;
		}
        
		private CalResult<byte[]> BuildWriteBoolCommand(byte station, string address, bool[] value, bool sumCheck = true, byte waitTime = 0)
		{
			CalResult<string> xktResult = this.FxAnalysisAddress(address);
			CalResult<byte[]> result;
			if (!xktResult.IsSuccess)
			{
				result = CalResult.CreateFailedResult<byte[]>(xktResult);
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(station.ToString("D2"));
				stringBuilder.Append("FF");
				stringBuilder.Append("BW");
				stringBuilder.Append(waitTime.ToString("X"));
				stringBuilder.Append(xktResult.Content);
				stringBuilder.Append(value.Length.ToString("D2"));
				for (int i = 0; i < value.Length; i++)
				{
					stringBuilder.Append(value[i] ? "1" : "0");
				}
				byte[] array;
				if (sumCheck)
				{
					array = Encoding.ASCII.GetBytes(this.CalculateAcc(stringBuilder.ToString()));
				}
				else
				{
					array = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				}
				array = ByteArrayLib.CombineTwoByteArray(new byte[]
				{
					5
				}, array);
				result = CalResult.CreateSuccessResult<byte[]>(array);
			}
			return result;
		}
        
		private CalResult<byte[]> BuildReadCommand(byte station, string address, ushort length, bool isBool, bool sumCheck = true, byte waitTime = 0)
		{
			CalResult<string> xktResult = this.FxAnalysisAddress(address);
			CalResult<byte[]> result;
			if (!xktResult.IsSuccess)
			{
				result = CalResult.CreateFailedResult<byte[]>(xktResult);
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(station.ToString("D2"));
				stringBuilder.Append("FF");
				if (isBool)
				{
					stringBuilder.Append("BR");
				}
				else
				{
					stringBuilder.Append("WR");
				}
				stringBuilder.Append(waitTime.ToString("X"));
				stringBuilder.Append(xktResult.Content);
				stringBuilder.Append(length.ToString("D2"));
				byte[] array;
				if (sumCheck)
				{
					array = Encoding.ASCII.GetBytes(this.CalculateAcc(stringBuilder.ToString()));
				}
				else
				{
					array = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				}
				array = ByteArrayLib.CombineTwoByteArray(new byte[]
				{
					5
				}, array);
				result = CalResult.CreateSuccessResult<byte[]>(array);
			}
			return result;
		}
        
		private CalResult<string> FxAnalysisAddress(string address)
		{
			CalResult<string> xktResult = new CalResult<string>();
			try
			{
				char c = address[0];
				char c2 = c;
				if (c2 <= 'Y')
				{
					if (c2 <= 'D')
					{
						if (c2 == 'C')
						{
							goto IL_A9;
						}
						if (c2 != 'D')
						{
							goto IL_281;
						}
					}
					else
					{
						if (c2 == 'M')
						{
							goto IL_302;
						}
						switch (c2)
						{
						case 'R':
							goto IL_180;
						case 'S':
							goto IL_1B0;
						case 'T':
							goto IL_1E0;
						case 'U':
						case 'V':
						case 'W':
							goto IL_281;
						case 'X':
							goto IL_28C;
						case 'Y':
							goto IL_2C7;
						default:
							goto IL_281;
						}
					}
				}
				else if (c2 <= 'd')
				{
					if (c2 == 'c')
					{
						goto IL_A9;
					}
					if (c2 != 'd')
					{
						goto IL_281;
					}
				}
				else
				{
					if (c2 == 'm')
					{
						goto IL_302;
					}
					switch (c2)
					{
					case 'r':
						goto IL_180;
					case 's':
						goto IL_1B0;
					case 't':
						goto IL_1E0;
					case 'u':
					case 'v':
					case 'w':
						goto IL_281;
					case 'x':
						goto IL_28C;
					case 'y':
						goto IL_2C7;
					default:
						goto IL_281;
					}
				}
				xktResult.Content = "D" + Convert.ToUInt16(address.Substring(1), 10).ToString("D4");
				goto IL_32D;
				IL_A9:
				if (address[1] == 'S' || address[1] == 's')
				{
					xktResult.Content = "CS" + Convert.ToUInt16(address.Substring(1), 10).ToString("D3");
					goto IL_32D;
				}
				if (address[1] == 'N' || address[1] == 'n')
				{
					xktResult.Content = "CN" + Convert.ToUInt16(address.Substring(1), 10).ToString("D3");
					goto IL_32D;
				}
				throw new Exception("不支持的变量类型");
				IL_180:
				xktResult.Content = "R" + Convert.ToUInt16(address.Substring(1), 10).ToString("D4");
				goto IL_32D;
				IL_1B0:
				xktResult.Content = "S" + Convert.ToUInt16(address.Substring(1), 10).ToString("D4");
				goto IL_32D;
				IL_1E0:
				if (address[1] == 'S' || address[1] == 's')
				{
					xktResult.Content = "TS" + Convert.ToUInt16(address.Substring(1), 10).ToString("D3");
					goto IL_32D;
				}
				if (address[1] == 'N' || address[1] == 'n')
				{
					xktResult.Content = "TN" + Convert.ToUInt16(address.Substring(1), 10).ToString("D3");
					goto IL_32D;
				}
				throw new Exception("不支持的变量类型");
				IL_281:
				throw new Exception("不支持的变量类型");
				IL_28C:
				Convert.ToUInt16(address.Substring(1), 8);
				xktResult.Content = "X" + Convert.ToUInt16(address.Substring(1), 10).ToString("D4");
				goto IL_32D;
				IL_2C7:
				Convert.ToUInt16(address.Substring(1), 8);
				xktResult.Content = "Y" + Convert.ToUInt16(address.Substring(1), 10).ToString("D4");
				goto IL_32D;
				IL_302:
				xktResult.Content = "M" + Convert.ToUInt16(address.Substring(1), 10).ToString("D4");
				IL_32D:;
			}
			catch (Exception ex)
			{
				xktResult.Message = ex.Message;
				return xktResult;
			}
			xktResult.IsSuccess = true;
			return xktResult;
		}
        
		private string CalculateAcc(string data)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(data);
			int num = 0;
			for (int i = 0; i < bytes.Length; i++)
			{
				num += (int)bytes[i];
			}
			return data + num.ToString("X4").Substring(2);
		}
        
		private byte station;
        
		private byte watiingTime;
        
		private bool sumCheck;
	}
}
