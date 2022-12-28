using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Pframe.PLC.Siemens
{
	public class Parameter
	{
		public static byte[] Template { get; }
        
		static Parameter()
		{
			Parameter.Template = new byte[]
			{
				18,
				10,
				16,
				2,
				0,
				0,
				0,
				0,
				132,
				0,
				0,
				0
			};
		}
        
		public static class Offsets
		{
			public const int WordLength = 3;
            
			public const int Amount = 4;
            
			public const int DbNumber = 6;
            
			public const int Area = 8;
            
			public const int Address = 9;
		}
	}
}
