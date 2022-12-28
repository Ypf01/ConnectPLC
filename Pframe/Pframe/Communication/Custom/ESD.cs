using System;
using System.Text;
using Pframe.Base;

namespace Pframe.Custom
{
	public class ESD : NetDeviceBase
	{
		public ESD(float maxVoltage, float maxCurrent)
		{
			this.MaxVoltage = 100f;
			this.MaxCurremt = 2.5f;
			this.voltageSet = 0f;
			this.VoltageSetBack = 0f;
			this.currentSet = 0f;
			this.CurrentSetBack = 0f;
			this.hVON = false;
			this.hVOFF = false;
			this.hVRESET = false;
			this.MaxCurremt = maxCurrent;
			this.MaxVoltage = maxVoltage;
		}

		public float VoltageSet
		{
			get
			{
				return this.voltageSet;
			}
			set
			{
				this.VoltageSetBack = value;
			}
		}

		public float CurrentSet
		{
			get
			{
				return this.currentSet;
			}
			set
			{
				this.CurrentSetBack = value;
			}
		}

		public bool HVON
		{
			get
			{
				return this.hVON;
			}
			set
			{
				if (value)
				{
					this.SetPowerSupply(2);
					this.HVON = false;
				}
			}
		}

		public bool HVOFF
		{
			get
			{
				return this.hVOFF;
			}
			set
			{
				if (value)
				{
					this.SetPowerSupply(1);
					this.HVOFF = false;
				}
			}
		}

		public bool HVRESET
		{
			get
			{
				return this.hVRESET;
			}
			set
			{
				if (value)
				{
					this.SetPowerSupply(4);
					this.HVRESET = false;
				}
			}
		}

