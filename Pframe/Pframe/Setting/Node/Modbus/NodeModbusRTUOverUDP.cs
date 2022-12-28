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
	public class NodeModbusRTUOverUDP : ModbusNode, IXmlConvert
	{
		public NodeModbusRTUOverUDP()
		{
			this.sw = new Stopwatch();
			this.ModbusRTUOverUDPGroupList = new List<ModbusRTUOverUDPGroup>();
			base.Name = "ModbusRTUOverUDP";
			base.Description = "1#软水系统";
			base.ModbusType = 6000;
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
			foreach (ModbusRTUOverUDPGroup modbusRTUOverUDPGroup in this.ModbusRTUOverUDPGroupList)
			{
				foreach (ModbusRTUOverUDPVariable modbusRTUOverUDPVariable in modbusRTUOverUDPGroup.varList)
				{
					if (modbusRTUOverUDPVariable.Config.ArchiveEnable)
					{
						this.StoreVarList.Add(modbusRTUOverUDPVariable);
					}
					if (this.CurrentVarList.ContainsKey(modbusRTUOverUDPVariable.KeyName))
					{
						this.CurrentVarList[modbusRTUOverUDPVariable.KeyName] = modbusRTUOverUDPVariable;
					}
					else
					{
						this.CurrentVarList.Add(modbusRTUOverUDPVariable.KeyName, modbusRTUOverUDPVariable);
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
					foreach (ModbusRTUOverUDPGroup modbusRTUOverUDPGroup in this.ModbusRTUOverUDPGroupList)
					{
						if (modbusRTUOverUDPGroup.IsActive)
						{
							byte[] array = this.modoverudp.Read((ModbusArea)modbusRTUOverUDPGroup.StoreArea, modbusRTUOverUDPGroup.SlaveID, modbusRTUOverUDPGroup.Start, modbusRTUOverUDPGroup.Length);
							if (modbusRTUOverUDPGroup.StoreArea == ModbusStoreArea.输入线圈 | modbusRTUOverUDPGroup.StoreArea == ModbusStoreArea.输出线圈)
							{
								if (array != null && array.Length == (int)((modbusRTUOverUDPGroup.Length % 8 == 0) ? (modbusRTUOverUDPGroup.Length / 8) : (modbusRTUOverUDPGroup.Length / 8 + 1)))
								{
									using (List<ModbusRTUOverUDPVariable>.Enumerator enumerator2 = modbusRTUOverUDPGroup.varList.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											ModbusRTUOverUDPVariable modbusRTUOverUDPVariable = enumerator2.Current;
											int num;
											int num2;
											if (base.VerifyModbusAddress(true, modbusRTUOverUDPVariable.VarAddress, out num, out num2))
											{
												num -= (int)modbusRTUOverUDPGroup.Start;
												if (modbusRTUOverUDPVariable.VarType == DataType.Bool)
												{
													modbusRTUOverUDPVariable.Value = BitLib.GetBitArrayFromByteArray(array, false)[num];
												}
											}
											base.UpdateCurrentValue(modbusRTUOverUDPVariable);
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
								if (array != null && array.Length == (int)(modbusRTUOverUDPGroup.Length * 2))
								{
									using (List<ModbusRTUOverUDPVariable>.Enumerator enumerator3 = modbusRTUOverUDPGroup.varList.GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											ModbusRTUOverUDPVariable modbusRTUOverUDPVariable2 = enumerator3.Current;
											int num3;
											int num4;
											if (base.VerifyModbusAddress(false, modbusRTUOverUDPVariable2.VarAddress, out num3, out num4))
											{
												num3 -= (int)modbusRTUOverUDPGroup.Start;
												num3 *= 2;
												switch (modbusRTUOverUDPVariable2.VarType)
												{
												case DataType.Bool:
													modbusRTUOverUDPVariable2.Value = BitLib.GetBitFrom2ByteArray(array, num3, num4, false);
													break;
												case DataType.Byte:
													modbusRTUOverUDPVariable2.Value = ByteLib.GetByteFromByteArray(array, num3);
													break;
												case DataType.Short:
													modbusRTUOverUDPVariable2.Value = ShortLib.GetShortFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.UShort:
													modbusRTUOverUDPVariable2.Value = UShortLib.GetUShortFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Int:
													modbusRTUOverUDPVariable2.Value = IntLib.GetIntFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.UInt:
													modbusRTUOverUDPVariable2.Value = UIntLib.GetUIntFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Float:
													modbusRTUOverUDPVariable2.Value = FloatLib.GetFloatFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Double:
													modbusRTUOverUDPVariable2.Value = DoubleLib.GetDoubleFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.Long:
													modbusRTUOverUDPVariable2.Value = LongLib.GetLongFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.ULong:
													modbusRTUOverUDPVariable2.Value = ULongLib.GetULongFromByteArray(array, num3, this.DataFormat);
													break;
												case DataType.String:
													modbusRTUOverUDPVariable2.Value = StringLib.GetStringFromByteArray(array, num3, num4 * 2, Encoding.ASCII);
													break;
												case DataType.ByteArray:
													modbusRTUOverUDPVariable2.Value = ByteArrayLib.GetByteArray(array, num3, num4 * 2);
													break;
												case DataType.HexString:
													modbusRTUOverUDPVariable2.Value = StringLib.GetHexStringFromByteArray(array, num3, num4 * 2, ' ');
													break;
												}
												modbusRTUOverUDPVariable2.Value = MigrationLib.GetMigrationValue(modbusRTUOverUDPVariable2.Value, modbusRTUOverUDPVariable2.Scale, modbusRTUOverUDPVariable2.Offset);
												base.UpdateCurrentValue(modbusRTUOverUDPVariable2);
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
					this.modoverudp = new ModbusRtuOverUdp(this.ServerURL, int.Parse(this.Port.ToString()), this.DataFormat);
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
				ModbusRTUOverUDPVariable modbusRTUOverUDPVariable = this.CurrentVarList[keyName] as ModbusRTUOverUDPVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(modbusRTUOverUDPVariable, modbusRTUOverUDPVariable.VarType, setValue);
				if (xktResult.IsSuccess)
				{
					result = this.modoverudp.Write(modbusRTUOverUDPVariable.GroupID, modbusRTUOverUDPVariable.VarAddress, xktResult.Content, modbusRTUOverUDPVariable.VarType);
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
        
		public ModbusRtuOverUdp modoverudp;
        
		public List<ModbusRTUOverUDPGroup> ModbusRTUOverUDPGroupList;
	}
}
