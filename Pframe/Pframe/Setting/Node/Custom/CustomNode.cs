using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Linq;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Custom
{
    public class CustomNode : NodeClass
    {
        public CustomNode()
        {
            this.KeyWay = KeyWay.VarName;
            this.CurrentValue = new Dictionary<string, object>();
            this.CurrentVarList = new Dictionary<string, VariableNode>();
            this.StoreVarList = new List<VariableNode>();
            this.UseAlarmCheck = false;
            base.NodeType = 2;
            base.NodeHead = "CustomNode";
            this.CreateTime = DateTime.Now;
            this.ConnectTimeOut = 2000;
            this.ReConnectTime = 5000;
            this.InstallationDate = DateTime.Now;
            this.IsActive = true;
            this.MaxErrorTimes = 5;
        }
        /// <summary>
        /// 设备的类别
        /// </summary>
        public int CustomType { get; set; }
        /// <summary>
        /// 安装的时间
        /// </summary>
        public DateTime InstallationDate { get; set; }
        /// <summary>
        /// 连接超时的时间，单位毫秒
        /// </summary>
        public int ConnectTimeOut { get; set; }
        /// <summary>
        /// 服务器的创建日期
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        ///  重连时间，单位毫秒
        /// </summary>
        public int ReConnectTime { get; set; }
        /// <summary>
        /// 该设备是否激活
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 允许的最大出错次数
        /// </summary>
        public int MaxErrorTimes { get; set; }
        /// <summary>
        /// 出错次数
        /// </summary>
        public int ErrorTimes { get; set; }
        /// <summary>
        /// 使用键的形式
        /// </summary>
        public KeyWay KeyWay { get; set; }
        /// <summary>
        /// 是否启用报警检测
        /// </summary>
        public bool UseAlarmCheck { get; set; }
        /// <summary>
        /// 加载Xml元素
        /// </summary>
        /// <param name="element"></param>
        public override void LoadByXmlElement(XElement element)
        {
            base.LoadByXmlElement(element);
            this.CustomType = int.Parse(element.Attribute("CustomType").Value);
            this.ConnectTimeOut = int.Parse(element.Attribute("ConnectTimeOut").Value);
            this.CreateTime = DateTime.Parse(element.Attribute("CreateTime").Value);
            this.ReConnectTime = int.Parse(element.Attribute("ReConnectTime").Value);
            this.InstallationDate = DateTime.Parse(element.Attribute("InstallationDate").Value);
            this.IsActive = bool.Parse(element.Attribute("IsActive").Value);
            this.KeyWay = (KeyWay)Enum.Parse(typeof(KeyWay), element.Attribute("KeyWay").Value, true);
            this.UseAlarmCheck = bool.Parse(element.Attribute("UseAlarmCheck").Value);
        }
        /// <summary>
        /// 获取Xml元素
        /// </summary>
        /// <returns></returns>
        public override XElement ToXmlElement()
        {
            XElement xelement = base.ToXmlElement();
            xelement.SetAttributeValue("CustomType", this.CustomType);
            xelement.SetAttributeValue("ConnectTimeOut", this.ConnectTimeOut);
            xelement.SetAttributeValue("CreateTime", this.CreateTime.ToString());
            xelement.SetAttributeValue("ReConnectTime", this.ReConnectTime);
            xelement.SetAttributeValue("InstallationDate", this.InstallationDate.ToString());
            xelement.SetAttributeValue("IsActive", this.IsActive.ToString());
            xelement.SetAttributeValue("KeyWay", this.KeyWay.ToString());
            xelement.SetAttributeValue("UseAlarmCheck", this.UseAlarmCheck.ToString());
            return xelement;
        }
        /// <summary>
        /// 获取用于在数据表信息中显示的键值数据对信息
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
                object result;
                if (this.CurrentValue.ContainsKey(key))
                    result = this.CurrentValue[key];
                else
                    result = null;
                return result;
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
        /// 报警检测
        /// </summary>
        /// <param name="variable"></param>
        public void CheckAlarm(VariableNode variable)
        {
            if (variable.Config.AlarmEnable)
            {
                #region MyRegion

                #endregion
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
        /// <summary>
        ///  ESD电源客户端
        /// </summary>
        public const int CustomESD = 100000;
        /// <summary>
        /// VD电源客户端
        /// </summary>
        public const int CustomVD = 200000;
        /// <summary>
        ///  宇电仪表客户端
        /// </summary>
        public const int CustomYD = 300000;
        /// <summary>
        /// MT850H变频器客户端
        /// </summary>
        public const int CustomMT = 400000;
        /// <summary>
        /// TCP自定义协议
        /// </summary>
        public const int CustomSK = 500000;
        /// <summary>
        ///  当前数据集合
        /// </summary>
        public Dictionary<string, object> CurrentValue;
        /// <summary>
        /// 当前变量集合
        /// </summary>
        public Dictionary<string, VariableNode> CurrentVarList;
        /// <summary>
        /// 归档变量集合
        /// </summary>
        public List<VariableNode> StoreVarList;
        /// <summary>
        /// 报警委托原型
        /// </summary>
        private Action<object, AlarmEventArgs> alarmEvent;
        public void UpInfo(VariableNode var, AlarmEventArgs arg)
        {
            Action<object, AlarmEventArgs> action = this.alarmEvent;
            if (action != null)
                action(var, arg);
        }
    }
}
