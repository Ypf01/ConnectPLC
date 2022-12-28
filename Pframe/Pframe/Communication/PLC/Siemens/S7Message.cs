using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Pframe.Base;

namespace Pframe.PLC.Siemens
{
	public class S7Message : IMessage
	{
		public int HeadDataLength
		{
			get
			{
				return 4;
			}
		}
        
		public byte[] HeadData { get; set; }
        
		public byte[] ContentData { get; set; }
        
		public bool CheckHeadDataLegal(byte[] token)
		{
			return this.HeadData != null && this.HeadData[0] == 3 && this.HeadData[1] == 0;
		}
        
		public int GetContentLength()
		{
			byte[] headData = this.HeadData;
			int result;
			if (headData != null && headData.Length >= 4)
				result = (int)this.HeadData[2] * 256 + (int)this.HeadData[3] - 4;
			else
				result = 0;
			return result;
		}
        
		public byte[] SendData { get; set; }
        
	}
}
