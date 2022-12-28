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
	public class NodeModbusUDP : ModbusNode, IXmlConvert
	{
		public NodeModbusUDP()
		{
			this.sw = new Stopwatch();
			this.ModbusUDPGroupList = new List<ModbusUDPGroup>();
			base.Name = "Modbus UDP Device";
			base.Description = "1#软水系统";
			base.ModbusType = 4000;
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
			foreach (ModbusUDPGroup modbusUDPGroup in this.ModbusUDPGroupList)
			{
				foreach (ModbusUDPVariable modbusUDPVariable in modbusUDPGroup.varList)
				{
					if (modbusUDPVariable.Config.ArchiveEnable)
					{
						this.StoreVarList.Add(modbusUDPVariable);
					}
					if (this.CurrentVarList.ContainsKey(modbusUDPVariable.KeyName))
					{
						this.CurrentVarList[modbusUDPVariable.KeyName] = modbusUDPVariable;
					}
					else
					{
						this.CurrentVarList.Add(modbusUDPVariable.KeyName, modbusUDPVariable);
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
					foreach (ModbusUDPGroup modbusUDPGroup in this.ModbusUDPGroupList)
					{
						if (modbusUDPGroup.IsActive)
						{
							byte[] array = this.modudp.Read((ModbusArea)modbusUDPGroup.StoreArea, modbusUDPGroup.Start, modbusUDPGroup.Length);
							if (modbusUDPGroup.StoreArea == ModbusStoreArea.输入线圈 | modbusUDPGroup.StoreArea == ModbusStoreArea.输出线圈)
							{
								if (array != null && array.Length == (int)((modbusUDPGroup.Length % 8 == 0) ? (modbusUDPGroup.Length / 8) : (modbusUDPGroup.Length / 8 + 1)))
								{
									using (List<ModbusUDPVariable>.Enumerator enumerator2 = modbusUDPGroup.varList.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											ModbusUDPVariable modbusUDPVariable = enumerator2.Current;
											int num;
											int num2;
											if (base.VerifyModbusAddress(true, modbusUDPVariable.VarAddress, out num, out num2))
											{
												num -= (int)modbusUDPGroup.Start;
												if (modbusUDPVariable.VarType == DataType.Bool)
												{
													modbusUDPVariable.Value = BitLib.GetBitArrayFromByteArray(array, false)[num];
												}
												base.UpdateCurrentValue(modbusUDPVariable);
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
								if (array != null && array.Length == (int)(modbusUDPGroup.Length * 2))
								{
									using (List<ModbusUDPVariable>.Enumerator enumerator3 = modbusUDPGroup.varList.GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											ModbusUDPVariable modbusUDPVariable2 = enumerator3.Current;
											int num3;
											int num4;
											if (base.VerifyModbusAddress(false, modbusUDPVariable2.VarAddress, out num3, out num4))
											{
												num3 -= (int)modbusUDPGroup.Start;
												num3 *= 2;
												switch (modbusUDPVariable2.VarType)
												{
												case DataType.Bool:
													modbusUDPVariable2.Value = BitLib.GetBitFrom2ByteArray(array, num3, num4, false);
													break;
												case DataType.Byte:
													modbusUDPVariable2.Value = ByteLib.GetByteFromByteArray(array, num3);
													break;
												case DataType.Short:
													modbusUDPVariable2.Value = ShortLib.GetShortFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.UShort:
													modbusUDPVariable2.Value = UShortLib.GetUShortFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Int:
													modbusUDPVariable2.Value = IntLib.GetIntFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.UInt:
													modbusUDPVariable2.Value = UIntLib.GetUIntFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Float:
													modbusUDPVariable2.Value = FloatLib.GetFloatFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Double:
													modbusUDPVariable2.Value = DoubleLib.GetDoubleFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Long:
													modbusUDPVariable2.Value = LongLib.GetLongFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.ULong:
													modbusUDPVariable2.Value = ULongLib.GetULongFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.String:
													modbusUDPVariable2.Value = StringLib.GetStringFromByteArray(array, num3, num4 * 2, Encoding.ASCII);
													break;
												case DataType.ByteArray:
													modbusUDPVariable2.Value = ByteArrayLib.GetByteArray(array, num3, num4 * 2);
													break;
												case DataType.HexString:
													modbusUDPVariable2.Value = StringLib.GetHexStringFromByteArray(array, num3, num4 * 2, ' ');
													break;
												}
												modbusUDPVariable2.Value = MigrationLib.GetMigrationValue(modbusUDPVariable2.Value, modbusUDPVariable2.Scale, modbusUDPVariable2.Offset);
												base.UpdateCurrentValue(modbusUDPVariable2);
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
					this.modudp = new ModbusUdp(this.ServerURL, int.Parse(this.Port.ToString()), this.DataFormat);
					base.ConnectTimeOut = base.ConnectTimeOut;
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
				ModbusUDPVariable modbusUDPVariable = this.CurrentVarList[keyName] as ModbusUDPVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(modbusUDPVariable, modbusUDPVariable.VarType, setValue);
				if (xktResult.IsSuccess)
				{
					result = this.modudp.Write(modbusUDPVariable.VarAddress, xktResult.Content, modbusUDPVariable.VarType);
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
        
		public ModbusUdp modudp;
        
		public List<ModbusUDPGroup> ModbusUDPGroupList;
	}
}
