using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using Pframe.Base;
using Pframe.DataConvert;

namespace Pframe.Custom
{
	public class OpenProtocol : NetDeviceBase
	{
		public new bool Connect(string Ip, int Port)
		{
			this.sck=new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, base.ReceiveTimeOut);
			this.sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, base.SendTimeOut);
			IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(Ip), Port);
			IAsyncResult asyncResult = this.sck.BeginConnect(remoteEP, null, null);
			asyncResult.AsyncWaitHandle.WaitOne(base.ConnectTimeOut, true);
			bool result;
			if (!asyncResult.IsCompleted)
			{
				this.sck.Close();
				result = false;
			}
			else
			{
				byte[] sendByte = new byte[]
				{
					48,
					48,
					50,
					48,
					48,
					48,
					48,
					49,
					48,
					48,
					51,
					32,
					32,
					32,
					32,
					32,
					32,
					32,
					32,
					32,
					0
				};
				byte[] source = null;
				if (base.SendAndReceive(sendByte, ref source, null, 5000))
				{
					this.controllerInfo_0 = new OpenProtocol.ControllerInfo();
					this.controllerInfo_0.CellID = Encoding.UTF8.GetString(ByteArrayLib.GetByteArray(source, 22, 4));
					this.controllerInfo_0.ChannelID = Encoding.UTF8.GetString(ByteArrayLib.GetByteArray(source, 28, 2));
					this.controllerInfo_0.ControllerName = Encoding.UTF8.GetString(ByteArrayLib.GetByteArray(source, 32, 25));
					this.controllerInfo_0.SupplierCode = Encoding.UTF8.GetString(ByteArrayLib.GetByteArray(source, 59, 3));
					this.controllerInfo_0.OpenProtocolVersion = Encoding.UTF8.GetString(ByteArrayLib.GetByteArray(source, 64, 19));
					this.controllerInfo_0.ControllerVersion = Encoding.UTF8.GetString(ByteArrayLib.GetByteArray(source, 85, 19));
					this.controllerInfo_0.ToolVersion = Encoding.UTF8.GetString(ByteArrayLib.GetByteArray(source, 106, 19));
				}
				result = this.method_7();
			}
			return result;
		}

		public new void DisConnect()
		{
			this.CommStop();
			Socket socket = this.sck;
			if (socket != null)
			{
				socket.Close();
			}
		}

		public bool CommStop()
		{
			bool result;
			try
			{
				byte[] sendByte = new byte[]
				{
					48,
					48,
					50,
					48,
					48,
					48,
					48,
					51,
					32,
					32,
					32,
					32,
					32,
					32,
					32,
					32,
					32,
					32,
					32,
					32,
					0
				};
				byte[] array = null;
				result = base.SendAndReceive(sendByte, ref array, null, 5000);
			}
			catch
			{
				result = false;
			}
			return result;
		}

		private bool method_7()
		{
			bool result;
			try
			{
				byte[] sendByte = new byte[]
				{
					48,
					48,
					50,
					48,
					48,
					48,
					54,
					48,
					48,
					48,
					49,
					48,
					32,
					32,
					32,
					32,
					32,
					32,
					32,
					32,
					0
				};
				byte[] array = null;
				result = base.SendAndReceive(sendByte, ref array, null, 5000);
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public OpenProtocol()
		{
			this.controllerInfo_0 = null;
		}

		private Socket sck;

		private OpenProtocol.ControllerInfo controllerInfo_0;

		public class ControllerInfo
		{
			public string CellID { get; set; }

			public string ChannelID { get; set; }

			public string ControllerName { get; set; }

			public string SupplierCode { get; set; }

			public string OpenProtocolVersion { get; set; }

			public string ControllerVersion { get; set; }

			public string ToolVersion { get; set; }
		}

		public class TightenData
		{
			public string TightenStatus { get; set; }

			public string TorqueStatus { get; set; }

			public string AngleStatus { get; set; }

			public string Torque { get; set; }

			public string Angle { get; set; }

			public DateTime TimeStamp { get; set; }

			public string BatchStatus { get; set; }

		}
	}
}
