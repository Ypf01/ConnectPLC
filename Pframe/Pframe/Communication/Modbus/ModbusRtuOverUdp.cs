using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Pframe.Base;
using Pframe.Common;
using Pframe.DataConvert;
using Pframe.Modbus.Message;

namespace Pframe.Modbus
{
	// Token: 0x02000050 RID: 80
	public class ModbusRtuOverUdp : ModbusRtuOverTcpMessage
	{
		public string IpAddress { get; set; }
        
		public int Port { get; set; }
        
		public int ReceiveBufferLength { get; set; }
        
		public ModbusRtuOverUdp(string ipAddress, int port, DataFormat dataFormat = DataFormat.ABCD)
		{
			this.ReceiveBufferLength = 2048;
			this.IpAddress = ipAddress;
			this.Port = port;
			base.DataFormat = dataFormat;
		}
        
		public override byte[] ReadOutputStatus(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
			ModbusMessage modbusMessage = new ModbusMessage
			{
				FunctionCode = FunctionCode.ReadOutputStatus,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				NumberOfPoints = numberOfPoints
			};
			byte[] byte_ = base.BuildMessageFrame(modbusMessage);
			byte[] response = null;
			if (this.method_6(byte_, ref response, new ModbusRTUMsg(modbusMessage), 5000))
			{
				modbusMessage.Response = response;
				if (base.ValidateResponse(modbusMessage))
				{
					return base.ReturnData(modbusMessage);
				}
			}
			return null;
		}
        
		public override byte[] ReadInputStatus(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
			ModbusMessage modbusMessage = new ModbusMessage
			{
				FunctionCode = FunctionCode.ReadInputStatus,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				NumberOfPoints = numberOfPoints
			};
			byte[] byte_ = base.BuildMessageFrame(modbusMessage);
			byte[] response = null;
			if (this.method_6(byte_, ref response, new ModbusRTUMsg(modbusMessage), 5000))
			{
				modbusMessage.Response = response;
				if (base.ValidateResponse(modbusMessage))
				{
					return base.ReturnData(modbusMessage);
				}
			}
			return null;
		}
        
		public override byte[] ReadKeepReg(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
			ModbusMessage modbusMessage = new ModbusMessage
			{
				FunctionCode = FunctionCode.ReadKeepReg,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				NumberOfPoints = numberOfPoints
			};
			byte[] byte_ = base.BuildMessageFrame(modbusMessage);
			byte[] response = null;
			if (this.method_6(byte_, ref response, new ModbusRTUMsg(modbusMessage), 5000))
			{
				modbusMessage.Response = response;
				if (base.ValidateResponse(modbusMessage))
				{
					return base.ReturnData(modbusMessage);
				}
			}
			return null;
		}
        
		public override byte[] ReadInputReg(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
			ModbusMessage modbusMessage = new ModbusMessage
			{
				FunctionCode = FunctionCode.ReadInputReg,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				NumberOfPoints = numberOfPoints
			};
			byte[] byte_ = base.BuildMessageFrame(modbusMessage);
			byte[] response = null;
			if (this.method_6(byte_, ref response, new ModbusRTUMsg(modbusMessage), 5000))
			{
				modbusMessage.Response = response;
				if (base.ValidateResponse(modbusMessage))
				{
					return base.ReturnData(modbusMessage);
				}
			}
			return null;
		}
        
		public override bool ForceCoil(byte slaveAddress, ushort startAddress, bool value)
		{
			ModbusMessage modbusMessage = new ModbusMessage
			{
				FunctionCode = FunctionCode.ForceCoil,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				WriteBool = value
			};
			byte[] byte_ = base.BuildMessageFrame(modbusMessage);
			byte[] response = null;
			if (this.method_6(byte_, ref response, new ModbusRTUMsg(modbusMessage), 5000))
			{
				modbusMessage.Response = response;
				if (base.ValidateResponse(modbusMessage))
				{
					return true;
				}
			}
			return false;
		}
        
		public override bool ForceMultiCoil(byte slaveAddress, ushort startAddress, bool[] data)
		{
			ModbusMessage modbusMessage = new ModbusMessage
			{
				FunctionCode = FunctionCode.ForceMultiCoil,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				NumberOfPoints = (ushort)data.Length,
				WriteData = ByteArrayLib.GetByteArrayFromBoolArray(data)
			};
			byte[] byte_ = base.BuildMessageFrame(modbusMessage);
			byte[] response = null;
			if (this.method_6(byte_, ref response, new ModbusRTUMsg(modbusMessage), 5000))
			{
				modbusMessage.Response = response;
				if (base.ValidateResponse(modbusMessage))
				{
					return true;
				}
			}
			return false;
		}
        
		public override bool PreSetSingleReg(byte slaveAddress, ushort startAddress, byte[] value)
		{
			ModbusMessage modbusMessage = new ModbusMessage
			{
				FunctionCode = FunctionCode.PreSetSingleReg,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				WriteData = value
			};
			byte[] byte_ = base.BuildMessageFrame(modbusMessage);
			byte[] response = null;
			if (this.method_6(byte_, ref response, new ModbusRTUMsg(modbusMessage), 5000))
			{
				modbusMessage.Response = response;
				if (base.ValidateResponse(modbusMessage))
				{
					return true;
				}
			}
			return false;
		}
        
		public override bool PreSetMultiReg(byte slaveAddress, ushort startAddress, byte[] data)
		{
			ModbusMessage modbusMessage = new ModbusMessage
			{
				FunctionCode = FunctionCode.PreSetMultiReg,
				SlaveAddress = slaveAddress,
				StartAddress = startAddress,
				NumberOfPoints = (ushort)(data.Length / 2),
				WriteData = data
			};
			byte[] byte_ = base.BuildMessageFrame(modbusMessage);
			byte[] response = null;
			if (this.method_6(byte_, ref response, new ModbusRTUMsg(modbusMessage), 5000))
			{
				modbusMessage.Response = response;
				if (base.ValidateResponse(modbusMessage))
				{
					return true;
				}
			}
			return false;
		}
        
		private bool method_6(byte[] byte_1, ref byte[] byte_2, IMessage imessage_0 = null, int int_6 = 5000)
		{
			this.InteractiveLock.Enter();
			bool result;
			try
			{
				IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(this.IpAddress), this.Port);
				Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				socket.SendTo(byte_1, byte_1.Length, SocketFlags.None, remoteEP);
				socket.ReceiveTimeout = int_6;
				IPEndPoint ipendPoint = new IPEndPoint(IPAddress.Any, 0);
				EndPoint endPoint = ipendPoint;
				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, int_6);
				byte[] array = new byte[this.ReceiveBufferLength];
				int count = socket.ReceiveFrom(array, ref endPoint);
				byte_2 = array.Take(count).ToArray<byte>();
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			finally
			{
				this.InteractiveLock.Leave();
			}
			return result;
		}        
	}
}
