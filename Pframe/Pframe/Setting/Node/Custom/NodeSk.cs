using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Text;

namespace NodeSettings.Node.Custom
{
    /// <summary>
    /// ESD节点
    /// </summary>
	public class NodeSk : CustomNode, IXmlConvert
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public NodeSk()
        {
            this.sw = new Stopwatch();
            this.CustomGroupList = new List<SkGroup>();
            Name = "ESD电源";
            base.Description = "SC200引出区电源";
            base.CustomType = 50000;
            this.IpAddress = "192.168.0.3";
            this.Port = 8080;
            FirstConnect = true;
            EncodeMode = Encoding.Default;
            UpdateRate = 250;
        }
        /// <summary>
        /// 第一次连接
        /// </summary>
		public bool FirstConnect { get; set; }
        /// <summary>
        /// 更新速率
        /// </summary>
        public int UpdateRate { get; set; }
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
        /// 编码格式
        /// </summary>
        public Encoding EncodeMode { get; set; }
        /// <summary>
        /// 协议类型
        /// </summary>
        public SkProtocol ProtocolClass { get; set; }

        /// <summary>
        /// 连接状态
        /// </summary>
		public bool IsConnected { get; set; }
        /// <summary>
        /// 加载Xml元素
        /// </summary>
        /// <param name="element"></param>
		public override void LoadByXmlElement(XElement element)
        {
            LoadByXmlElement(element);
            this.IpAddress = element.Attribute("IpAddress").Value;
            this.Port = int.Parse(element.Attribute("Port").Value);
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
            nodeClassRenders.Add(new NodeClassRenderItem("激活情况", base.IsActive ? "已激活" : "未激活"));
            nodeClassRenders.Add(new NodeClassRenderItem("连接情况", this.IsConnected ? "已连接" : "未连接"));
            return nodeClassRenders;
        }
        /// <summary>
        /// 开启线程
        /// </summary>
		public void Start()
        {
            foreach (SkGroup skgroup in this.CustomGroupList)
            {
                if (skgroup.IsActive)
                    foreach (SKVariable variable in skgroup.varList)
                    {
                        if (variable.Config.ArchiveEnable)
                            if (!StoreVarList.Contains(variable))
                                this.StoreVarList.Add(variable);
                        if (this.CurrentVarList.ContainsKey(variable.KeyName))
                            this.CurrentVarList[variable.KeyName] = variable;
                        else
                            this.CurrentVarList.Add(variable.KeyName, variable);
                        if (this.CurrentValue.ContainsKey(variable.KeyName))
                            this.CurrentValue[variable.KeyName] = "NA";
                        else
                            this.CurrentValue.Add(variable.KeyName, "NA");
                    }
            }
            if (this.ProtocolClass == SkProtocol.Recive)
            {
                dics.Clear();
                dics.Add("counter", "0");
                dics.Add("weight", "0");
                dics.Add("result", "0");
                dics.Add("codecontent", "0");
            }
            this.cts = new CancellationTokenSource();
            FirstConnect = true;
            Task.Run(new Action(this.GetValue), this.cts.Token);
        }
        /// <summary>
        /// 停止线程
        /// </summary>
		public void Stop()
        {
            if (sk != null)
                sk.isStop = true;
            if (cts != null)
                this.cts.Cancel();
        }

