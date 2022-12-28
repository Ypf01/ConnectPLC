using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	public class NodeOmron : DeviceNode, IXmlConvert
	{
		public NodeOmron()
		{
			this.sw = new Stopwatch();
			this.DeviceGroupList = new List<OmronDeviceGroup>();
			this.DA2 = 0;
			this.DA1 = 11;
			this.SA1 = 12;
			base.Name = "欧姆龙PLC";
			base.Description = "真空系统1#PLC";
			base.DeviceType = 40;
			this.IpAddress = "192.168.0.3";
			this.Port = 9600;
			this.PlcType = OmronProtocol.FinsTCP;
			this.FirstConnect = true;
		}
        
		public bool FirstConnect { get; set; }
        
		public long CommRate { get; set; }
        
		public string IpAddress { get; set; }
        
		public int Port { get; set; }
        
		public OmronProtocol PlcType { get; set; }
        
		public bool IsConnected { get; set; }
        
		public byte DA2 { get; set; }
        
		public byte DA1 { get; set; }
        
		public byte SA1 { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.IpAddress = element.Attribute("IpAddress").Value;
			this.Port = int.Parse(element.Attribute("Port").Value);
			this.PlcType = (OmronProtocol)Enum.Parse(typeof(OmronProtocol), element.Attribute("PlcType").Value, true);
			this.DA2 = Convert.ToByte(element.Attribute("DA2").Value);
			this.DA1 = Convert.ToByte(element.Attribute("DA1").Value);
			this.SA1 = Convert.ToByte(element.Attribute("SA1").Value);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("IpAddress", this.IpAddress);
			xelement.SetAttributeValue("Port", this.Port);
			xelement.SetAttributeValue("PlcType", this.PlcType);
			xelement.SetAttributeValue("DA1", this.DA1);
			xelement.SetAttributeValue("DA2", this.DA2);
			xelement.SetAttributeValue("SA1", this.SA1);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("IP地址", this.IpAddress));
			nodeClassRenders.Add(new NodeClassRenderItem("端口号", this.Port.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("协议类型", this.PlcType.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("PLC单元号", this.DA2.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("PLC节点号", this.DA1.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("PC节点号", this.SA1.ToString()));
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
			foreach (OmronDeviceGroup omronDeviceGroup in this.DeviceGroupList)
			{
				foreach (OmronVariable omronVariable in omronDeviceGroup.varList)
				{
					if (omronVariable.Config.ArchiveEnable)
					{
						this.StoreVarList.Add(omronVariable);
					}
					if (this.CurrentVarList.ContainsKey(omronVariable.KeyName))
					{
						this.CurrentVarList[omronVariable.KeyName] = omronVariable;
					}
					else
					{
						this.CurrentVarList.Add(omronVariable.KeyName, omronVariable);
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
					OmronProtocol plcType = this.PlcType;
					OmronProtocol omronProtocol = plcType;
					if (omronProtocol == OmronProtocol.FinsTCP)
					{
						goto IL_329;
					}
					if (omronProtocol == OmronProtocol.FinsUDP)
					{
						using (List<OmronDeviceGroup>.Enumerator enumerator = this.DeviceGroupList.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								OmronDeviceGroup omronDeviceGroup = enumerator.Current;
								if (omronDeviceGroup.IsActive)
								{
									OmronStoreArea storeArea = omronDeviceGroup.StoreArea;
									OmronStoreArea omronStoreArea = storeArea;
									if (omronStoreArea <= OmronStoreArea.E2存储区)
									{
										byte[] array = this.finsudp.ReadBytes(omronDeviceGroup.Start, Convert.ToUInt16(omronDeviceGroup.Length));
										if (array != null && array.Length == omronDeviceGroup.Length * 2)
										{
											base.ErrorTimes = 0;
											int omronStartByte = this.GetOmronStartByte(omronDeviceGroup.Start);
											using (List<OmronVariable>.Enumerator enumerator2 = omronDeviceGroup.varList.GetEnumerator())
											{
												while (enumerator2.MoveNext())
												{
													OmronVariable omronVariable = enumerator2.Current;
													int num;
													int num2;
													if (this.VerifyOmronAddress(omronVariable.Start, out num, out num2))
													{
														num -= omronStartByte;
														num *= 2;
														switch (omronVariable.VarType)
														{
														case DataType.Bool:
															omronVariable.Value = BitLib.GetBitFrom2ByteArray(array, num, num2, true);
															break;
														case DataType.Short:
															omronVariable.Value = ShortLib.GetShortFromByteArray(array, num, this.finsudp.DataFormat);
															break;
														case DataType.UShort:
															omronVariable.Value = UShortLib.GetUShortFromByteArray(array, num, this.finsudp.DataFormat);
															break;
														case DataType.Int:
															omronVariable.Value = IntLib.GetIntFromByteArray(array, num, this.finsudp.DataFormat);
															break;
														case DataType.UInt:
															omronVariable.Value = UIntLib.GetUIntFromByteArray(array, num, this.finsudp.DataFormat);
															break;
														case DataType.Float:
															omronVariable.Value = FloatLib.GetFloatFromByteArray(array, num, this.finsudp.DataFormat);
															break;
														case DataType.Double:
															omronVariable.Value = DoubleLib.GetDoubleFromByteArray(array, num, this.finsudp.DataFormat);
															break;
														case DataType.String:
															omronVariable.Value = StringLib.GetStringFromByteArray(array, num, num2 * 2, Encoding.ASCII);
															break;
														case DataType.ByteArray:
															omronVariable.Value = ByteArrayLib.GetByteArray(array, num, num2 * 2);
															break;
														case DataType.HexString:
															omronVariable.Value = StringLib.GetHexStringFromByteArray(array, num, num2 * 2, ' ');
															break;
														}
														omronVariable.Value = MigrationLib.GetMigrationValue(omronVariable.Value, omronVariable.Scale, omronVariable.Offset);
														base.UpdateCurrentValue(omronVariable);
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
							goto IL_729;
						}
						//goto IL_329;
					}
					IL_729:
					this.CommRate = this.sw.ElapsedMilliseconds;
					continue;
					IL_329:
					using (List<OmronDeviceGroup>.Enumerator enumerator3 = this.DeviceGroupList.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							OmronDeviceGroup omronDeviceGroup2 = enumerator3.Current;
							if (omronDeviceGroup2.IsActive)
							{
								OmronStoreArea storeArea2 = omronDeviceGroup2.StoreArea;
								OmronStoreArea omronStoreArea2 = storeArea2;
								if (omronStoreArea2 <= OmronStoreArea.E2存储区)
								{
									byte[] array2 = this.finstcp.ReadBytes(omronDeviceGroup2.Start, Convert.ToUInt16(omronDeviceGroup2.Length));
									if (array2 != null)
									{
										base.ErrorTimes = 0;
										int omronStartByte2 = this.GetOmronStartByte(omronDeviceGroup2.Start);
										using (List<OmronVariable>.Enumerator enumerator4 = omronDeviceGroup2.varList.GetEnumerator())
										{
											while (enumerator4.MoveNext())
											{
												OmronVariable omronVariable2 = enumerator4.Current;
												int num3;
												int num4;
												if (this.VerifyOmronAddress(omronVariable2.Start, out num3, out num4))
												{
													num3 -= omronStartByte2;
													num3 *= 2;
													switch (omronVariable2.VarType)
													{
													case DataType.Bool:
														omronVariable2.Value = BitLib.GetBitFrom2ByteArray(array2, num3, num4, true);
														break;
													case DataType.Short:
														omronVariable2.Value = ShortLib.GetShortFromByteArray(array2, num3, this.finstcp.DataFormat);
														break;
													case DataType.UShort:
														omronVariable2.Value = UShortLib.GetUShortFromByteArray(array2, num3, this.finstcp.DataFormat);
														break;
													case DataType.Int:
														omronVariable2.Value = IntLib.GetIntFromByteArray(array2, num3, this.finstcp.DataFormat);
														break;
													case DataType.UInt:
														omronVariable2.Value = UIntLib.GetUIntFromByteArray(array2, num3, this.finstcp.DataFormat);
														break;
													case DataType.Float:
														omronVariable2.Value = FloatLib.GetFloatFromByteArray(array2, num3, this.finstcp.DataFormat);
														break;
													case DataType.Double:
														omronVariable2.Value = DoubleLib.GetDoubleFromByteArray(array2, num3, this.finstcp.DataFormat);
														break;
													case DataType.String:
														omronVariable2.Value = StringLib.GetStringFromByteArray(array2, num3, num4 * 2, Encoding.ASCII);
														break;
													case DataType.ByteArray:
														omronVariable2.Value = ByteArrayLib.GetByteArray(array2, num3, num4 * 2);
														break;
													case DataType.HexString:
														omronVariable2.Value = StringLib.GetHexStringFromByteArray(array2, num3, num4 * 2, ' ');
														break;
													}
													omronVariable2.Value = MigrationLib.GetMigrationValue(omronVariable2.Value, omronVariable2.Scale, omronVariable2.Offset);
													base.UpdateCurrentValue(omronVariable2);
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
						goto IL_729;
					}
				}
				if (!this.FirstConnect)
				{
					Thread.Sleep(base.ReConnectTime);
					OmronProtocol plcType2 = this.PlcType;
					OmronProtocol omronProtocol2 = plcType2;
					if (omronProtocol2 != OmronProtocol.FinsTCP)
					{
						if (omronProtocol2 != OmronProtocol.FinsUDP)
						{
						}
					}
					else
					{
						byte b;
						do
						{
							b = (byte)new Random().Next(0, 255);
						}
						while (b == this.finstcp.SA1);
						this.finstcp.SA1 = b;
						this.finstcp.DisConnect();
					}
				}
				OmronProtocol plcType3 = this.PlcType;
				OmronProtocol omronProtocol3 = plcType3;
				if (omronProtocol3 != OmronProtocol.FinsTCP)
				{
					if (omronProtocol3 == OmronProtocol.FinsUDP)
					{
						this.finsudp = new OmronFinsUDP(this.IpAddress, this.Port, DataFormat.CDAB);
						this.finsudp.SA1 = this.SA1;
						this.IsConnected = true;
					}
				}
				else
				{
					this.finstcp = new OmronFinsTCP(DataFormat.CDAB);
					this.finstcp.ConnectTimeOut = base.ConnectTimeOut;
					this.finstcp.DA1 = this.DA1;
					this.finstcp.DA2 = this.DA2;
					this.finstcp.SA1 = this.SA1;
					this.IsConnected = this.finstcp.Connect(this.IpAddress, this.Port);
				}
				this.FirstConnect = false;
			}
		}        

		public bool VerifyOmronAddress(string address, out int start, out int offset)
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
				OmronVariable omronVariable = this.CurrentVarList[keyName] as OmronVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(omronVariable, omronVariable.VarType, setValue);
				if (xktResult.IsSuccess)
				{
					OmronProtocol plcType = this.PlcType;
					OmronProtocol omronProtocol = plcType;
					if (omronProtocol != OmronProtocol.FinsTCP)
					{
						if (omronProtocol != OmronProtocol.FinsUDP)
						{
							result = CalResult.CreateFailedResult();
						}
						else
						{
							result = this.finsudp.Write(omronVariable.VarAddress, xktResult.Content, omronVariable.VarType);
						}
					}
					else
					{
						result = this.finstcp.Write(omronVariable.VarAddress, xktResult.Content, omronVariable.VarType);
					}
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
        
		public List<OmronDeviceGroup> DeviceGroupList;
        
		public OmronFinsTCP finstcp;
        
		public OmronFinsUDP finsudp;
        
	}
}
