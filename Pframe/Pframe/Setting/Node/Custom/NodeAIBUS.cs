using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Pframe;
using Pframe.Common;
using Pframe.Custom;
using Pframe.DataConvert;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Custom
{
    public class NodeAIBUS : CustomNode, IXmlConvert
    {
        public NodeAIBUS()
        {
            this.sw = new Stopwatch();
            this.CustomGroupList = new List<AIBUSGroup>();
            base.Name = "AIBUS仪表";
            base.Description = "AIBUS仪表";
            base.CustomType = 300000;
            this.PortNum = "COM3";
            this.Paud = 9600;
            this.Parity = Parity.None;
            this.DataBits = "8";
            this.StopBits = StopBits.One;
            this.SleepTime = 20;
            this.FirstConnect = true;
        }

        public bool ConnectState { get; set; }

        public bool FirstConnect { get; set; }

        public long CommRate { get; set; }

        public string PortNum { get; set; }

        public int Paud { get; set; }

        public Parity Parity { get; set; }

        public string DataBits { get; set; }

        public StopBits StopBits { get; set; }

        public DataFormat DataFormat { get; set; }

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
            foreach (AIBUSGroup aibusgroup in this.CustomGroupList)
            {
                foreach (AIBUSVariable aibusvariable in aibusgroup.varList)
                {
                    if (aibusvariable.Config.ArchiveEnable)
                        this.StoreVarList.Add(aibusvariable);
                    if (this.CurrentVarList.ContainsKey(aibusvariable.KeyName))
                        this.CurrentVarList[aibusvariable.KeyName] = aibusvariable;
                    else
                        this.CurrentVarList.Add(aibusvariable.KeyName, aibusvariable);
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
                    foreach (AIBUSGroup aibusgroup in this.CustomGroupList)
                    {
                        AIBUSParam aibusparam = null;
                        if (aibusgroup.IsActive && aibusgroup.varList.Count > 0)
                        {
                            foreach (AIBUSVariable aibusvariable in aibusgroup.varList)
                            {
                                if (aibusvariable.VarAddress != "255")
                                {
                                    aibusparam = this.aibus.ReadParam(Convert.ToByte(aibusvariable.VarAddress), aibusgroup.DevID);
                                    aibusvariable.Value = ((aibusparam != null) ? new int?(aibusparam.ParamValue) : null);
                                    this.ConnectState = (aibusparam != null);
                                }
                                else
                                {
                                    string name = aibusvariable.Name;
                                    string a = name;
                                    if (!(a == "实际值"))
                                    {
                                        if (!(a == "设定值"))
                                        {
                                            if (!(a == "HIAL上限报警状态"))
                                            {
                                                if (a == "LoAL下限报警状态")
                                                {
                                                    aibusvariable.Value = ((aibusparam != null) ? new bool?(aibusparam.LoAL) : null);
                                                }
                                            }
                                            else
                                            {
                                                aibusvariable.Value = ((aibusparam != null) ? new bool?(aibusparam.HIAL) : null);
                                            }
                                        }
                                        else
                                        {
                                            aibusvariable.Value = ((aibusparam != null) ? new int?(aibusparam.SetValue) : null);
                                        }
                                    }
                                    else
                                    {
                                        aibusvariable.Value = ((aibusparam != null) ? new int?(aibusparam.ActualValue) : null);
                                    }
                                }
                                if (aibusparam != null)
                                {
                                    aibusvariable.Value = MigrationLib.GetMigrationValue(aibusvariable.Value, aibusvariable.Scale, aibusvariable.Offset);
                                    base.UpdateCurrentValue(aibusvariable);
                                }
                                else
                                {
                                    int errorTimes = base.ErrorTimes;
                                    base.ErrorTimes = errorTimes + 1;
                                    if (base.ErrorTimes >= base.MaxErrorTimes)
                                    {
                                        if (SerialPort.GetPortNames().Contains(this.PortNum))
                                        {
                                            Thread.Sleep(10);
                                            continue;
                                        }
                                        this.IsConnected = false;
                                    }
                                }
                                Thread.Sleep((int)aibusgroup.DelayTime);
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
                        AIBUS aibus = this.aibus;
                        if (aibus != null)
                            aibus.DisConnect();
                    }
                    this.aibus = new AIBUS();
                    this.aibus.SleepTime = this.SleepTime;
                    this.IsConnected = this.aibus.Connect(this.PortNum, this.Paud, int.Parse(this.DataBits), this.Parity, this.StopBits);
                    this.FirstConnect = false;
                }
            }
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
                AIBUSVariable aibusvariable = this.CurrentVarList[keyName] as AIBUSVariable;
                if (aibusvariable.VarAddress != "255")
                {
                    result = new CalResult
                    {
                        IsSuccess = false,
                        Message = "该变量不支持修改"
                    };
                }
                else
                {
                    CalResult<string> xktResult = Common.VerifyInputValue(aibusvariable, aibusvariable.VarType, setValue);
                    if (xktResult.IsSuccess)
                    {
                        result = new CalResult
                        {
                            IsSuccess = (this.aibus.SetParam(Convert.ToByte(aibusvariable.VarAddress), Convert.ToInt16(xktResult.Content), aibusvariable.DeviceID) != null)
                        };
                    }
                    else
                    {
                        result = xktResult;
                    }
                }
            }
            return result;
        }
        

        public CancellationTokenSource cts;

        public Stopwatch sw;

        public const int Paud9600 = 9600;

        public const int Paud19200 = 19200;

        public const int Paud38400 = 38400;

        public const int Paud57600 = 57600;

        public const string DataBitsSeven = "7";

        public const string DataBitsEight = "8";

        public AIBUS aibus;

        public List<AIBUSGroup> CustomGroupList;
    }
}
