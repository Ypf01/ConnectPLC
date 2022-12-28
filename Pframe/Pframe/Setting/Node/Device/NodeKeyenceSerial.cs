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
using Pframe.PLC.Keyence;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Device
{
	public class NodeKeyenceSerial : DeviceNode, IXmlConvert
	{
		public NodeKeyenceSerial()
		{
			this.sw = new Stopwatch();
			this.DeviceGroupList = new List<KeyenceSerialDeviceGroup>();
			base.Name = "基恩士PLC Nano";
			base.Description = "液压系统1#PLC";
			base.DeviceType = 150;
			this.PortNum = "COM3";
			this.Paud = 9600;
			this.Parity = Parity.Even;
			this.DataBits = "7";
			this.StopBits = StopBits.One;
			this.FirstConnect = true;
			this.SleepTime = 20;
		}
        
		public bool FirstConnect { get; set; }
        
		public bool ConnectState { get; set; }
        
		public long CommRate { get; set; }
        
		public string PortNum { get; set; }
        
		public int Paud { get; set; }
        
		public Parity Parity { get; set; }
        
		public string DataBits { get; set; }
        
		public StopBits StopBits { get; set; }
        
		public int SleepTime { get; set; }
        
		public bool IsConnected { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.PortNum = element.Attribute("PortNum").Value;
			this.Paud = int.Parse(element.Attribute("Paud").Value);
			this.DataBits = element.Attribute("DataBits").Value;
			this.Parity = (Parity)Enum.Parse(typeof(Parity), element.Attribute("Parity").Value, true);
			this.StopBits = (StopBits)Enum.Parse(typeof(StopBits), element.Attribute("StopBits").Value, true);
			this.SleepTime = int.Parse(element.Attribute("SleepTime").Value);
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
			foreach (KeyenceSerialDeviceGroup keyenceSerialDeviceGroup in this.DeviceGroupList)
			{
				foreach (KeyenceSerialVariable keyenceSerialVariable in keyenceSerialDeviceGroup.varList)
				{
					if (keyenceSerialVariable.Config.ArchiveEnable)
					{
						this.StoreVarList.Add(keyenceSerialVariable);
					}
					if (this.CurrentVarList.ContainsKey(keyenceSerialVariable.KeyName))
					{
						this.CurrentVarList[keyenceSerialVariable.KeyName] = keyenceSerialVariable;
					}
					else
					{
						this.CurrentVarList.Add(keyenceSerialVariable.KeyName, keyenceSerialVariable);
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
					foreach (KeyenceSerialDeviceGroup keyenceSerialDeviceGroup in this.DeviceGroupList)
					{
						if (keyenceSerialDeviceGroup.IsActive)
						{
							KeyenceStoreArea storeArea = keyenceSerialDeviceGroup.StoreArea;
							KeyenceStoreArea keyenceStoreArea = storeArea;
							if (keyenceStoreArea > KeyenceStoreArea.CR存储区)
							{
								if (keyenceStoreArea - KeyenceStoreArea.DM存储区 <= 1)
								{
									byte[] array = this.keyenceSerial.ReadBytes(keyenceSerialDeviceGroup.Start, Convert.ToUInt16(keyenceSerialDeviceGroup.Length));
									if (array != null && array.Length == keyenceSerialDeviceGroup.Length * 2)
									{
										base.ErrorTimes = 0;
										int keyenceStart = this.GetKeyenceStart(false, keyenceSerialDeviceGroup.Start.Substring(keyenceSerialDeviceGroup.Start.IndexOf(keyenceSerialDeviceGroup.Start.First((char c) => char.IsDigit(c)))), keyenceSerialDeviceGroup.StoreArea);
										using (List<KeyenceSerialVariable>.Enumerator enumerator2 = keyenceSerialDeviceGroup.varList.GetEnumerator())
										{
											while (enumerator2.MoveNext())
											{
												KeyenceSerialVariable keyenceSerialVariable = enumerator2.Current;
												int num;
												int num2;
												if (this.VerifyKeyenceAddress(false, false, keyenceSerialDeviceGroup.StoreArea, keyenceSerialVariable.Start, out num, out num2))
												{
													num -= keyenceStart;
													num *= 2;
													switch (keyenceSerialVariable.VarType)
													{
													case DataType.Bool:
														keyenceSerialVariable.Value = BitLib.GetBitFrom2ByteArray(array, num, num2, false);
														break;
													case DataType.Short:
														keyenceSerialVariable.Value = ShortLib.GetShortFromByteArray(array, num, this.keyenceSerial.DataFormat);
														break;
													case DataType.UShort:
														keyenceSerialVariable.Value = UShortLib.GetUShortFromByteArray(array, num, this.keyenceSerial.DataFormat);
														break;
													case DataType.Int:
														keyenceSerialVariable.Value = IntLib.GetIntFromByteArray(array, num, this.keyenceSerial.DataFormat);
														break;
													case DataType.UInt:
														keyenceSerialVariable.Value = UIntLib.GetUIntFromByteArray(array, num, this.keyenceSerial.DataFormat);
														break;
													case DataType.Float:
														keyenceSerialVariable.Value = FloatLib.GetFloatFromByteArray(array, num, this.keyenceSerial.DataFormat);
														break;
													case DataType.Double:
														keyenceSerialVariable.Value = DoubleLib.GetDoubleFromByteArray(array, num, this.keyenceSerial.DataFormat);
														break;
													case DataType.String:
													{
														string[] array2 = keyenceSerialVariable.Start.Split(new char[]
														{
															'.'
														});
														int num3 = this.GetKeyenceStart(false, array2[0], keyenceSerialDeviceGroup.StoreArea) - keyenceStart;
														int.Parse(array2[1]);
														keyenceSerialVariable.Value = StringLib.GetStringFromByteArray(array, num, num2, Encoding.ASCII);
														break;
													}
													}
													keyenceSerialVariable.Value = MigrationLib.GetMigrationValue(keyenceSerialVariable.Value, keyenceSerialVariable.Scale, keyenceSerialVariable.Offset);
													base.UpdateCurrentValue(keyenceSerialVariable);
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
								byte[] array = this.keyenceSerial.ReadBytes(keyenceSerialDeviceGroup.Start, Convert.ToUInt16(keyenceSerialDeviceGroup.Length));
								if (array != null && array.Length >= keyenceSerialDeviceGroup.Length)
								{
									base.ErrorTimes = 0;
									int keyenceStart2 = this.GetKeyenceStart(false, keyenceSerialDeviceGroup.Start.Substring(keyenceSerialDeviceGroup.Start.IndexOf(keyenceSerialDeviceGroup.Start.First((char c) => char.IsDigit(c)))), keyenceSerialDeviceGroup.StoreArea);
									using (List<KeyenceSerialVariable>.Enumerator enumerator3 = keyenceSerialDeviceGroup.varList.GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											KeyenceSerialVariable keyenceSerialVariable2 = enumerator3.Current;
											int num4;
											int num5;
											if (this.VerifyKeyenceAddress(false, false, keyenceSerialDeviceGroup.StoreArea, keyenceSerialVariable2.Start, out num4, out num5))
											{
												num4 -= keyenceStart2;
												if (keyenceSerialVariable2.VarType == DataType.Bool)
												{
													keyenceSerialVariable2.Value = (ByteLib.GetByteFromByteArray(array, num4) == 1);
												}
												base.UpdateCurrentValue(keyenceSerialVariable2);
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
						KeyenceSerial keyenceSerial = this.keyenceSerial;
						if (keyenceSerial != null)
						{
							keyenceSerial.DisConnect();
						}
					}
					this.keyenceSerial = new KeyenceSerial();
					this.keyenceSerial.SleepTime = this.SleepTime;
					this.IsConnected = this.keyenceSerial.Connect(this.PortNum, this.Paud, int.Parse(this.DataBits), this.Parity, this.StopBits);
					this.FirstConnect = false;
				}
			}
		}
        
		public bool VerifyKeyenceAddress(bool isBoolStore, bool isMC, KeyenceStoreArea store, string address, out int start, out int offset)
		{
			bool result;
			if (isBoolStore)
			{
				offset = 0;
				start = 0;
				try
				{
					start = this.GetKeyenceStart(isMC, address, store);
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
						start = this.GetKeyenceStart(isMC, array[0], store);
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
				start = this.GetKeyenceStart(isMC, address, store);
				result = true;
			}
			return result;
		}
        
		public int GetKeyenceStart(bool isMC, string start, KeyenceStoreArea store)
		{
			int result;
			if (isMC)
			{
				switch (store)
				{
				case KeyenceStoreArea.RX存储区:
					result = KeyenceMcDataType.TranslateKeyenceMCRAddress(start);
					break;
				case KeyenceStoreArea.RY存储区:
					result = KeyenceMcDataType.TranslateKeyenceMCRAddress(start);
					break;
				case KeyenceStoreArea.B存储区:
					result = Convert.ToInt32(start, KeyenceMcDataType.B.FromBase);
					break;
				case KeyenceStoreArea.MR存储区:
					result = KeyenceMcDataType.TranslateKeyenceMCRAddress(start);
					break;
				case KeyenceStoreArea.LR存储区:
					result = KeyenceMcDataType.TranslateKeyenceMCRAddress(start);
					break;
				case KeyenceStoreArea.CR存储区:
					result = KeyenceMcDataType.TranslateKeyenceMCRAddress(start);
					break;
				case KeyenceStoreArea.DM存储区:
					result = Convert.ToInt32(start, KeyenceMcDataType.D.FromBase);
					break;
				case KeyenceStoreArea.W存储区:
					result = Convert.ToInt32(start, KeyenceMcDataType.W.FromBase);
					break;
				default:
					result = 0;
					break;
				}
			}
			else
			{
				switch (store)
				{
				case KeyenceStoreArea.RX存储区:
					result = KeyenceMcDataType.TranslateKeyenceMCRAddress(start);
					break;
				case KeyenceStoreArea.RY存储区:
					result = KeyenceMcDataType.TranslateKeyenceMCRAddress(start);
					break;
				case KeyenceStoreArea.B存储区:
					result = Convert.ToInt32(start, KeyenceMcDataType.B.FromBase);
					break;
				case KeyenceStoreArea.MR存储区:
					result = KeyenceMcDataType.TranslateKeyenceMCRAddress(start);
					break;
				case KeyenceStoreArea.LR存储区:
					result = KeyenceMcDataType.TranslateKeyenceMCRAddress(start);
					break;
				case KeyenceStoreArea.CR存储区:
					result = KeyenceMcDataType.TranslateKeyenceMCRAddress(start);
					break;
				case KeyenceStoreArea.DM存储区:
					result = Convert.ToInt32(start, KeyenceMcDataType.D.FromBase);
					break;
				case KeyenceStoreArea.W存储区:
					result = Convert.ToInt32(start, KeyenceMcDataType.W.FromBase);
					break;
				default:
					result = 0;
					break;
				}
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
				KeyenceSerialVariable keyenceSerialVariable = this.CurrentVarList[keyName] as KeyenceSerialVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(keyenceSerialVariable, keyenceSerialVariable.VarType, setValue);
				if (xktResult.IsSuccess)
				{
					result = this.keyenceSerial.Write(keyenceSerialVariable.VarAddress, xktResult.Content, keyenceSerialVariable.VarType);
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
        
		public KeyenceSerial keyenceSerial;
        
		public List<KeyenceSerialDeviceGroup> DeviceGroupList;
	}
}
