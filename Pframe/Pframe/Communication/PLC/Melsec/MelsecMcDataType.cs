using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Pframe.PLC.Melsec
{
	public class MelsecMcDataType
	{
		public MelsecMcDataType(byte code, byte type, string asciiCode, int fromBase)
		{
			this.DataCode = 0;
			this.DataType = 0;
			this.DataCode = code;
			this.AsciiCode = asciiCode;
			this.FromBase = fromBase;
			if (type < 2)
				this.DataType = type;
		}
        
		public byte DataCode { get; private set; }
        
		public byte DataType { get; private set; }
        
		public string AsciiCode { get; private set; }
        
		public int FromBase { get; private set; }
        
		static MelsecMcDataType()
		{
			MelsecMcDataType.X = new MelsecMcDataType(156, 1, "X*", 16);
			MelsecMcDataType.Y = new MelsecMcDataType(157, 1, "Y*", 16);
			MelsecMcDataType.X5U = new MelsecMcDataType(156, 1, "X*", 8);
			MelsecMcDataType.Y5U = new MelsecMcDataType(157, 1, "Y*", 8);
			MelsecMcDataType.M = new MelsecMcDataType(144, 1, "M*", 10);
			MelsecMcDataType.D = new MelsecMcDataType(168, 0, "D*", 10);
			MelsecMcDataType.W = new MelsecMcDataType(180, 0, "W*", 16);
			MelsecMcDataType.L = new MelsecMcDataType(146, 1, "L*", 10);
			MelsecMcDataType.F = new MelsecMcDataType(147, 1, "F*", 10);
			MelsecMcDataType.V = new MelsecMcDataType(148, 1, "V*", 10);
			MelsecMcDataType.B = new MelsecMcDataType(160, 1, "B*", 16);
			MelsecMcDataType.R = new MelsecMcDataType(175, 0, "R*", 10);
			MelsecMcDataType.S = new MelsecMcDataType(152, 1, "S*", 10);
			MelsecMcDataType.Z = new MelsecMcDataType(204, 0, "Z*", 10);
			MelsecMcDataType.TN = new MelsecMcDataType(194, 0, "TN", 10);
			MelsecMcDataType.TS = new MelsecMcDataType(193, 1, "TS", 10);
			MelsecMcDataType.TC = new MelsecMcDataType(192, 1, "TC", 10);
			MelsecMcDataType.SS = new MelsecMcDataType(199, 1, "SS", 10);
			MelsecMcDataType.SC = new MelsecMcDataType(198, 1, "SC", 10);
			MelsecMcDataType.SN = new MelsecMcDataType(200, 0, "SN", 100);
			MelsecMcDataType.CN = new MelsecMcDataType(197, 0, "CN", 10);
			MelsecMcDataType.CS = new MelsecMcDataType(196, 1, "CS", 10);
			MelsecMcDataType.CC = new MelsecMcDataType(195, 1, "CC", 10);
			MelsecMcDataType.ZR = new MelsecMcDataType(176, 0, "ZR", 16);
			MelsecMcDataType.Panasonic_X = new MelsecMcDataType(156, 1, "X*", 10);
			MelsecMcDataType.Panasonic_Y = new MelsecMcDataType(157, 1, "Y*", 10);
			MelsecMcDataType.Panasonic_L = new MelsecMcDataType(160, 1, "L*", 10);
			MelsecMcDataType.Panasonic_R = new MelsecMcDataType(144, 1, "R*", 10);
			MelsecMcDataType.Panasonic_DT = new MelsecMcDataType(168, 0, "D*", 10);
			MelsecMcDataType.Panasonic_LD = new MelsecMcDataType(180, 0, "W*", 10);
			MelsecMcDataType.Panasonic_TN = new MelsecMcDataType(194, 0, "TN", 10);
			MelsecMcDataType.Panasonic_TS = new MelsecMcDataType(193, 1, "TS", 10);
			MelsecMcDataType.Panasonic_CN = new MelsecMcDataType(197, 0, "CN", 10);
			MelsecMcDataType.Panasonic_CS = new MelsecMcDataType(196, 1, "CS", 10);
			MelsecMcDataType.Panasonic_SM = new MelsecMcDataType(145, 1, "SM", 10);
			MelsecMcDataType.Panasonic_SD = new MelsecMcDataType(169, 0, "SD", 10);
		}      
        
		public static readonly MelsecMcDataType X;
        
		public static readonly MelsecMcDataType Y;
        
		public static readonly MelsecMcDataType X5U;
        
		public static readonly MelsecMcDataType Y5U;
        
		public static readonly MelsecMcDataType M;
        
		public static readonly MelsecMcDataType D;
        
		public static readonly MelsecMcDataType W;
        
		public static readonly MelsecMcDataType L;
        
		public static readonly MelsecMcDataType F;
        
		public static readonly MelsecMcDataType V;
        
		public static readonly MelsecMcDataType B;
        
		public static readonly MelsecMcDataType R;
        
		public static readonly MelsecMcDataType S;
        
		public static readonly MelsecMcDataType Z;
        
		public static readonly MelsecMcDataType TN;
        
		public static readonly MelsecMcDataType TS;
        
		public static readonly MelsecMcDataType TC;
        
		public static readonly MelsecMcDataType SS;
        
		public static readonly MelsecMcDataType SC;
        
		public static readonly MelsecMcDataType SN;
        
		public static readonly MelsecMcDataType CN;
        
		public static readonly MelsecMcDataType CS;
        
		public static readonly MelsecMcDataType CC;
        
		public static readonly MelsecMcDataType ZR;
        
		public static readonly MelsecMcDataType Panasonic_X;
        
		public static readonly MelsecMcDataType Panasonic_Y;
        
		public static readonly MelsecMcDataType Panasonic_L;
        
		public static readonly MelsecMcDataType Panasonic_R;
        
		public static readonly MelsecMcDataType Panasonic_DT;
        
		public static readonly MelsecMcDataType Panasonic_LD;
        
		public static readonly MelsecMcDataType Panasonic_TN;
        
		public static readonly MelsecMcDataType Panasonic_TS;
        
		public static readonly MelsecMcDataType Panasonic_CN;
        
		public static readonly MelsecMcDataType Panasonic_CS;
        
		public static readonly MelsecMcDataType Panasonic_SM;
        
		public static readonly MelsecMcDataType Panasonic_SD;
	}
}
