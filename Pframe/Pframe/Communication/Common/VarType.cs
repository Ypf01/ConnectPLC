using System;

namespace Pframe.Common
{
	/// <summary>
    /// 西门子变量类型
    /// </summary>
	public enum VarType
	{
        //布尔
        Bit = 0,
        //字节
        Byte = 1,
        //Word ushort
        Word = 2,
        //DWord uint
        DWord = 3,
        //Int  short
        Int = 4,
        //DInt int
        DInt = 5,
        //Real float
        Real = 6,
        //LReal
        LReal = 7,
        //Long
        Long = 8,
        //ULong
        ULong = 9,
        //String
        String = 10,
        //Timer
        Timer = 11,
        //Counter
        Counter = 12
    }
}
