using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Pframe.Base;

namespace Pframe
{
	public class MCAsciiMessage : IMessage
	{
		public int GetContentLength()
		{
			byte[] bytes = new byte[]
			{
				this.HeadData[14],
				this.HeadData[15],
				this.HeadData[16],
				this.HeadData[17]
			};
			return Convert.ToInt32(Encoding.ASCII.GetString(bytes), 16);
		}
        
		public bool CheckHeadDataLegal(byte[] token)
		{
			return this.HeadData != null && (this.HeadData[0] == 68 && this.HeadData[1] == 48 && this.HeadData[2] == 48) && this.HeadData[3] == 48;
		}
        
		public int HeadDataLength
		{
			get
			{
				return 18;
			}
		}
        
		public byte[] HeadData { get; set; }
        
		public byte[] ContentData { get; set; }
        
		public byte[] SendData { get; set; }
        
	}
}
