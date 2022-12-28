using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Pframe.PLC.Inovance
{
	public class InovanceDataType
	{
		public InovanceDataType(int fromBase)
		{
			this.FromBase = fromBase;
		}
        
		public int FromBase { get; private set; }
        
		static InovanceDataType()
		{
			InovanceDataType.X = new InovanceDataType(8);
			InovanceDataType.Y = new InovanceDataType(8);
			InovanceDataType.M = new InovanceDataType(10);
			InovanceDataType.S = new InovanceDataType(10);
			InovanceDataType.T = new InovanceDataType(10);
			InovanceDataType.C = new InovanceDataType(10);
			InovanceDataType.D = new InovanceDataType(10);
			InovanceDataType.TR = new InovanceDataType(10);
			InovanceDataType.CR = new InovanceDataType(10);
		}
        
		public static readonly InovanceDataType X;
        
		public static readonly InovanceDataType Y;
        
		public static readonly InovanceDataType M;
        
		public static readonly InovanceDataType S;
        
		public static readonly InovanceDataType T;
        
		public static readonly InovanceDataType C;
        
		public static readonly InovanceDataType D;
        
		public static readonly InovanceDataType TR;
        
		public static readonly InovanceDataType CR;
	}
}
