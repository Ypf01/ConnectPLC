using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Pframe.PLC.Keyence
{
	public class KeyenceMcDataType
	{
		public KeyenceMcDataType(byte code, byte type, string asciiCode, int fromBase)
		{
			this.DataCode = 0;
			this.DataType = 0;
			this.DataCode = code;
			this.AsciiCode = asciiCode;
			this.FromBase = fromBase;
			if (type < 2)
			{
				this.DataType = type;
			}
		}
        
		public byte DataCode { get; private set; }
        
		public byte DataType { get; private set; }
        
		public string AsciiCode { get; private set; }
        
		public int FromBase { get; private set; }
        
		public static int TranslateKeyenceMCRAddress(string start)
		{
			int result;
			if (start.Length < 3)
				result = Convert.ToInt32(start);
			else
			{
				int num = Convert.ToInt32(start.Substring(0, start.Length - 2));
				int num2 = Convert.ToInt32(start.Substring(start.Length - 2, 2));
				result = num * 16 + num2;
			}
			return result;
		}
        
		static KeyenceMcDataType()
		{
			KeyenceMcDataType.X = new KeyenceMcDataType(156, 1, "X*", 10);
			KeyenceMcDataType.Y = new KeyenceMcDataType(157, 1, "Y*", 10);
			KeyenceMcDataType.M = new KeyenceMcDataType(144, 1, "M*", 10);
			KeyenceMcDataType.D = new KeyenceMcDataType(168, 0, "D*", 10);
			KeyenceMcDataType.W = new KeyenceMcDataType(180, 0, "W*", 16);
			KeyenceMcDataType.L = new KeyenceMcDataType(146, 1, "L*", 10);
			KeyenceMcDataType.F = new KeyenceMcDataType(147, 1, "F*", 10);
			KeyenceMcDataType.V = new KeyenceMcDataType(148, 1, "V*", 10);
			KeyenceMcDataType.B = new KeyenceMcDataType(160, 1, "B*", 16);
			KeyenceMcDataType.R = new KeyenceMcDataType(175, 0, "R*", 10);
			KeyenceMcDataType.S = new KeyenceMcDataType(152, 1, "S*", 10);
			KeyenceMcDataType.Z = new KeyenceMcDataType(204, 0, "Z*", 10);
			KeyenceMcDataType.TN = new KeyenceMcDataType(194, 0, "TN", 10);
			KeyenceMcDataType.TS = new KeyenceMcDataType(193, 1, "TS", 10);
			KeyenceMcDataType.TC = new KeyenceMcDataType(192, 1, "TC", 10);
			KeyenceMcDataType.SS = new KeyenceMcDataType(199, 1, "SS", 10);
			KeyenceMcDataType.SC = new KeyenceMcDataType(198, 1, "SC", 10);
			KeyenceMcDataType.SN = new KeyenceMcDataType(200, 0, "SN", 100);
			KeyenceMcDataType.CN = new KeyenceMcDataType(197, 0, "CN", 10);
			KeyenceMcDataType.CS = new KeyenceMcDataType(196, 1, "CS", 10);
			KeyenceMcDataType.CC = new KeyenceMcDataType(195, 1, "CC", 10);
			KeyenceMcDataType.ZR = new KeyenceMcDataType(176, 0, "ZR", 16);
		}
               
		public static readonly KeyenceMcDataType X;
        
		public static readonly KeyenceMcDataType Y;
        
		public static readonly KeyenceMcDataType M;
        
		public static readonly KeyenceMcDataType D;
        
		public static readonly KeyenceMcDataType W;
        
		public static readonly KeyenceMcDataType L;
        
		public static readonly KeyenceMcDataType F;
        
		public static readonly KeyenceMcDataType V;
        
		public static readonly KeyenceMcDataType B;
        
		public static readonly KeyenceMcDataType R;
        
		public static readonly KeyenceMcDataType S;
        
		public static readonly KeyenceMcDataType Z;
        
		public static readonly KeyenceMcDataType TN;
        
		public static readonly KeyenceMcDataType TS;
        
		public static readonly KeyenceMcDataType TC;
        
		public static readonly KeyenceMcDataType SS;
        
		public static readonly KeyenceMcDataType SC;
        
		public static readonly KeyenceMcDataType SN;
        
		public static readonly KeyenceMcDataType CN;
        
		public static readonly KeyenceMcDataType CS;
        
		public static readonly KeyenceMcDataType CC;
        
		public static readonly KeyenceMcDataType ZR;
	}
}
