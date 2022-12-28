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
	public class NodeModbusASCIIOverTCP : ModbusNode, IXmlConvert
	{
		public NodeModbusASCIIOverTCP()
		{
			this.sw = new Stopwatch();
			this.ModbusAsciiOverTCPGroupList = new List<ModbusASCIIOverTCPGroup>();
			base.Name = "ModbusASCIIOverTCP";
			base.Description = "1#软水系统";
			base.ModbusType = 7000;
			this.ServerURL = "127.0.0.1";
			this.Port = 502;
			this.FirstConnect = true;
		}
        
		public bool FirstConnect { get; set; }
        
		public long CommRate { get; set; }
        
		public string ServerURL { get; set; }
        
		public int Port { get; set; }
        
		public int SlaveAddress { get; set; }
        
		public bool IsConnected { get; set; }
        
		public DataFormat DataFormat { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.ServerURL = element.Attribute("ServerURL").Value;
			this.Port = int.Parse(element.Attribute("Port").Value);
			this.DataFormat = (DataFormat)Enum.Parse(typeof(DataFormat), element.Attribute("DataFormat").Value, true);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("ServerURL", this.ServerURL);
			xelement.SetAttributeValue("Port", this.Port);
			xelement.SetAttributeValue("DataFormat", this.DataFormat);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("IP地址", this.ServerURL));
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
			foreach (ModbusASCIIOverTCPGroup modbusASCIIOverTCPGroup in this.ModbusAsciiOverTCPGroupList)
			{
				foreach (ModbusASCIIOverTCPVariable modbusASCIIOverTCPVariable in modbusASCIIOverTCPGroup.varList)
				{
					if (modbusASCIIOverTCPVariable.Config.ArchiveEnable)
					{
						this.StoreVarList.Add(modbusASCIIOverTCPVariable);
					}
					if (this.CurrentVarList.ContainsKey(modbusASCIIOverTCPVariable.KeyName))
					{
						this.CurrentVarList[modbusASCIIOverTCPVariable.KeyName] = modbusASCIIOverTCPVariable;
					}
					else
					{
						this.CurrentVarList.Add(modbusASCIIOverTCPVariable.KeyName, modbusASCIIOverTCPVariable);
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
					foreach (ModbusASCIIOverTCPGroup modbusASCIIOverTCPGroup in this.ModbusAsciiOverTCPGroupList)
					{
						if (modbusASCIIOverTCPGroup.IsActive)
						{
							byte[] array = this.modasciiovertcp.Read((ModbusArea)modbusASCIIOverTCPGroup.StoreArea, modbusASCIIOverTCPGroup.SlaveID, modbusASCIIOverTCPGroup.Start, modbusASCIIOverTCPGroup.Length);
							if (modbusASCIIOverTCPGroup.StoreArea == ModbusStoreArea.输入线圈 | modbusASCIIOverTCPGroup.StoreArea == ModbusStoreArea.输出线圈)
							{
								if (array != null && array.Length == (int)((modbusASCIIOverTCPGroup.Length % 8 == 0) ? (modbusASCIIOverTCPGroup.Length / 8) : (modbusASCIIOverTCPGroup.Length / 8 + 1)))
								{
									base.ErrorTimes = 0;
									using (List<ModbusASCIIOverTCPVariable>.Enumerator enumerator2 = modbusASCIIOverTCPGroup.varList.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											ModbusASCIIOverTCPVariable modbusASCIIOverTCPVariable = enumerator2.Current;
											int num;
											int num2;
											if (base.VerifyModbusAddress(true, modbusASCIIOverTCPVariable.VarAddress, out num, out num2))
											{
												num -= (int)modbusASCIIOverTCPGroup.Start;
												if (modbusASCIIOverTCPVariable.VarType == DataType.Bool)
												{
													modbusASCIIOverTCPVariable.Value = BitLib.GetBitArrayFromByteArray(array, false)[num];
												}
												base.UpdateCurrentValue(modbusASCIIOverTCPVariable);
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
								if (array != null && array.Length == (int)(modbusASCIIOverTCPGroup.Length * 2))
								{
									base.ErrorTimes = 0;
									using (List<ModbusASCIIOverTCPVariable>.Enumerator enumerator3 = modbusASCIIOverTCPGroup.varList.GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											ModbusASCIIOverTCPVariable modbusASCIIOverTCPVariable2 = enumerator3.Current;
											int num3;
											int num4;
											if (base.VerifyModbusAddress(false, modbusASCIIOverTCPVariable2.VarAddress, out num3, out num4))
											{
												num3 -= (int)modbusASCIIOverTCPGroup.Start;
												num3 *= 2;
												switch (modbusASCIIOverTCPVariable2.VarType)
												{
												case DataType.Bool:
													modbusASCIIOverTCPVariable2.Value = BitLib.GetBitFrom2ByteArray(array, num3, num4, true);
													break;
												case DataType.Byte:
													modbusASCIIOverTCPVariable2.Value = ByteLib.GetByteFromByteArray(array, num3);
													break;
												case DataType.Short:
													modbusASCIIOverTCPVariable2.Value = ShortLib.GetShortFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.UShort:
													modbusASCIIOverTCPVariable2.Value = UShortLib.GetUShortFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Int:
													modbusASCIIOverTCPVariable2.Value = IntLib.GetIntFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.UInt:
													modbusASCIIOverTCPVariable2.Value = UIntLib.GetUIntFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Float:
													modbusASCIIOverTCPVariable2.Value = FloatLib.GetFloatFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Double:
													modbusASCIIOverTCPVariable2.Value = DoubleLib.GetDoubleFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Long:
													modbusASCIIOverTCPVariable2.Value = LongLib.GetLongFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.ULong:
													modbusASCIIOverTCPVariable2.Value = ULongLib.GetULongFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.String:
													modbusASCIIOverTCPVariable2.Value = StringLib.GetStringFromByteArray(array, num3, num4 * 2, Encoding.ASCII);
													break;
												case DataType.ByteArray:
													modbusASCIIOverTCPVariable2.Value = ByteArrayLib.GetByteArray(array, num3, num4 * 2);
													break;
												case DataType.HexString:
													modbusASCIIOverTCPVariable2.Value = StringLib.GetHexStringFromByteArray(array, num3, num4 * 2, ' ');
													break;
												}
												modbusASCIIOverTCPVariable2.Value = MigrationLib.GetMigrationValue(modbusASCIIOverTCPVariable2.Value, modbusASCIIOverTCPVariable2.Scale, modbusASCIIOverTCPVariable2.Offset);
												base.UpdateCurrentValue(modbusASCIIOverTCPVariable2);
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
						ModbusAsciiOverTcp modbusAsciiOverTcp = this.modasciiovertcp;
						if (modbusAsciiOverTcp != null)
						{
							modbusAsciiOverTcp.DisConnect();
						}
					}
					this.modasciiovertcp = new ModbusAsciiOverTcp();
					this.modasciiovertcp.DataFormat = this.DataFormat;
					this.IsConnected = this.modasciiovertcp.Connect(this.ServerURL, this.Port);
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
				ModbusASCIIOverTCPVariable modbusASCIIOverTCPVariable = this.CurrentVarList[keyName] as ModbusASCIIOverTCPVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(modbusASCIIOverTCPVariable, modbusASCIIOverTCPVariable.VarType, setValue);
				if (xktResult.IsSuccess)
				{
					result = this.modasciiovertcp.Write(modbusASCIIOverTCPVariable.GroupID, modbusASCIIOverTCPVariable.VarAddress, xktResult.Content, modbusASCIIOverTCPVariable.VarType);
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
        
		public ModbusAsciiOverTcp modasciiovertcp;
        
		public List<ModbusASCIIOverTCPGroup> ModbusAsciiOverTCPGroupList;
	}
}
