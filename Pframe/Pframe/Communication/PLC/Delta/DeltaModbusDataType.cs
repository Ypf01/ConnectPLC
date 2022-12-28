using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Pframe.PLC.Delta
{
    /// <summary>
    /// 台达PLC数据类型
    /// </summary>
	public class DeltaModbusDataType
	{
		public DeltaModbusDataType(int fromBase)
		{
			this.FromBase = fromBase;
		}
        
		public int FromBase { get; private set; }
        
		static DeltaModbusDataType()
		{
			DeltaModbusDataType.X = new DeltaModbusDataType(8);
			DeltaModbusDataType.Y = new DeltaModbusDataType(8);
			DeltaModbusDataType.M = new DeltaModbusDataType(10);
			DeltaModbusDataType.S = new DeltaModbusDataType(10);
			DeltaModbusDataType.T = new DeltaModbusDataType(10);
			DeltaModbusDataType.C = new DeltaModbusDataType(10);
			DeltaModbusDataType.D = new DeltaModbusDataType(10);
			DeltaModbusDataType.TR = new DeltaModbusDataType(10);
			DeltaModbusDataType.CR = new DeltaModbusDataType(10);
		}
             
		public static readonly DeltaModbusDataType X;
        
		public static readonly DeltaModbusDataType Y;
        
		public static readonly DeltaModbusDataType M;
        
		public static readonly DeltaModbusDataType S;
        
		public static readonly DeltaModbusDataType T;
        
		public static readonly DeltaModbusDataType C;
        
		public static readonly DeltaModbusDataType D;
        
		public static readonly DeltaModbusDataType TR;
        
		public static readonly DeltaModbusDataType CR;
	}
}
