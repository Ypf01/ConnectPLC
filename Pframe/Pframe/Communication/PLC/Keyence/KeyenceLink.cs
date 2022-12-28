using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Pframe.Base;
using Pframe.Common;
using Pframe.DataConvert;

namespace Pframe.PLC.Keyence
{
    /// <summary>
    /// 基恩士Link通信库
    /// </summary>
	public class KeyenceLink : NetDeviceBase
	{
		public KeyenceLink(DataFormat DataFormat = DataFormat.DCBA)
		{
			this.encoding = Encoding.ASCII;
			base.DataFormat = DataFormat;
		}
        
		public Encoding encoding { get; set; }
        
		public override CalResult<byte[]> ReadByteArray(string address, ushort length)
		{
			string text = this.Read(address, (int)length, LinkDataType.Short);
			CalResult<byte[]> result;
			if (text == null || text.Length == 0)
			{
				result = CalResult.CreateFailedResult<byte[]>(new CalResult());
			}
			else
			{
				short[] shortArrayFromString = ShortLib.GetShortArrayFromString(text, ' ');
				if (shortArrayFromString != null)
				{
					ByteArray byteArray = new ByteArray();
					foreach (short value in shortArrayFromString)
					{
						byteArray.Add(BitConverter.GetBytes(value));
					}
					result = CalResult.CreateSuccessResult<byte[]>(ByteArrayLib.GetByteArrayFromShortArray(shortArrayFromString, base.DataFormat));
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
			string text = this.Read(address, (int)length, LinkDataType.None);
			CalResult<bool[]> result;
			if (text == null || text.Length == 0)
			{
				result = CalResult.CreateFailedResult<bool[]>(new CalResult());
			}
			else
			{
				bool[] bitArrayFromBitArrayString = BitLib.GetBitArrayFromBitArrayString(text, ' ');
				if (bitArrayFromBitArrayString != null)
				{
					result = CalResult.CreateSuccessResult<bool[]>(bitArrayFromBitArrayString);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}
        
		public override CalResult WriteByteArray(string address, byte[] value)
		{
			CalResult result;
			if (value.Length % 2 != 0)
			{
				result = CalResult.CreateFailedResult();
			}
			else
			{
				short[] shortArrayFromByteArray = ShortLib.GetShortArrayFromByteArray(value, base.DataFormat);
				List<string> list = new List<string>();
				foreach (short num in shortArrayFromByteArray)
				{
					list.Add(num.ToString());
				}
				result = (this.Write(address, list.Count, list, LinkDataType.Short) ? CalResult.CreateSuccessResult() : CalResult.CreateFailedResult());
			}
			return result;
		}
        
		public override CalResult WriteBoolArray(string address, bool[] values)
		{
			CalResult result;
			if (values[0])
			{
				result = (this.SetBool(address, values.Length) ? CalResult.CreateSuccessResult() : CalResult.CreateFailedResult());
			}
			else
			{
				result = (this.RstBool(address, values.Length) ? CalResult.CreateSuccessResult() : CalResult.CreateFailedResult());
			}
			return result;
		}
        
		private string Read(string address, LinkDataType linkDataType_0 = LinkDataType.None)
		{
            address = this.AddressHandle(address);
			string string_ = "RD " + address + this.GetDataType(linkDataType_0) + "\r";
			string text = string.Empty;
			if (this.GetResult(string_, ref text))
			{
				text = text.Replace("\r\n", "");
			}
			return text;
		}
        
		private string Read(string address, int value, LinkDataType linkDataType_0 = LinkDataType.None)
		{
            address = this.AddressHandle(address);
			string string_ = string.Concat(new string[]
			{
				"RDE ",
                address,
				this.GetDataType(linkDataType_0),
				" ",
                value.ToString(),
				"\r"
			});
			string text = string.Empty;
			if (this.GetResult(string_, ref text))
			{
				text = text.Replace("\r\n", "");
			}
			return text;
		}
        
		private bool Write(string address, string value, LinkDataType linkDataType_0 = LinkDataType.None)
		{
            address = this.AddressHandle(address);
			string string_2 = string.Concat(new string[]
			{
				"WR ",
                address,
				this.GetDataType(linkDataType_0),
				" ",
                value,
				"\r"
			});
			string empty = string.Empty;
			return this.GetResult(string_2, ref empty) && empty.Substring(0, 2) == "OK";
		}
        
		private bool Write(string address, int count, List<string> value, LinkDataType linkDataType_0 = LinkDataType.None)
		{
            address = this.AddressHandle(address);
			StringBuilder stringBuilder = new StringBuilder(string.Concat(new string[]
			{
				"WRS ",
                address,
				this.GetDataType(linkDataType_0),
				" ",
                count.ToString()
			}));
			bool result;
			if (count > value.Count)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < count; i++)
					stringBuilder.Append(" " + value[i]);
				stringBuilder.Append("\r");
				string empty = string.Empty;
				result = (this.GetResult(stringBuilder.ToString(), ref empty) && empty.Substring(0, 2) == "OK");
			}
			return result;
		}
        
		private bool SetBool(string address)
		{
            address = this.AddressHandle(address);
			string string_ = "ST " + address + "\r";
			string empty = string.Empty;
			return this.GetResult(string_, ref empty) && empty.Substring(0, 2) == "OK";
		}
        
		private bool SetBool(string address, int count)
		{
            address = this.AddressHandle(address);
			string string_ = string.Concat(new string[]
			{
				"STS ",
                address,
				" ",
                count.ToString(),
				"\r"
			});
			string empty = string.Empty;
			return this.GetResult(string_, ref empty) && empty.Substring(0, 2) == "OK";
		}
        
		private bool RstBool(string address)
		{
            address = this.AddressHandle(address);
			string string_ = "RS " + address + "\r";
			string empty = string.Empty;
			return this.GetResult(string_, ref empty) && empty.Substring(0, 2) == "OK";
		}
        
		private bool RstBool(string address, int count)
		{
            address = this.AddressHandle(address);
			string string_ = string.Concat(new string[]
			{
				"RSS ",
                address,
				" ",
                count.ToString(),
				"\r"
			});
			string empty = string.Empty;
			return this.GetResult(string_, ref empty) && empty.Substring(0, 2) == "OK";
		}
        
		public bool StartPLC()
		{
			string string_ = "M1\r";
			string empty = string.Empty;
			return this.GetResult(string_, ref empty) && empty.Substring(0, 2) == "OK";
		}
        
		public bool StopPLC()
		{
			string string_ = "M0\r";
			string empty = string.Empty;
			return this.GetResult(string_, ref empty) && empty.Substring(0, 2) == "OK";
		}
        
		public bool GetPLCState()
		{
			string string_ = "?M\r";
			string empty = string.Empty;
			return this.GetResult(string_, ref empty) && empty.Substring(0, 2) == "OK";
		}
        
		private string GetDataType(LinkDataType linkDataType_0)
		{
			string result = string.Empty;
			switch (linkDataType_0)
			{
			case LinkDataType.UShort:
				result = ".U";
				break;
			case LinkDataType.Short:
				result = ".S";
				break;
			case LinkDataType.UInt:
				result = ".D";
				break;
			case LinkDataType.Int:
				result = ".L";
				break;
			case LinkDataType.Hex:
				result = ".H";
				break;
			case LinkDataType.None:
				result = "";
				break;
			default:
				result = "";
				break;
			}
			return result;
		}
        
		private string AddressHandle(string address)
		{
			if (address.Contains('X'))
			{
                address = address.Replace("X", "");
			}
			else if (address.Contains('x'))
			{
                address = address.Replace("x", "");
			}
			else if (address.Contains('Y'))
			{
                address = address.Replace("Y", "");
			}
			else if (address.Contains('y'))
			{
                address = address.Replace("y", "");
			}
			return address;
		}
        
		private bool GetResult(string send, ref string resultb)
		{
			byte[] bytes = null;
			bool result;
			if (result = base.SendAndReceive(this.encoding.GetBytes(send), ref bytes, null, 5000))
			{
                resultb = this.encoding.GetString(bytes);
			}
			return result;
		}
        
	}
}
