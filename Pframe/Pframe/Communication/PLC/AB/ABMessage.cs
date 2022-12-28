using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Pframe.Base;

namespace Pframe.PLC.AB
{
	public class ABMessage : IMessage
	{
		public int HeadDataLength
		{
			get
			{
				return 24;
			}
		}
        
		public int GetContentLength()
		{
			return (int)BitConverter.ToUInt16(this.HeadData, 2);
		}
        
		public bool CheckHeadDataLegal(byte[] token)
		{
			return true;
		}
        
		public int GetHeadDataIdentity()
		{
			return 0;
		}
        
		public byte[] HeadData { get; set; }
        
		public byte[] ContentData { get; set; }
		
		public byte[] SendData { get; set; }
               
	}
}
