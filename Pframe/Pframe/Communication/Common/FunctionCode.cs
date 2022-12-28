using System;

namespace Pframe.Common
{
	public enum FunctionCode
	{
        //读取输出线圈
        ReadOutputStatus = 1,
        //读取输入线圈
        ReadInputStatus = 2,
        //读取保持寄存器
        ReadKeepReg = 3,
        //读取输入寄存器
        ReadInputReg = 4,
        //预置单线圈
        ForceCoil = 5,
        //预置单寄存器
        PreSetSingleReg = 6,
        //预置多线圈
        ForceMultiCoil = 15,
        //预置多寄存器
        PreSetMultiReg = 16

    }
}
