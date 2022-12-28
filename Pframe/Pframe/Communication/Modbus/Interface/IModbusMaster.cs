using System;

namespace Pframe.Modbus.Interface
{
	public interface IModbusMaster
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="startAddress"></param>
        /// <param name="numberOfPoints"></param>
        /// <returns></returns>
		byte[] ReadOutputStatus(byte slaveAddress, ushort startAddress, ushort numberOfPoints);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="startAddress"></param>
        /// <param name="numberOfPoints"></param>
        /// <returns></returns>
		byte[] ReadInputStatus(byte slaveAddress, ushort startAddress, ushort numberOfPoints);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="startAddress"></param>
        /// <param name="numberOfPoints"></param>
        /// <returns></returns>
		byte[] ReadKeepReg(byte slaveAddress, ushort startAddress, ushort numberOfPoints);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="startAddress"></param>
        /// <param name="numberOfPoints"></param>
        /// <returns></returns>
		byte[] ReadInputReg(byte slaveAddress, ushort startAddress, ushort numberOfPoints);
        /// <summary>
        /// 预置单线圈
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="coilAddress">线圈地址</param>
        /// <param name="value">写入值</param>
        /// <returns></returns>
		bool ForceCoil(byte slaveAddress, ushort coilAddress, bool value);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="registerAddress"></param>
        /// <param name="value"></param>
        /// <returns></returns>
		bool PreSetSingleReg(byte slaveAddress, ushort registerAddress, byte[] value);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="startAddress"></param>
        /// <param name="data"></param>
        /// <returns></returns>
		bool ForceMultiCoil(byte slaveAddress, ushort startAddress, bool[] data);

		bool PreSetMultiReg(byte slaveAddress, ushort startAddress, byte[] data);
	}
}
