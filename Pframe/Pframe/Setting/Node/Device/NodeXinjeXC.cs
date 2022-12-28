using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Pframe;
using Pframe.Common;
using Pframe.DataConvert;
using Pframe.PLC.Xinje;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Device
{
	public class NodeXinjeXC : DeviceNode, IXmlConvert
	{
		public NodeXinjeXC()
		{
			this.sw = new Stopwatch();
			this.DeviceGroupList = new List<XinjeXCDeviceGroup>();
			base.Name = "信捷XC系列PLC";
			base.Description = "称重系统1#PLC";
			base.DeviceType = 100;
			this.PortNum = "COM3";
			this.Paud = 19200;
			this.Parity = Parity.Even;
			this.DataBits = "8";
			this.StopBits = StopBits.One;
			this.FirstConnect = true;
			this.SleepTime = 20;
		}
        
		public bool FirstConnect { get; set; }
        
		public bool ConnectState { get; set; }
        
		public long CommRate { get; set; }
        
		public string PortNum { get; set; }
        
		public int Paud { get; set; }

		public Parity Parity { get; set; }
        
		public string DataBits { get; set; }
        
		public StopBits StopBits { get; set; }
        
		public bool IsConnected { get; set; }
        
		public int SleepTime { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.PortNum = element.Attribute("PortNum").Value;
			this.Paud = int.Parse(element.Attribute("Paud").Value);
			this.DataBits = element.Attribute("DataBits").Value;
			this.Parity = (Parity)Enum.Parse(typeof(Parity), element.Attribute("Parity").Value, true);
			this.StopBits = (StopBits)Enum.Parse(typeof(StopBits), element.Attribute("StopBits").Value, true);
			this.SleepTime = int.Parse(element.Attribute("SleepTime").Value);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("PortNum", this.PortNum);
			xelement.SetAttributeValue("Paud", this.Paud);
			xelement.SetAttributeValue("DataBits", this.DataBits);
			xelement.SetAttributeValue("Parity", this.Parity);
			xelement.SetAttributeValue("StopBits", this.StopBits);
			xelement.SetAttributeValue("SleepTime", this.SleepTime);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("端口号", this.PortNum));
			nodeClassRenders.Add(new NodeClassRenderItem("波特率", this.Paud.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("校验位", this.Parity.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("数据位", this.DataBits));
			nodeClassRenders.Add(new NodeClassRenderItem("停止位", this.StopBits.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("延迟时间", this.SleepTime.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("激活情况", base.IsActive ? "已激活" : "未激活"));
			nodeClassRenders.Add(new NodeClassRenderItem("连接情况", this.ConnectState ? "已连接" : "未连接"));
			if (this.ConnectState)
			{
				nodeClassRenders.Add(new NodeClassRenderItem("通信周期", this.CommRate.ToString() + "ms"));
			}
			return nodeClassRenders;
		}
        
		public void Start()
		{
			foreach (XinjeXCDeviceGroup gclass in this.DeviceGroupList)
			{
				foreach (XinjeXCVariable xinjeXCVariable in gclass.varList)
				{
					if (xinjeXCVariable.Config.ArchiveEnable)
					{
						this.StoreVarList.Add(xinjeXCVariable);
					}
					if (this.CurrentVarList.ContainsKey(xinjeXCVariable.KeyName))
					{
						this.CurrentVarList[xinjeXCVariable.KeyName] = xinjeXCVariable;
					}
					else
					{
						this.CurrentVarList.Add(xinjeXCVariable.KeyName, xinjeXCVariable);
					}
				}
			}
			this.cts = new CancellationTokenSource();
			Task.Run(new Action(this.GetValue), this.cts.Token);
		}
        
		public void Stop()
		{
			this.cts.Cancel();
		}
        
		private void GetValue()
		{
			while (!this.cts.IsCancellationRequested)
			{
				if (this.IsConnected)
				{
					this.sw.Restart();
					foreach (XinjeXCDeviceGroup gclass in this.DeviceGroupList)
					{
						if (gclass.IsActive)
						{
							XinjeXCStoreArea storeArea = gclass.StoreArea;
							XinjeXCStoreArea xinjeXCStoreArea = storeArea;
							if (xinjeXCStoreArea > XinjeXCStoreArea.C存储区)
							{
								if (xinjeXCStoreArea - XinjeXCStoreArea.D存储区 <= 4)
								{
									CalResult<byte[]> xktResult = this.xinjeXC.ReadBytes(gclass.Start, Convert.ToUInt16(gclass.Length), gclass.SlaveID);
									this.ConnectState = xktResult.IsSuccess;
									if (xktResult.IsSuccess)
									{
										base.ErrorTimes = 0;
										int num = this.GetAdress(gclass.Start.Substring(gclass.Start.IndexOf(gclass.Start.First((char c) => char.IsDigit(c)))), gclass.StoreArea);
										using (List<XinjeXCVariable>.Enumerator enumerator2 = gclass.varList.GetEnumerator())
										{
											while (enumerator2.MoveNext())
											{
												XinjeXCVariable xinjeXCVariable = enumerator2.Current;
												int num2;
												int num3;
												if (this.VerifyXinjeXCAddress(false, gclass.StoreArea, xinjeXCVariable.Start, out num2, out num3))
												{
													num2 -= num;
													num2 *= 2;
													switch (xinjeXCVariable.VarType)
													{
													case DataType.Bool:
														xinjeXCVariable.Value = BitLib.GetBitFrom2ByteArray(xktResult.Content, num2, num3, false);
														break;
													case DataType.Short:
														xinjeXCVariable.Value = ShortLib.GetShortFromByteArray(xktResult.Content, num2, this.xinjeXC.DataFormat);
														break;
													case DataType.UShort:
														xinjeXCVariable.Value = UShortLib.GetUShortFromByteArray(xktResult.Content, num2, this.xinjeXC.DataFormat);
														break;
													case DataType.Int:
														xinjeXCVariable.Value = IntLib.GetIntFromByteArray(xktResult.Content, num2, this.xinjeXC.DataFormat);
														break;
													case DataType.UInt:
														xinjeXCVariable.Value = UIntLib.GetUIntFromByteArray(xktResult.Content, num2, this.xinjeXC.DataFormat);
														break;
													case DataType.Float:
														xinjeXCVariable.Value = FloatLib.GetFloatFromByteArray(xktResult.Content, num2, this.xinjeXC.DataFormat);
														break;
													case DataType.Double:
														xinjeXCVariable.Value = DoubleLib.GetDoubleFromByteArray(xktResult.Content, num2, this.xinjeXC.DataFormat);
														break;
													case DataType.Long:
														xinjeXCVariable.Value = LongLib.GetLongFromByteArray(xktResult.Content, num2, this.xinjeXC.DataFormat);
														break;
													case DataType.ULong:
														xinjeXCVariable.Value = ULongLib.GetULongFromByteArray(xktResult.Content, num2, this.xinjeXC.DataFormat);
														break;
													case DataType.String:
														xinjeXCVariable.Value = StringLib.GetStringFromByteArray(xktResult.Content, num2, num3 * 2, Encoding.ASCII);
														break;
													case DataType.ByteArray:
														xinjeXCVariable.Value = ByteArrayLib.GetByteArray(xktResult.Content, num2, num3 * 2);
														break;
													case DataType.HexString:
														xinjeXCVariable.Value = StringLib.GetHexStringFromByteArray(xktResult.Content, num2, num3 * 2, ' ');
														break;
													}
													xinjeXCVariable.Value = MigrationLib.GetMigrationValue(xinjeXCVariable.Value, xinjeXCVariable.Scale, xinjeXCVariable.Offset);
													base.UpdateCurrentValue(xinjeXCVariable);
												}
											}
											continue;
										}
									}
									int errorTimes = base.ErrorTimes;
									base.ErrorTimes = errorTimes + 1;
									if (base.ErrorTimes >= base.MaxErrorTimes)
									{
										if (SerialPort.GetPortNames().Contains(this.PortNum))
										{
											Thread.Sleep(10);
										}
										else
										{
											this.IsConnected = false;
										}
									}
								}
							}
							else
							{
								CalResult<byte[]> xktResult2 = this.xinjeXC.ReadBytes(gclass.Start, Convert.ToUInt16(gclass.Length), gclass.SlaveID);
								this.ConnectState = xktResult2.IsSuccess;
								if (xktResult2.IsSuccess)
								{
									base.ErrorTimes = 0;
									int num4 = this.GetAdress(gclass.Start.Substring(gclass.Start.IndexOf(gclass.Start.First((char c) => char.IsDigit(c)))), gclass.StoreArea);
									using (List<XinjeXCVariable>.Enumerator enumerator3 = gclass.varList.GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											XinjeXCVariable xinjeXCVariable2 = enumerator3.Current;
											int num5;
											int num6;
											if (this.VerifyXinjeXCAddress(true, gclass.StoreArea, xinjeXCVariable2.Start, out num5, out num6))
											{
												if (xinjeXCVariable2.VarType == DataType.Bool)
												{
													num5 -= num4;
													xinjeXCVariable2.Value = BitLib.GetBitArrayFromByteArray(xktResult2.Content, false)[num5];
												}
												base.UpdateCurrentValue(xinjeXCVariable2);
											}
										}
										continue;
									}
								}
								int errorTimes = base.ErrorTimes;
								base.ErrorTimes = errorTimes + 1;
								if (base.ErrorTimes >= base.MaxErrorTimes)
								{
									if (SerialPort.GetPortNames().Contains(this.PortNum))
									{
										Thread.Sleep(10);
									}
									else
									{
										this.IsConnected = false;
									}
								}
							}
						}
					}
					this.CommRate = this.sw.ElapsedMilliseconds;
				}
				else
				{
					if (!this.FirstConnect)
					{
						Thread.Sleep(base.ReConnectTime);
						XinjeXCModbus xinjeXCModbus = this.xinjeXC;
						if (xinjeXCModbus != null)
						{
							xinjeXCModbus.DisConnect();
						}
					}
					this.xinjeXC = new XinjeXCModbus();
					this.xinjeXC.SleepTime = this.SleepTime;
					this.IsConnected = this.xinjeXC.Connect(this.PortNum, this.Paud, int.Parse(this.DataBits), this.Parity, this.StopBits);
					this.FirstConnect = false;
				}
			}
		}
        
		private int GetAdress(string string_5, XinjeXCStoreArea xinjeXCStoreArea_0)
		{
			int result;
			switch (xinjeXCStoreArea_0)
			{
			case XinjeXCStoreArea.M存储区:
				result = Convert.ToInt32(string_5, XinjeXCDataType.M.FromBase);
				break;
			case XinjeXCStoreArea.X存储区:
				result = Convert.ToInt32(string_5, XinjeXCDataType.X.FromBase);
				break;
			case XinjeXCStoreArea.Y存储区:
				result = Convert.ToInt32(string_5, XinjeXCDataType.Y.FromBase);
				break;
			case XinjeXCStoreArea.S存储区:
				result = Convert.ToInt32(string_5, XinjeXCDataType.S.FromBase);
				break;
			case XinjeXCStoreArea.T存储区:
				result = Convert.ToInt32(string_5, XinjeXCDataType.T.FromBase);
				break;
			case XinjeXCStoreArea.C存储区:
				result = Convert.ToInt32(string_5, XinjeXCDataType.C.FromBase);
				break;
			case XinjeXCStoreArea.D存储区:
				result = Convert.ToInt32(string_5, XinjeXCDataType.D.FromBase);
				break;
			case XinjeXCStoreArea.TD存储区:
				result = Convert.ToInt32(string_5, XinjeXCDataType.TD.FromBase);
				break;
			case XinjeXCStoreArea.CD存储区:
				result = Convert.ToInt32(string_5, XinjeXCDataType.CD.FromBase);
				break;
			case XinjeXCStoreArea.FD存储区:
				result = Convert.ToInt32(string_5, XinjeXCDataType.FD.FromBase);
				break;
			case XinjeXCStoreArea.ED存储区:
				result = Convert.ToInt32(string_5, XinjeXCDataType.ED.FromBase);
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}
        
		public bool VerifyXinjeXCAddress(bool isBoolStore, XinjeXCStoreArea store, string address, out int start, out int offset)
		{
			bool result;
			if (isBoolStore)
			{
				offset = 0;
				start = 0;
				try
				{
					start = this.GetAdress(address, store);
				}
				catch (Exception)
				{
					return false;
				}
				result = true;
			}
			else if (address.Contains('.'))
			{
				string[] array = address.Split(new char[]
				{
					'.'
				});
				offset = 0;
				start = 0;
				if (array.Length == 2)
				{
					try
					{
						start = this.GetAdress(array[0], store);
					}
					catch (Exception)
					{
						return false;
					}
					try
					{
						offset = int.Parse(array[1]);
					}
					catch (Exception)
					{
						return false;
					}
					result = true;
				}
				else
				{
					start = 0;
					offset = 0;
					result = false;
				}
			}
			else
			{
				offset = 0;
				result = int.TryParse(address, out start);
			}
			return result;
		}
        
		public CalResult Write(string keyName, string setValue)
		{
			CalResult result;
			if (!this.CurrentVarList.ContainsKey(keyName))
			{
				result = new CalResult
				{
					IsSuccess = false,
					Message = "无法通过变量名称获取到变量"
				};
			}
			else
			{
				XinjeXCVariable xinjeXCVariable = this.CurrentVarList[keyName] as XinjeXCVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(xinjeXCVariable, xinjeXCVariable.VarType, setValue);
				if (xktResult.IsSuccess)
				{
					result = this.xinjeXC.Write(xinjeXCVariable.VarAddress, xktResult.Content, xinjeXCVariable.VarType, xinjeXCVariable.GroupID);
				}
				else
				{
					result = xktResult;
				}
			}
			return result;
		}
        
		public CancellationTokenSource cts;
        
		public Stopwatch sw;
        
		public const int Paud9600 = 9600;
        
		public const int Paud19200 = 19200;
        
		public const int Paud38400 = 38400;
        
		public const string DataBitsSeven = "7";
        
		public const string DataBitsEight = "8";
        
		public XinjeXCModbus xinjeXC;
        
		public List<XinjeXCDeviceGroup> DeviceGroupList;
	}
}
