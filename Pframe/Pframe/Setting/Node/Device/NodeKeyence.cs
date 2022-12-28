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
using Pframe.PLC.Keyence;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Device
{
	public class NodeKeyence : DeviceNode, IXmlConvert
	{
		public NodeKeyence()
		{
			this.sw = new Stopwatch();
			this.DeviceGroupList = new List<KeyenceDeviceGroup>();
			base.Name = "基恩士PLC";
			base.Description = "制冷系统1#PLC";
			base.DeviceType = 20;
			this.IpAddress = "192.168.0.3";
			this.Port = 6000;
			this.PlcType = KeyenceProtocol.KeyenceMCBinary;
			this.FirstConnect = true;
		}
        
		public bool FirstConnect { get; set; }
        
		public long CommRate { get; set; }
        
		public string IpAddress { get; set; }
        
		public int Port { get; set; }
        
		public KeyenceProtocol PlcType { get; set; }
        
		public bool IsConnected { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.IpAddress = element.Attribute("IpAddress").Value;
			this.Port = int.Parse(element.Attribute("Port").Value);
			this.PlcType = (KeyenceProtocol)Enum.Parse(typeof(KeyenceProtocol), element.Attribute("PlcType").Value, true);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("IpAddress", this.IpAddress);
			xelement.SetAttributeValue("Port", this.Port);
			xelement.SetAttributeValue("PlcType", this.PlcType);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("IP地址", this.IpAddress));
			nodeClassRenders.Add(new NodeClassRenderItem("端口号", this.Port.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("协议类型", this.PlcType.ToString()));
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
			foreach (KeyenceDeviceGroup keyenceDeviceGroup in this.DeviceGroupList)
			{
				foreach (KeyenceVariable keyenceVariable in keyenceDeviceGroup.varList)
				{
					if (keyenceVariable.Config.ArchiveEnable)
					{
						this.StoreVarList.Add(keyenceVariable);
					}
					if (this.CurrentVarList.ContainsKey(keyenceVariable.KeyName))
					{
						this.CurrentVarList[keyenceVariable.KeyName] = keyenceVariable;
					}
					else
					{
						this.CurrentVarList.Add(keyenceVariable.KeyName, keyenceVariable);
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
					foreach (KeyenceDeviceGroup keyenceDeviceGroup in this.DeviceGroupList)
					{
						if (keyenceDeviceGroup.IsActive)
						{
							bool isMC = this.PlcType != KeyenceProtocol.KeyenceLink;
							KeyenceStoreArea storeArea = keyenceDeviceGroup.StoreArea;
							KeyenceStoreArea keyenceStoreArea = storeArea;
							if (keyenceStoreArea > KeyenceStoreArea.CR存储区)
							{
								if (keyenceStoreArea - KeyenceStoreArea.DM存储区 <= 1)
								{
									byte[] array = this.keyence.ReadBytes(keyenceDeviceGroup.Start, Convert.ToUInt16(keyenceDeviceGroup.Length));
									if (array != null && array.Length == keyenceDeviceGroup.Length * 2)
									{
										base.ErrorTimes = 0;
										int keyenceStart = this.GetKeyenceStart(isMC, keyenceDeviceGroup.Start.Substring(keyenceDeviceGroup.Start.IndexOf(keyenceDeviceGroup.Start.First((char c) => char.IsDigit(c)))), keyenceDeviceGroup.StoreArea);
										using (List<KeyenceVariable>.Enumerator enumerator2 = keyenceDeviceGroup.varList.GetEnumerator())
										{
											while (enumerator2.MoveNext())
											{
												KeyenceVariable keyenceVariable = enumerator2.Current;
												int num;
												int num2;
												if (this.VerifyKeyenceAddress(false, isMC, keyenceDeviceGroup.StoreArea, keyenceVariable.Start, out num, out num2))
												{
													num -= keyenceStart;
													num *= 2;
													switch (keyenceVariable.VarType)
													{
													case DataType.Bool:
														keyenceVariable.Value = BitLib.GetBitFrom2ByteArray(array, num, num2, false);
														break;
													case DataType.Short:
														keyenceVariable.Value = ShortLib.GetShortFromByteArray(array, num, this.keyence.DataFormat);
														break;
													case DataType.UShort:
														keyenceVariable.Value = UShortLib.GetUShortFromByteArray(array, num, this.keyence.DataFormat);
														break;
													case DataType.Int:
														keyenceVariable.Value = IntLib.GetIntFromByteArray(array, num, this.keyence.DataFormat);
														break;
													case DataType.UInt:
														keyenceVariable.Value = UIntLib.GetUIntFromByteArray(array, num, this.keyence.DataFormat);
														break;
													case DataType.Float:
														keyenceVariable.Value = FloatLib.GetFloatFromByteArray(array, num, this.keyence.DataFormat);
														break;
													case DataType.Double:
														keyenceVariable.Value = DoubleLib.GetDoubleFromByteArray(array, num, this.keyence.DataFormat);
														break;
													case DataType.String:
														keyenceVariable.Value = StringLib.GetStringFromByteArray(array, num, num2 * 2, Encoding.ASCII);
														break;
													case DataType.ByteArray:
														keyenceVariable.Value = ByteArrayLib.GetByteArray(array, num, num2 * 2);
														break;
													case DataType.HexString:
														keyenceVariable.Value = StringLib.GetHexStringFromByteArray(array, num, num2 * 2, ' ');
														break;
													}
													keyenceVariable.Value = MigrationLib.GetMigrationValue(keyenceVariable.Value, keyenceVariable.Scale, keyenceVariable.Offset);
													base.UpdateCurrentValue(keyenceVariable);
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
								byte[] array = this.keyence.ReadBytes(keyenceDeviceGroup.Start, Convert.ToUInt16(keyenceDeviceGroup.Length));
								if (array != null && array.Length >= keyenceDeviceGroup.Length)
								{
									base.ErrorTimes = 0;
									int keyenceStart2 = this.GetKeyenceStart(isMC, keyenceDeviceGroup.Start.Substring(keyenceDeviceGroup.Start.IndexOf(keyenceDeviceGroup.Start.First((char c) => char.IsDigit(c)))), keyenceDeviceGroup.StoreArea);
									using (List<KeyenceVariable>.Enumerator enumerator3 = keyenceDeviceGroup.varList.GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											KeyenceVariable keyenceVariable2 = enumerator3.Current;
											int num3;
											int num4;
											if (this.VerifyKeyenceAddress(false, isMC, keyenceDeviceGroup.StoreArea, keyenceVariable2.Start, out num3, out num4))
											{
												num3 -= keyenceStart2;
												if (keyenceVariable2.VarType == DataType.Bool)
												{
													keyenceVariable2.Value = (ByteLib.GetByteFromByteArray(array, num3) == 1);
												}
												base.UpdateCurrentValue(keyenceVariable2);
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
						Keyence keyence = this.keyence;
						if (keyence != null)
						{
							keyence.DisConnect();
						}
					}
					switch (this.PlcType)
					{
					case KeyenceProtocol.KeyenceMCBinary:
						this.keyence = new Keyence(KeyenceProtocolType.KeyenceMCBinary, DataFormat.DCBA);
						this.keyence.ConnectTimeOut = base.ConnectTimeOut;
						this.IsConnected = this.keyence.Connect(this.IpAddress, this.Port);
						break;
					case KeyenceProtocol.KeyenceMCASCII:
						this.keyence = new Keyence(KeyenceProtocolType.KeyenceMCASCII, DataFormat.DCBA);
						this.keyence.ConnectTimeOut = base.ConnectTimeOut;
						this.IsConnected = this.keyence.Connect(this.IpAddress, this.Port);
						break;
					case KeyenceProtocol.KeyenceLink:
						this.keyence = new Keyence(KeyenceProtocolType.KeyenceLink, DataFormat.DCBA);
						this.keyence.ConnectTimeOut = base.ConnectTimeOut;
						this.IsConnected = this.keyence.Connect(this.IpAddress, this.Port);
						break;
					}
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
				KeyenceVariable keyenceVariable = this.CurrentVarList[keyName] as KeyenceVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(keyenceVariable, keyenceVariable.VarType, setValue);
				if (xktResult.IsSuccess)
				{
					result = this.keyence.Write(keyenceVariable.VarAddress, xktResult.Content, keyenceVariable.VarType);
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
        
		public List<KeyenceDeviceGroup> DeviceGroupList;
        
		public Keyence keyence;
	}
}
