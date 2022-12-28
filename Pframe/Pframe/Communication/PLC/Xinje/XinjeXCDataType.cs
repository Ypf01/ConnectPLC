using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Pframe.PLC.Xinje
{
	public class XinjeXCDataType
	{
		public XinjeXCDataType(int fromBase)
		{
			this.FromBase = fromBase;
		}
        
		public int FromBase { get; private set; }
        
		static XinjeXCDataType()
		{
			XinjeXCDataType.X = new XinjeXCDataType(8);
			XinjeXCDataType.Y = new XinjeXCDataType(8);
			XinjeXCDataType.M = new XinjeXCDataType(10);
			XinjeXCDataType.S = new XinjeXCDataType(10);
			XinjeXCDataType.T = new XinjeXCDataType(10);
			XinjeXCDataType.C = new XinjeXCDataType(10);
			XinjeXCDataType.D = new XinjeXCDataType(10);
			XinjeXCDataType.TD = new XinjeXCDataType(10);
			XinjeXCDataType.CD = new XinjeXCDataType(10);
			XinjeXCDataType.FD = new XinjeXCDataType(10);
			XinjeXCDataType.ED = new XinjeXCDataType(10);
		}
        
        
		public static readonly XinjeXCDataType X;
        
		public static readonly XinjeXCDataType Y;
        
		public static readonly XinjeXCDataType M;
        
		public static readonly XinjeXCDataType S;

		public static readonly XinjeXCDataType T;
        
		public static readonly XinjeXCDataType C;
        
		public static readonly XinjeXCDataType D;
        
		public static readonly XinjeXCDataType TD;
        
		public static readonly XinjeXCDataType CD;
        
		public static readonly XinjeXCDataType FD;
        
		public static readonly XinjeXCDataType ED;
	}
}
