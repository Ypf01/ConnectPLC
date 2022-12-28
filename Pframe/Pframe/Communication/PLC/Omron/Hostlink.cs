using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Pframe.Base;
using Pframe.Common;

namespace Pframe.PLC.Omron
{
	public class Hostlink : SerialDeviceBase
	{
		public Hostlink(DataFormat dataFormat = DataFormat.CDAB)
		{
			this.ICF = 0;
			this.DA2 = 0;
			this.SID = 0;
			this.ResponseWaitTime = 48;
			this.hexCharList = new List<char>
			{
				'0',
				'1',
				'2',
				'3',
				'4',
				'5',
				'6',
				'7',
				'8',
				'9',
				'A',
				'B',
				'C',
				'D',
				'E',
				'F'
			};
			base.DataFormat = dataFormat;
		}
        
		public byte ICF { get; set; }
        
		public byte DA2 { get; set; }
        
		public byte SA2 { get; set; }
        
		public byte SID { get; set; }
        
		public byte ResponseWaitTime { get; set; }
        
		public byte UnitNumber { get; set; }
        
		public override CalResult<byte[]> ReadByteArray(string address, ushort length)
		{
			byte[] array = this.BuildReadCommand(address, length, false);
			CalResult<byte[]> result;
			if (array == null)
			{
				result = null;
			}
			else
			{
				byte[] byte_ = null;
				if (base.SendAndReceive(this.PackCommand(array), ref byte_, 0))
				{
					result = this.ResponseValidAnalysis(byte_, true);
				}
				else
				{
					result = CalResult.CreateFailedResult<byte[]>(new CalResult());
				}
			}
			return result;
		}
        
		public override CalResult<bool[]> ReadBoolArray(string address, ushort length)
		{
			byte[] array = this.BuildReadCommand(address, length, true);
			CalResult<bool[]> result;
			if (array == null)
			{
				result = null;
			}
			else
			{
				byte[] byte_ = null;
				if (base.SendAndReceive(this.PackCommand(array), ref byte_, 0))
				{
					result = CalResult.CreateSuccessResult<bool[]>((from m in this.ResponseValidAnalysis(byte_, true).Content
					select m != 0).ToArray<bool>());
				}
				else
				{
					result = CalResult.CreateFailedResult<bool[]>(new CalResult());
				}
			}
			return result;
		}
        
		public override CalResult WriteBoolArray(string address, bool[] values)
		{
			byte[] array = this.BuildWriteWordCommand(address, values.Select(new Func<bool, byte>(c => (byte)(c ? 1 : 0))).ToArray<byte>(), true);
			CalResult result;
			if (array == null)
			{
				result = CalResult.CreateFailedResult();
			}
			else
			{
				byte[] byte_ = null;
				if (base.SendAndReceive(this.PackCommand(array), ref byte_, 0))
					result = this.ResponseValidAnalysis(byte_, false);
				else
					result = CalResult.CreateFailedResult();
			}
			return result;
		}
        
		public override CalResult WriteByteArray(string address, byte[] value)
		{
			byte[] array = this.BuildWriteWordCommand(address, value, false);
			CalResult result;
			if (array == null)
			{
				result = CalResult.CreateFailedResult();
			}
			else
			{
				byte[] byte_ = null;
				if (base.SendAndReceive(this.PackCommand(array), ref byte_, 0))
				{
					result = this.ResponseValidAnalysis(byte_, false);
				}
				else
				{
					result = CalResult.CreateFailedResult();
				}
			}
			return result;
		}
        
		private CalResult<byte[]> ResponseValidAnalysis(byte[] response, bool isRead)
		{
			CalResult<byte[]> result;
			if (response.Length >= 27)
			{
				int num = Convert.ToInt32(Encoding.ASCII.GetString(response, 19, 4));
				byte[] array = null;
				if (response.Length > 27)
					array = this.HexStringToBytes(Encoding.ASCII.GetString(response, 23, response.Length - 27));
				if (num > 0)
				{
					result = new CalResult<byte[]>
					{
						ErrorCode = num,
						Content = array
					};
				}
				else
					result = CalResult.CreateSuccessResult<byte[]>(array);
			}
			else
				result = new CalResult<byte[]>("数据接收错误");
			return result;
		}
        
