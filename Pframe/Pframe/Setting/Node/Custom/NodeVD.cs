using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Pframe;
using Pframe.Custom;
using Pframe.DataConvert;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Custom
{
    /// <summary>
    /// VD节点信息
    /// </summary>
    public class NodeVD : CustomNode, IXmlConvert
    {
        public NodeVD()
        {
            this.sw = new Stopwatch();
            this.CustomGroupList = new List<VDGroup>();
            base.Name = "VD电源";
            base.Description = "SC200中心区电源";
            base.CustomType = 200000;
            this.IpAddress = "192.168.0.3";
            this.Port = 9600;
            this.VDType = "SigMaphi";
        }
        /// <summary>
        ///  第一次连接
        /// </summary>
        public bool FirstConnect { get; set; }
        /// <summary>
        /// 通信周期
        /// </summary>
        public long CommRate { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// VD类型
        /// </summary>
        public string VDType { get; set; }
        /// <summary>
        ///  连接状态
        /// </summary>
        public bool IsConnected { get; set; }
        /// <summary>
        ///   加载Xml元素
        /// </summary>
        /// <param name="element"></param>
        public override void LoadByXmlElement(XElement element)
        {
            base.LoadByXmlElement(element);
            this.IpAddress = element.Attribute("IpAddress").Value;
            this.Port = int.Parse(element.Attribute("Port").Value);
            this.VDType = element.Attribute("VDType").Value;
        }
        /// <summary>
        /// 获取Xml元素
        /// </summary>
        /// <returns></returns>
        public override XElement ToXmlElement()
        {
            XElement xelement = base.ToXmlElement();
            xelement.SetAttributeValue("IpAddress", this.IpAddress);
            xelement.SetAttributeValue("Port", this.Port);
            xelement.SetAttributeValue("VDType", this.VDType);
            return xelement;
        }
        /// <summary>
        /// 获取用于在数据表信息中显示的键值数据对信息
        /// </summary>
        /// <returns></returns>
        public override List<NodeClassRenderItem> GetNodeClassRenders()
        {
            List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
            nodeClassRenders.Add(new NodeClassRenderItem("IP地址", this.IpAddress));
            nodeClassRenders.Add(new NodeClassRenderItem("端口号", this.Port.ToString()));
            nodeClassRenders.Add(new NodeClassRenderItem("VD电源型号", this.VDType));
            nodeClassRenders.Add(new NodeClassRenderItem("激活情况", base.IsActive ? "已激活" : "未激活"));
            nodeClassRenders.Add(new NodeClassRenderItem("连接情况", this.IsConnected ? "已连接" : "未连接"));
            return nodeClassRenders;
        }
        /// <summary>
        /// 开启线程
        /// </summary>
        public void Start()
        {
            foreach (VDGroup vdgroup in this.CustomGroupList)
            {
                foreach (VDVariable vdvariable in vdgroup.varList)
                {
                    if (vdvariable.Config.ArchiveEnable)
                        this.StoreVarList.Add(vdvariable);
                    if (this.CurrentVarList.ContainsKey(vdvariable.KeyName))
                        this.CurrentVarList[vdvariable.KeyName] = vdvariable;
                    else
                        this.CurrentVarList.Add(vdvariable.KeyName, vdvariable);
                }
            }
            this.cts = new CancellationTokenSource();
            Task.Run(new Action(this.GetValue), this.cts.Token);
        }
        /// <summary>
        /// 停止线程
        /// </summary>
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
                    VDState currentStatus = this.vd.GetCurrentStatus();
                    if (currentStatus != null)
                    {
                        using (List<VDGroup>.Enumerator enumerator = this.CustomGroupList.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                VDGroup vdgroup = enumerator.Current;
                                foreach (VDVariable vdvariable in vdgroup.varList)
                                {
                                    string text = vdvariable.VarAddress.ToLower();
                                    string text2 = text;
                                    uint num = PrivateImplementationDetails.ComputeStringHash(text2);
                                    if (num <= 1479788386U)
                                    {
                                        if (num <= 557985109U)
                                        {
                                            if (num <= 79371175U)
                                            {
                                                if (num != 35244351U)
                                                {
                                                    if (num == 79371175U)
                                                        if (text2 == "externalerror1")
                                                            vdvariable.Value = currentStatus.Externalerror1;
                                                }
                                                else if (text2 == "arcplus")
                                                {
                                                    vdvariable.Value = currentStatus.ARCPlus;
                                                }
                                            }
                                            else if (num != 96148794U)
                                            {
                                                if (num != 355442229U)
                                                {
                                                    if (num == 557985109U)
                                                        if (text2 == "endpointurl")
                                                            vdvariable.Value = this.IpAddress;
                                                }
                                                else if (text2 == "regulationerror")
                                                {
                                                    vdvariable.Value = currentStatus.Regulationerror;
                                                }
                                            }
                                            else if (text2 == "externalerror2")
                                            {
                                                vdvariable.Value = currentStatus.Externalerror2;
                                            }
                                        }
                                        else if (num <= 852568230U)
                                        {
                                            if (num != 661984850U)
                                            {
                                                if (num != 717240056U)
                                                {
                                                    if (num == 852568230U)
                                                        if (text2 == "powerpolarity")
                                                            vdvariable.Value = currentStatus.PowerPolarity;
                                                }
                                                else if (text2 == "negativevoltage")
                                                    vdvariable.Value = currentStatus.NegativeVoltage;
                                            }
                                            else if (text2 == "hvsocket")
                                            {
                                                vdvariable.Value = currentStatus.HVSocket;
                                            }
                                        }
                                        else if (num != 1270093079U)
                                        {
                                            if (num != 1474661871U)
                                            {
                                                if (num == 1479788386U)
                                                    if (text2 == "outvoltagesetback")
                                                        vdvariable.Value = currentStatus.OutVoltage;
                                            }
                                            else if (text2 == "overvoltage")
                                                vdvariable.Value = currentStatus.OverVoltage;
                                        }
                                        else if (text2 == "endpointport")
                                            vdvariable.Value = this.Port;
                                    }
                                    else if (num <= 3278035522U)
                                    {
                                        if (num <= 1927901941U)
                                        {
                                            if (num != 1639973432U)
                                            {
                                                if (num != 1796476105U)
                                                {
                                                    if (num == 1927901941U)
                                                        if (text2 == "dooropen")
                                                            vdvariable.Value = currentStatus.Dooropen;
                                                }
                                                else if (text2 == "dcpower")
                                                    vdvariable.Value = currentStatus.DCPower;
                                            }
                                            else if (text2 == "positivevoltage")
                                                vdvariable.Value = currentStatus.PositiveVoltage;
                                        }
                                        else if (num != 2314006659U)
                                        {
                                            if (num != 3103597448U)
                                            {
                                                if (num == 3278035522U)
                                                    if (text2 == "remotelocal")
                                                        vdvariable.Value = currentStatus.Remotelocal;
                                            }
                                            else if (text2 == "inrush")
                                                vdvariable.Value = currentStatus.Inrush;
                                        }
                                        else if (text2 == "connectionflag")
                                        {
                                            vdvariable.Value = this.IsConnected;
                                        }
                                    }
                                    else if (num <= 4156994128U)
                                    {
                                        if (num != 3306052808U)
                                        {
                                            if (num != 3377248321U)
                                            {
                                                if (num == 4156994128U)
                                                    if (text2 == "outcurrent")
                                                        vdvariable.Value = currentStatus.OutCurrent;
                                            }
                                            else if (text2 == "arcminus")
                                                vdvariable.Value = currentStatus.ARCMinus;
                                        }
                                        else if (text2 == "powerstatus")
                                            vdvariable.Value = currentStatus.PowerStatus;
                                    }
                                    else if (num != 4187136672U)
                                    {
                                        if (num != 4200571092U)
                                        {
                                            if (num == 4202597758U)
                                                if (text2 == "phaseerror")
                                                    vdvariable.Value = currentStatus.Phaseerror;
                                        }
                                        else if (text2 == "temperaturemodule1")
                                            vdvariable.Value = currentStatus.TemperatureModule1;
                                    }
                                    else if (text2 == "overpower")
                                        vdvariable.Value = currentStatus.OverPower;
                                    vdvariable.Value = MigrationLib.GetMigrationValue(vdvariable.Value, vdvariable.Scale, vdvariable.Offset);
                                    base.UpdateCurrentValue(vdvariable);
                                }
                            }
                            continue;
                        }
                    }
                    this.IsConnected = false;
                }
                else
                {
                    if (!this.FirstConnect)
                    {
                        Thread.Sleep(base.ReConnectTime);
                        VD vd = this.vd;
                        if (vd != null)
                            vd.DisConnect();
                    }
                    this.vd = new VD();
                    this.IsConnected = this.vd.Connect(this.IpAddress, this.Port);
                }
            }
        }
        /// <summary>
        /// 通用数据写入
        /// </summary>
        /// <param name="keyName">变量键名称</param>
        /// <param name="setValue">设定值</param>
        /// <returns></returns>
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
                VDVariable vdvariable = this.CurrentVarList[keyName] as VDVariable;
                CalResult<string> xktResult = Common.VerifyInputValue(vdvariable, vdvariable.VarType, setValue);
                if (xktResult.IsSuccess)
                {
                    string text = vdvariable.VarAddress.ToLower();
                    string text2 = text;
                    uint num = PrivateImplementationDetails.ComputeStringHash(text2);
                    if (num <= 1909160276U)
                    {
                        if (num <= 1314246537U)
                        {
                            if (num != 483773830U)
                            {
                                if (num == 1314246537U)
                                {
                                    if (text2 == "resetcommandflow")
                                        return new CalResult
                                        {
                                            IsSuccess = this.vd.ResetSTA()
                                        };
                                }
                            }
                            else if (text2 == "reseterror")
                            {
                                return new CalResult
                                {
                                    IsSuccess = this.vd.Reset()
                                };
                            }
                        }
                        else if (num != 1750757060U)
                        {
                            if (num == 1909160276U)
                            {
                                if (text2 == "resetarccounter")
                                    return new CalResult
                                    {
                                        IsSuccess = this.vd.ResetARC()
                                    };
                            }
                        }
                        else if (text2 == "setpositive")
                        {
                            return new CalResult
                            {
                                IsSuccess = this.vd.SetPOL(true)
                            };
                        }
                    }
                    else if (num <= 3284990853U)
                    {
                        if (num != 2683918925U)
                        {
                            if (num == 3284990853U)
                            {
                                if (text2 == "outvoltageset")
                                {
                                    this.vd.VoltageSet = float.Parse(xktResult.Content);
                                    return CalResult.CreateSuccessResult();
                                }
                            }
                        }
                        else if (text2 == "setpara")
                        {
                            return new CalResult
                            {
                                IsSuccess = this.vd.SetCur()
                            };
                        }
                    }
                    else if (num != 3389644492U)
                    {
                        if (num == 3810347193U)
                        {
                            if (text2 == "outputcontrol")
                                return new CalResult
                                {
                                    IsSuccess = this.vd.SetDCP(xktResult.Content == "1" || xktResult.Content == "true")
                                };
                        }
                    }
                    else if (text2 == "setnegative")
                    {
                        return new CalResult
                        {
                            IsSuccess = this.vd.SetPOL(false)
                        };
                    }
                    result = CalResult.CreateFailedResult();
                }
                else
                    result = xktResult;
            }
            return result;
        }
        /// <summary>
        /// 定义一个信号源
        /// </summary>
        public CancellationTokenSource cts;
        /// <summary>
        /// 计时器
        /// </summary>
        public Stopwatch sw;
        /// <summary>
        /// SG类型
        /// </summary>
        public const string SGType = "SigMaphi";
        /// <summary>
        /// VD通信对象
        /// </summary>
        public VD vd;
        /// <summary>
        /// 自定义组集合
        /// </summary>
        public List<VDGroup> CustomGroupList;
    }
}
