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
using Pframe.Modbus;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Modbus
{
	public class NodeModbusASCII : ModbusNode, IXmlConvert
	{
		public NodeModbusASCII()
		{
			this.sw = new Stopwatch();
			this.ModbusASCIIGroupList = new List<ModbusASCIIGroup>();
			base.Name = "Modbus ASCII Client";
			base.Description = "1#制冷系统";
			base.ModbusType = 3000;
			this.PortNum = "COM3";
			this.Paud = 9600;
			this.Parity = Parity.None;
			this.DataBits = "8";
			this.StopBits = StopBits.One;
			this.SleepTime = 20;
			this.DataFormat = DataFormat.ABCD;
			this.FirstConnect = true;
		}
        
		public bool FirstConnect { get; set; }
        
		public bool ConnectState { get; set; }
        
		public long CommRate { get; set; }
        
		public int SleepTime { get; set; }
        
		public string PortNum { get; set; }
        
		public int Paud { get; set; }
        
		public Parity Parity { get; set; }
        
		public string DataBits { get; set; }
        
		public StopBits StopBits { get; set; }
        
		public bool IsConnected { get; set; }
        
		public DataFormat DataFormat { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.PortNum = element.Attribute("PortNum").Value;
			this.Paud = int.Parse(element.Attribute("Paud").Value);
			this.DataBits = element.Attribute("DataBits").Value;
			this.Parity = (Parity)Enum.Parse(typeof(Parity), element.Attribute("Parity").Value, true);
			this.StopBits = (StopBits)Enum.Parse(typeof(StopBits), element.Attribute("StopBits").Value, true);
			this.SleepTime = int.Parse(element.Attribute("SleepTime").Value);
			this.DataFormat = (DataFormat)Enum.Parse(typeof(DataFormat), element.Attribute("DataFormat").Value, true);
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
			xelement.SetAttributeValue("DataFormat", this.DataFormat);
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
			nodeClassRenders.Add(new NodeClassRenderItem("数据格式", this.DataFormat.ToString()));
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
			foreach (ModbusASCIIGroup modbusASCIIGroup in this.ModbusASCIIGroupList)
			{
				foreach (ModbusASCIIVariable gclass in modbusASCIIGroup.varList)
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
					foreach (ModbusASCIIGroup modbusASCIIGroup in this.ModbusASCIIGroupList)
					{
						if (modbusASCIIGroup.IsActive)
						{
							byte[] array = this.modascii.Read((ModbusArea)modbusASCIIGroup.StoreArea, modbusASCIIGroup.SlaveID, modbusASCIIGroup.Start, modbusASCIIGroup.Length);
							this.ConnectState = (array != null);
							if (modbusASCIIGroup.StoreArea == ModbusStoreArea.输入线圈 | modbusASCIIGroup.StoreArea == ModbusStoreArea.输出线圈)
							{
								if (array != null && array.Length == (int)((modbusASCIIGroup.Length % 8 == 0) ? (modbusASCIIGroup.Length / 8) : (modbusASCIIGroup.Length / 8 + 1)))
								{
									base.ErrorTimes = 0;
									using (List<ModbusASCIIVariable>.Enumerator enumerator2 = modbusASCIIGroup.varList.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											ModbusASCIIVariable gclass = enumerator2.Current;
											int num;
											int num2;
											if (base.VerifyModbusAddress(true, gclass.VarAddress, out num, out num2))
											{
												num -= (int)modbusASCIIGroup.Start;
												if (gclass.VarType == DataType.Bool)
												{
													gclass.Value = BitLib.GetBitArrayFromByteArray(array, false)[num];
												}
												base.UpdateCurrentValue(gclass);
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
							else
							{
								if (array != null && array.Length == (int)(modbusASCIIGroup.Length * 2))
								{
									base.ErrorTimes = 0;
									using (List<ModbusASCIIVariable>.Enumerator enumerator3 = modbusASCIIGroup.varList.GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											ModbusASCIIVariable gclass2 = enumerator3.Current;
											int num3;
											int num4;
											if (base.VerifyModbusAddress(false, gclass2.VarAddress, out num3, out num4))
											{
												num3 -= (int)modbusASCIIGroup.Start;
												num3 *= 2;
												switch (gclass2.VarType)
												{
												case DataType.Bool:
													gclass2.Value = BitLib.GetBitFrom2ByteArray(array, num3, num4, true);
													break;
												case DataType.Byte:
													gclass2.Value = ByteLib.GetByteFromByteArray(array, num3);
													break;
												case DataType.Short:
													gclass2.Value = ShortLib.GetShortFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.UShort:
													gclass2.Value = UShortLib.GetUShortFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Int:
													gclass2.Value = IntLib.GetIntFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.UInt:
													gclass2.Value = UIntLib.GetUIntFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Float:
													gclass2.Value = FloatLib.GetFloatFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Double:
													gclass2.Value = DoubleLib.GetDoubleFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Long:
													gclass2.Value = LongLib.GetLongFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.ULong:
													gclass2.Value = ULongLib.GetULongFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.String:
													gclass2.Value = StringLib.GetStringFromByteArray(array, num3, num4, Encoding.ASCII);
													break;
												}
												gclass2.Value = MigrationLib.GetMigrationValue(gclass2.Value, gclass2.Scale, gclass2.Offset);
												base.UpdateCurrentValue(gclass2);
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
						ModbusAscii modbusAscii = this.modascii;
						if (modbusAscii != null)
						{
							modbusAscii.DisConnect();
						}
					}
					this.modascii = new ModbusAscii();
					this.SleepTime = this.SleepTime;
					this.DataFormat = this.DataFormat;
					this.IsConnected = this.modascii.Connect(this.PortNum, this.Paud, int.Parse(this.DataBits), this.Parity, this.StopBits);
					this.FirstConnect = false;
				}
			}
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
				ModbusASCIIVariable gclass = this.CurrentVarList[keyName] as ModbusASCIIVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(gclass, gclass.VarType, setValue);
				if (xktResult.IsSuccess)
				{
					result = this.modascii.Write(gclass.GroupID, gclass.VarAddress, xktResult.Content, gclass.VarType);
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
        
		public ModbusAscii modascii;
        
		public List<ModbusASCIIGroup> ModbusASCIIGroupList;
	}
}