		private byte[] BuildReadCommand(string address, ushort length, bool isBit)
		{
			CalResult<FinsDataType, byte[]> xktResult = this.AnalysisAddress(address, isBit);
			byte[] result;
			if (!xktResult.IsSuccess)
			{
				result = null;
			}
			else
			{
				byte[] array = new byte[8];
				array[0] = 1;
				array[1] = 1;
				if (isBit)
				{
					array[2] = xktResult.Content1.BitCode;
				}
				else
				{
					array[2] = xktResult.Content1.WordCode;
				}
				xktResult.Content2.CopyTo(array, 3);
				array[6] = (byte)(length / 256);
				array[7] = (byte)(length % 256);
				result = array;
			}
			return result;
		}
        
		private CalResult<FinsDataType, byte[]> AnalysisAddress(string address, bool isBit)
		{
			CalResult<FinsDataType, byte[]> xktResult = new CalResult<FinsDataType, byte[]>();
			try
			{
				char c = address[0];
				char c2 = c;
				if (c2 <= 'W')
				{
					switch (c2)
					{
					case 'A':
						goto IL_8B;
					case 'B':
					case 'F':
					case 'G':
						goto IL_126;
					case 'C':
						goto IL_9B;
					case 'D':
						goto IL_AB;
					case 'E':
						goto IL_BB;
					case 'H':
						goto IL_131;
					default:
						if (c2 != 'W')
						{
							goto IL_126;
						}
						break;
					}
				}
				else
				{
					switch (c2)
					{
					case 'a':
						goto IL_8B;
					case 'b':
					case 'f':
					case 'g':
						goto IL_126;
					case 'c':
						goto IL_9B;
					case 'd':
						goto IL_AB;
					case 'e':
						goto IL_BB;
					case 'h':
						goto IL_131;
					default:
						if (c2 != 'w')
						{
							goto IL_126;
						}
						break;
					}
				}
				xktResult.Content1 = FinsDataType.WR;
				goto IL_13C;
				IL_8B:
				xktResult.Content1 = FinsDataType.AR;
				goto IL_13C;
				IL_9B:
				xktResult.Content1 = FinsDataType.CIO;
				goto IL_13C;
				IL_AB:
				xktResult.Content1 = FinsDataType.DM;
				goto IL_13C;
				IL_BB:
				string[] array = address.Split(new char[]
				{
					'.'
				}, StringSplitOptions.RemoveEmptyEntries);
				int num = Convert.ToInt32(array[0].Substring(1), 16);
				if (num < 16)
				{
					xktResult.Content1 = new FinsDataType((byte)(32 + num), (byte)(160 + num));
					goto IL_13C;
				}
				xktResult.Content1 = new FinsDataType((byte)(224 + num - 16), (byte)(96 + num - 16));
				goto IL_13C;
				IL_126:
				throw new Exception("输入的类型不支持，请重新输入");
				IL_131:
				xktResult.Content1 = FinsDataType.HR;
				IL_13C:
				if (address[0] == 'E' || address[0] == 'e')
				{
					string[] array2 = address.Split(new char[]
					{
						'.'
					}, StringSplitOptions.RemoveEmptyEntries);
					if (isBit)
					{
						ushort value = ushort.Parse(array2[1]);
						xktResult.Content2 = new byte[3];
						xktResult.Content2[0] = BitConverter.GetBytes(value)[1];
						xktResult.Content2[1] = BitConverter.GetBytes(value)[0];
						if (array2.Length > 2)
						{
							xktResult.Content2[2] = byte.Parse(array2[2]);
							if (xktResult.Content2[2] > 15)
							{
								throw new Exception("输入的类型不支持，请重新输入");
							}
						}
					}
					else
					{
						ushort value2 = ushort.Parse(array2[1]);
						xktResult.Content2 = new byte[3];
						xktResult.Content2[0] = BitConverter.GetBytes(value2)[1];
						xktResult.Content2[1] = BitConverter.GetBytes(value2)[0];
					}
				}
				else if (isBit)
				{
					string[] array3 = address.Substring(1).Split(new char[]
					{
						'.'
					}, StringSplitOptions.RemoveEmptyEntries);
					ushort value3 = ushort.Parse(array3[0]);
					xktResult.Content2 = new byte[3];
					xktResult.Content2[0] = BitConverter.GetBytes(value3)[1];
					xktResult.Content2[1] = BitConverter.GetBytes(value3)[0];
					if (array3.Length > 1)
					{
						xktResult.Content2[2] = byte.Parse(array3[1]);
						if (xktResult.Content2[2] > 15)
						{
							throw new Exception("输入的类型不支持，请重新输入");
						}
					}
				}
				else
				{
					ushort value4 = ushort.Parse(address.Substring(1));
					xktResult.Content2 = new byte[3];
					xktResult.Content2[0] = BitConverter.GetBytes(value4)[1];
					xktResult.Content2[1] = BitConverter.GetBytes(value4)[0];
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
        
		private byte[] BuildWriteWordCommand(string address, byte[] value, bool isBit)
		{
			CalResult<FinsDataType, byte[]> xktResult = this.AnalysisAddress(address, isBit);
			byte[] result;
			if (!xktResult.IsSuccess)
			{
				result = null;
			}
			else
			{
				byte[] array = new byte[8 + value.Length];
				array[0] = 1;
				array[1] = 2;
				if (isBit)
				{
					array[2] = xktResult.Content1.BitCode;
				}
				else
				{
					array[2] = xktResult.Content1.WordCode;
				}
				xktResult.Content2.CopyTo(array, 3);
				if (isBit)
				{
					array[6] = (byte)(value.Length / 256);
					array[7] = (byte)(value.Length % 256);
				}
				else
				{
					array[6] = (byte)(value.Length / 2 / 256);
					array[7] = (byte)(value.Length / 2 % 256);
				}
                value.CopyTo(array, 8);
				result = array;
			}
			return result;
		}
        
		private byte[] PackCommand(byte[] cmd)
		{
            cmd = this.BytesToAsciiBytes(cmd);
			byte[] array = new byte[18 + cmd.Length];
			array[0] = 64;
			array[1] = this.BuildAsciiBytesFrom(this.UnitNumber)[0];
			array[2] = this.BuildAsciiBytesFrom(this.UnitNumber)[1];
			array[3] = 70;
			array[4] = 65;
			array[5] = this.ResponseWaitTime;
			array[6] = this.BuildAsciiBytesFrom(this.ICF)[0];
			array[7] = this.BuildAsciiBytesFrom(this.ICF)[1];
			array[8] = this.BuildAsciiBytesFrom(this.DA2)[0];
			array[9] = this.BuildAsciiBytesFrom(this.DA2)[1];
			array[10] = this.BuildAsciiBytesFrom(this.SA2)[0];
			array[11] = this.BuildAsciiBytesFrom(this.SA2)[1];
			array[12] = this.BuildAsciiBytesFrom(this.SID)[0];
			array[13] = this.BuildAsciiBytesFrom(this.SID)[1];
			array[array.Length - 2] = 42;
			array[array.Length - 1] = 13;
            cmd.CopyTo(array, 14);
			int num = (int)array[0];
			for (int i = 1; i < array.Length - 4; i++)
				num ^= (int)array[i];
			array[array.Length - 4] = this.BuildAsciiBytesFrom((byte)num)[0];
			array[array.Length - 3] = this.BuildAsciiBytesFrom((byte)num)[1];
			string @string = Encoding.ASCII.GetString(array);
			Console.WriteLine(@string);
			return array;
		}
		private byte[] HexStringToBytes(string hex)
		{
            hex = hex.ToUpper();
			MemoryStream memoryStream = new MemoryStream();
			for (int i = 0; i < hex.Length; i++)
			{
				if (i + 1 < hex.Length && (this.hexCharList.Contains(hex[i]) && this.hexCharList.Contains(hex[i + 1])))
				{
					memoryStream.WriteByte((byte)(this.hexCharList.IndexOf(hex[i]) * 16 + this.hexCharList.IndexOf(hex[i + 1])));
					i++;
				}
			}
			byte[] result = memoryStream.ToArray();
			memoryStream.Dispose();
			return result;
		}        
		private byte[] BytesToAsciiBytes(byte[] inBytes)
		{
			return Encoding.ASCII.GetBytes(this.ByteToHexString(inBytes));
		}        
		private string ByteToHexString(byte[] InBytes)
		{
			return this.ByteToHexString(InBytes, '\0');
		}       
		private string ByteToHexString(byte[] InBytes, char segment)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in InBytes)
			{
				if (segment == '\0')
				{
					stringBuilder.Append(string.Format("{0:X2}", b));
				}
				else
				{
					stringBuilder.Append(string.Format("{0:X2}{1}", b, segment));
				}
			}
			if (segment != '\0' && stringBuilder.Length > 1 && stringBuilder[stringBuilder.Length - 1] == segment)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			return stringBuilder.ToString();
		}
		private byte[] BuildAsciiBytesFrom(byte value)
		{
			return Encoding.ASCII.GetBytes(value.ToString("X2"));
		}
        
		private List<char> hexCharList;
	}
}
