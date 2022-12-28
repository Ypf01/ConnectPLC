using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Pframe.PLC.Melsec
{
	public class FXStoreType
	{
		public FXStoreType(byte code, byte type, string asciiCode, int fromBase)
		{
			this.Code = 0;
			this.BitType = 0;
			this.AsciiCode = string.Empty;
			this.FromBase = 10;
			this.Code = code;
			this.AsciiCode = asciiCode;
			this.FromBase = fromBase;
			if (type < 2)
				this.BitType = type;
		}
        
		public byte Code { get; set; }
        
		public byte BitType { get; set; }
        
		public string AsciiCode { get; set; }
        
		public int FromBase { get; set; }
        
		static FXStoreType()
		{
			FXStoreType.X = new FXStoreType(156, 1, "X*", 8);
			FXStoreType.Y = new FXStoreType(157, 1, "Y*", 8);
			FXStoreType.M = new FXStoreType(144, 1, "M*", 10);
			FXStoreType.D = new FXStoreType(168, 0, "D*", 10);
			FXStoreType.S = new FXStoreType(152, 1, "S*", 10);
			FXStoreType.C = new FXStoreType(152, 1, "C*", 10);
			FXStoreType.T = new FXStoreType(152, 1, "T*", 10);
		}
        
		public static FXStoreType X;
        
		public static FXStoreType Y;
        
		public static FXStoreType M;
        
		public static FXStoreType D;
        
		public static FXStoreType S;
        
		public static FXStoreType C;
        
		public static FXStoreType T;
	}
}
