using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Pframe.PLC.Omron
{
	public class FinsDataType
	{
		public FinsDataType(byte bitCode, byte wordCode)
		{
			this.BitCode = bitCode;
			this.WordCode = wordCode;
		}
        
		public byte BitCode { get; private set; }
        
		public byte WordCode { get; private set; }
        
		static FinsDataType()
		{
			FinsDataType.DM = new FinsDataType(2, 130);
			FinsDataType.CIO = new FinsDataType(48, 176);
			FinsDataType.WR = new FinsDataType(49, 177);
			FinsDataType.HR = new FinsDataType(50, 178);
			FinsDataType.AR = new FinsDataType(51, 179);
		}
        
		public static readonly FinsDataType DM;
        
		public static readonly FinsDataType CIO;
        
		public static readonly FinsDataType WR;
        
		public static readonly FinsDataType HR;
        
		public static readonly FinsDataType AR;
	}
}
