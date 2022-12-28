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
using Pframe.PLC.Melsec;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Device
{
	public class NodeFXLink : DeviceNode, IXmlConvert
	{
        /// <summary>
        /// FX3U 485扩展模块
        /// </summary>
        public NodeFXLink()
		{
			this.sw = new Stopwatch();
			this.DeviceGroupList = new List<FXLinkDeviceGroup>();
			base.Name = "三菱FXPLC";
			base.Description = "称重系统1#PLC";
			base.DeviceType = 80;
			this.PortNum = "COM3";
			this.Paud = 9600;
			this.Parity = Parity.Even;
			this.DataBits = "7";
			this.StopBits = StopBits.One;
			this.StationNo = 0;
			this.TimeOut = 0;
			this.SumCheckEnable = true;
			this.FirstConnect = true;
			this.SleepTime = 20;
		}
        
		public bool FirstConnect { get; set; }
        
		public bool ConnectState { get; set; }
        
		public long CommRate { get; set; }
        
		public int SleepTime { get; set; }
        
		public string PortNum { get; set; }
        
		public int Paud { get; set; }
        
		public Parity Parity { get; set; }
        
		public string DataBits { get; set; }
        
		public StopBits StopBits { get; set; }
        
		public bool IsConnected { get; set; }
        
		public int StationNo { get; set; }
        
		public int TimeOut { get; set; }
        
		public bool SumCheckEnable { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.PortNum = element.Attribute("PortNum").Value;
			this.Paud = int.Parse(element.Attribute("Paud").Value);
			this.DataBits = element.Attribute("DataBits").Value;
			this.Parity = (Parity)Enum.Parse(typeof(Parity), element.Attribute("Parity").Value, true);
			this.StopBits = (StopBits)Enum.Parse(typeof(StopBits), element.Attribute("StopBits").Value, true);
			this.StationNo = int.Parse(element.Attribute("StationNo").Value);
			this.TimeOut = int.Parse(element.Attribute("TimeOut").Value);
			this.SleepTime = int.Parse(element.Attribute("SleepTime").Value);
			this.SumCheckEnable = bool.Parse(element.Attribute("SumCheckEnable").Value);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("PortNum", this.PortNum);
			xelement.SetAttributeValue("Paud", this.Paud);
			xelement.SetAttributeValue("DataBits", this.DataBits);
			xelement.SetAttributeValue("Parity", this.Parity);
			xelement.SetAttributeValue("StopBits", this.StopBits);
			xelement.SetAttributeValue("StationNo", this.StationNo);
			xelement.SetAttributeValue("TimeOut", this.TimeOut);
			xelement.SetAttributeValue("SleepTime", this.SleepTime);
			xelement.SetAttributeValue("SumCheckEnable", this.SumCheckEnable);
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
			nodeClassRenders.Add(new NodeClassRenderItem("站号", this.StationNo.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("超时时间", this.TimeOut.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("校验和", this.SumCheckEnable ? "启用" : "禁用"));
			nodeClassRenders.Add(new NodeClassRenderItem("延迟时间", this.SleepTime.ToString()));
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
			foreach (FXLinkDeviceGroup fxlinkDeviceGroup in this.DeviceGroupList)
			{
				foreach (FXLinkVariable fxlinkVariable in fxlinkDeviceGroup.varList)
				{
					if (fxlinkVariable.Config.ArchiveEnable)
					{
						this.StoreVarList.Add(fxlinkVariable);
					}
					if (this.CurrentVarList.ContainsKey(fxlinkVariable.KeyName))
					{
						this.CurrentVarList[fxlinkVariable.KeyName] = fxlinkVariable;
					}
					else
					{
						this.CurrentVarList.Add(fxlinkVariable.KeyName, fxlinkVariable);
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
					foreach (FXLinkDeviceGroup fxlinkDeviceGroup in this.DeviceGroupList)
					{
						if (fxlinkDeviceGroup.IsActive)
						{
							switch (fxlinkDeviceGroup.StoreArea)
							{
							case MelsecStoreArea.M存储区:
							case MelsecStoreArea.X存储区:
							case MelsecStoreArea.Y存储区:
							case MelsecStoreArea.L存储区:
							case MelsecStoreArea.S存储区:
							case MelsecStoreArea.B存储区:
							{
								bool[] array = this.fxlink.ReadBool(fxlinkDeviceGroup.Start, Convert.ToUInt16(fxlinkDeviceGroup.Length));
								this.ConnectState = (array != null);
								if (array != null)
								{
									base.ErrorTimes = 0;
									int fxserialStart = this.GetFXSerialStart(fxlinkDeviceGroup.Start.Substring(fxlinkDeviceGroup.Start.IndexOf(fxlinkDeviceGroup.Start.First((char c) => char.IsDigit(c)))), fxlinkDeviceGroup.StoreArea);
									using (List<FXLinkVariable>.Enumerator enumerator2 = fxlinkDeviceGroup.varList.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											FXLinkVariable fxlinkVariable = enumerator2.Current;
											int num;
											int num2;
											if (this.VerifyFXAddress(true, fxlinkDeviceGroup.StoreArea, fxlinkVariable.Start, out num, out num2))
											{
												if (fxlinkVariable.VarType == DataType.Bool)
												{
													num -= fxserialStart;
													fxlinkVariable.Value = array[num];
												}
												base.UpdateCurrentValue(fxlinkVariable);
											}
										}
										break;
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
								break;
							}
							case MelsecStoreArea.D存储区:
							case MelsecStoreArea.W存储区:
							{
								byte[] array2 = this.fxlink.ReadBytes(fxlinkDeviceGroup.Start, Convert.ToUInt16(fxlinkDeviceGroup.Length));
								this.ConnectState = (array2 != null);
								if (array2 != null)
								{
									base.ErrorTimes = 0;
									int fxserialStart2 = this.GetFXSerialStart(fxlinkDeviceGroup.Start.Substring(fxlinkDeviceGroup.Start.IndexOf(fxlinkDeviceGroup.Start.First((char c) => char.IsDigit(c)))), fxlinkDeviceGroup.StoreArea);
									using (List<FXLinkVariable>.Enumerator enumerator3 = fxlinkDeviceGroup.varList.GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											FXLinkVariable fxlinkVariable2 = enumerator3.Current;
											int num3;
											int num4;
											if (this.VerifyFXAddress(false, fxlinkDeviceGroup.StoreArea, fxlinkVariable2.Start, out num3, out num4))
											{
												num3 -= fxserialStart2;
												num3 *= 2;
												switch (fxlinkVariable2.VarType)
												{
												case DataType.Bool:
													fxlinkVariable2.Value = BitLib.GetBitFrom2ByteArray(array2, num3, num4, false);
													break;
												case DataType.Short:
													fxlinkVariable2.Value = ShortLib.GetShortFromByteArray(array2, num3, this.fxlink.DataFormat);
													break;
												case DataType.UShort:
													fxlinkVariable2.Value = UShortLib.GetUShortFromByteArray(array2, num3, this.fxlink.DataFormat);
													break;
												case DataType.Int:
													fxlinkVariable2.Value = IntLib.GetIntFromByteArray(array2, num3, this.fxlink.DataFormat);
													break;
												case DataType.UInt:
													fxlinkVariable2.Value = UIntLib.GetUIntFromByteArray(array2, num3, this.fxlink.DataFormat);
													break;
												case DataType.Float:
													fxlinkVariable2.Value = FloatLib.GetFloatFromByteArray(array2, num3, this.fxlink.DataFormat);
													break;
												case DataType.Double:
													fxlinkVariable2.Value = DoubleLib.GetDoubleFromByteArray(array2, num3, this.fxlink.DataFormat);
													break;
												case DataType.String:
													fxlinkVariable2.Value = StringLib.GetStringFromByteArray(array2, num3, num4 * 2, Encoding.ASCII);
													break;
												case DataType.ByteArray:
													fxlinkVariable2.Value = ByteArrayLib.GetByteArray(array2, num3, num4 * 2);
													break;
												case DataType.HexString:
													fxlinkVariable2.Value = StringLib.GetHexStringFromByteArray(array2, num3, num4 * 2, ' ');
													break;
												}
												fxlinkVariable2.Value = MigrationLib.GetMigrationValue(fxlinkVariable2.Value, fxlinkVariable2.Scale, fxlinkVariable2.Offset);
												base.UpdateCurrentValue(fxlinkVariable2);
											}
										}
										break;
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
								break;
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
						MelsecFXLink melsecFXLink = this.fxlink;
						if (melsecFXLink != null)
						{
							melsecFXLink.DisConnect();
						}
					}
					this.fxlink = new MelsecFXLink(DataFormat.DCBA);
					this.SleepTime = this.SleepTime;
					this.fxlink.Station = (byte)this.StationNo;
					this.fxlink.WaittingTime = (byte)this.TimeOut;
					this.fxlink.SumCheck = this.SumCheckEnable;
					this.IsConnected = this.fxlink.Connect(this.PortNum, this.Paud, int.Parse(this.DataBits), this.Parity, this.StopBits);
					this.FirstConnect = false;
				}
			}
		}
        
		public int GetFXSerialStart(string start, MelsecStoreArea store)
		{
			int result;
			switch (store)
			{
			case MelsecStoreArea.M存储区:
				result = Convert.ToInt32(start, MelsecA1EDataType.M.FromBase);
				break;
			case MelsecStoreArea.X存储区:
				result = Convert.ToInt32(start, MelsecA1EDataType.X.FromBase);
				break;
			case MelsecStoreArea.Y存储区:
				result = Convert.ToInt32(start, MelsecA1EDataType.Y.FromBase);
				break;
			case MelsecStoreArea.D存储区:
				result = Convert.ToInt32(start, MelsecA1EDataType.D.FromBase);
				break;
			case MelsecStoreArea.L存储区:
			case MelsecStoreArea.B存储区:
			case MelsecStoreArea.W存储区:
				result = 0;
				break;
			case MelsecStoreArea.S存储区:
				result = Convert.ToInt32(start, MelsecA1EDataType.S.FromBase);
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}
        
		public bool VerifyFXAddress(bool isBoolStore, MelsecStoreArea store, string address, out int start, out int offset)
		{
			bool result;
			if (isBoolStore)
			{
				offset = 0;
				start = 0;
				try
				{
					start = this.GetFXSerialStart(address, store);
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
						start = this.GetFXSerialStart(array[0], store);
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
				FXLinkVariable fxlinkVariable = this.CurrentVarList[keyName] as FXLinkVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(fxlinkVariable, fxlinkVariable.VarType, setValue);
				if (xktResult.IsSuccess)
				{
					result = this.fxlink.Write(fxlinkVariable.VarAddress, xktResult.Content, fxlinkVariable.VarType);
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
        
		public MelsecFXLink fxlink;
        
		public List<FXLinkDeviceGroup> DeviceGroupList;
	}
}
