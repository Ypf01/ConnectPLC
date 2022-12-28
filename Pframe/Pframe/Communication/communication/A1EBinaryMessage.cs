using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Pframe.Base;

namespace Pframe
{
	public class A1EBinaryMessage : IMessage
	{
		public int GetContentLength()
		{
			int result;
			if (this.HeadData[1] == 91)
				result = 2;
			else if (this.HeadData[1] == 0)
			{
				switch (this.HeadData[0])
				{
				case 128:
					result = (int)((this.SendData[10] != 0) ? ((this.SendData[10] + 1) / 2) : 128);
					break;
				case 129:
					result = (int)(this.SendData[10] * 2);
					break;
				case 130:
				case 131:
					result = 0;
					break;
				default:
					result = 0;
					break;
				}
			}
			else
				result = 0;
			return result;
		}

		public bool CheckHeadDataLegal(byte[] token)
		{
			return this.HeadData != null && this.HeadData[0] - this.HeadData[0] == 128;
		}

		public int HeadDataLength
		{
			get
			{
				return 2;
			}
		}

		public byte[] HeadData { get; set; }

		public byte[] ContentData { get; set; }

		public byte[] SendData { get; set; }
        
	}
}
