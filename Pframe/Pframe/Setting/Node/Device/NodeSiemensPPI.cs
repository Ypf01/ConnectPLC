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
using Pframe.PLC.Siemens;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Device
{
	public class NodeSiemensPPI : DeviceNode, IXmlConvert
	{
		public NodeSiemensPPI()
		{
			this.sw = new Stopwatch();
			this.DeviceGroupList = new List<SiemensPPIDeviceGroup>();
			base.Name = "西门子S7-200协议";
			base.Description = "称重系统1#PLC";
			base.DeviceType = 90;
			this.PortNum = "COM3";
			this.Paud = 9600;
			this.Parity = Parity.Even;
			this.DataBits = "8";
			this.StopBits = StopBits.One;
			this.Station = 2;
			this.FirstConnect = true;
			this.SleepTime = 20;
		}
        
		public bool FirstConnect { get; set; }
        
		public bool ConnectState { get; set; }
        
		public long CommRate { get; set; }
        
		public string PortNum { get; set; }
        
		public int Paud { get; set; }
        
		public byte Station { get; set; }
        
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
			this.Station = byte.Parse(element.Attribute("Station").Value);
			this.SleepTime = int.Parse(element.Attribute("SleepTime").Value);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("Station", this.Station);
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
			nodeClassRenders.Add(new NodeClassRenderItem("站号", this.Station.ToString()));
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
			foreach (SiemensPPIDeviceGroup siemensPPIDeviceGroup in this.DeviceGroupList)
			{
				foreach (SiemensPPIVariable gclass in siemensPPIDeviceGroup.varList)
				{
					if (gclass.Config.ArchiveEnable)
					{
						this.StoreVarList.Add(gclass);
					}
					if (this.CurrentVarList.ContainsKey(gclass.KeyName))
					{
						this.CurrentVarList[gclass.KeyName] = gclass;
					}
					else
					{
						this.CurrentVarList.Add(gclass.KeyName, gclass);
					}
				}
			}
			this.cts = new CancellationTokenSource();
			Task.Run(new Action(this.GetValue), this.cts.Token);
		}
        
		private void GetValue()
		{
			while (!this.cts.IsCancellationRequested)
			{
				if (this.IsConnected)
				{
					this.sw.Restart();
					foreach (SiemensPPIDeviceGroup siemensPPIDeviceGroup in this.DeviceGroupList)
					{
						if (siemensPPIDeviceGroup.IsActive)
						{
							string address = siemensPPIDeviceGroup.StoreArea.ToString().Substring(0, siemensPPIDeviceGroup.StoreArea.ToString().Length - 3) + siemensPPIDeviceGroup.Start.ToString();
							byte[] array = this.siemensPPI_0.ReadBytes(address, (ushort)siemensPPIDeviceGroup.Length);
							this.ConnectState = (array != null && array.Length == siemensPPIDeviceGroup.Length);
							if (array != null && array.Length == siemensPPIDeviceGroup.Length)
							{
								base.ErrorTimes = 0;
								using (List<SiemensPPIVariable>.Enumerator enumerator2 = siemensPPIDeviceGroup.varList.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										SiemensPPIVariable gclass = enumerator2.Current;
										int num;
										int num2;
										if (this.VerifySiemensAddress(gclass.Start, out num, out num2))
										{
											num -= siemensPPIDeviceGroup.Start;
											switch (gclass.VarType)
											{
											case DataType.Bool:
												gclass.Value = BitLib.GetBitFromByteArray(array, num, num2);
												break;
											case DataType.Byte:
												gclass.Value = ByteLib.GetByteFromByteArray(array, num);
												break;
											case DataType.Short:
												gclass.Value = ShortLib.GetShortFromByteArray(array, num, DataFormat.ABCD);
												break;
											case DataType.UShort:
												gclass.Value = UShortLib.GetUShortFromByteArray(array, num, DataFormat.ABCD);
												break;
											case DataType.Int:
												gclass.Value = IntLib.GetIntFromByteArray(array, num, DataFormat.ABCD);
												break;
											case DataType.UInt:
												gclass.Value = UIntLib.GetUIntFromByteArray(array, num, DataFormat.ABCD);
												break;
											case DataType.Float:
												gclass.Value = FloatLib.GetFloatFromByteArray(array, num, DataFormat.ABCD);
												break;
											case DataType.Double:
												gclass.Value = DoubleLib.GetDoubleFromByteArray(array, num, DataFormat.ABCD);
												break;
											case DataType.Long:
												gclass.Value = LongLib.GetLongFromByteArray(array, num, DataFormat.ABCD);
												break;
											case DataType.ULong:
												gclass.Value = ULongLib.GetULongFromByteArray(array, num, DataFormat.ABCD);
												break;
											case DataType.String:
												gclass.Value = StringLib.GetStringFromByteArray(array, num, num2, Encoding.GetEncoding("GBK"));
												break;
											}
											gclass.Value = MigrationLib.GetMigrationValue(gclass.Value, gclass.Scale, gclass.Offset);
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
									Thread.Sleep(10);
								else
									this.IsConnected = false;
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
						SiemensPPI siemensPPI = this.siemensPPI_0;
						if (siemensPPI != null)
						{
							siemensPPI.DisConnect();
						}
					}
					this.siemensPPI_0 = new SiemensPPI(DataFormat.DCBA);
					this.siemensPPI_0.SleepTime = this.SleepTime;
					this.siemensPPI_0.Station = this.Station;
					this.IsConnected = this.siemensPPI_0.Connect(this.PortNum, this.Paud, int.Parse(this.DataBits), this.Parity, this.StopBits);
					this.FirstConnect = false;
				}
			}
		}
        
		private void method_1(SiemensPPIVariable gclass5_0)
		{
			switch (base.KeyWay)
			{
			case KeyWay.VarName:
				if (this.CurrentValue.ContainsKey(gclass5_0.Name))
				{
					this.CurrentValue[gclass5_0.Name] = gclass5_0.Value;
				}
				else
				{
					this.CurrentValue.Add(gclass5_0.Name, gclass5_0.Value);
				}
				break;
			case KeyWay.VarAddress:
				if (this.CurrentValue.ContainsKey(gclass5_0.VarAddress))
				{
					this.CurrentValue[gclass5_0.VarAddress] = gclass5_0.Value;
				}
				else
				{
					this.CurrentValue.Add(gclass5_0.VarAddress, gclass5_0.Value);
				}
				break;
			case KeyWay.VarDescription:
				if (this.CurrentValue.ContainsKey(gclass5_0.Description))
				{
					this.CurrentValue[gclass5_0.Description] = gclass5_0.Value;
				}
				else
				{
					this.CurrentValue.Add(gclass5_0.Description, gclass5_0.Value);
				}
				break;
			case KeyWay.GroupVarName:
				if (this.CurrentValue.ContainsKey(gclass5_0.GroupVarName))
				{
					this.CurrentValue[gclass5_0.GroupVarName] = gclass5_0.Value;
				}
				else
				{
					this.CurrentValue.Add(gclass5_0.GroupVarName, gclass5_0.Value);
				}
				break;
			case KeyWay.DeviceVarName:
				if (this.CurrentValue.ContainsKey(gclass5_0.DeviceVarName))
				{
					this.CurrentValue[gclass5_0.DeviceVarName] = gclass5_0.Value;
				}
				else
				{
					this.CurrentValue.Add(gclass5_0.DeviceVarName, gclass5_0.Value);
				}
				break;
			case KeyWay.DeviceGroupVarName:
				if (this.CurrentValue.ContainsKey(gclass5_0.DeviceVarName))
				{
					this.CurrentValue[gclass5_0.DeviceGroupVarName] = gclass5_0.Value;
				}
				else
				{
					this.CurrentValue.Add(gclass5_0.DeviceGroupVarName, gclass5_0.Value);
				}
				break;
			}
		}
        
		public bool VerifySiemensAddress(string address, out int start, out int offset)
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
			else if (address.Contains('|'))
			{
				string[] array2 = address.Split(new char[]
				{
					'|'
				});
				if (array2.Length == 2)
				{
					int num2 = 0;
					bool flag2 = int.TryParse(array2[0], out num2);
					start = num2;
					flag2 = (flag2 && int.TryParse(array2[1], out num2));
					offset = num2;
					result = flag2;
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
        
		public void Stop()
		{
			this.cts.Cancel();
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
				SiemensPPIVariable gclass = this.CurrentVarList[keyName] as SiemensPPIVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(gclass, gclass.VarType, setValue);
				if (xktResult.IsSuccess)
				{
					result = this.siemensPPI_0.Write(gclass.VarAddress, xktResult.Content, gclass.VarType);
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
        
		public SiemensPPI siemensPPI_0;
        
		public List<SiemensPPIDeviceGroup> DeviceGroupList;
	}
}
