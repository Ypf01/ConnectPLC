using System;

namespace Pframe.Common
{
    /// <summary>
    /// 链路协议数据类型
    /// </summary>
    public enum LinkDataType
	{
        // 16位无符号
        UShort = 0,
        // 16位有符号
        Short = 1,
        // 32位无符号
        UInt = 2,
        // 32位有符号
        Int = 3,
        // 16位16进制数
        Hex = 4,
        // 默认
        None = 5
    }
}
