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
using Pframe.PLC.Melsec;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Device
{
    /// <summary>
    /// NodeMelsec节点对象
    /// </summary>
    public class NodeMelsec : DeviceNode, IXmlConvert
    {
        public NodeMelsec()
        {
            this.sw = new Stopwatch();
            this.DeviceGroupList = new List<MelsecDeviceGroup>();
            base.Name = "三菱PLC";
            base.Description = "锅炉系统1#PLC";
            base.DeviceType = 10;
            this.IpAddress = "192.168.0.3";
            this.Port = 4096;
            this.PlcType = MelsecProtocol.MCBinary;
            this.IsFX5UMC = false;
            this.FirstConnect = true;
        }
        /// <summary>
        /// 第一次连接
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
        /// 三菱协议
        /// </summary>
        public MelsecProtocol PlcType { get; set; }
        /// <summary>
        /// 是否为FX5U
        /// </summary>
        public bool IsFX5UMC { get; set; }
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool IsConnected { get; set; }
        /// <summary>
        ///  从XML元素对象中获取对象属性
        /// </summary>
        /// <param name="element"></param>
        public override void LoadByXmlElement(XElement element)
        {
            base.LoadByXmlElement(element);
            this.IpAddress = element.Attribute("IpAddress").Value;
            this.Port = int.Parse(element.Attribute("Port").Value);
            this.IsFX5UMC = bool.Parse(element.Attribute("IsFX5UMC").Value);
            this.PlcType = (MelsecProtocol)Enum.Parse(typeof(MelsecProtocol), element.Attribute("PlcType").Value, true);
        }

        /// <summary>
        /// 将对象属性保存至XML元素对象
        /// </summary>
        /// <returns></returns>
        public override XElement ToXmlElement()
        {
            XElement xelement = base.ToXmlElement();
            xelement.SetAttributeValue("IpAddress", this.IpAddress);
            xelement.SetAttributeValue("Port", this.Port);
            xelement.SetAttributeValue("IsFX5UMC", this.IsFX5UMC);
            xelement.SetAttributeValue("PlcType", this.PlcType);
            return xelement;
        }

        /// <summary>
        ///  获取用于在数据表信息中显示的键值数据对信息
        /// </summary>
        /// <returns></returns>
        public override List<NodeClassRenderItem> GetNodeClassRenders()
        {
            List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
            nodeClassRenders.Add(new NodeClassRenderItem("IP地址", this.IpAddress));
            nodeClassRenders.Add(new NodeClassRenderItem("端口号", this.Port.ToString()));
            nodeClassRenders.Add(new NodeClassRenderItem("协议类型", this.PlcType.ToString()));
            nodeClassRenders.Add(new NodeClassRenderItem("是否为FX5U", this.IsFX5UMC ? "是" : "否"));
            nodeClassRenders.Add(new NodeClassRenderItem("激活情况", base.IsActive ? "已激活" : "未激活"));
            nodeClassRenders.Add(new NodeClassRenderItem("连接情况", this.IsConnected ? "已连接" : "未连接"));
            if (this.IsConnected)
            {
                nodeClassRenders.Add(new NodeClassRenderItem("通信周期", this.CommRate.ToString() + "ms"));
            }
            return nodeClassRenders;
        }

        /// <summary>
        /// 开启线程
        /// </summary>
        public void Start()
        {
            foreach (MelsecDeviceGroup melsecDeviceGroup in this.DeviceGroupList)
            {
                foreach (MelsecVariable melsecVariable in melsecDeviceGroup.varList)
                {
                    if (melsecVariable.Config.ArchiveEnable)
                        this.StoreVarList.Add(melsecVariable);
                    if (this.CurrentVarList.ContainsKey(melsecVariable.KeyName))
                        this.CurrentVarList[melsecVariable.KeyName] = melsecVariable;
                    else
                        this.CurrentVarList.Add(melsecVariable.KeyName, melsecVariable);
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
                    this.sw.Restart();

                    #region 读取通信组
                    using (List<MelsecDeviceGroup>.Enumerator enumerator = this.DeviceGroupList.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            MelsecDeviceGroup melsecDeviceGroup = enumerator.Current;
                            if (melsecDeviceGroup.IsActive)
                            {
                                bool isA1E = this.PlcType == MelsecProtocol.MCA1E;
                                switch (melsecDeviceGroup.StoreArea)
                                {
                                    case MelsecStoreArea.M存储区:
                                    case MelsecStoreArea.X存储区:
                                    case MelsecStoreArea.Y存储区:
                                    case MelsecStoreArea.L存储区:
                                    case MelsecStoreArea.S存储区:
                                    case MelsecStoreArea.B存储区:
                                        #region 读取BOOL类型
                                        {
                                            byte[] array = this.melsec.ReadBytes(melsecDeviceGroup.Start, Convert.ToUInt16(melsecDeviceGroup.Length));
                                            if (array != null && array.Length >= melsecDeviceGroup.Length)
                                            {
                                                ErrorTimes = 0;
                                                int melsecStart = this.GetMelsecStart(isA1E, melsecDeviceGroup.Start.Substring(1), melsecDeviceGroup.StoreArea, this.IsFX5UMC);
                                                using (List<MelsecVariable>.Enumerator enumerator2 = melsecDeviceGroup.varList.GetEnumerator())
                                                {
                                                    while (enumerator2.MoveNext())
                                                    {
                                                        MelsecVariable melsecVariable = enumerator2.Current;
                                                        int num;
                                                        int num2;
                                                        if (this.VerifyMelsecAddress(true, isA1E, melsecDeviceGroup.StoreArea, melsecVariable.Start, this.IsFX5UMC, out num, out num2))
                                                        {
                                                            num -= melsecStart;
                                                            if (melsecVariable.VarType == DataType.Bool)
                                                                melsecVariable.Value = (ByteLib.GetByteFromByteArray(array, num) == 1);
                                                            UpdateCurrentValue(melsecVariable);
                                                        }
                                                    }
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                ErrorTimes++;
                                                if (ErrorTimes >= MaxErrorTimes)
                                                    this.IsConnected = false;
                                                break;
                                            }
                                        }
                                    #endregion
                                    case MelsecStoreArea.D存储区:
                                    case MelsecStoreArea.W存储区:
                                        #region 读取其它类型
                                        {
                                            byte[] array = this.melsec.ReadBytes(melsecDeviceGroup.Start, Convert.ToUInt16(melsecDeviceGroup.Length));
                                            if (array != null && array.Length == melsecDeviceGroup.Length * 2)
                                            {
                                                base.ErrorTimes = 0;
                                                int melsecStart2 = this.GetMelsecStart(isA1E, melsecDeviceGroup.Start.Substring(1), melsecDeviceGroup.StoreArea, this.IsFX5UMC);
                                                using (List<MelsecVariable>.Enumerator enumerator3 = melsecDeviceGroup.varList.GetEnumerator())
                                                {
                                                    while (enumerator3.MoveNext())
                                                    {
                                                        MelsecVariable melsecVariable2 = enumerator3.Current;
                                                        int num3;
                                                        int num4;
                                                        if (this.VerifyMelsecAddress(false, isA1E, melsecDeviceGroup.StoreArea, melsecVariable2.Start, this.IsFX5UMC, out num3, out num4))
                                                        {
                                                            num3 -= melsecStart2;
                                                            num3 *= 2;
                                                            switch (melsecVariable2.VarType)
                                                            {
                                                                case DataType.Bool:
                                                                    melsecVariable2.Value = BitLib.GetBitFrom2ByteArray(array, num3, num4, false);
                                                                    break;
                                                                case DataType.Short:
                                                                    melsecVariable2.Value = ShortLib.GetShortFromByteArray(array, num3, this.melsec.DataFormat);
                                                                    break;
                                                                case DataType.UShort:
                                                                    melsecVariable2.Value = UShortLib.GetUShortFromByteArray(array, num3, this.melsec.DataFormat);
                                                                    break;
                                                                case DataType.Int:
                                                                    melsecVariable2.Value = IntLib.GetIntFromByteArray(array, num3, this.melsec.DataFormat);
                                                                    break;
                                                                case DataType.UInt:
                                                                    melsecVariable2.Value = UIntLib.GetUIntFromByteArray(array, num3, this.melsec.DataFormat);
                                                                    break;
                                                                case DataType.Float:
                                                                    melsecVariable2.Value = FloatLib.GetFloatFromByteArray(array, num3, this.melsec.DataFormat);
                                                                    break;
                                                                case DataType.Double:
                                                                    melsecVariable2.Value = DoubleLib.GetDoubleFromByteArray(array, num3, this.melsec.DataFormat);
                                                                    break;
                                                                case DataType.String:
                                                                    melsecVariable2.Value = StringLib.GetStringFromByteArray(array, num3, num4 * 2, Encoding.ASCII);
                                                                    break;
                                                                case DataType.ByteArray:
                                                                    melsecVariable2.Value = ByteArrayLib.GetByteArray(array, num3, num4 * 2);
                                                                    break;
                                                                case DataType.HexString:
                                                                    melsecVariable2.Value = StringLib.GetHexStringFromByteArray(array, num3, num4 * 2, ' ');
                                                                    break;
                                                            }
                                                            melsecVariable2.Value = MigrationLib.GetMigrationValue(melsecVariable2.Value, melsecVariable2.Scale, melsecVariable2.Offset);
                                                            UpdateCurrentValue(melsecVariable2);
                                                        }
                                                    }
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                ErrorTimes++;
                                                if (ErrorTimes >= MaxErrorTimes)
                                                    this.IsConnected = false;
                                                break;
                                            }
                                        }
                                        #endregion
                                }
                                this.CommRate = this.sw.ElapsedMilliseconds;
                            }
                        }
                        continue;
                    }
                    #endregion
                }
                else
                {
                    if (!this.FirstConnect)
                    {
                        Thread.Sleep(base.ReConnectTime);
                        this.melsec.DisConnect();
                    }
                    this.melsec = new Melsec(this.GetType(this.PlcType), this.IsFX5UMC, DataFormat.DCBA);
                    this.melsec.ConnectTimeOut = base.ConnectTimeOut;
                    this.IsConnected = this.melsec.Connect(this.IpAddress, this.Port);
                    this.FirstConnect = false;
                }
            }
            sw.Reset();
        }
        private MelsecProtocolType GetType(MelsecProtocol _type)
        {
            MelsecProtocolType result;
            switch (_type)
            {
                case MelsecProtocol.MCBinary:
                    result = MelsecProtocolType.MCBinary;
                    break;
                case MelsecProtocol.MCASCII:
                    result = MelsecProtocolType.MCASCII;
                    break;
                case MelsecProtocol.MCA1E:
                    result = MelsecProtocolType.MCA1E;
                    break;
                default:
                    result = MelsecProtocolType.MCBinary;
                    break;
            }
            return result;
        }

        /// <summary>
        /// 获取三菱PLC起始地址
        /// </summary>
        /// <param name="isA1E">是否是3U</param>
        /// <param name="start">起始地址</param>
        /// <param name="store">存储区</param>
        /// <param name="isFX5U">是否为5U</param>
        /// <returns></returns>
        public int GetMelsecStart(bool isA1E, string start, MelsecStoreArea store, bool isFX5U)
        {
            int result;
            if (isA1E)
            {
                switch (store)
                {
                    case MelsecStoreArea.M存储区:
                        result = Convert.ToInt32(start, MelsecA1EDataType.M.FromBase);
                        break;
                    case MelsecStoreArea.X存储区:
                        result = Convert.ToInt32(start, MelsecA1EDataType.X.FromBase);
                        break;
                    case MelsecStoreArea.Y存储区:
                        result = Convert.ToInt32(start, MelsecA1EDataType.Y.FromBase);
                        break;
                    case MelsecStoreArea.D存储区:
                        result = Convert.ToInt32(start, MelsecA1EDataType.D.FromBase);
                        break;
                    case MelsecStoreArea.L存储区:
                    case MelsecStoreArea.B存储区:
                    case MelsecStoreArea.W存储区:
                        result = 0;
                        break;
                    case MelsecStoreArea.S存储区:
                        result = Convert.ToInt32(start, MelsecA1EDataType.S.FromBase);
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
                    case MelsecStoreArea.M存储区:
                        result = Convert.ToInt32(start, MelsecMcDataType.M.FromBase);
                        break;
                    case MelsecStoreArea.X存储区:
                        result = (isFX5U ? Convert.ToInt32(start, MelsecMcDataType.X5U.FromBase) : Convert.ToInt32(start, MelsecMcDataType.X.FromBase));
                        break;
                    case MelsecStoreArea.Y存储区:
                        result = (isFX5U ? Convert.ToInt32(start, MelsecMcDataType.Y5U.FromBase) : Convert.ToInt32(start, MelsecMcDataType.Y.FromBase));
                        break;
                    case MelsecStoreArea.D存储区:
                        result = Convert.ToInt32(start, MelsecMcDataType.D.FromBase);
                        break;
                    case MelsecStoreArea.L存储区:
                        result = Convert.ToInt32(start, MelsecMcDataType.L.FromBase);
                        break;
                    case MelsecStoreArea.S存储区:
                        result = Convert.ToInt32(start, MelsecMcDataType.S.FromBase);
                        break;
                    case MelsecStoreArea.B存储区:
                        result = Convert.ToInt32(start, MelsecMcDataType.B.FromBase);
                        break;
                    case MelsecStoreArea.W存储区:
                        result = Convert.ToInt32(start, MelsecMcDataType.W.FromBase);
                        break;
                    default:
                        result = 0;
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 验证三菱地址并解析
        /// </summary>
        /// <param name="isBoolStore">是否为触电或线圈</param>
        /// <param name="isA1E">是否为3U</param>
        /// <param name="store">存储区</param>
        /// <param name="address">地址</param>
        /// <param name="isFX5U">是否为5U</param>
        /// <param name="start">开始地址</param>
        /// <param name="offset">偏移</param>
        /// <returns></returns>
        public bool VerifyMelsecAddress(bool isBoolStore, bool isA1E, MelsecStoreArea store, string address, bool isFX5U, out int start, out int offset)
        {
            bool result;
            if (isBoolStore)
            {
                offset = 0;
                start = 0;
                try
                {
                    start = this.GetMelsecStart(isA1E, address, store, isFX5U);
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
                        start = this.GetMelsecStart(isA1E, array[0], store, isFX5U);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                    try
                    {
                        offset = Convert.ToInt32(array[1], 16);
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
                start = this.GetMelsecStart(isA1E, address, store, isFX5U);
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 通用数据写入
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="setValue"></param>
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
                MelsecVariable melsecVariable = this.CurrentVarList[keyName] as MelsecVariable;
                CalResult<string> xktResult = Common.VerifyInputValue(melsecVariable, melsecVariable.VarType, setValue);
                result = xktResult.IsSuccess ? melsec.Write(melsecVariable.VarAddress, xktResult.Content, melsecVariable.VarType) : xktResult;
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
        /// MelsecDeviceGroupList组集合
        /// </summary>
        /// 
        public List<MelsecDeviceGroup> DeviceGroupList;

        /// <summary>
        /// Melsec通信对象
        /// </summary>
        public Melsec melsec;
    }
}
