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
using Pframe.PLC.Panasonic;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Device
{
	public class NodeMewtocol : DeviceNode, IXmlConvert
	{
		public NodeMewtocol()
		{
			this.sw = new Stopwatch();
			this.DeviceGroupList = new List<MewtocolDeviceGroup>();
			base.Name = "松下PLC Mewtocol";
			base.Description = "举升系统1#PLC";
			base.DeviceType = 70;
			this.PortNum = "COM3";
			this.Paud = 9600;
			this.Parity = Parity.Odd;
			this.DataBits = "8";
			this.StopBits = StopBits.One;
			this.PLCStation = 1;
			this.FirstConnect = true;
			this.SleepTime = 20;
		}
        
		public bool FirstConnect { get; set; }
        
		public bool ConnectState { get; set; }
        /// <summary>
        /// 通信周期
        /// </summary>
		public long CommRate { get; set; }
        
		public string PortNum { get; set; }
        
		public int Paud { get; set; }
        
		public Parity Parity { get; set; }
        
		public int SleepTime { get; set; }
        
		public string DataBits { get; set; }
        
		public StopBits StopBits { get; set; }
        /// <summary>
        /// PLC站地址
        /// </summary>
		public int PLCStation { get; set; }
        
		public bool IsConnected { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.PortNum = element.Attribute("PortNum").Value;
			this.Paud = int.Parse(element.Attribute("Paud").Value);
			this.DataBits = element.Attribute("DataBits").Value;
			this.Parity = (Parity)Enum.Parse(typeof(Parity), element.Attribute("Parity").Value, true);
			this.StopBits = (StopBits)Enum.Parse(typeof(StopBits), element.Attribute("StopBits").Value, true);
			this.PLCStation = int.Parse(element.Attribute("PLCStation").Value);
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
			xelement.SetAttributeValue("PLCStation", this.PLCStation);
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
			nodeClassRenders.Add(new NodeClassRenderItem("PLC站号", this.PLCStation.ToString()));
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
			foreach (MewtocolDeviceGroup mewtocolDeviceGroup in this.DeviceGroupList)
			{
				foreach (MewtocolVariable mewtocolVariable in mewtocolDeviceGroup.varList)
				{
					if (mewtocolVariable.Config.ArchiveEnable)
					{
						this.StoreVarList.Add(mewtocolVariable);
					}
					if (this.CurrentVarList.ContainsKey(mewtocolVariable.KeyName))
					{
						this.CurrentVarList[mewtocolVariable.KeyName] = mewtocolVariable;
					}
					else
					{
						this.CurrentVarList.Add(mewtocolVariable.KeyName, mewtocolVariable);
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
					foreach (MewtocolDeviceGroup mewtocolDeviceGroup in this.DeviceGroupList)
					{
						if (mewtocolDeviceGroup.IsActive)
						{
							switch (mewtocolDeviceGroup.StoreArea)
							{
							case PanasonicStoreArea.X存储区:
							case PanasonicStoreArea.Y存储区:
							case PanasonicStoreArea.R存储区:
							{
								bool[] array = this.mewtocol.ReadBool(mewtocolDeviceGroup.Start, Convert.ToUInt16(mewtocolDeviceGroup.Length));
								this.ConnectState = (array != null);
								if (array != null)
								{
									base.ErrorTimes = 0;
									int mewtocolStartByte = this.GetMewtocolStartByte(mewtocolDeviceGroup.Start.Substring(mewtocolDeviceGroup.Start.IndexOf(mewtocolDeviceGroup.Start.First((char c) => char.IsDigit(c)))), mewtocolDeviceGroup.StoreArea);
									using (List<MewtocolVariable>.Enumerator enumerator2 = mewtocolDeviceGroup.varList.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											MewtocolVariable mewtocolVariable = enumerator2.Current;
											int num;
											int num2;
											if (this.VerifyMewtocolAddress(true, mewtocolDeviceGroup.StoreArea, mewtocolVariable.Start, out num, out num2))
											{
												num -= mewtocolStartByte;
												if (mewtocolVariable.VarType == DataType.Bool)
												{
													mewtocolVariable.Value = array[num];
												}
												mewtocolVariable.Value = MigrationLib.GetMigrationValue(mewtocolVariable.Value, mewtocolVariable.Scale, mewtocolVariable.Offset);
												base.UpdateCurrentValue(mewtocolVariable);
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
							case PanasonicStoreArea.D存储区:
							case PanasonicStoreArea.T存储区:
							case PanasonicStoreArea.C存储区:
							{
								byte[] array2 = this.mewtocol.ReadBytes(mewtocolDeviceGroup.Start, Convert.ToUInt16(mewtocolDeviceGroup.Length));
								this.ConnectState = (array2 != null);
								if (array2 != null)
								{
									base.ErrorTimes = 0;
									int mewtocolStartByte2 = this.GetMewtocolStartByte(mewtocolDeviceGroup.Start.Substring(mewtocolDeviceGroup.Start.IndexOf(mewtocolDeviceGroup.Start.First((char c) => char.IsDigit(c)))), mewtocolDeviceGroup.StoreArea);
									using (List<MewtocolVariable>.Enumerator enumerator3 = mewtocolDeviceGroup.varList.GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											MewtocolVariable mewtocolVariable2 = enumerator3.Current;
											int num3;
											int num4;
											if (this.VerifyMewtocolAddress(false, mewtocolDeviceGroup.StoreArea, mewtocolVariable2.Start, out num3, out num4))
											{
												num3 -= mewtocolStartByte2;
												num3 *= 2;
												switch (mewtocolVariable2.VarType)
												{
												case DataType.Bool:
													mewtocolVariable2.Value = BitLib.GetBitFrom2ByteArray(array2, num3, num4, true);
													break;
												case DataType.Short:
													mewtocolVariable2.Value = ShortLib.GetShortFromByteArray(array2, num3, this.mewtocol.DataFormat);
													break;
												case DataType.UShort:
													mewtocolVariable2.Value = UShortLib.GetUShortFromByteArray(array2, num3, this.mewtocol.DataFormat);
													break;
												case DataType.Int:
													mewtocolVariable2.Value = IntLib.GetIntFromByteArray(array2, num3, this.mewtocol.DataFormat);
													break;
												case DataType.UInt:
													mewtocolVariable2.Value = UIntLib.GetUIntFromByteArray(array2, num3, this.mewtocol.DataFormat);
													break;
												case DataType.Float:
													mewtocolVariable2.Value = FloatLib.GetFloatFromByteArray(array2, num3, this.mewtocol.DataFormat);
													break;
												case DataType.Double:
													mewtocolVariable2.Value = DoubleLib.GetDoubleFromByteArray(array2, num3, this.mewtocol.DataFormat);
													break;
												case DataType.String:
													mewtocolVariable2.Value = StringLib.GetStringFromByteArray(array2, num3, num4 * 2, Encoding.ASCII);
													break;
												case DataType.ByteArray:
													mewtocolVariable2.Value = ByteArrayLib.GetByteArray(array2, num3, num4 * 2);
													break;
												case DataType.HexString:
													mewtocolVariable2.Value = StringLib.GetHexStringFromByteArray(array2, num3, num4 * 2, ' ');
													break;
												}
												mewtocolVariable2.Value = MigrationLib.GetMigrationValue(mewtocolVariable2.Value, mewtocolVariable2.Scale, mewtocolVariable2.Offset);
												base.UpdateCurrentValue(mewtocolVariable2);
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
						Mewtocol mewtocol = this.mewtocol;
						if (mewtocol != null)
						{
							mewtocol.DisConnect();
						}
					}
					this.mewtocol = new Mewtocol(DataFormat.DCBA, 238);
					this.mewtocol.SleepTime = this.SleepTime;
					this.mewtocol.Station = (byte)this.PLCStation;
					this.IsConnected = this.mewtocol.Connect(this.PortNum, this.Paud, int.Parse(this.DataBits), this.Parity, this.StopBits);
					this.FirstConnect = false;
				}
			}
		}
        
		public bool VerifyMewtocolAddress(bool isBoolStore, PanasonicStoreArea store, string address, out int start, out int offset)
		{
			bool result;
			if (isBoolStore)
			{
				offset = 0;
				start = 0;
				try
				{
					start = this.GetMewtocolStartByte(address, store);
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
						start = this.GetMewtocolStartByte(array[0], store);
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
        
		public int GetMewtocolStartByte(string start, PanasonicStoreArea store)
		{
			int result;
			switch (store)
			{
			case PanasonicStoreArea.X存储区:
			case PanasonicStoreArea.Y存储区:
			case PanasonicStoreArea.R存储区:
				result = Convert.ToInt32(start, 16);
				break;
			case PanasonicStoreArea.D存储区:
			case PanasonicStoreArea.T存储区:
			case PanasonicStoreArea.C存储区:
				result = Convert.ToInt32(start, 10);
				break;
			default:
				result = 0;
				break;
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
				MewtocolVariable mewtocolVariable = this.CurrentVarList[keyName] as MewtocolVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(mewtocolVariable, mewtocolVariable.VarType, setValue);
				if (xktResult.IsSuccess)
				{
					result = this.mewtocol.Write(mewtocolVariable.VarAddress, xktResult.Content, mewtocolVariable.VarType);
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
        
		public Mewtocol mewtocol;
        
		public List<MewtocolDeviceGroup> DeviceGroupList;
	}
}
