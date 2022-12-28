using System;

namespace Pframe.Base
{
	public interface IMessage
	{
        /// <summary>
        /// 包头长度
        /// </summary>
		int HeadDataLength { get; }
        /// <summary>
        /// 返回的字节长度
        /// </summary>
        /// <returns></returns>
		int GetContentLength();
        /// <summary>
        /// 检查包头是否正确
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
		bool CheckHeadDataLegal(byte[] token);
        /// <summary>
        /// 包头字节
        /// </summary>
		byte[] HeadData { get; set; }
        /// <summary>
        /// 报文字节
        /// </summary>
		byte[] ContentData { get; set; }
        /// <summary>
        /// 发送字节
        /// </summary>
		byte[] SendData { get; set; }
	}
}
