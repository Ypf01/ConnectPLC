using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Pframe.PLC.Melsec
{
    /// <summary>
    /// 三菱PLC的数据类型，此处包含了几个常用的类型
    /// </summary>
	public class MelsecA1EDataType
	{
		public MelsecA1EDataType(byte[] code, byte type, string asciiCode, int fromBase)
		{
			this.DataCode = new byte[2];
			this.DataType = 0;
			this.DataCode = code;
			this.AsciiCode = asciiCode;
			this.FromBase = fromBase;
			if (type < 2)
				this.DataType = type;
		}
        
		public byte[] DataCode { get; private set; }
        
		public byte DataType { get; private set; }
        
		public string AsciiCode { get; private set; }
        
		public int FromBase { get; private set; }
        
		static MelsecA1EDataType()
		{
			MelsecA1EDataType.X = new MelsecA1EDataType(new byte[]
			{
				88,
				32
			}, 1, "X*", 8);
			MelsecA1EDataType.Y = new MelsecA1EDataType(new byte[]
			{
				89,
				32
			}, 1, "Y*", 8);
			MelsecA1EDataType.M = new MelsecA1EDataType(new byte[]
			{
				77,
				32
			}, 1, "M*", 10);
			MelsecA1EDataType.S = new MelsecA1EDataType(new byte[]
			{
				83,
				32
			}, 1, "S*", 10);
			MelsecA1EDataType.D = new MelsecA1EDataType(new byte[]
			{
				68,
				32
			}, 0, "D*", 10);
			MelsecA1EDataType.R = new MelsecA1EDataType(new byte[]
			{
				82,
				32
			}, 0, "R*", 10);
		}
        
		public static readonly MelsecA1EDataType X;
        
		public static readonly MelsecA1EDataType Y;
        
		public static readonly MelsecA1EDataType M;
        
		public static readonly MelsecA1EDataType S;
        
		public static readonly MelsecA1EDataType D;
        
		public static readonly MelsecA1EDataType R;
	}
}
