using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	public class NodeModbusASCIIOverUDP : ModbusNode, IXmlConvert
	{
		public NodeModbusASCIIOverUDP()
		{
			this.sw = new Stopwatch();
			this.ModbusAsciiOverUDPGroupList = new List<ModbusASCIIOverUDPGroup>();
			base.Name = "ModbusASCIIOverUDP";
			base.Description = "1#软水系统";
			base.ModbusType = 8000;
			this.String_0 = "127.0.0.1";
			this.Port = 502;
			this.FirstConnect = true;
		}
        
		public bool FirstConnect { get; set; }
        
		public long CommRate { get; set; }
        
		public string String_0 { get; set; }
        
		public int Port { get; set; }
        
		public int SlaveAddress { get; set; }
        
		public bool IsConnected { get; set; }
        
		public DataFormat DataFormat { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.String_0 = element.Attribute("ServerURL").Value;
			this.Port = int.Parse(element.Attribute("Port").Value);
			this.DataFormat = (DataFormat)Enum.Parse(typeof(DataFormat), element.Attribute("DataFormat").Value, true);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("ServerURL", this.String_0);
			xelement.SetAttributeValue("Port", this.Port);
			xelement.SetAttributeValue("DataFormat", this.DataFormat);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("IP地址", this.String_0));
			nodeClassRenders.Add(new NodeClassRenderItem("端口号", this.Port.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("数据格式", this.DataFormat.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("激活情况", base.IsActive ? "已激活" : "未激活"));
			nodeClassRenders.Add(new NodeClassRenderItem("连接情况", this.IsConnected ? "已连接" : "未连接"));
			if (this.IsConnected)
			{
				nodeClassRenders.Add(new NodeClassRenderItem("通信周期", this.CommRate.ToString() + "ms"));
			}
			return nodeClassRenders;
		}
        
		public void Start()
		{
			foreach (ModbusASCIIOverUDPGroup modbusASCIIOverUDPGroup in this.ModbusAsciiOverUDPGroupList)
			{
				foreach (ModbusASCIIOverUDPVariable modbusASCIIOverUDPVariable in modbusASCIIOverUDPGroup.varList)
				{
					if (modbusASCIIOverUDPVariable.Config.ArchiveEnable)
					{
						this.StoreVarList.Add(modbusASCIIOverUDPVariable);
					}
					if (this.CurrentVarList.ContainsKey(modbusASCIIOverUDPVariable.KeyName))
					{
						this.CurrentVarList[modbusASCIIOverUDPVariable.KeyName] = modbusASCIIOverUDPVariable;
					}
					else
					{
						this.CurrentVarList.Add(modbusASCIIOverUDPVariable.KeyName, modbusASCIIOverUDPVariable);
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
					foreach (ModbusASCIIOverUDPGroup modbusASCIIOverUDPGroup in this.ModbusAsciiOverUDPGroupList)
					{
						if (modbusASCIIOverUDPGroup.IsActive)
						{
							byte[] array = this.modasciioverudp.Read((ModbusArea)modbusASCIIOverUDPGroup.StoreArea, modbusASCIIOverUDPGroup.SlaveID, modbusASCIIOverUDPGroup.Start, modbusASCIIOverUDPGroup.Length);
							if (modbusASCIIOverUDPGroup.StoreArea == ModbusStoreArea.输入线圈 | modbusASCIIOverUDPGroup.StoreArea == ModbusStoreArea.输出线圈)
							{
								if (array != null && array.Length == (int)((modbusASCIIOverUDPGroup.Length % 8 == 0) ? (modbusASCIIOverUDPGroup.Length / 8) : (modbusASCIIOverUDPGroup.Length / 8 + 1)))
								{
									using (List<ModbusASCIIOverUDPVariable>.Enumerator enumerator2 = modbusASCIIOverUDPGroup.varList.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											ModbusASCIIOverUDPVariable modbusASCIIOverUDPVariable = enumerator2.Current;
											int num;
											int num2;
											if (base.VerifyModbusAddress(true, modbusASCIIOverUDPVariable.VarAddress, out num, out num2))
											{
												num -= (int)modbusASCIIOverUDPGroup.Start;
												if (modbusASCIIOverUDPVariable.VarType == DataType.Bool)
												{
													modbusASCIIOverUDPVariable.Value = BitLib.GetBitArrayFromByteArray(array, false)[num];
												}
												base.UpdateCurrentValue(modbusASCIIOverUDPVariable);
											}
										}
										continue;
									}
								}
								int errorTimes = base.ErrorTimes;
								base.ErrorTimes = errorTimes + 1;
								if (base.ErrorTimes >= base.MaxErrorTimes)
								{
									this.IsConnected = false;
								}
							}
							else
							{
								if (array != null && array.Length == (int)(modbusASCIIOverUDPGroup.Length * 2))
								{
									using (List<ModbusASCIIOverUDPVariable>.Enumerator enumerator3 = modbusASCIIOverUDPGroup.varList.GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											ModbusASCIIOverUDPVariable modbusASCIIOverUDPVariable2 = enumerator3.Current;
											int num3;
											int num4;
											if (base.VerifyModbusAddress(false, modbusASCIIOverUDPVariable2.VarAddress, out num3, out num4))
											{
												num3 -= (int)modbusASCIIOverUDPGroup.Start;
												num3 *= 2;
												switch (modbusASCIIOverUDPVariable2.VarType)
												{
												case DataType.Bool:
													modbusASCIIOverUDPVariable2.Value = BitLib.GetBitFrom2ByteArray(array, num3, num4, false);
													break;
												case DataType.Byte:
													modbusASCIIOverUDPVariable2.Value = ByteLib.GetByteFromByteArray(array, num3);
													break;
												case DataType.Short:
													modbusASCIIOverUDPVariable2.Value = ShortLib.GetShortFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.UShort:
													modbusASCIIOverUDPVariable2.Value = UShortLib.GetUShortFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Int:
													modbusASCIIOverUDPVariable2.Value = IntLib.GetIntFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.UInt:
													modbusASCIIOverUDPVariable2.Value = UIntLib.GetUIntFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Float:
													modbusASCIIOverUDPVariable2.Value = FloatLib.GetFloatFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Double:
													modbusASCIIOverUDPVariable2.Value = DoubleLib.GetDoubleFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Long:
													modbusASCIIOverUDPVariable2.Value = LongLib.GetLongFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.ULong:
													modbusASCIIOverUDPVariable2.Value = ULongLib.GetULongFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.String:
													modbusASCIIOverUDPVariable2.Value = StringLib.GetStringFromByteArray(array, num3, num4 * 2, Encoding.ASCII);
													break;
												case DataType.ByteArray:
													modbusASCIIOverUDPVariable2.Value = ByteArrayLib.GetByteArray(array, num3, num4 * 2);
													break;
												case DataType.HexString:
													modbusASCIIOverUDPVariable2.Value = StringLib.GetHexStringFromByteArray(array, num3, num4 * 2, ' ');
													break;
												}
												modbusASCIIOverUDPVariable2.Value = MigrationLib.GetMigrationValue(modbusASCIIOverUDPVariable2.Value, modbusASCIIOverUDPVariable2.Scale, modbusASCIIOverUDPVariable2.Offset);
												base.UpdateCurrentValue(modbusASCIIOverUDPVariable2);
											}
										}
										continue;
									}
								}
								int errorTimes = base.ErrorTimes;
								base.ErrorTimes = errorTimes + 1;
								if (base.ErrorTimes >= base.MaxErrorTimes)
								{
									this.IsConnected = false;
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
					}
					this.modasciioverudp = new ModbusAsciiOverUdp(this.String_0, int.Parse(this.Port.ToString()), this.DataFormat);
					this.modasciioverudp.ConnectTimeOut = base.ConnectTimeOut;
					this.IsConnected = true;
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
				ModbusASCIIOverUDPVariable modbusASCIIOverUDPVariable = this.CurrentVarList[keyName] as ModbusASCIIOverUDPVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(modbusASCIIOverUDPVariable, modbusASCIIOverUDPVariable.VarType, setValue);
				if (xktResult.IsSuccess)
				{
					result = this.modasciioverudp.Write(modbusASCIIOverUDPVariable.GroupID, modbusASCIIOverUDPVariable.VarAddress, xktResult.Content, modbusASCIIOverUDPVariable.VarType);
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
        
		public ModbusAsciiOverUdp modasciioverudp;
        
		public List<ModbusASCIIOverUDPGroup> ModbusAsciiOverUDPGroupList;
	}
}
