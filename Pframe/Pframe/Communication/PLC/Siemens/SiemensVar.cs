using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Pframe.Common;

namespace Pframe.PLC.Siemens
{
	public class SiemensVar
	{
		public SiemensVar(string VarAddress, DataType VarType)
		{
			this.VarAddress = VarAddress;
			this.VarType = VarType;
		}
		public SiemensVar(string VarAddress, DataType VarType, object VarValue)
		{
			this.VarAddress = VarAddress;
			this.VarType = VarType;
			this.VarValue = VarValue;
		}
        
		public string VarAddress { get; set; }
        
		public DataType VarType { get; set; }
        
		public object VarValue { get; set; }
        
	}
}
