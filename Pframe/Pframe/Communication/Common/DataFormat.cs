using System;

namespace Pframe.Common
{
	public enum DataFormat
	{
        //按照顺序排序
        ABCD = 0,
        //按照单字反转
        BADC = 1,
        //按照双字反转
        CDAB = 2,
        //按照倒序排序
        DCBA = 3
    }
}
