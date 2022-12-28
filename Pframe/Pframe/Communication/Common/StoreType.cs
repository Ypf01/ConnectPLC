using System;

namespace Pframe.Common
{
    /// <summary>
    /// 西门子PLC存储区
    /// </summary>
    public enum StoreType
	{
        //Counter存储区
        Counter = 28,
        //Timer存储区
        Timer = 29,
        //I存储区
        Input = 129,
        //Q存储区
        Output = 130,
        //M存储区
        Marker = 131,
        //DB存储区
        DataBlock = 132
    }
}
