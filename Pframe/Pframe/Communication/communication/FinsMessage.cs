using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Pframe.Base;

namespace Pframe
{
	public class FinsMessage : IMessage
	{
		public int GetContentLength()
		{
			return BitConverter.ToInt32(new byte[]
			{
				this.HeadData[7],
				this.HeadData[6],
				this.HeadData[5],
				this.HeadData[4]
			}, 0);
		}
        
		public bool CheckHeadDataLegal(byte[] token)
		{
			return this.HeadData != null && (this.HeadData[0] == 70 && this.HeadData[1] == 73 && this.HeadData[2] == 78) && this.HeadData[3] == 83;
		}
        
		public int HeadDataLength
		{
			get
			{
				return 8;
			}
		}
        
		public byte[] HeadData { get; set; }
        
		public byte[] ContentData { get; set; }
        
		public byte[] SendData { get; set; }
        
	}
}
