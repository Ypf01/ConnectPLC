using System;

namespace NodeSettings
{
    /// <summary>
    ///  使用键的形式
    /// </summary>
	public enum KeyWay
    {
        /// <summary>
        /// 变量名称
        /// </summary>
        VarName,
        /// <summary>
        /// 变量地址
        /// </summary>
        VarAddress,
        /// <summary>
        /// 变量描述
        /// </summary>
        VarDescription,
        /// <summary>
        /// 完全限定名1：组名+变量名
        /// </summary>
        GroupVarName,
        /// <summary>
        ///  完全限定名2：设备名+变量名
        /// </summary>
        DeviceVarName,
        /// <summary>
        ///  完全限定名3：设备名+组名+变量名
        /// </summary>
        DeviceGroupVarName
    }
}
