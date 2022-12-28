using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Pframe.Base;
using Pframe.Common;
using Pframe.DataConvert;
using Pframe.Modbus.Message;

namespace Pframe
{
	public class ModbusRTUMsg : IMessage
	{
		public ModbusRTUMsg(ModbusMessage message = null)
		{
			this.Message = message;
		}
        
		public ModbusMessage Message { get; set; }
        
		public int HeadDataLength
		{
			get
			{
				return 0;
			}
		}
        
		public byte[] HeadData { get; set; }
        
		public byte[] ContentData { get; set; }
        
		public byte[] SendData { get; set; }
        
		public int GetContentLength()
		{
			if (this.Message != null)
			{
				FunctionCode functionCode = this.Message.FunctionCode;
				FunctionCode functionCode2 = functionCode;
				switch (functionCode2)
				{
				case FunctionCode.ReadOutputStatus:
				case FunctionCode.ReadInputStatus:
					return 5 + IntLib.GetIntFromBoolLength((int)this.Message.NumberOfPoints);
				case FunctionCode.ReadKeepReg:
				case FunctionCode.ReadInputReg:
					return (int)(5 + this.Message.NumberOfPoints * 2);
				case FunctionCode.ForceCoil:
				case FunctionCode.PreSetSingleReg:
					break;
				default:
					if (functionCode2 - FunctionCode.ForceMultiCoil > 1)
					{
						goto IL_6D;
					}
					break;
				}
				return 8;
			}
			IL_6D:
			return 0;
		}
        
		public bool CheckHeadDataLegal(byte[] token)
		{
			return true;
		}
        
	}
}
