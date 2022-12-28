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
	public class NodeModbusRTU : ModbusNode, IXmlConvert
	{
		public NodeModbusRTU()
		{
			this.sw = new Stopwatch();
			this.ModbusRTUGroupList = new List<ModbusRTUGroup>();
			base.Name = "Modbus RTU Client";
			base.Description = "1#制冷系统";
			base.ModbusType = 1000;
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
        
		public string PortNum { get; set; }
        
		public int Paud { get; set; }
        
		public Parity Parity { get; set; }
        
		public string DataBits { get; set; }
        
		public StopBits StopBits { get; set; }
        
		public bool IsConnected { get; set; }
        
		public int SleepTime { get; set; }
        
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
			foreach (ModbusRTUGroup modbusRTUGroup in this.ModbusRTUGroupList)
			{
				foreach (ModbusRTUVariable modbusRTUVariable in modbusRTUGroup.varList)
				{
					if (modbusRTUVariable.Config.ArchiveEnable)
					{
						this.StoreVarList.Add(modbusRTUVariable);
					}
					if (this.CurrentVarList.ContainsKey(modbusRTUVariable.KeyName))
					{
						this.CurrentVarList[modbusRTUVariable.KeyName] = modbusRTUVariable;
					}
					else
					{
						this.CurrentVarList.Add(modbusRTUVariable.KeyName, modbusRTUVariable);
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
					foreach (ModbusRTUGroup modbusRTUGroup in this.ModbusRTUGroupList)
					{
						if (modbusRTUGroup.IsActive)
						{
							byte[] array = this.modrtu.Read((ModbusArea)modbusRTUGroup.StoreArea, modbusRTUGroup.SlaveID, modbusRTUGroup.Start, modbusRTUGroup.Length);
							this.ConnectState = (array != null);
							if (modbusRTUGroup.StoreArea == ModbusStoreArea.输入线圈 | modbusRTUGroup.StoreArea == ModbusStoreArea.输出线圈)
							{
								if (array != null && array.Length == (int)((modbusRTUGroup.Length % 8 == 0) ? (modbusRTUGroup.Length / 8) : (modbusRTUGroup.Length / 8 + 1)))
								{
									base.ErrorTimes = 0;
									using (List<ModbusRTUVariable>.Enumerator enumerator2 = modbusRTUGroup.varList.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											ModbusRTUVariable modbusRTUVariable = enumerator2.Current;
											int num;
											int num2;
											if (base.VerifyModbusAddress(true, modbusRTUVariable.VarAddress, out num, out num2))
											{
												num -= (int)modbusRTUGroup.Start;
												if (modbusRTUVariable.VarType == DataType.Bool)
												{
													modbusRTUVariable.Value = BitLib.GetBitArrayFromByteArray(array, false)[num];
												}
												base.UpdateCurrentValue(modbusRTUVariable);
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
								if (array != null && array.Length == (int)(modbusRTUGroup.Length * 2))
								{
									base.ErrorTimes = 0;
									using (List<ModbusRTUVariable>.Enumerator enumerator3 = modbusRTUGroup.varList.GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											ModbusRTUVariable modbusRTUVariable2 = enumerator3.Current;
											int num3;
											int num4;
											if (base.VerifyModbusAddress(false, modbusRTUVariable2.VarAddress, out num3, out num4))
											{
												num3 -= (int)modbusRTUGroup.Start;
												num3 *= 2;
												switch (modbusRTUVariable2.VarType)
												{
												case DataType.Bool:
													modbusRTUVariable2.Value = BitLib.GetBitFrom2ByteArray(array, num3, num4, true);
													break;
												case DataType.Byte:
													modbusRTUVariable2.Value = ByteLib.GetByteFromByteArray(array, num3);
													break;
												case DataType.Short:
													modbusRTUVariable2.Value = ShortLib.GetShortFromByteArray(array, num3, this.modrtu.DataFormat);
													break;
												case DataType.UShort:
													modbusRTUVariable2.Value = UShortLib.GetUShortFromByteArray(array, num3, this.modrtu.DataFormat);
													break;
												case DataType.Int:
													modbusRTUVariable2.Value = IntLib.GetIntFromByteArray(array, num3, this.modrtu.DataFormat);
													break;
												case DataType.UInt:
													modbusRTUVariable2.Value = UIntLib.GetUIntFromByteArray(array, num3, this.modrtu.DataFormat);
													break;
												case DataType.Float:
													modbusRTUVariable2.Value = FloatLib.GetFloatFromByteArray(array, num3, this.modrtu.DataFormat);
													break;
												case DataType.Double:
													modbusRTUVariable2.Value = DoubleLib.GetDoubleFromByteArray(array, num3, this.modrtu.DataFormat);
													break;
												case DataType.Long:
													modbusRTUVariable2.Value = LongLib.GetLongFromByteArray(array, num3, this.modrtu.DataFormat);
													break;
												case DataType.ULong:
													modbusRTUVariable2.Value = ULongLib.GetULongFromByteArray(array, num3, this.modrtu.DataFormat);
													break;
												case DataType.String:
													modbusRTUVariable2.Value = StringLib.GetStringFromByteArray(array, num3, num4 * 2, Encoding.ASCII);
													break;
												case DataType.ByteArray:
													modbusRTUVariable2.Value = ByteArrayLib.GetByteArray(array, num3, num4 * 2);
													break;
												case DataType.HexString:
													modbusRTUVariable2.Value = StringLib.GetHexStringFromByteArray(array, num3, num4 * 2, ' ');
													break;
												}
												modbusRTUVariable2.Value = MigrationLib.GetMigrationValue(modbusRTUVariable2.Value, modbusRTUVariable2.Scale, modbusRTUVariable2.Offset);
												base.UpdateCurrentValue(modbusRTUVariable2);
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
						ModbusRtu modbusRtu = this.modrtu;
						if (modbusRtu != null)
						{
							modbusRtu.DisConnect();
						}
					}
					this.modrtu = new ModbusRtu();
					this.modrtu.SleepTime = this.SleepTime;
					this.modrtu.DataFormat = this.DataFormat;
					this.IsConnected = this.modrtu.Connect(this.PortNum, this.Paud, int.Parse(this.DataBits), this.Parity, this.StopBits);
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
				ModbusRTUVariable modbusRTUVariable = this.CurrentVarList[keyName] as ModbusRTUVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(modbusRTUVariable, modbusRTUVariable.VarType, setValue);
				if (xktResult.IsSuccess)
				{
					result = this.modrtu.Write(modbusRTUVariable.GroupID, modbusRTUVariable.VarAddress, xktResult.Content, modbusRTUVariable.VarType);
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
        
		public ModbusRtu modrtu;
        
		public List<ModbusRTUGroup> ModbusRTUGroupList;
	}
}
