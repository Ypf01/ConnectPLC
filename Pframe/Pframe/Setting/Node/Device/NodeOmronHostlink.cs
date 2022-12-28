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
using Pframe.PLC.Omron;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Device
{
	public class NodeOmronHostlink : DeviceNode, IXmlConvert
	{
		public NodeOmronHostlink()
		{
			this.sw = new Stopwatch();
			this.DeviceGroupList = new List<OmronHostlinkDeviceGroup>();
			base.Name = "欧姆龙PLC Hostlink";
			base.Description = "液压系统1#PLC";
			base.DeviceType = 60;
			this.PortNum = "COM3";
			this.Paud = 9600;
			this.Parity = Parity.Even;
			this.DataBits = "7";
			this.StopBits = StopBits.One;
			this.PLCStation = 0;
			this.PLCUnit = 0;
			this.PCUnit = 0;
			this.DeviceNum = 0;
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
        
		public int PLCStation { get; set; }
        
		public int PLCUnit { get; set; }
        
		public int SleepTime { get; set; }
        
		public int PCUnit { get; set; }
        
		public int DeviceNum { get; set; }
        
		public bool IsConnected { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.PortNum = element.Attribute("PortNum").Value;
			this.Paud = int.Parse(element.Attribute("Paud").Value);
			this.DataBits = element.Attribute("DataBits").Value;
			this.Parity = (Parity)Enum.Parse(typeof(Parity), element.Attribute("Parity").Value, true);
			this.StopBits = (StopBits)Enum.Parse(typeof(StopBits), element.Attribute("StopBits").Value, true);
			this.PLCStation = int.Parse(element.Attribute("PLCStation").Value);
			this.PLCUnit = int.Parse(element.Attribute("PLCUnit").Value);
			this.PCUnit = int.Parse(element.Attribute("PCUnit").Value);
			this.SleepTime = int.Parse(element.Attribute("SleepTime").Value);
			this.DeviceNum = int.Parse(element.Attribute("DeviceNum").Value);
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
			xelement.SetAttributeValue("PLCStation", this.PLCStation);
			xelement.SetAttributeValue("PLCUnit", this.PLCUnit);
			xelement.SetAttributeValue("PCUnit", this.PCUnit);
			xelement.SetAttributeValue("DeviceNum", this.DeviceNum);
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
			nodeClassRenders.Add(new NodeClassRenderItem("PLC站号", this.PLCStation.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("PLC单元号", this.PLCUnit.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("PC单元号", this.PCUnit.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("设备标识符", this.DeviceNum.ToString()));
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
			foreach (OmronHostlinkDeviceGroup omronHostlinkDeviceGroup in this.DeviceGroupList)
			{
				foreach (OmronHostlinkVariable omronHostlinkVariable in omronHostlinkDeviceGroup.varList)
				{
					if (omronHostlinkVariable.Config.ArchiveEnable)
					{
						this.StoreVarList.Add(omronHostlinkVariable);
					}
					if (this.CurrentVarList.ContainsKey(omronHostlinkVariable.KeyName))
					{
						this.CurrentVarList[omronHostlinkVariable.KeyName] = omronHostlinkVariable;
					}
					else
					{
						this.CurrentVarList.Add(omronHostlinkVariable.KeyName, omronHostlinkVariable);
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
					foreach (OmronHostlinkDeviceGroup omronHostlinkDeviceGroup in this.DeviceGroupList)
					{
						if (omronHostlinkDeviceGroup.IsActive)
						{
							OmronStoreArea storeArea = omronHostlinkDeviceGroup.StoreArea;
							OmronStoreArea omronStoreArea = storeArea;
							if (omronStoreArea <= OmronStoreArea.E2存储区)
							{
								byte[] array = this.hostlink.ReadBytes(omronHostlinkDeviceGroup.Start, Convert.ToUInt16(omronHostlinkDeviceGroup.Length));
								this.ConnectState = (array != null);
								if (array != null)
								{
									base.ErrorTimes = 0;
									int omronStartByte = this.GetOmronStartByte(omronHostlinkDeviceGroup.Start);
									using (List<OmronHostlinkVariable>.Enumerator enumerator2 = omronHostlinkDeviceGroup.varList.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											OmronHostlinkVariable omronHostlinkVariable = enumerator2.Current;
											int num;
											int num2;
											if (this.VerifyHostlinkAddress(omronHostlinkVariable.Start, out num, out num2))
											{
												num -= omronStartByte;
												num *= 2;
												switch (omronHostlinkVariable.VarType)
												{
												case DataType.Bool:
													omronHostlinkVariable.Value = BitLib.GetBitFrom2ByteArray(array, num, num2, true);
													break;
												case DataType.Short:
													omronHostlinkVariable.Value = ShortLib.GetShortFromByteArray(array, num, this.hostlink.DataFormat);
													break;
												case DataType.UShort:
													omronHostlinkVariable.Value = UShortLib.GetUShortFromByteArray(array, num, this.hostlink.DataFormat);
													break;
												case DataType.Int:
													omronHostlinkVariable.Value = IntLib.GetIntFromByteArray(array, num, this.hostlink.DataFormat);
													break;
												case DataType.UInt:
													omronHostlinkVariable.Value = UIntLib.GetUIntFromByteArray(array, num, this.hostlink.DataFormat);
													break;
												case DataType.Float:
													omronHostlinkVariable.Value = FloatLib.GetFloatFromByteArray(array, num, this.hostlink.DataFormat);
													break;
												case DataType.Double:
													omronHostlinkVariable.Value = DoubleLib.GetDoubleFromByteArray(array, num, this.hostlink.DataFormat);
													break;
												case DataType.String:
													omronHostlinkVariable.Value = StringLib.GetStringFromByteArray(array, num, num2 * 2, Encoding.ASCII);
													break;
												case DataType.ByteArray:
													omronHostlinkVariable.Value = ByteArrayLib.GetByteArray(array, num, num2 * 2);
													break;
												case DataType.HexString:
													omronHostlinkVariable.Value = StringLib.GetHexStringFromByteArray(array, num, num2 * 2, ' ');
													break;
												}
												omronHostlinkVariable.Value = MigrationLib.GetMigrationValue(omronHostlinkVariable.Value, omronHostlinkVariable.Scale, omronHostlinkVariable.Offset);
												base.UpdateCurrentValue(omronHostlinkVariable);
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
						Hostlink hostlink = this.hostlink;
						if (hostlink != null)
						{
							hostlink.DisConnect();
						}
					}
					this.hostlink = new Hostlink(DataFormat.CDAB);
					this.hostlink.SleepTime = this.SleepTime;
					this.hostlink.UnitNumber = (byte)this.PLCStation;
					this.hostlink.DA2 = (byte)this.PLCUnit;
					this.hostlink.SA2 = (byte)this.PCUnit;
					this.hostlink.SID = (byte)this.DeviceNum;
					this.IsConnected = this.hostlink.Connect(this.PortNum, this.Paud, int.Parse(this.DataBits), this.Parity, this.StopBits);
					this.FirstConnect = false;
				}
			}
		}
        
		public int GetOmronStartByte(string gpStart)
		{
			int result = 0;
			if (gpStart[0] == 'E')
			{
				if (gpStart.Contains('.'))
				{
					string[] array = gpStart.Split(new char[]
					{
						'.'
					});
					if (array.Length == 2)
					{
						result = Convert.ToInt32(array[1]);
					}
				}
			}
			else
			{
				result = Convert.ToInt32(gpStart.Substring(gpStart.IndexOf(gpStart.First((char c) => char.IsDigit(c)))));
			}
			return result;
		}
        
		public bool VerifyHostlinkAddress(string address, out int start, out int offset)
		{
			bool result;
			if (address.Contains('.'))
			{
				string[] array = address.Split(new char[]
				{
					'.'
				});
				if (array.Length == 2)
				{
					int num = 0;
					bool flag = int.TryParse(array[0], out num);
					start = num;
					flag = (flag && int.TryParse(array[1], out num));
					offset = num;
					result = flag;
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
				OmronHostlinkVariable omronHostlinkVariable = this.CurrentVarList[keyName] as OmronHostlinkVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(omronHostlinkVariable, omronHostlinkVariable.VarType, setValue);
				if (xktResult.IsSuccess)
				{
					result = this.hostlink.Write(omronHostlinkVariable.VarAddress, xktResult.Content, omronHostlinkVariable.VarType);
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
        
		public Hostlink hostlink;
        
		public List<OmronHostlinkDeviceGroup> DeviceGroupList;
	}
}
