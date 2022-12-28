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
	public class NodeModbusTCP : ModbusNode, IXmlConvert
	{
		public NodeModbusTCP()
		{
			this.sw = new Stopwatch();
			this.ModbusTCPGroupList = new List<ModbusTCPGroup>();
			base.Name = "Modbus TCP Client";
			base.Description = "1#软水系统";
			base.ModbusType = 2000;
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
			foreach (ModbusTCPGroup modbusTCPGroup in this.ModbusTCPGroupList)
			{
				foreach (ModbusTCPVariable modbusTCPVariable in modbusTCPGroup.varList)
				{
					if (modbusTCPVariable.Config.ArchiveEnable)
					{
						this.StoreVarList.Add(modbusTCPVariable);
					}
					if (this.CurrentVarList.ContainsKey(modbusTCPVariable.KeyName))
					{
						this.CurrentVarList[modbusTCPVariable.KeyName] = modbusTCPVariable;
					}
					else
					{
						this.CurrentVarList.Add(modbusTCPVariable.KeyName, modbusTCPVariable);
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
					foreach (ModbusTCPGroup modbusTCPGroup in this.ModbusTCPGroupList)
					{
						if (modbusTCPGroup.IsActive)
						{
							byte[] array = this.modtcp.Read((ModbusArea)modbusTCPGroup.StoreArea, modbusTCPGroup.Start, modbusTCPGroup.Length);
							if (modbusTCPGroup.StoreArea == ModbusStoreArea.输入线圈 | modbusTCPGroup.StoreArea == ModbusStoreArea.输出线圈)
							{
								if (array != null && array.Length == (int)((modbusTCPGroup.Length % 8 == 0) ? (modbusTCPGroup.Length / 8) : (modbusTCPGroup.Length / 8 + 1)))
								{
									base.ErrorTimes = 0;
									using (List<ModbusTCPVariable>.Enumerator enumerator2 = modbusTCPGroup.varList.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											ModbusTCPVariable modbusTCPVariable = enumerator2.Current;
											int num;
											int num2;
											if (base.VerifyModbusAddress(true, modbusTCPVariable.VarAddress, out num, out num2))
											{
												num -= (int)modbusTCPGroup.Start;
												if (modbusTCPVariable.VarType == DataType.Bool)
												{
													modbusTCPVariable.Value = BitLib.GetBitArrayFromByteArray(array, false)[num];
												}
												base.UpdateCurrentValue(modbusTCPVariable);
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
								if (array != null && array.Length == (int)(modbusTCPGroup.Length * 2))
								{
									base.ErrorTimes = 0;
									using (List<ModbusTCPVariable>.Enumerator enumerator3 = modbusTCPGroup.varList.GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											ModbusTCPVariable modbusTCPVariable2 = enumerator3.Current;
											int num3;
											int num4;
											if (base.VerifyModbusAddress(false, modbusTCPVariable2.VarAddress, out num3, out num4))
											{
												num3 -= (int)modbusTCPGroup.Start;
												num3 *= 2;
												switch (modbusTCPVariable2.VarType)
												{
												case DataType.Bool:
													modbusTCPVariable2.Value = BitLib.GetBitFrom2ByteArray(array, num3, num4, false);
													break;
												case DataType.Byte:
													modbusTCPVariable2.Value = ByteLib.GetByteFromByteArray(array, num3);
													break;
												case DataType.Short:
													modbusTCPVariable2.Value = ShortLib.GetShortFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.UShort:
													modbusTCPVariable2.Value = UShortLib.GetUShortFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Int:
													modbusTCPVariable2.Value = IntLib.GetIntFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.UInt:
													modbusTCPVariable2.Value = UIntLib.GetUIntFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Float:
													modbusTCPVariable2.Value = FloatLib.GetFloatFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Double:
													modbusTCPVariable2.Value = DoubleLib.GetDoubleFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Long:
													modbusTCPVariable2.Value = LongLib.GetLongFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.ULong:
													modbusTCPVariable2.Value = ULongLib.GetULongFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.String:
													modbusTCPVariable2.Value = StringLib.GetStringFromByteArray(array, num3, num4 * 2, Encoding.ASCII);
													break;
												case DataType.ByteArray:
													modbusTCPVariable2.Value = ByteArrayLib.GetByteArray(array, num3, num4 * 2);
													break;
												case DataType.HexString:
													modbusTCPVariable2.Value = StringLib.GetHexStringFromByteArray(array, num3, num4 * 2, ' ');
													break;
												}
												modbusTCPVariable2.Value = MigrationLib.GetMigrationValue(modbusTCPVariable2.Value, modbusTCPVariable2.Scale, modbusTCPVariable2.Offset);
												base.UpdateCurrentValue(modbusTCPVariable2);
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
						ModbusTcp modbusTcp = this.modtcp;
						if (modbusTcp != null)
						{
							modbusTcp.DisConnect();
						}
					}
					this.modtcp = new ModbusTcp();
					this.modtcp.ConnectTimeOut = base.ConnectTimeOut;
					this.modtcp.DataFormat = this.DataFormat;
					this.IsConnected = this.modtcp.Connect(this.ServerURL, this.Port);
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
				ModbusTCPVariable modbusTCPVariable = this.CurrentVarList[keyName] as ModbusTCPVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(modbusTCPVariable, modbusTCPVariable.VarType, setValue);
				if (xktResult.IsSuccess)
				{
					result = this.modtcp.Write(modbusTCPVariable.VarAddress, xktResult.Content, modbusTCPVariable.VarType);
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
        
		public ModbusTcp modtcp;
        
		public List<ModbusTCPGroup> ModbusTCPGroupList;
	}
}
