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
using Pframe.PLC.Delta;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Device
{
	public class NodeDeltaEthernet : DeviceNode, IXmlConvert
	{
		public NodeDeltaEthernet()
		{
			this.sw = new Stopwatch();
			this.DeviceGroupList = new List<DeltaEthernetDeviceGroup>();
			base.Name = "台达PLC";
			base.Description = "真空系统1#PLC";
			base.DeviceType = 140;
			this.IpAddress = "127.0.0.1";
			this.Port = 502;
			this.WaitTimes = 5;
			this.DeltaEthernetType = DeltaModbusEthernetType.DeltaTCP;
			this.FirstConnect = true;
		}
        
		public bool FirstConnect { get; set; }
        
		public long CommRate { get; set; }
        
		public string IpAddress { get; set; }
        
		public int WaitTimes { get; set; }
        
		public int Port { get; set; }
        
		public DeltaModbusEthernetType DeltaEthernetType { get; set; }
        
		public bool IsConnected { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.IpAddress = element.Attribute("IpAddress").Value;
			this.Port = int.Parse(element.Attribute("Port").Value);
			this.DeltaEthernetType = (DeltaModbusEthernetType)Enum.Parse(typeof(DeltaModbusEthernetType), element.Attribute("PlcType").Value, true);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("IpAddress", this.IpAddress);
			xelement.SetAttributeValue("Port", this.Port);
			xelement.SetAttributeValue("PlcType", this.DeltaEthernetType);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("IP地址", this.IpAddress));
			nodeClassRenders.Add(new NodeClassRenderItem("端口号", this.Port.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("协议类型", this.DeltaEthernetType.ToString()));
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
			foreach (DeltaEthernetDeviceGroup deltaEthernetDeviceGroup in this.DeviceGroupList)
			{
				foreach (DeltaEthernetVariable deltaEthernetVariable in deltaEthernetDeviceGroup.varList)
				{
					if (deltaEthernetVariable.Config.ArchiveEnable)
					{
						this.StoreVarList.Add(deltaEthernetVariable);
					}
					if (this.CurrentVarList.ContainsKey(deltaEthernetVariable.KeyName))
					{
						this.CurrentVarList[deltaEthernetVariable.KeyName] = deltaEthernetVariable;
					}
					else
					{
						this.CurrentVarList.Add(deltaEthernetVariable.KeyName, deltaEthernetVariable);
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
					foreach (DeltaEthernetDeviceGroup deltaEthernetDeviceGroup in this.DeviceGroupList)
					{
						if (deltaEthernetDeviceGroup.IsActive)
						{
							DeltaStoreArea storeArea = deltaEthernetDeviceGroup.StoreArea;
							DeltaStoreArea deltaStoreArea = storeArea;
							if (deltaStoreArea > DeltaStoreArea.C存储区)
							{
								if (deltaStoreArea - DeltaStoreArea.D存储区 <= 2)
								{
									byte[] array = this.deltaEthernet.ReadBytes(deltaEthernetDeviceGroup.Start, Convert.ToUInt16(deltaEthernetDeviceGroup.Length));
									if (array != null)
									{
										base.ErrorTimes = 0;
										int deltaStart = this.GetDeltaStart(deltaEthernetDeviceGroup.Start.Substring(deltaEthernetDeviceGroup.Start.IndexOf(deltaEthernetDeviceGroup.Start.First((char c) => char.IsDigit(c)))), deltaEthernetDeviceGroup.StoreArea);
										using (List<DeltaEthernetVariable>.Enumerator enumerator2 = deltaEthernetDeviceGroup.varList.GetEnumerator())
										{
											while (enumerator2.MoveNext())
											{
												DeltaEthernetVariable deltaEthernetVariable = enumerator2.Current;
												int num;
												int num2;
												if (this.VerifyDeltaAddress(false, deltaEthernetDeviceGroup.StoreArea, deltaEthernetVariable.Start, out num, out num2))
												{
													num -= deltaStart;
													num *= 2;
													switch (deltaEthernetVariable.VarType)
													{
													case DataType.Bool:
														deltaEthernetVariable.Value = BitLib.GetBitFrom2ByteArray(array, num, num2, false);
														break;
													case DataType.Short:
														deltaEthernetVariable.Value = ShortLib.GetShortFromByteArray(array, num, this.deltaEthernet.DataFormat);
														break;
													case DataType.UShort:
														deltaEthernetVariable.Value = UShortLib.GetUShortFromByteArray(array, num, this.deltaEthernet.DataFormat);
														break;
													case DataType.Int:
														deltaEthernetVariable.Value = IntLib.GetIntFromByteArray(array, num, this.deltaEthernet.DataFormat);
														break;
													case DataType.UInt:
														deltaEthernetVariable.Value = UIntLib.GetUIntFromByteArray(array, num, this.deltaEthernet.DataFormat);
														break;
													case DataType.Float:
														deltaEthernetVariable.Value = FloatLib.GetFloatFromByteArray(array, num, this.deltaEthernet.DataFormat);
														break;
													case DataType.Double:
														deltaEthernetVariable.Value = DoubleLib.GetDoubleFromByteArray(array, num, this.deltaEthernet.DataFormat);
														break;
													case DataType.Long:
														deltaEthernetVariable.Value = LongLib.GetLongFromByteArray(array, num, this.deltaEthernet.DataFormat);
														break;
													case DataType.ULong:
														deltaEthernetVariable.Value = ULongLib.GetULongFromByteArray(array, num, this.deltaEthernet.DataFormat);
														break;
													case DataType.String:
														deltaEthernetVariable.Value = StringLib.GetStringFromByteArray(array, num, num2 * 2, Encoding.ASCII);
														break;
													case DataType.ByteArray:
														deltaEthernetVariable.Value = ByteArrayLib.GetByteArray(array, num, num2 * 2);
														break;
													case DataType.HexString:
														deltaEthernetVariable.Value = StringLib.GetHexStringFromByteArray(array, num, num2 * 2, ' ');
														break;
													}
													deltaEthernetVariable.Value = MigrationLib.GetMigrationValue(deltaEthernetVariable.Value, deltaEthernetVariable.Scale, deltaEthernetVariable.Offset);
													base.UpdateCurrentValue(deltaEthernetVariable);
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
							else
							{
								byte[] array2 = this.deltaEthernet.ReadBytes(deltaEthernetDeviceGroup.Start, Convert.ToUInt16(deltaEthernetDeviceGroup.Length));
								if (array2 != null)
								{
									base.ErrorTimes = 0;
									int deltaStart2 = this.GetDeltaStart(deltaEthernetDeviceGroup.Start.Substring(deltaEthernetDeviceGroup.Start.IndexOf(deltaEthernetDeviceGroup.Start.First((char c) => char.IsDigit(c)))), deltaEthernetDeviceGroup.StoreArea);
									using (List<DeltaEthernetVariable>.Enumerator enumerator3 = deltaEthernetDeviceGroup.varList.GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											DeltaEthernetVariable deltaEthernetVariable2 = enumerator3.Current;
											int num3;
											int num4;
											if (this.VerifyDeltaAddress(true, deltaEthernetDeviceGroup.StoreArea, deltaEthernetVariable2.Start, out num3, out num4))
											{
												if (deltaEthernetVariable2.VarType == DataType.Bool)
												{
													num3 -= deltaStart2;
													deltaEthernetVariable2.Value = BitLib.GetBitArrayFromByteArray(array2, false)[num3];
												}
												base.UpdateCurrentValue(deltaEthernetVariable2);
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
						DeltaModbusEthernet deltaModbusEthernet = this.deltaEthernet;
						if (deltaModbusEthernet != null)
						{
							deltaModbusEthernet.DisConnect();
						}
					}
					DeltaModbusEthernetType deltaEthernetType = this.DeltaEthernetType;
					DeltaModbusEthernetType deltaModbusEthernetType = deltaEthernetType;
					if (deltaModbusEthernetType != DeltaModbusEthernetType.DeltaTCP)
					{
						if (deltaModbusEthernetType == DeltaModbusEthernetType.DeltaUDP)
						{
							this.deltaEthernet = new DeltaModbusEthernet(DeltaModbusEthernetType.DeltaUDP, this.WaitTimes, DataFormat.CDAB);
							this.IsConnected = this.deltaEthernet.Connect(this.IpAddress, this.Port);
						}
					}
					else
					{
						this.deltaEthernet = new DeltaModbusEthernet(DeltaModbusEthernetType.DeltaTCP, this.WaitTimes, DataFormat.CDAB);
						this.IsConnected = this.deltaEthernet.Connect(this.IpAddress, this.Port);
					}
					this.FirstConnect = true;
				}
			}
		}
        
		public int GetDeltaStart(string start, DeltaStoreArea store)
		{
			int result;
			switch (store)
			{
			case DeltaStoreArea.M存储区:
				result = Convert.ToInt32(start, DeltaModbusDataType.M.FromBase);
				break;
			case DeltaStoreArea.X存储区:
				result = Convert.ToInt32(start, DeltaModbusDataType.X.FromBase);
				break;
			case DeltaStoreArea.Y存储区:
				result = Convert.ToInt32(start, DeltaModbusDataType.Y.FromBase);
				break;
			case DeltaStoreArea.S存储区:
				result = Convert.ToInt32(start, DeltaModbusDataType.S.FromBase);
				break;
			case DeltaStoreArea.T存储区:
				result = Convert.ToInt32(start, DeltaModbusDataType.T.FromBase);
				break;
			case DeltaStoreArea.C存储区:
				result = Convert.ToInt32(start, DeltaModbusDataType.C.FromBase);
				break;
			case DeltaStoreArea.D存储区:
				result = Convert.ToInt32(start, DeltaModbusDataType.D.FromBase);
				break;
			case DeltaStoreArea.TR存储区:
				result = Convert.ToInt32(start, DeltaModbusDataType.TR.FromBase);
				break;
			case DeltaStoreArea.CR存储区:
				result = Convert.ToInt32(start, DeltaModbusDataType.CR.FromBase);
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}
        
		public bool VerifyDeltaAddress(bool isBoolStore, DeltaStoreArea store, string address, out int start, out int offset)
		{
			bool result;
			if (isBoolStore)
			{
				offset = 0;
				start = 0;
				try
				{
					start = this.GetDeltaStart(address, store);
				}
				catch (Exception)
				{
					return false;
				}
				result = true;
			}
			else if (address.Contains('.'))
			{
				string[] array = address.Split(new char[]
				{
					'.'
				});
				offset = 0;
				start = 0;
				if (array.Length == 2)
				{
					try
					{
						start = this.GetDeltaStart(array[0], store);
					}
					catch (Exception)
					{
						return false;
					}
					try
					{
						offset = int.Parse(array[1]);
					}
					catch (Exception)
					{
						return false;
					}
					result = true;
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
				DeltaEthernetVariable deltaEthernetVariable = this.CurrentVarList[keyName] as DeltaEthernetVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(deltaEthernetVariable, deltaEthernetVariable.VarType, setValue);
				if (xktResult.IsSuccess)
				{
					result = this.deltaEthernet.Write(deltaEthernetVariable.VarAddress, xktResult.Content, deltaEthernetVariable.VarType);
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
        
		public List<DeltaEthernetDeviceGroup> DeviceGroupList;
        
		public DeltaModbusEthernet deltaEthernet;
	}
}
