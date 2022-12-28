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
	public class NodeModbusRTUOverTCP : ModbusNode, IXmlConvert
	{
		public NodeModbusRTUOverTCP()
		{
			this.sw = new Stopwatch();
			this.ModbusRtuOverTCPGroupList = new List<ModbusRTUOverTCPGroup>();
			base.Name = "ModbusRTUOverTCP";
			base.Description = "1#软水系统";
			base.ModbusType = 5000;
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
			foreach (ModbusRTUOverTCPGroup modbusRTUOverTCPGroup in this.ModbusRtuOverTCPGroupList)
			{
				foreach (ModbusRTUOverTCPVariable modbusRTUOverTCPVariable in modbusRTUOverTCPGroup.varList)
				{
					if (modbusRTUOverTCPVariable.Config.ArchiveEnable)
					{
						this.StoreVarList.Add(modbusRTUOverTCPVariable);
					}
					if (this.CurrentVarList.ContainsKey(modbusRTUOverTCPVariable.KeyName))
					{
						this.CurrentVarList[modbusRTUOverTCPVariable.KeyName] = modbusRTUOverTCPVariable;
					}
					else
					{
						this.CurrentVarList.Add(modbusRTUOverTCPVariable.KeyName, modbusRTUOverTCPVariable);
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
					foreach (ModbusRTUOverTCPGroup modbusRTUOverTCPGroup in this.ModbusRtuOverTCPGroupList)
					{
						if (modbusRTUOverTCPGroup.IsActive)
						{
							byte[] array = this.modovertcp.Read((ModbusArea)modbusRTUOverTCPGroup.StoreArea, modbusRTUOverTCPGroup.SlaveID, modbusRTUOverTCPGroup.Start, modbusRTUOverTCPGroup.Length);
							if (modbusRTUOverTCPGroup.StoreArea == ModbusStoreArea.输入线圈 | modbusRTUOverTCPGroup.StoreArea == ModbusStoreArea.输出线圈)
							{
								if (array != null && array.Length == (int)((modbusRTUOverTCPGroup.Length % 8 == 0) ? (modbusRTUOverTCPGroup.Length / 8) : (modbusRTUOverTCPGroup.Length / 8 + 1)))
								{
									base.ErrorTimes = 0;
									using (List<ModbusRTUOverTCPVariable>.Enumerator enumerator2 = modbusRTUOverTCPGroup.varList.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											ModbusRTUOverTCPVariable modbusRTUOverTCPVariable = enumerator2.Current;
											int num;
											int num2;
											if (base.VerifyModbusAddress(true, modbusRTUOverTCPVariable.VarAddress, out num, out num2))
											{
												num -= (int)modbusRTUOverTCPGroup.Start;
												if (modbusRTUOverTCPVariable.VarType == DataType.Bool)
												{
													modbusRTUOverTCPVariable.Value = BitLib.GetBitArrayFromByteArray(array, false)[num];
												}
												base.UpdateCurrentValue(modbusRTUOverTCPVariable);
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
								if (array != null && array.Length == (int)(modbusRTUOverTCPGroup.Length * 2))
								{
									base.ErrorTimes = 0;
									using (List<ModbusRTUOverTCPVariable>.Enumerator enumerator3 = modbusRTUOverTCPGroup.varList.GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											ModbusRTUOverTCPVariable modbusRTUOverTCPVariable2 = enumerator3.Current;
											int num3;
											int num4;
											if (base.VerifyModbusAddress(false, modbusRTUOverTCPVariable2.VarAddress, out num3, out num4))
											{
												num3 -= (int)modbusRTUOverTCPGroup.Start;
												num3 *= 2;
												switch (modbusRTUOverTCPVariable2.VarType)
												{
												case DataType.Bool:
													modbusRTUOverTCPVariable2.Value = BitLib.GetBitFrom2ByteArray(array, num3, num4, true);
													break;
												case DataType.Byte:
													modbusRTUOverTCPVariable2.Value = ByteLib.GetByteFromByteArray(array, num3);
													break;
												case DataType.Short:
													modbusRTUOverTCPVariable2.Value = ShortLib.GetShortFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.UShort:
													modbusRTUOverTCPVariable2.Value = UShortLib.GetUShortFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Int:
													modbusRTUOverTCPVariable2.Value = IntLib.GetIntFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.UInt:
													modbusRTUOverTCPVariable2.Value = UIntLib.GetUIntFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Float:
													modbusRTUOverTCPVariable2.Value = FloatLib.GetFloatFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Double:
													modbusRTUOverTCPVariable2.Value = DoubleLib.GetDoubleFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Long:
													modbusRTUOverTCPVariable2.Value = LongLib.GetLongFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.ULong:
													modbusRTUOverTCPVariable2.Value = ULongLib.GetULongFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.String:
													modbusRTUOverTCPVariable2.Value = StringLib.GetStringFromByteArray(array, num3, num4 * 2, Encoding.ASCII);
													break;
												case DataType.ByteArray:
													modbusRTUOverTCPVariable2.Value = ByteArrayLib.GetByteArray(array, num3, num4 * 2);
													break;
												case DataType.HexString:
													modbusRTUOverTCPVariable2.Value = StringLib.GetHexStringFromByteArray(array, num3, num4 * 2, ' ');
													break;
												}
												modbusRTUOverTCPVariable2.Value = MigrationLib.GetMigrationValue(modbusRTUOverTCPVariable2.Value, modbusRTUOverTCPVariable2.Scale, modbusRTUOverTCPVariable2.Offset);
												base.UpdateCurrentValue(modbusRTUOverTCPVariable2);
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
						ModbusRtuOverTcp modbusRtuOverTcp = this.modovertcp;
						if (modbusRtuOverTcp != null)
						{
							modbusRtuOverTcp.DisConnect();
						}
					}
					this.modovertcp = new ModbusRtuOverTcp();
					this.modovertcp.DataFormat = this.DataFormat;
					this.IsConnected = this.modovertcp.Connect(this.ServerURL, this.Port);
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
				ModbusRTUOverTCPVariable modbusRTUOverTCPVariable = this.CurrentVarList[keyName] as ModbusRTUOverTCPVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(modbusRTUOverTCPVariable, modbusRTUOverTCPVariable.VarType, setValue);
				if (xktResult.IsSuccess)
				{
					result = this.modovertcp.Write(modbusRTUOverTCPVariable.GroupID, modbusRTUOverTCPVariable.VarAddress, xktResult.Content, modbusRTUOverTCPVariable.VarType);
				}
				else
				{
					result = xktResult;
				}
			}
			return result;
		}
        
        public Stopwatch sw;
        
		public CancellationTokenSource cts;
        
		public ModbusRtuOverTcp modovertcp;
        
		public List<ModbusRTUOverTCPGroup> ModbusRtuOverTCPGroupList;
	}
}
