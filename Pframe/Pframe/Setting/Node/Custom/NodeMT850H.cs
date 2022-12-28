using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Pframe.Common;
using Pframe.Custom;
using Pframe.DataConvert;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Custom
{
    public class NodeMT850H : CustomNode, IXmlConvert
    {
        public NodeMT850H()
        {
            this.sw = new Stopwatch();
            this.CustomGroupList = new List<MT850HGroup>();
            base.Name = "MT850H变频器";
            base.Description = "MT850H变频器";
            base.CustomType = 400000;
            this.PortNum = "COM3";
            this.Paud = 9600;
            this.Parity = Parity.None;
            this.DataBits = "8";
            this.StopBits = StopBits.One;
            this.SleepTime = 20;
            this.DataFormat = 0;
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
            this.DataFormat = (DataFormat)Enum.Parse(typeof(DataFormat), element.Attribute("DataFormat").Value, true);
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
            xelement.SetAttributeValue("DataFormat", this.DataFormat);
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
            nodeClassRenders.Add(new NodeClassRenderItem("数据格式", this.DataFormat.ToString()));
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
                    foreach (MT850HGroup mt850HGroup in this.CustomGroupList)
                    {
                        if (mt850HGroup.IsActive && mt850HGroup.varList.Count > 0)
                        {
                            foreach (MT850HVariable mt850HVariable in mt850HGroup.varList)
                            {
                                byte[] array = this.mt850H.ReadSingleReg(mt850HVariable.VarAddress);
                                this.ConnectState = (array != null);
                                if (array != null)
                                {
                                    DataType varType = mt850HVariable.VarType;
                                    DataType dataType = varType;
                                    if ((int)dataType != 4)
                                    {
                                        if ((int)dataType == 6)
                                            mt850HVariable.Value = FloatLib.GetFloatFromByteArray(array, 0, this.DataFormat);
                                    }
                                    else
                                        mt850HVariable.Value = IntLib.GetIntFromByteArray(array, 0, this.DataFormat);
                                    mt850HVariable.Value = MigrationLib.GetMigrationValue(mt850HVariable.Value, mt850HVariable.Scale, mt850HVariable.Offset);
                                    base.UpdateCurrentValue(mt850HVariable);
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
                                Thread.Sleep((int)mt850HGroup.DelayTime);
                            }
                        }
                    }
                    string empty = string.Empty;
                    if (!this.mt850H.LinkStatus(ref empty))
                    {
                        int errorTimes = base.ErrorTimes;
                        base.ErrorTimes = errorTimes + 1;
                        if (base.ErrorTimes >= base.MaxErrorTimes)
                        {
                            if (SerialPort.GetPortNames().Contains(this.PortNum))
                                Thread.Sleep(10);
                            else
                                this.IsConnected = false;
                        }
                    }
                }
                else
                {
                    if (!this.FirstConnect)
                    {
                        Thread.Sleep(base.ReConnectTime);
                        MT850H mt850H = this.mt850H;
                        if (mt850H != null)
                            mt850H.DisConnect();
                    }
                    this.mt850H = new MT850H();
                    this.SleepTime = this.SleepTime;
                    this.IsConnected = this.mt850H.Connect(this.PortNum, this.Paud, int.Parse(this.DataBits), this.Parity, this.StopBits);
                    this.FirstConnect = false;
                }
            }
        }


        public CancellationTokenSource cts;

        public Stopwatch sw;

        public const int Paud9600 = 9600;

        public const int Paud19200 = 19200;

        public const int Paud38400 = 38400;

        public const int Paud57600 = 57600;

        public const string DataBitsSeven = "7";

        public const string DataBitsEight = "8";

        public MT850H mt850H;

        public List<MT850HGroup> CustomGroupList;
    }
}
