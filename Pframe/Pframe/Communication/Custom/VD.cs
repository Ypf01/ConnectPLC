using System;
using System.Text;
using Pframe.Base;
using Pframe.DataConvert;

namespace Pframe.Custom
{
	public class VD : NetDeviceBase
	{
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
		public VDState GetCurrentStatus()
		{
			byte[] array = new byte[512];
			VDState vdstate = new VDState();
			byte[] bytes = Encoding.UTF8.GetBytes("DCP/\r\n");
			if (base.SendAndReceive(bytes, ref array, null, 5000) && (array != null && array.Length > 4) && Encoding.UTF8.GetString(ByteArrayLib.GetByteArray(array, 0, 4)) == "DCP/\r\n".Substring(0, 4))
			{
				vdstate.PowerStatus = (Encoding.UTF8.GetString(array).Substring(4).Trim() == "1");
			}
			bytes = Encoding.UTF8.GetBytes("CUR/\r\n");
			if (base.SendAndReceive(bytes, ref array, null, 5000) && (array != null && array.Length > 4) && Encoding.UTF8.GetString(ByteArrayLib.GetByteArray(array, 0, 4)) == "CUR/\r\n".Substring(0, 4))
			{
				vdstate.OutVoltage = float.Parse(Encoding.UTF8.GetString(array).Substring(6).Trim());
			}
			bytes = Encoding.UTF8.GetBytes("CHN/\r\n");
			if (base.SendAndReceive(bytes, ref array, null, 5000) && (array != null && array.Length > 4) && Encoding.UTF8.GetString(ByteArrayLib.GetByteArray(array, 0, 4)) == "CHN/\r\n".Substring(0, 4))
			{
				vdstate.OutCurrent = float.Parse(Encoding.UTF8.GetString(array).Substring(6).Trim());
			}
			bytes = Encoding.UTF8.GetBytes("VL+/\r\n");
			if (base.SendAndReceive(bytes, ref array, null, 5000) && (array != null && array.Length > 4) && Encoding.UTF8.GetString(ByteArrayLib.GetByteArray(array, 0, 4)) == "VL+/\r\n".Substring(0, 4))
			{
				vdstate.PositiveVoltage = float.Parse(Encoding.UTF8.GetString(array).Substring(6).Trim());
			}
			bytes = Encoding.UTF8.GetBytes("VL-/\r\n");
			if (base.SendAndReceive(bytes, ref array, null, 5000) && (array != null && array.Length > 4) && Encoding.UTF8.GetString(ByteArrayLib.GetByteArray(array, 0, 4)) == "VL-/\r\n".Substring(0, 4))
			{
				vdstate.NegativeVoltage = float.Parse(Encoding.UTF8.GetString(array).Substring(6).Trim());
			}
			bytes = Encoding.UTF8.GetBytes("POL/\r\n");
			if (base.SendAndReceive(bytes, ref array, null, 5000) && (array != null && array.Length > 4) && Encoding.UTF8.GetString(ByteArrayLib.GetByteArray(array, 0, 4)) == "POL/\r\n".Substring(0, 4))
			{
				vdstate.PowerPolarity = (Encoding.UTF8.GetString(array).Substring(4).Trim() == "0");
			}
			bytes = Encoding.UTF8.GetBytes("STA/\r\n");
			if (base.SendAndReceive(bytes, ref array, null, 5000) && (array != null && array.Length > 4) && Encoding.UTF8.GetString(ByteArrayLib.GetByteArray(array, 0, 4)) == "STA/\r\n".Substring(0, 4))
			{
				string text = Encoding.UTF8.GetString(array).Substring(4).Trim();
				if (text.Length == 8)
				{
					vdstate.Remotelocal = BitLib.GetBitFromByte(Convert.ToByte(text.Substring(2, 2), 16), 0);
					vdstate.DCPower = BitLib.GetBitFromByte(Convert.ToByte(text.Substring(2, 2), 16), 6);
					vdstate.OverVoltage = BitLib.GetBitFromByte(Convert.ToByte(text.Substring(4, 2), 16), 0);
					vdstate.OverPower = BitLib.GetBitFromByte(Convert.ToByte(text.Substring(4, 2), 16), 3);
					vdstate.Regulationerror = BitLib.GetBitFromByte(Convert.ToByte(text.Substring(4, 2), 16), 4);
					vdstate.Inrush = BitLib.GetBitFromByte(Convert.ToByte(text.Substring(4, 2), 16), 5);
					vdstate.ARCPlus = BitLib.GetBitFromByte(Convert.ToByte(text.Substring(6, 2), 16), 0);
					vdstate.Phaseerror = BitLib.GetBitFromByte(Convert.ToByte(text.Substring(6, 2), 16), 1);
					vdstate.TemperatureModule1 = BitLib.GetBitFromByte(Convert.ToByte(text.Substring(6, 2), 16), 2);
					vdstate.Externalerror1 = BitLib.GetBitFromByte(Convert.ToByte(text.Substring(6, 2), 16), 3);
					vdstate.Dooropen = BitLib.GetBitFromByte(Convert.ToByte(text.Substring(6, 2), 16), 4);
					vdstate.HVSocket = BitLib.GetBitFromByte(Convert.ToByte(text.Substring(6, 2), 16), 5);
					vdstate.Externalerror2 = BitLib.GetBitFromByte(Convert.ToByte(text.Substring(6, 2), 16), 6);
					vdstate.ARCMinus = BitLib.GetBitFromByte(Convert.ToByte(text.Substring(6, 2), 16), 7);
				}
			}
			return vdstate;
		}

		public bool SetDCP(bool set)
		{
			string text = string.Empty;
			if (set)
			{
				text = "DCP=1\r\n";
			}
			else
			{
				text = "DCP=0\r\n";
			}
			byte[] array = new byte[512];
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			return base.SendAndReceive(bytes, ref array, null, 5000) && array != null && Encoding.UTF8.GetString(array) == text.Replace("\n", "");
		}

		public bool SetPOL(bool set)
		{
			string text = string.Empty;
			if (set)
			{
				text = "POL=1\r\n";
			}
			else
			{
				text = "POL=0\r\n";
			}
			byte[] array = new byte[512];
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			return base.SendAndReceive(bytes, ref array, null, 5000) && array != null && Encoding.UTF8.GetString(array) == text.Replace("\n", "");
		}

		public bool SetCur()
		{
			string text = "CUR=" + this.VoltageSetBack.ToString() + "\r\n";
			byte[] array = new byte[512];
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			return base.SendAndReceive(bytes, ref array, null, 5000) && array != null && Encoding.UTF8.GetString(array) == text.Replace("\r\n", "");
		}

		public bool Reset()
		{
			string text = "RST=0\r\n";
			byte[] array = new byte[512];
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			return base.SendAndReceive(bytes, ref array, null, 5000) && array != null && Encoding.UTF8.GetString(array) == text.Replace("\n", "");
		}

		public bool ResetSTA()
		{
			string text = "STA=0\r\n";
			byte[] array = new byte[512];
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			return base.SendAndReceive(bytes, ref array, null, 5000) && array != null && Encoding.UTF8.GetString(array) == text.Replace("\n", "");
		}

		public bool ResetARC()
		{
			string text = "RAR=0\r\n";
			byte[] array = new byte[512];
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			return base.SendAndReceive(bytes, ref array, null, 5000) && array != null && Encoding.UTF8.GetString(array) == text.Replace("\n", "");
		}

		public VD()
		{
			this.voltageSet = 0f;
			this.VoltageSetBack = 0f;
		}

		private float voltageSet;

		public float VoltageSetBack;
	}
}
