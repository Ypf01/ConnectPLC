using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Pframe.PLC.Siemens
{
	public class Header
	{
		public static byte[] Template { get; }      
		static Header()
		{
			Header.Template = new byte[]
			{
				3,
				0,
				0,
				0,
				2,
				240,
				128,
				50,
				1,
				0,
				0,
				5,
				0,
				0,
				14,
				0,
				0,
				5,
				0
			};
		}
		public static class Offsets
		{
			public const int MessageLength = 2;
            
			public const int ParameterSize = 13;
            
			public const int DataLength = 15;
            
			public const int ParameterCount = 18;
		}
	}
}