        byte[] reciveBytes = null;
        /// <summary>
        /// 扫描变量
        /// </summary>
        private void GetValue()
        {
            if (this.ProtocolClass == SkProtocol.Recive)
                reciveBytes = new byte[200];
            while (!this.cts.IsCancellationRequested)
            {
                if (this.IsConnected)
                {
                    #region 接收模式
                    if (this.ProtocolClass == SkProtocol.Recive)
                    {
                        byte[] result = sk.Recive(reciveBytes);

                        #region 读取成功
                        if (result != null)
                        {
                            #region 解析返回值是否正确
                            if (!ResultAnalysis(result))
                            {
                                ErrorTimes++;
                                continue;
                            }
                            #endregion

                            foreach (SkGroup item in CustomGroupList)
                            {
                                if (ErrorTimes > MaxErrorTimes)
                                    break;
                                if (item.IsActive && item.varList.Count > 0)
                                {
                                    ErrorTimes = 0;
                                    foreach (SKVariable varable in item.varList)
                                    {
                                        #region 检测返回值是否包含此变量
                                        if (!dics.ContainsKey(varable.VarAddress.ToLower()))
                                            continue;
                                        #endregion

                                        switch (varable.VarType)
                                        {
                                            case DataType.Bool:
                                                varable.Value = (dics[varable.VarAddress.ToLower()] == "1" || dics[varable.VarAddress.ToLower()].ToLower() == "true");
                                                break;
                                            case DataType.Byte:
                                            case DataType.Short:
                                            case DataType.UShort:
                                                varable.Value = Convert.ToUInt16(dics[varable.VarAddress.ToLower()]);
                                                break;
                                            case DataType.Int:
                                            case DataType.UInt:
                                                varable.Value = Convert.ToUInt32(dics[varable.VarAddress.ToLower()]);
                                                break;
                                            case DataType.Float:
                                                varable.Value = Convert.ToSingle(dics[varable.VarAddress.ToLower()]);
                                                break;
                                            case DataType.Double:
                                            case DataType.Long:
                                            case DataType.ULong:
                                            case DataType.String:
                                                varable.Value = dics[varable.VarAddress.ToLower()];
                                                break;
                                            case DataType.ByteArray:
                                                break;
                                            case DataType.HexString:
                                                break;
                                        }
                                        varable.Value = MigrationLib.GetMigrationValue(varable.Value, varable.Scale, varable.Offset);
                                        UpdateCurrentValue(varable);
                                    }
                                }
                            }
                        }
                        #endregion

                        #region 读取失败
                        else
                        {
                            ErrorTimes++;
                            if (ErrorTimes > MaxErrorTimes)
                            {
                                IsConnected = false;
                                continue;
                            }
                        }
                        #endregion
                    }
                    #endregion

                    #region 问答模式
                    else
                    {
                        this.sw.Restart();
                        Thread.Sleep(this.UpdateRate);
                        foreach (SkGroup item in CustomGroupList)
                        {
                            if (ErrorTimes > MaxErrorTimes)
                                break;
                            if (item.IsActive && item.varList.Count > 0)
                            {
                                #region 根据不同协议类型解析对象
                                switch (ProtocolClass)
                                {
                                    case SkProtocol.Custom:

                                        #region 自定义协议一问多答
                                        byte[] result = sk.Read(item.Code, EncodeMode);

                                        #region 读取成功
                                        if (result != null)
                                        {
                                            #region 解析返回值是否正确
                                            if (!ResultAnalysis(result))
                                                continue;
                                            #endregion

                                            ErrorTimes = 0;
                                            foreach (SKVariable varable in item.varList)
                                            {
                                                #region 检测返回值是否包含此变量
                                                if (!dics.ContainsKey(varable.VarAddress.ToLower()))
                                                    continue;
                                                #endregion

                                                switch (varable.VarType)
                                                {
                                                    case DataType.Bool:
                                                        varable.Value = (dics[varable.VarAddress.ToLower()] == "1" || dics[varable.VarAddress.ToLower()].ToLower() == "true");
                                                        break;
                                                    case DataType.Byte:
                                                    case DataType.Short:
                                                    case DataType.UShort:
                                                    case DataType.Int:
                                                    case DataType.UInt:
                                                    case DataType.Float:
                                                    case DataType.Double:
                                                    case DataType.Long:
                                                    case DataType.ULong:
                                                    case DataType.String:
                                                        varable.Value = dics[varable.VarAddress.ToLower()];
                                                        break;
                                                    case DataType.ByteArray:
                                                        break;
                                                    case DataType.HexString:
                                                        break;
                                                }
                                                varable.Value = MigrationLib.GetMigrationValue(varable.Value, varable.Scale, varable.Offset);
                                                UpdateCurrentValue(varable);
                                            }
                                        }
                                        #endregion

                                        #region 读取失败
                                        else
                                        {
                                            ErrorTimes++;
                                            if (ErrorTimes > MaxErrorTimes)
                                            {
                                                sw.Stop();
                                                IsConnected = false;
                                                continue;
                                            }
                                        }
                                        #endregion

                                        #endregion

                                        break;
                                    case SkProtocol.Standard:
                                        bool readIsSucc = false;

                                        #region 自定义协议一问一答
                                        foreach (SKVariable varable in item.varList)
                                        {
                                            byte[] result1 = sk.Read(varable.Code, EncodeMode);

                                            #region 读取成功
                                            if (result1 != null)
                                            {
                                                ErrorTimes = 0;
                                                varable.Value = EncodeMode.GetString(result1);
                                                varable.Value = MigrationLib.GetMigrationValue(varable.Value, varable.Scale, varable.Offset);
                                                UpdateCurrentValue(varable);
                                            }
                                            #endregion

                                            #region 读取失败
                                            else
                                            {
                                                ErrorTimes++;
                                                if (ErrorTimes >= MaxErrorTimes)
                                                {
                                                    sw.Stop();
                                                    IsConnected = false;
                                                    readIsSucc = true;
                                                    break;
                                                }
                                            }
                                            #endregion
                                        }
                                        if (readIsSucc)
                                            continue;
                                        #endregion

                                        break;
                                }
                                #endregion
                            }
                        }
                        this.CommRate = this.sw.ElapsedMilliseconds;
                    }
                    #endregion
                }
                else
                {
                    if (!this.FirstConnect)
                    {
                        ErrorTimes = 0;
                        Thread.Sleep(base.ReConnectTime);
                        sk?.DisConnect();
                    }
                    if (FirstConnect) FirstConnect = !FirstConnect;
                    sk = new Sk();
                    IsConnected = sk.Connect(this.IpAddress, this.Port);
                }
            }
            sk?.DisConnect();
            IsConnected = false;
            sw.Reset();
        }
        /// <summary>
        /// 带有错误信息的返回值
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="setValue"></param>
        /// <returns></returns>
        public CalResult Write(string keyName, string setValue = "")
        {
            if (IsConnected)
                return sk?.Write(EncodeMode.GetBytes(keyName));
            else
                return new CalResult() { Message = "未连接到从站！", IsSuccess = false };
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
        /// GM类型
        /// </summary>
        public const string GMType = "Glassman";
        /// <summary>
        /// 通信对象
        /// </summary>
        public Sk sk;
        /// <summary>
        ///  自定义组集合
        /// </summary>
        public List<SkGroup> CustomGroupList;
        /// <summary>
        /// 对象解析结果
        /// </summary>
        private Dictionary<string, string> dics = new Dictionary<string, string>();
        /// <summary>
        /// 结果解析
        /// </summary>
        /// <param name="bytes">需要解析的字节数组</param>
        /// <returns></returns>
        private bool ResultAnalysis(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return false;
            if (ProtocolClass != SkProtocol.Recive)
            {
                string[] resultStr = this.EncodeMode.GetString(bytes).Split('|');
                dics.Clear();
                foreach (string item in resultStr)
                {
                    string[] varResult = item.Split(':');
                    if (varResult.Length == 1)
                        return false;
                    dics.Add(varResult[0].ToLower(), varResult[1]);
                }
            }
            else
            {
                string[] resultStr = this.EncodeMode.GetString(bytes).Split(',');
                if (resultStr.Length < 3)
                    return false;
                dics["counter"] = resultStr[0];
                dics["weight"] = resultStr[1];
                dics["result"] = resultStr[2];
                dics["codecontent"] = resultStr[3].Length > 5 ? resultStr[3] : "";
            }
            return true;
        }
    }
    /// <summary>
    /// 协议类型
    /// </summary>
    public enum SkProtocol
    {
        /// <summary>
        /// 自定义协议
        /// </summary>
        Custom,
        /// <summary>
        /// 标准的一问一答
        /// </summary>
        Standard,
        /// <summary>
        /// 接收模式
        /// </summary>
        Recive
    }
}
