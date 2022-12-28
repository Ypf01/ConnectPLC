using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Linq;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Device
{
    /// <summary>
    /// DeviceNode节点
    /// </summary>
	public class DeviceNode : NodeClass
    {
        public DeviceNode()
        {
            this.KeyWay = KeyWay.VarName;
            this.CurrentValue = new Dictionary<string, object>();
            this.CurrentVarList = new Dictionary<string, VariableNode>();
            this.StoreVarList = new List<VariableNode>();
            this.UseAlarmCheck = false;
            base.NodeType = 2;
            base.NodeHead = "DeviceNode";
            this.CreateTime = DateTime.Now;
            this.ConnectTimeOut = 2000;
            this.ReConnectTime = 5000;
            this.InstallationDate = DateTime.Now;
            this.IsActive = true;
            this.MaxErrorTimes = 1;
        }
        /// <summary>
        /// 设备的类别
        /// </summary>
		public int DeviceType { get; set; }
        /// <summary>
        /// 安装的时间
        /// </summary>
		public DateTime InstallationDate { get; set; }
        /// <summary>
        ///  连接超时的时间，单位毫秒
        /// </summary>
		public int ConnectTimeOut { get; set; }
        /// <summary>
        /// 重连时间，单位毫秒
        /// </summary>
		public int ReConnectTime { get; set; }
        /// <summary>
        /// 服务器的创建日期
        /// </summary>
		public DateTime CreateTime { get; set; }
        /// <summary>
        /// 允许的最大出错次数
        /// </summary>
		public int MaxErrorTimes { get; set; }
        /// <summary>
        /// 出错次数
        /// </summary>
		public int ErrorTimes { get; set; }
        /// <summary>
        /// PLC设备是否激活
        /// </summary>
		public bool IsActive { get; set; }
        /// <summary>
        /// 使用键的形式
        /// </summary>
		public KeyWay KeyWay { get; set; }
        /// <summary>
        ///  是否启用报警检测
        /// </summary>
		public bool UseAlarmCheck { get; set; }
        /// <summary>
        /// 从XML元素对象中获取对象属性
        /// </summary>
        /// <param name="element"></param>
		public override void LoadByXmlElement(XElement element)
        {
            base.LoadByXmlElement(element);
            this.DeviceType = int.Parse(element.Attribute("DeviceType").Value);
            this.ConnectTimeOut = int.Parse(element.Attribute("ConnectTimeOut").Value);
            this.ReConnectTime = int.Parse(element.Attribute("ReConnectTime").Value);
            this.CreateTime = DateTime.Parse(element.Attribute("CreateTime").Value);
            this.InstallationDate = DateTime.Parse(element.Attribute("InstallationDate").Value);
            this.IsActive = bool.Parse(element.Attribute("IsActive").Value);
            this.ReConnectTime = int.Parse(element.Attribute("ReConnectTime").Value);
            this.MaxErrorTimes = int.Parse(element.Attribute("MaxErrorTimes").Value);
            this.KeyWay = (KeyWay)Enum.Parse(typeof(KeyWay), element.Attribute("KeyWay").Value, true);
            this.UseAlarmCheck = bool.Parse(element.Attribute("UseAlarmCheck").Value);
        }
        /// <summary>
        /// 将对象属性保存至XML元素对象
        /// </summary>
        /// <returns></returns>
		public override XElement ToXmlElement()
        {
            XElement xelement = base.ToXmlElement();
            xelement.SetAttributeValue("DeviceType", this.DeviceType);
            xelement.SetAttributeValue("ConnectTimeOut", this.ConnectTimeOut);
            xelement.SetAttributeValue("ReConnectTime", this.ReConnectTime);
            xelement.SetAttributeValue("CreateTime", this.CreateTime.ToString());
            xelement.SetAttributeValue("InstallationDate", this.InstallationDate.ToString());
            xelement.SetAttributeValue("IsActive", this.IsActive.ToString());
            xelement.SetAttributeValue("ReConnectTime", this.ReConnectTime.ToString());
            xelement.SetAttributeValue("MaxErrorTimes", this.MaxErrorTimes.ToString());
            xelement.SetAttributeValue("KeyWay", this.KeyWay.ToString());
            xelement.SetAttributeValue("UseAlarmCheck", this.UseAlarmCheck.ToString());
            return xelement;
        }
        /// <summary>
        ///  获取用于在数据表信息中显示的键值数据对信息
        /// </summary>
        /// <returns></returns>
		public override List<NodeClassRenderItem> GetNodeClassRenders()
        {
            List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
            nodeClassRenders.Add(new NodeClassRenderItem("键使用形式", this.KeyWay.ToString()));
            nodeClassRenders.Add(new NodeClassRenderItem("启用报警检测", this.UseAlarmCheck ? "启用" : "禁用"));
            return nodeClassRenders;
        }
        /// <summary>
        /// 索引器通过键查找值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		public object this[string key]
        {
            get
            {
                return this.CurrentValue.ContainsKey(key) ? this.CurrentValue[key] : null;
            }
        }
        /// <summary>
        /// 报警事件
        /// </summary>
		public event Action<object, AlarmEventArgs> AlarmEvent
        {
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
        /// <summary>
        ///  报警检测
        /// </summary>
        /// <param name="variable"></param>
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
        /// <summary>
        /// 变量更新及检测
        /// </summary>
        /// <param name="variable"></param>
		public void UpdateCurrentValue(VariableNode variable)
        {
            if (this.CurrentValue.ContainsKey(variable.KeyName))
                this.CurrentValue[variable.KeyName] = variable.Value;
            else
                this.CurrentValue.Add(variable.KeyName, variable.Value);
            if (this.UseAlarmCheck)
                this.CheckAlarm(variable);
        }

        public T GetValue<T>(string valueName)
        {
            if (CurrentValue == null || CurrentValue.Count == 0 || !CurrentValue.ContainsKey(valueName))
                return default(T);
            object result = CurrentValue[valueName];
            Type type = typeof(T);
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = type.GetGenericArguments()[0];

            var tryParse = type.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder, new Type[] { result.GetType(), type.MakeByRefType() }, new ParameterModifier[] { new ParameterModifier(2) });
            if (tryParse != null)
            {
                var paramerts = new object[] { result, Activator.CreateInstance(type) };
                bool success = (bool)tryParse.Invoke(null, paramerts);
                return success ? (T)paramerts[1] : default(T);
            }
            return (T)Convert.ChangeType(result, typeof(T));
        }

        public object GetValue(string valueName)
        {
            if (CurrentValue == null || CurrentValue.Count == 0 || !CurrentValue.ContainsKey(valueName))
                return default;
            return CurrentValue[valueName];
        }

        public VariableNode GetVariableNode(string valueName)
        {
            if (CurrentVarList == null || CurrentVarList.Keys.Count == 0 || !CurrentVarList.ContainsKey(valueName))
                return null;
            return CurrentVarList[valueName];
        }

        /// <summary>
        /// 三菱的Qna兼容3E帧协议的客户端
        /// </summary>
        public const int Melsec = 10;
        /// <summary>
        /// 基恩士PLC
        /// </summary>
		public const int Keyence = 20;
        /// <summary>
        ///  西门子的PLC设备
        /// </summary>
		public const int Siemens = 30;
        /// <summary>
        /// 欧姆龙的PLC设备
        /// </summary>
		public const int Omron = 40;
        /// <summary>
        /// 三菱FX PLC设备编程口
        /// </summary>
		public const int FXSerial = 50;
        /// <summary>
        /// 欧姆龙 PLC Hostlink设备
        /// </summary>
		public const int OmronHostlink = 60;
        /// <summary>
        /// 松下 PLC Mewtocol设备
        /// </summary>
		public const int Mewtocol = 70;
        /// <summary>
        /// 三菱FX PLC设备专用协议
        /// </summary>
		public const int FXLink = 80;
        /// <summary>
        ///  西门子S7-200 PPI协议
        /// </summary>
		public const int SiemensPPI = 90;
        /// <summary>
        /// 信捷XC系列 Modbus协议
        /// </summary>
		public const int XinjeXC = 100;
        /// <summary>
        /// 汇川系列 Modbus协议
        /// </summary>
		public const int Inovance = 110;
        /// <summary>
        /// 台达系列 串口协议
        /// </summary>
		public const int DeltaSerial = 120;
        /// <summary>
        /// 倍福系列 串口协议
        /// </summary>
		public const int Beckhoff = 130;
        /// <summary>
        ///  台达系列 网口协议
        /// </summary>
		public const int DeltaEthernet = 140;
        /// <summary>
        /// 基恩士PLC 串口协议
        /// </summary>
		public const int KeyenceSerial = 150;
        /// <summary>
        /// 欧姆龙CIP协议
        /// </summary>
		public const int OmronCIP = 160;
        /// <summary>
        ///  AB CIP协议
        /// </summary>
		public const int ABCIP = 170;
        /// <summary>
        /// 当前数据集合
        /// </summary>
		public Dictionary<string, object> CurrentValue;
        /// <summary>
        ///  当前变量集合
        /// </summary>
		public Dictionary<string, VariableNode> CurrentVarList;
        /// <summary>
        /// 归档变量集合
        /// </summary>
		public List<VariableNode> StoreVarList;
        /// <summary>
        /// 报警事件
        /// </summary>
		private Action<object, AlarmEventArgs> alarmEvent;
    }
}
