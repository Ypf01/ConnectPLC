using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Pframe.Common;

namespace Pframe.Modbus.Message
{
	public class ModbusMessage
	{
		public FunctionCode FunctionCode { get; set; }
        
		public byte SlaveAddress { get; set; }
        
		public ushort StartAddress { get; set; }
        
		public ushort NumberOfPoints { get; set; }
        
		public byte[] WriteData { get; set; }
        
		public bool WriteBool { get; set; }
        
		public byte[] Response { get; set; }
        
		public int DataCount { get; set; }
        
	}
}
