using System;
using Pframe.Modbus.Message;

namespace Pframe.Modbus.Interface
{
	public interface IModbusMessage
	{
		byte[] BuildMessageFrame(ModbusMessage Message);

		byte[] ReturnData(ModbusMessage Message);

		bool ValidateResponse(ModbusMessage Message);
	}
}