		public ESDState GetCurrentStatus()
		{
			byte[] array = new byte[512];
			byte[] sendByte = new byte[]
			{
				1,
				81,
				53,
				49,
				13
			};
			ESDState result;
			if (base.SendAndReceive(sendByte, ref array, null, 5000))
			{
				ESDState esdstate = new ESDState();
				if (array.Length == 16)
				{
					if (array[0] == 82)
					{
						byte[] array2 = this.CheckSum(this.GetByteArray(array, 1, 12));
						if (array2[0] == array[13] && array2[1] == array[14])
						{
							esdstate.ESDCurrent = (float)Convert.ToInt32(Encoding.ASCII.GetString(this.GetByteArray(array, 1, 3)), 16) / 1023f * this.MaxVoltage;
							esdstate.ESDVoltage = (float)Convert.ToInt32(Encoding.ASCII.GetString(this.GetByteArray(array, 4, 3)), 16) / 1023f * this.MaxCurremt;
							esdstate.ESDMode = ((this.GetbitValue(array[10], 0) == "1") ? 1 : 0);
							esdstate.ESDFault = (this.GetbitValue(array[10], 1) == "1");
							esdstate.ESDEnable = (this.GetbitValue(array[10], 2) == "1");
							esdstate.ESDFault = false;
							esdstate.ESDFaultDescrib = "无故障描述";
							result = esdstate;
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
				else if (array.Length == 5)
				{
					if (array[0] == 69)
					{
						esdstate.ESDFault = true;
						esdstate.ESDFaultDescrib = this.GetFaultDes((int)array[1]);
						result = esdstate;
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
			return result;
		}

		private string GetFaultDes(int ErrorCode)
		{
			string result = string.Empty;
			switch (ErrorCode)
			{
			case 1:
				result = "未知控制命令代码";
				break;
			case 2:
				result = "校验码错误";
				break;
			case 3:
				result = "字节接受数量错误";
				break;
			case 4:
				result = "非正常控制命令";
				break;
			case 5:
				result = "电源存在故障";
				break;
			case 6:
				result = "字节处理错误";
				break;
			default:
				result = "其他未知错误";
				break;
			}
			return result;
		}

		public bool SetPowerSupply(int Mode)
		{
			byte[] array = new byte[]
			{
				1,
				83,
				0,
				0,
				0,
				0,
				0,
				0,
				48,
				48,
				48,
				48,
				48,
				48,
				48,
				0,
				0,
				13
			};
			byte[] array2 = this.method_6(Convert.ToInt32(this.VoltageSet * 4095f / this.MaxVoltage));
			array[2] = array2[0];
			array[3] = array2[1];
			array[4] = array2[2];
			byte[] array3 = this.method_6(Convert.ToInt32(this.CurrentSet * 4095f / this.MaxCurremt));
			array[5] = array3[0];
			array[6] = array3[1];
			array[7] = array3[2];
			switch (Mode)
			{
			case 1:
				array[14] = 49;
				break;
			case 2:
				array[14] = 50;
				break;
			case 4:
				array[14] = 52;
				break;
			}
			byte[] array4 = this.CheckSum(this.GetByteArray(array, 1, 14));
			array[15] = array4[0];
			array[16] = array4[1];
			byte[] array5 = new byte[512];
			return base.SendAndReceive(array, ref array5, null, 5000) && array5.Length == 2;
		}

		private byte[] method_6(int int_5)
		{
			byte[] array = new byte[3];
			byte[] bytes = BitConverter.GetBytes(int_5);
			string text = bytes[0].ToString("X");
			if (text.Length == 0)
			{
				array[1] = Encoding.ASCII.GetBytes("0")[0];
				array[2] = Encoding.ASCII.GetBytes("0")[0];
			}
			if (text.Length == 1)
			{
				array[1] = Encoding.ASCII.GetBytes("0")[0];
				array[2] = Encoding.ASCII.GetBytes(text.Substring(0, 1))[0];
			}
			if (text.Length == 2)
			{
				array[1] = Encoding.ASCII.GetBytes(text.Substring(0, 1))[0];
				array[2] = Encoding.ASCII.GetBytes(text.Substring(1, 1))[0];
			}
			string text2 = bytes[1].ToString("X");
			if (text2.Length == 0)
			{
				array[0] = 30;
			}
			else
			{
				array[0] = Encoding.ASCII.GetBytes(text2.Substring(0, 1))[0];
			}
			return array;
		}

		private byte[] CheckSum(byte[] byte_0)
		{
			byte[] array = new byte[2];
			int num = 0;
			foreach (byte b in byte_0)
			{
				num = (num + (int)b) % 65535;
			}
			string text = ((byte)(num & 255)).ToString("X");
			if (text.Length == 0)
			{
				array[0] = Encoding.ASCII.GetBytes("0")[0];
				array[1] = Encoding.ASCII.GetBytes("0")[0];
			}
			if (text.Length == 1)
			{
				array[0] = Encoding.ASCII.GetBytes("0")[0];
				array[1] = Encoding.ASCII.GetBytes(text.Substring(0, 1))[0];
			}
			else
			{
				array[0] = Encoding.ASCII.GetBytes(text.Substring(0, 1))[0];
				array[1] = Encoding.ASCII.GetBytes(text.Substring(1, 1))[0];
			}
			return array;
		}

		private string GetbitValue(byte input, int index)
		{
			string result;
			switch (index)
			{
			case 0:
				result = (input & 1).ToString();
				break;
			case 1:
				result = ((byte)(input >> 1 & 1)).ToString();
				break;
			case 2:
				result = ((byte)(input >> 2 & 1)).ToString();
				break;
			case 3:
				result = ((byte)(input >> 3 & 1)).ToString();
				break;
			case 4:
				result = ((byte)(input >> 4 & 1)).ToString();
				break;
			case 5:
				result = ((byte)(input >> 5 & 1)).ToString();
				break;
			case 6:
				result = ((byte)(input >> 6 & 1)).ToString();
				break;
			case 7:
				result = ((byte)(input >> 7 & 1)).ToString();
				break;
			default:
				result = "-1";
				break;
			}
			return result;
		}

		private byte[] GetByteArray(byte[] byteArr, int start, int length)
		{
			byte[] array = new byte[length];
			if (byteArr != null && byteArr.Length >= length)
			{
				for (int i = 0; i < length; i++)
				{
					array[i] = byteArr[i + start];
				}
			}
			return array;
		}

		private float MaxVoltage;

		private float MaxCurremt;

		private float voltageSet;

		public float VoltageSetBack;

		private float currentSet;

		public float CurrentSetBack;

		private bool hVON;

		private bool hVOFF;

		private bool hVRESET;
	}
}
