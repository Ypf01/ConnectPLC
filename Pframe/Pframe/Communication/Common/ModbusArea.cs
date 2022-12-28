using System;

namespace Pframe.Common
{
    /// <summary>
    /// Modbus存储区
    /// </summary>
	public enum ModbusArea
	{
       //保持寄存器
        保持寄存器 = 0,
       //输入寄存器
        输入寄存器 = 1,
       //输出线圈
        输出线圈 = 2,
       //输入线圈
        输入线圈 = 3
    }
}
