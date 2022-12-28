using System;

namespace Pframe.PLC.Omron
{
	public enum ErrorStatus
	{
		通讯正常,
		消息头不是FINS,
		数据长度太长,
		命令不支持,
		超过连接上限 = 20,
		节点已经处于连接中,
		节点还未配置到PLC中,
		当前客户端的网络节点超过正常范围,
		当前客户端的网络节点已经被使用,
		所有的网络节点已经被使用
	}
}
