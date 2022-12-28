using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Linq;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Modbus
{
	public class ModbusNode : NodeClass
	{
		public ModbusNode()
		{
			this.KeyWay = KeyWay.VarName;
			this.CurrentValue = new Dictionary<string, object>();
			this.CurrentVarList = new Dictionary<string, VariableNode>();
			this.StoreVarList = new List<VariableNode>();
			this.UseAlarmCheck = false;
			base.NodeType = 2;
			base.NodeHead = "ModbusNode";
			this.CreateTime = DateTime.Now;
			this.ConnectTimeOut = 2000;
			this.ReConnectTime = 5000;
			this.InstallationDate = DateTime.Now;
			this.IsActive = true;
			this.MaxErrorTimes = 1;
		}
        
		public int ModbusType { get; set; }
        
		public DateTime InstallationDate { get; set; }
        
		public int ConnectTimeOut { get; set; }
        
		public DateTime CreateTime { get; set; }
        
		public int ReConnectTime { get; set; }
        
		public int MaxErrorTimes { get; set; }
        
		public int ErrorTimes { get; set; }
		
		public bool IsActive { get; set; }
        
		public KeyWay KeyWay { get; set; }
        
		public bool UseAlarmCheck { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.ModbusType = int.Parse(element.Attribute("ModbusType").Value);
			this.ConnectTimeOut = int.Parse(element.Attribute("ConnectTimeOut").Value);
			this.CreateTime = DateTime.Parse(element.Attribute("CreateTime").Value);
			this.ReConnectTime = int.Parse(element.Attribute("ReConnectTime").Value);
			this.InstallationDate = DateTime.Parse(element.Attribute("InstallationDate").Value);
			this.IsActive = bool.Parse(element.Attribute("IsActive").Value);
			this.ReConnectTime = int.Parse(element.Attribute("ReConnectTime").Value);
			this.MaxErrorTimes = int.Parse(element.Attribute("MaxErrorTimes").Value);
			this.KeyWay = (KeyWay)Enum.Parse(typeof(KeyWay), element.Attribute("KeyWay").Value, true);
			this.UseAlarmCheck = bool.Parse(element.Attribute("UseAlarmCheck").Value);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("ModbusType", this.ModbusType);
			xelement.SetAttributeValue("ConnectTimeOut", this.ConnectTimeOut);
			xelement.SetAttributeValue("CreateTime", this.CreateTime.ToString());
			xelement.SetAttributeValue("ReConnectTime", this.ReConnectTime);
			xelement.SetAttributeValue("InstallationDate", this.InstallationDate.ToString());
			xelement.SetAttributeValue("IsActive", this.IsActive.ToString());
			xelement.SetAttributeValue("ReConnectTime", this.ReConnectTime.ToString());
			xelement.SetAttributeValue("MaxErrorTimes", this.MaxErrorTimes.ToString());
			xelement.SetAttributeValue("KeyWay", this.KeyWay.ToString());
			xelement.SetAttributeValue("UseAlarmCheck", this.UseAlarmCheck.ToString());
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("键使用形式", this.KeyWay.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("启用报警检测", this.UseAlarmCheck ? "启用" : "禁用"));
			return nodeClassRenders;
		}
        
		public bool VerifyModbusAddress(bool isBit, string address, out int start, out int offset)
		{
			bool result;
			if (isBit)
			{
				offset = 0;
				result = int.TryParse(address, out start);
			}
			else if (address.Contains('.'))
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
        
		public object this[string key]
		{
			get
			{
				object result;
				if (this.CurrentValue.ContainsKey(key))
				{
					result = this.CurrentValue[key];
				}
				else
				{
					result = null;
				}
				return result;
			}
		}
        
		public event Action<object, AlarmEventArgs> AlarmEvent
		{
			[CompilerGenerated]
			add
			{
				Action<object, AlarmEventArgs> action = this.alarmEvent;
				Action<object, AlarmEventArgs> action2;
				do
				{
					action2 = action;
					Action<object, AlarmEventArgs> value2 = (Action<object, AlarmEventArgs>)Delegate.Combine(action2, value);
					action = Interlocked.CompareExchange<Action<object, AlarmEventArgs>>(ref this.alarmEvent, value2, action2);
				}
				while (action != action2);
			}
			[CompilerGenerated]
			remove
			{
				Action<object, AlarmEventArgs> action = this.alarmEvent;
				Action<object, AlarmEventArgs> action2;
				do
				{
					action2 = action;
					Action<object, AlarmEventArgs> value2 = (Action<object, AlarmEventArgs>)Delegate.Remove(action2, value);
					action = Interlocked.CompareExchange<Action<object, AlarmEventArgs>>(ref this.alarmEvent, value2, action2);
				}
				while (action != action2);
			}
		}
        
		public void CheckAlarm(VariableNode variable)
		{
			if (variable.Config.AlarmEnable)
			{
				if (variable.Config.IsConditionAlarmType)
				{
					string s = variable.Value.ToString();
					float num = 0f;
					if (float.TryParse(s, out num))
					{
						if (variable.Config.HiHiAlarmEnable)
						{
							int num2 = Common.Compare(num, variable.Config.HiHiAlarmValue, variable.Config.HiHiCacheValue, true);
							if (num2 != 0)
							{
								Action<object, AlarmEventArgs> action = this.alarmEvent;
								if (action != null)
								{
									action(variable, new AlarmEventArgs
									{
										alarmInfo = variable.Config.HiHiAlarmNote,
										CurrentValue = num.ToString(),
										SetValue = variable.Config.HiHiAlarmValue.ToString(),
										IsACK = (num2 == 1)
									});
								}
							}
							variable.Config.HiHiCacheValue = num;
						}
						if (variable.Config.HighAlarmEnable)
						{
							int num3 = Common.Compare(num, variable.Config.HighAlarmValue, variable.Config.HighCacheValue, true);
							if (num3 != 0)
							{
								Action<object, AlarmEventArgs> action2 = this.alarmEvent;
								if (action2 != null)
								{
									action2(variable, new AlarmEventArgs
									{
										alarmInfo = variable.Config.HighAlarmNote,
										CurrentValue = num.ToString(),
										SetValue = variable.Config.HighAlarmValue.ToString(),
										IsACK = (num3 == 1)
									});
								}
							}
							variable.Config.HighCacheValue = num;
						}
						if (variable.Config.LoLoAlarmEnable)
						{
							int num4 = Common.Compare(num, variable.Config.LoLoAlarmValue, variable.Config.LoLoCacheValue, false);
							if (num4 != 0)
							{
								Action<object, AlarmEventArgs> action3 = this.alarmEvent;
								if (action3 != null)
								{
									action3(variable, new AlarmEventArgs
									{
										alarmInfo = variable.Config.LoLoAlarmNote,
										CurrentValue = num.ToString(),
										SetValue = variable.Config.LoLoAlarmValue.ToString(),
										IsACK = (num4 == 1)
									});
								}
							}
							variable.Config.LoLoCacheValue = num;
						}
						if (variable.Config.LowAlarmEnable)
						{
							int num5 = Common.Compare(num, variable.Config.LowAlarmValue, variable.Config.LowCacheValue, false);
							if (num5 != 0)
							{
								Action<object, AlarmEventArgs> action4 = this.alarmEvent;
								if (action4 != null)
								{
									action4(variable, new AlarmEventArgs
									{
										alarmInfo = variable.Config.LowAlarmNote,
										CurrentValue = num.ToString(),
										SetValue = variable.Config.LowAlarmValue.ToString(),
										IsACK = (num5 == 1)
									});
								}
							}
							variable.Config.LowCacheValue = num;
						}
					}
				}
				else
				{
					bool flag = variable.Value.ToString().ToLower() == "true";
					if (variable.Config.DiscreteAlarmType)
					{
						int num6 = Common.Compare(flag ? 1f : 0f, 1f, variable.Config.DiscreteRiseCacheValue ? 1f : 0f, true);
						if (num6 != 0)
						{
							Action<object, AlarmEventArgs> action5 = this.alarmEvent;
							if (action5 != null)
							{
								action5(variable, new AlarmEventArgs
								{
									alarmInfo = variable.Config.DiscreteAlarmNote,
									CurrentValue = flag.ToString(),
									SetValue = "True",
									IsACK = (num6 == 1)
								});
							}
						}
						variable.Config.DiscreteRiseCacheValue = flag;
					}
					else
					{
						int num7 = Common.Compare(flag ? 1f : 0f, 0f, variable.Config.DiscreteFallCacheValue ? 1f : 0f, false);
						if (num7 != 0)
						{
							Action<object, AlarmEventArgs> action6 = this.alarmEvent;
							if (action6 != null)
							{
								action6(variable, new AlarmEventArgs
								{
									alarmInfo = variable.Config.DiscreteAlarmNote,
									CurrentValue = flag.ToString(),
									SetValue = "False",
									IsACK = (num7 == 1)
								});
							}
						}
						variable.Config.DiscreteFallCacheValue = flag;
					}
				}
			}
		}
        
		public void UpdateCurrentValue(VariableNode variable)
		{
			if (this.CurrentValue.ContainsKey(variable.KeyName))
			{
				this.CurrentValue[variable.KeyName] = variable.Value;
			}
			else
			{
				this.CurrentValue.Add(variable.KeyName, variable.Value);
			}
			if (this.UseAlarmCheck)
			{
				this.CheckAlarm(variable);
			}
		}
        
		public const int ModbusRTU = 1000;
        
		public const int ModbusTCP = 2000;
        
		public const int ModbusASCII = 3000;
        
		public const int ModbusUDP = 4000;
        
		public const int ModbusRTUOverTCP = 5000;
        
		public const int ModbusRTUOverUDP = 6000;
        
		public const int ModbusASCIIOverTCP = 7000;
        
		public const int ModbusASCIIOverUDP = 8000;
        
		public Dictionary<string, object> CurrentValue;
        
		public Dictionary<string, VariableNode> CurrentVarList;
        
		public List<VariableNode> StoreVarList;
        
		private Action<object, AlarmEventArgs> alarmEvent;
	}
}
