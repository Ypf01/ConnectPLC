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
using Pframe.PLC.Siemens;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Device
{
	public class NodeSiemens : DeviceNode, IXmlConvert
	{
		public NodeSiemens()
		{
			this.DeviceGroupList = new List<SiemensDeviceGroup>();
			this.AlarmVarList = new List<SiemensVariable>();
			this.siemens = new SiemensS7();
			this.sw = new Stopwatch();
			base.Name = "西门子PLC";
			base.Description = "空压系统1#PLC";
			base.DeviceType = 30;
			this.IpAddress = "192.168.0.1";
			this.Port = 102;
			this.Rack = 0;
			this.Slot = 0;
			this.IsUseMultiRead = false;
			this.PlcType = SiemensPLCType.S71200;
			this.FirstConnect = true;
		}
        
		public bool FirstConnect { get; set; }
        
		public bool IsConnected { get; set; }
        
		public long CommRate { get; set; }
        
		public bool IsUseMultiRead { get; set; }
        
		public string IpAddress { get; set; }
        
		public int Port { get; set; }
        
		public int Rack { get; set; }
        
		public int Slot { get; set; }
        
		public SiemensPLCType PlcType { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.IpAddress = element.Attribute("IpAddress").Value;
			this.Port = int.Parse(element.Attribute("Port").Value);
			this.PlcType = (SiemensPLCType)Enum.Parse(typeof(SiemensPLCType), element.Attribute("PlcType").Value, true);
			this.Rack = int.Parse(element.Attribute("Rack").Value);
			this.Slot = int.Parse(element.Attribute("Slot").Value);
			this.IsUseMultiRead = bool.Parse(element.Attribute("IsUseMultiRead").Value);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("IpAddress", this.IpAddress);
			xelement.SetAttributeValue("Port", this.Port);
			xelement.SetAttributeValue("PlcType", this.PlcType);
			xelement.SetAttributeValue("Rack", this.Rack);
			xelement.SetAttributeValue("Slot", this.Slot);
			xelement.SetAttributeValue("IsUseMultiRead", this.IsUseMultiRead);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("IP地址", this.IpAddress));
			nodeClassRenders.Add(new NodeClassRenderItem("端口号", this.Port.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("PLC类型", this.PlcType.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("机架号", this.Rack.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("插槽号", this.Slot.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("读取方式", this.IsUseMultiRead ? "多组读取" : "单组读取"));
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
			foreach (SiemensDeviceGroup siemensDeviceGroup in this.DeviceGroupList)
			{
				foreach (SiemensVariable siemensVariable in siemensDeviceGroup.varList)
				{
					if (siemensVariable.Config.ArchiveEnable)
					{
						this.StoreVarList.Add(siemensVariable);
					}
					if (this.CurrentVarList.ContainsKey(siemensVariable.KeyName))
					{
						this.CurrentVarList[siemensVariable.KeyName] = siemensVariable;
					}
					else
					{
						this.CurrentVarList.Add(siemensVariable.KeyName, siemensVariable);
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
					int errorTimes;
					if (!this.IsUseMultiRead)
					{
						using (List<SiemensDeviceGroup>.Enumerator enumerator = this.DeviceGroupList.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								SiemensDeviceGroup siemensDeviceGroup = enumerator.Current;
								if (siemensDeviceGroup.IsActive)
								{
									byte[] array = this.siemens.ReadBytes(this.GetStoreTypeByStoreArea(siemensDeviceGroup.StoreArea), siemensDeviceGroup.DBNo, siemensDeviceGroup.Start, siemensDeviceGroup.Length);
									if (array != null && array.Length == siemensDeviceGroup.Length)
									{
										base.ErrorTimes = 0;
										using (List<SiemensVariable>.Enumerator enumerator2 = siemensDeviceGroup.varList.GetEnumerator())
										{
											while (enumerator2.MoveNext())
											{
												SiemensVariable siemensVariable = enumerator2.Current;
												int num;
												int num2;
												if (this.VerifySiemensAddress(siemensVariable.Start, out num, out num2))
												{
													num -= siemensDeviceGroup.Start;
													switch (siemensVariable.VarType)
													{
													case DataType.Bool:
														siemensVariable.Value = BitLib.GetBitFromByteArray(array, num, num2);
														break;
													case DataType.Byte:
														siemensVariable.Value = ByteLib.GetByteFromByteArray(array, num);
														break;
													case DataType.Short:
														siemensVariable.Value = ShortLib.GetShortFromByteArray(array, num, DataFormat.ABCD);
														break;
													case DataType.UShort:
														siemensVariable.Value = UShortLib.GetUShortFromByteArray(array, num, DataFormat.ABCD);
														break;
													case DataType.Int:
														siemensVariable.Value = IntLib.GetIntFromByteArray(array, num, DataFormat.ABCD);
														break;
													case DataType.UInt:
														siemensVariable.Value = UIntLib.GetUIntFromByteArray(array, num, DataFormat.ABCD);
														break;
													case DataType.Float:
														siemensVariable.Value = FloatLib.GetFloatFromByteArray(array, num, DataFormat.ABCD);
														break;
													case DataType.Double:
														siemensVariable.Value = DoubleLib.GetDoubleFromByteArray(array, num, DataFormat.ABCD);
														break;
													case DataType.Long:
														siemensVariable.Value = LongLib.GetLongFromByteArray(array, num, DataFormat.ABCD);
														break;
													case DataType.ULong:
														siemensVariable.Value = ULongLib.GetULongFromByteArray(array, num, DataFormat.ABCD);
														break;
													case DataType.String:
														if (siemensVariable.Start.Contains('.'))
														{
															siemensVariable.Value = StringLib.GetSiemensStringFromByteArray(array, num, num2);
														}
														else
														{
															siemensVariable.Value = StringLib.GetStringFromByteArray(array, num, num2, Encoding.GetEncoding("GBK"));
														}
														break;
													case DataType.ByteArray:
														if (siemensVariable.Start.Contains('.'))
														{
															siemensVariable.Value = ByteArrayLib.GetByteArray(array, num, num2);
														}
														break;
													case DataType.HexString:
														if (siemensVariable.Start.Contains('.'))
														{
															siemensVariable.Value = StringLib.GetHexStringFromByteArray(array, num, num2, ' ');
														}
														break;
													}
													siemensVariable.Value = MigrationLib.GetMigrationValue(siemensVariable.Value, siemensVariable.Scale, siemensVariable.Offset);
													base.UpdateCurrentValue(siemensVariable);
												}
											}
											continue;
										}
									}
									errorTimes = base.ErrorTimes;
									base.ErrorTimes = errorTimes + 1;
									if (base.ErrorTimes >= base.MaxErrorTimes)
									{
										this.IsConnected = false;
									}
								}
							}
							goto IL_699;
						}
						//goto IL_347;
					}
					goto IL_347;
					IL_699:
					this.CommRate = this.sw.ElapsedMilliseconds;
					continue;
					IL_347:
					if (this.GetSiemensGroupValue())
					{
						using (List<SiemensDeviceGroup>.Enumerator enumerator3 = this.DeviceGroupList.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								SiemensDeviceGroup siemensDeviceGroup2 = enumerator3.Current;
								byte[] value = siemensDeviceGroup2.Value;
								if (value != null && value.Length == siemensDeviceGroup2.Length)
								{
									base.ErrorTimes = 0;
									using (List<SiemensVariable>.Enumerator enumerator4 = siemensDeviceGroup2.varList.GetEnumerator())
									{
										while (enumerator4.MoveNext())
										{
											SiemensVariable siemensVariable2 = enumerator4.Current;
											int num3;
											int num4;
											if (this.VerifySiemensAddress(siemensVariable2.Start, out num3, out num4))
											{
												num3 -= siemensDeviceGroup2.Start;
												switch (siemensVariable2.VarType)
												{
												case DataType.Bool:
													siemensVariable2.Value = BitLib.GetBitFromByteArray(value, num3, num4);
													break;
												case DataType.Byte:
													siemensVariable2.Value = ByteLib.GetByteFromByteArray(value, num3);
													break;
												case DataType.Short:
													siemensVariable2.Value = ShortLib.GetShortFromByteArray(value, num3, DataFormat.ABCD);
													break;
												case DataType.UShort:
													siemensVariable2.Value = UShortLib.GetUShortFromByteArray(value, num3, DataFormat.ABCD);
													break;
												case DataType.Int:
													siemensVariable2.Value = IntLib.GetIntFromByteArray(value, num3, DataFormat.ABCD);
													break;
												case DataType.UInt:
													siemensVariable2.Value = UIntLib.GetUIntFromByteArray(value, num3, DataFormat.ABCD);
													break;
												case DataType.Float:
													siemensVariable2.Value = FloatLib.GetFloatFromByteArray(value, num3, DataFormat.ABCD);
													break;
												case DataType.Double:
													siemensVariable2.Value = DoubleLib.GetDoubleFromByteArray(value, num3, DataFormat.ABCD);
													break;
												case DataType.Long:
													siemensVariable2.Value = LongLib.GetLongFromByteArray(value, num3, DataFormat.ABCD);
													break;
												case DataType.ULong:
													siemensVariable2.Value = ULongLib.GetULongFromByteArray(value, num3, DataFormat.ABCD);
													break;
												case DataType.String:
													if (siemensVariable2.Start.Contains('.'))
													{
														siemensVariable2.Value = StringLib.GetSiemensStringFromByteArray(value, num3, num4);
													}
													else
													{
														siemensVariable2.Value = StringLib.GetStringFromByteArray(value, num3, num4, Encoding.GetEncoding("GBK"));
													}
													break;
												}
												siemensVariable2.Value = MigrationLib.GetMigrationValue(siemensVariable2.Value, siemensVariable2.Scale, siemensVariable2.Offset);
												base.UpdateCurrentValue(siemensVariable2);
											}
										}
										continue;
									}
								}
								errorTimes = base.ErrorTimes;
								base.ErrorTimes = errorTimes + 1;
								if (base.ErrorTimes >= base.MaxErrorTimes)
								{
									this.IsConnected = false;
								}
							}
							goto IL_699;
						}
					}
					errorTimes = base.ErrorTimes;
					base.ErrorTimes = errorTimes + 1;
					if (base.ErrorTimes >= base.MaxErrorTimes)
					{
						this.IsConnected = false;
						goto IL_699;
					}
					goto IL_699;
				}
				else
				{
					if (!this.FirstConnect)
					{
						Thread.Sleep(base.ReConnectTime);
						this.siemens.DisConnect();
					}
					this.siemens.ConnectTimeOut = this.siemens.ConnectTimeOut;
					this.IsConnected = this.siemens.Connect(this.IpAddress, this.GetCPUTypeFromPLCType(this.PlcType), this.Rack, this.Slot, this.Port);
					this.FirstConnect = false;
				}
			}
		}
        
		public bool GetSiemensGroupValue()
		{
			int num = (int)(this.siemens.MaxPDUSize - 18);
			this.DeviceGroupList = (from c in this.DeviceGroupList
			where c.IsActive
			select c).ToList<SiemensDeviceGroup>();
			this.DeviceGroupList = (from c in this.DeviceGroupList
			orderby c.Length
			select c).ToList<SiemensDeviceGroup>();
			List<List<SiemensGroup>> list = new List<List<SiemensGroup>>();
			List<SiemensGroup> list2 = new List<SiemensGroup>();
			foreach (SiemensDeviceGroup siemensDeviceGroup in this.DeviceGroupList)
			{
				if (siemensDeviceGroup.Length > num)
				{
					list.Add(new List<SiemensGroup>
					{
						new SiemensGroup(siemensDeviceGroup.Name, this.GetStoreTypeByStoreArea(siemensDeviceGroup.StoreArea), siemensDeviceGroup.DBNo, siemensDeviceGroup.Start, siemensDeviceGroup.Length)
					});
				}
				else
				{
					list2.Add(new SiemensGroup(siemensDeviceGroup.Name, this.GetStoreTypeByStoreArea(siemensDeviceGroup.StoreArea), siemensDeviceGroup.DBNo, siemensDeviceGroup.Start, siemensDeviceGroup.Length));
					if (list2.Sum((SiemensGroup dataItem) => dataItem.Count) > num - this.GetGroup(list2) || list2.Count >= (int)(this.siemens.MaxPDUSize / 12))
					{
						list2.RemoveAt(list2.Count - 1);
						list.Add(list2);
						list2 = new List<SiemensGroup>
						{
							new SiemensGroup(siemensDeviceGroup.Name, this.GetStoreTypeByStoreArea(siemensDeviceGroup.StoreArea), siemensDeviceGroup.DBNo, siemensDeviceGroup.Start, siemensDeviceGroup.Length)
						};
					}
				}
				if (siemensDeviceGroup == this.DeviceGroupList.Last<SiemensDeviceGroup>() && list2.Count > 0)
				{
					list.Add(list2);
				}
			}
			foreach (List<SiemensGroup> siemensGroup in list)
			{
				List<SiemensGroup> list3 = this.siemens.ReadMultipleVars(siemensGroup);
				if (list3 == null)
				{
					return false;
				}
				using (List<SiemensGroup>.Enumerator enumerator3 = list3.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						SiemensGroup gp = enumerator3.Current;
						SiemensDeviceGroup siemensDeviceGroup2 = this.DeviceGroupList.FirstOrDefault((SiemensDeviceGroup c) => c.Name == gp.GroupName && c.DBNo == gp.DB && c.Start == gp.StartByteAdr && c.Length == gp.Count && this.GetStoreTypeByStoreArea(c.StoreArea) == gp.StoreType);
						siemensDeviceGroup2.Value = gp.Value;
					}
				}
			}
			return true;
		}
        
		private int GetGroup(List<SiemensGroup> list_0)
		{
			int result;
			if (list_0.Count == 1)
			{
				result = 0;
			}
			else
			{
				int num = 0;
				for (int i = 1; i < list_0.Count; i++)
				{
					num += ((list_0[i].Count % 2 == 0) ? 4 : 5);
				}
				result = num;
			}
			return result;
		}
        
		public bool VerifySiemensAddress(string address, out int start, out int offset)
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
			else if (address.Contains('|'))
			{
				string[] array2 = address.Split(new char[]
				{
					'|'
				});
				if (array2.Length == 2)
				{
					int num2 = 0;
					bool flag2 = int.TryParse(array2[0], out num2);
					start = num2;
					flag2 = (flag2 && int.TryParse(array2[1], out num2));
					offset = num2;
					result = flag2;
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
        
		public StoreType GetStoreTypeByStoreArea(SiemensStoreArea siemensStoreArea)
		{
			StoreType result;
			switch (siemensStoreArea)
			{
			case SiemensStoreArea.M存储区:
				result = StoreType.Marker;
				break;
			case SiemensStoreArea.I存储区:
				result = StoreType.Input;
				break;
			case SiemensStoreArea.Q存储区:
				result = StoreType.Output;
				break;
			case SiemensStoreArea.DB存储区:
				result = StoreType.DataBlock;
				break;
			case SiemensStoreArea.T存储区:
				result = StoreType.Timer;
				break;
			case SiemensStoreArea.C存储区:
				result = StoreType.Counter;
				break;
			default:
				result = StoreType.DataBlock;
				break;
			}
			return result;
		}
        
		public CPU_Type GetCPUTypeFromPLCType(SiemensPLCType type)
		{
			CPU_Type result;
			switch (type)
			{
			case SiemensPLCType.S7200:
				result = CPU_Type.S7200;
				break;
			case SiemensPLCType.S7200SMART:
				result = CPU_Type.S7200SMART;
				break;
			case SiemensPLCType.S7300:
				result = CPU_Type.S7300;
				break;
			case SiemensPLCType.S7400:
				result = CPU_Type.S7400;
				break;
			case SiemensPLCType.S71200:
				result = CPU_Type.S71200;
				break;
			case SiemensPLCType.S71500:
				result = CPU_Type.S71500;
				break;
			default:
				result = CPU_Type.S71200;
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
				SiemensVariable siemensVariable = this.CurrentVarList[keyName] as SiemensVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(siemensVariable, siemensVariable.VarType, setValue);
                result = xktResult.IsSuccess ? this.siemens.Write(siemensVariable.VarAddress, xktResult.Content, siemensVariable.VarType) : xktResult;
            }
			return result;
		}
        
		public CancellationTokenSource cts;
        
		public List<SiemensDeviceGroup> DeviceGroupList;
        
		public List<SiemensVariable> AlarmVarList;
        
		public SiemensS7 siemens;
        
		public Stopwatch sw;
        
	}
}
