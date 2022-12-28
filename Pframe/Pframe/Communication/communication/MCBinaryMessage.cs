using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Pframe.Base;

namespace Pframe
{
	public class MCBinaryMessage : IMessage
	{
		public int GetContentLength()
		{
			return (int)BitConverter.ToUInt16(this.HeadData, 7);
		}
        
		public bool CheckHeadDataLegal(byte[] token)
		{
			return this.HeadData != null && this.HeadData[0] == 208 && this.HeadData[1] == 0;
		}
        
		public int HeadDataLength
		{
			get
			{
				return 9;
			}
		}
        
		public byte[] HeadData { get; set; }
        
		public byte[] ContentData { get; set; }
        
		public byte[] SendData { get; set; }
        
	}
}
