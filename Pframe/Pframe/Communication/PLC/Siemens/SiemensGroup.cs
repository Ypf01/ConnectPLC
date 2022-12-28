using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Pframe.Common;

namespace Pframe.PLC.Siemens
{
	public class SiemensGroup
	{
		public SiemensGroup()
		{
			this.StartByteAdr = 0;
			this.Count = 10;
		}
        
		public SiemensGroup(string groupName, StoreType StoreType, int DB, int StartByteAdr, int Count)
		{
			this.GroupName = groupName;
			this.StoreType = StoreType;
			this.DB = DB;
			this.StartByteAdr = StartByteAdr;
			this.Count = Count;
		}
        
		public string GroupName { get; set; }
        
		public StoreType StoreType { get; set; }
        
		public int DB { get; set; }
		public int StartByteAdr { get; set; }
        
		public int Count { get; set; }
        
		public byte[] Value { get; set; }
        
	}
}
