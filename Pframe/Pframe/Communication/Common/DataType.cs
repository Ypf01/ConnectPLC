using System;

namespace Pframe.Common
{
	public enum DataType
	{
        //布尔
        Bool = 0,
        //字节
        Byte = 1,
        //有符号16位整型
        Short = 2,
        //无符号16位整型
        UShort = 3,
        //有符号32位整型
        Int = 4,
        //无符号32位整型
        UInt = 5,
        //32位浮点型
        Float = 6,
        //64位浮点型
        Double = 7,
        //有符号64位整型
        Long = 8,
        //无符号64位整型
        ULong = 9,
        //字符串
        String = 10,
        //字节数组
        ByteArray = 11,
        //16进制字符串
        HexString = 12
    }
}
