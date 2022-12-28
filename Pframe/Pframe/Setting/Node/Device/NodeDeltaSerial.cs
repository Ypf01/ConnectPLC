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
using Pframe.PLC.Delta;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Device
{
	public class NodeDeltaSerial : DeviceNode, IXmlConvert
	{
		public NodeDeltaSerial()
		{
			this.sw = new Stopwatch();
			this.DeviceGroupList = new List<DeltaSerialDeviceGroup>();
			base.Name = "台达PLC_Serial";
			base.Description = "称重系统1#PLC";
			base.DeviceType = 120;
			this.PortNum = "COM3";
			this.Paud = 19200;
			this.Parity = Parity.Even;
			this.DataBits = "8";
			this.StopBits = StopBits.One;
			this.SleepTime = 20;
			this.FirstConnect = true;
		}
        
		public bool FirstConnect { get; set; }
        
		public bool ConnectState { get; set; }
        
		public DeltaModbusSerialType DeltaSerialType { get; set; }
        
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
			this.DeltaSerialType = (DeltaModbusSerialType)Enum.Parse(typeof(DeltaModbusSerialType), element.Attribute("PlcType").Value, true);
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
			xelement.SetAttributeValue("PlcType", this.DeltaSerialType);
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
			nodeClassRenders.Add(new NodeClassRenderItem("协议类型", this.DeltaSerialType.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("延时时间", this.SleepTime.ToString()));
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
			foreach (DeltaSerialDeviceGroup deltaSerialDeviceGroup in this.DeviceGroupList)
			{
				foreach (DeltaSerialVariable deltaSerialVariable in deltaSerialDeviceGroup.varList)
				{
					if (deltaSerialVariable.Config.ArchiveEnable)
					{
						this.StoreVarList.Add(deltaSerialVariable);
					}
					if (this.CurrentVarList.ContainsKey(deltaSerialVariable.KeyName))
					{
						this.CurrentVarList[deltaSerialVariable.KeyName] = deltaSerialVariable;
					}
					else
					{
						this.CurrentVarList.Add(deltaSerialVariable.KeyName, deltaSerialVariable);
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
					foreach (DeltaSerialDeviceGroup deltaSerialDeviceGroup in this.DeviceGroupList)
					{
						if (deltaSerialDeviceGroup.IsActive)
						{
							DeltaStoreArea storeArea = deltaSerialDeviceGroup.StoreArea;
							DeltaStoreArea deltaStoreArea = storeArea;
							if (deltaStoreArea > DeltaStoreArea.C存储区)
							{
								if (deltaStoreArea - DeltaStoreArea.D存储区 <= 2)
								{
									CalResult<byte[]> xktResult = this.deltaSerial.ReadBytes(deltaSerialDeviceGroup.Start, Convert.ToUInt16(deltaSerialDeviceGroup.Length), deltaSerialDeviceGroup.SlaveID);
									this.ConnectState = xktResult.IsSuccess;
									if (xktResult.IsSuccess)
									{
										base.ErrorTimes = 0;
										int deltaStart = this.GetDeltaStart(deltaSerialDeviceGroup.Start.Substring(deltaSerialDeviceGroup.Start.IndexOf(deltaSerialDeviceGroup.Start.First((char c) => char.IsDigit(c)))), deltaSerialDeviceGroup.StoreArea);
										using (List<DeltaSerialVariable>.Enumerator enumerator2 = deltaSerialDeviceGroup.varList.GetEnumerator())
										{
											while (enumerator2.MoveNext())
											{
												DeltaSerialVariable deltaSerialVariable = enumerator2.Current;
												int num;
												int num2;
												if (this.VerifyDeltaAddress(false, deltaSerialDeviceGroup.StoreArea, deltaSerialVariable.Start, out num, out num2))
												{
													num -= deltaStart;
													num *= 2;
													switch (deltaSerialVariable.VarType)
													{
													case DataType.Bool:
														deltaSerialVariable.Value = BitLib.GetBitFrom2ByteArray(xktResult.Content, num, num2, false);
														break;
													case DataType.Short:
														deltaSerialVariable.Value = ShortLib.GetShortFromByteArray(xktResult.Content, num, this.DataFormat);
														break;
													case DataType.UShort:
														deltaSerialVariable.Value = UShortLib.GetUShortFromByteArray(xktResult.Content, num, this.DataFormat);
														break;
													case DataType.Int:
														deltaSerialVariable.Value = IntLib.GetIntFromByteArray(xktResult.Content, num, this.DataFormat);
														break;
													case DataType.UInt:
														deltaSerialVariable.Value = UIntLib.GetUIntFromByteArray(xktResult.Content, num, this.DataFormat);
														break;
													case DataType.Float:
														deltaSerialVariable.Value = FloatLib.GetFloatFromByteArray(xktResult.Content, num, this.DataFormat);
														break;
													case DataType.Double:
														deltaSerialVariable.Value = DoubleLib.GetDoubleFromByteArray(xktResult.Content, num, this.DataFormat);
														break;
													case DataType.Long:
														deltaSerialVariable.Value = LongLib.GetLongFromByteArray(xktResult.Content, num, this.DataFormat);
														break;
													case DataType.ULong:
														deltaSerialVariable.Value = ULongLib.GetULongFromByteArray(xktResult.Content, num, this.DataFormat);
														break;
													case DataType.String:
														deltaSerialVariable.Value = StringLib.GetStringFromByteArray(xktResult.Content, num, num2 * 2, Encoding.ASCII);
														break;
													case DataType.ByteArray:
														deltaSerialVariable.Value = ByteArrayLib.GetByteArray(xktResult.Content, num, num2 * 2);
														break;
													case DataType.HexString:
														deltaSerialVariable.Value = StringLib.GetHexStringFromByteArray(xktResult.Content, num, num2 * 2, ' ');
														break;
													}
													deltaSerialVariable.Value = MigrationLib.GetMigrationValue(deltaSerialVariable.Value, deltaSerialVariable.Scale, deltaSerialVariable.Offset);
													base.UpdateCurrentValue(deltaSerialVariable);
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
							else
							{
								CalResult<byte[]> xktResult2 = this.deltaSerial.ReadBytes(deltaSerialDeviceGroup.Start, Convert.ToUInt16(deltaSerialDeviceGroup.Length), deltaSerialDeviceGroup.SlaveID);
								this.ConnectState = xktResult2.IsSuccess;
								if (xktResult2.IsSuccess)
								{
									base.ErrorTimes = 0;
									int deltaStart2 = this.GetDeltaStart(deltaSerialDeviceGroup.Start.Substring(deltaSerialDeviceGroup.Start.IndexOf(deltaSerialDeviceGroup.Start.First((char c) => char.IsDigit(c)))), deltaSerialDeviceGroup.StoreArea);
									using (List<DeltaSerialVariable>.Enumerator enumerator3 = deltaSerialDeviceGroup.varList.GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											DeltaSerialVariable deltaSerialVariable2 = enumerator3.Current;
											int num3;
											int num4;
											if (this.VerifyDeltaAddress(true, deltaSerialDeviceGroup.StoreArea, deltaSerialVariable2.Start, out num3, out num4))
											{
												if (deltaSerialVariable2.VarType == DataType.Bool)
												{
													num3 -= deltaStart2;
													deltaSerialVariable2.Value = BitLib.GetBitArrayFromByteArray(xktResult2.Content, false)[num3];
												}
												base.UpdateCurrentValue(deltaSerialVariable2);
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
						DeltaModbusSerial deltaModbusSerial = this.deltaSerial;
						if (deltaModbusSerial != null)
						{
							deltaModbusSerial.DisConnect();
						}
					}
					DeltaModbusSerialType deltaSerialType = this.DeltaSerialType;
					DeltaModbusSerialType deltaModbusSerialType = deltaSerialType;
					if (deltaModbusSerialType != DeltaModbusSerialType.DeltaRTU)
					{
						if (deltaModbusSerialType == DeltaModbusSerialType.AscII)
						{
							this.deltaSerial = new DeltaModbusSerial(DeltaModbusSerialType.AscII, this.SleepTime, DataFormat.CDAB);
							this.IsConnected = this.deltaSerial.Connect(this.PortNum, this.Paud, int.Parse(this.DataBits), this.Parity, this.StopBits);
						}
					}
					else
					{
						this.deltaSerial = new DeltaModbusSerial(DeltaModbusSerialType.DeltaRTU, this.SleepTime, DataFormat.CDAB);
						this.IsConnected = this.deltaSerial.Connect(this.PortNum, this.Paud, int.Parse(this.DataBits), this.Parity, this.StopBits);
					}
					this.FirstConnect = false;
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
				DeltaSerialVariable deltaSerialVariable = this.CurrentVarList[keyName] as DeltaSerialVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(deltaSerialVariable, deltaSerialVariable.VarType, setValue);
				if (xktResult.IsSuccess)
				{
					result = this.deltaSerial.Write(deltaSerialVariable.VarAddress, xktResult.Content, deltaSerialVariable.VarType, 1);
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
        
		public DeltaModbusSerial deltaSerial;
        
		public List<DeltaSerialDeviceGroup> DeviceGroupList;
	}
}
