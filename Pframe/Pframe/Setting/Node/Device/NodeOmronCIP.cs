using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Pframe;
using Pframe.Common;
using Pframe.DataConvert;
using Pframe.PLC.Omron;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Device
{
    public class NodeOmronCIP : DeviceNode, IXmlConvert
    {
        /// <summary>
        /// NodeOmronCIP节点对象
        /// </summary>
        public NodeOmronCIP()
        {
            this.sw = new Stopwatch();
            this.DeviceGroupList = new List<OmronCIPDeviceGroup>();
            this.Slot = 0;
            Name = "欧姆龙PLC";
            Description = "真空系统1#PLC";
            DeviceType = 160;
            this.IpAddress = "192.168.1.14";
            this.Port = 44818;
            this.FirstConnect = true;
            this.IsConnected = false;
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
        /// 连接状态
        /// </summary>
        public bool IsConnected { get; set; }
        /// <summary>
        /// 插槽号
        /// </summary>
        public byte Slot { get; set; }
        /// <summary>
        /// 从XML元素对象中获取对象属性
        /// </summary>
        /// <param name="element"></param>
        public override void LoadByXmlElement(XElement element)
        {
            base.LoadByXmlElement(element);
            this.IpAddress = element.Attribute("IpAddress").Value;
            this.Port = int.Parse(element.Attribute("Port").Value);
            this.Slot = Convert.ToByte(element.Attribute("Slot").Value);
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
            xelement.SetAttributeValue("Slot", this.Slot);
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
            nodeClassRenders.Add(new NodeClassRenderItem("插槽号", this.Slot.ToString()));
            nodeClassRenders.Add(new NodeClassRenderItem("激活情况", base.IsActive ? "已激活" : "未激活"));
            nodeClassRenders.Add(new NodeClassRenderItem("连接情况", this.IsConnected ? "已连接" : "未连接"));
            if (this.IsConnected)
            {
                nodeClassRenders.Add(new NodeClassRenderItem("通信周期", this.CommRate.ToString() + "ms"));
            }
            return nodeClassRenders;
        }
        /// <summary>
        /// 开启线程扫描
        /// </summary>
        public void Start()
        {
            foreach (OmronCIPDeviceGroup gclass in this.DeviceGroupList)
            {
                foreach (OmronCIPVariable omronCIPVariable in gclass.varList)
                {
                    if (omronCIPVariable.Config.ArchiveEnable)
                        this.StoreVarList.Add(omronCIPVariable);
                    if (this.CurrentVarList.ContainsKey(omronCIPVariable.KeyName))
                        this.CurrentVarList[omronCIPVariable.KeyName] = omronCIPVariable;
                    else
                        this.CurrentVarList.Add(omronCIPVariable.KeyName, omronCIPVariable);
                    if (this.CurrentValue.ContainsKey(omronCIPVariable.KeyName))
                        this.CurrentValue[omronCIPVariable.KeyName] = "NA";
                    else
                        this.CurrentValue.Add(omronCIPVariable.KeyName, "NA");
                }
            }
            FirstConnect = true;
            this.cts = new CancellationTokenSource();
            Task.Run(new Action(this.GetValue), this.cts.Token);
        }
        /// <summary>
        /// 停止线程扫描
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

                    #region 遍历节点对象组
                    foreach (OmronCIPDeviceGroup gclass in this.DeviceGroupList)
                    {
                        if (gclass.IsActive && gclass.VarNameList.Count > 0)
                        {
                            CalResult<byte[]> xktResult = this.omronCip.Read(gclass.VarNameList.ToArray());

                            #region 读取成功
                            if (xktResult.IsSuccess)
                            {
                                ErrorTimes = 0;
                                byte[] content = xktResult.Content;
                                int num = 0;
                                using (List<OmronCIPVariable>.Enumerator enumerator2 = gclass.VariableList.GetEnumerator())
                                {
                                    #region 循环组内对象
                                    while (enumerator2.MoveNext())
                                    {
                                        OmronCIPVariable omronCIPVariable = enumerator2.Current;

                                        #region 根据变量类型转换
                                        switch (omronCIPVariable.VarType)
                                        {
                                            case ComplexDataType.Bool:
                                                omronCIPVariable.Value = (ShortLib.GetShortFromByteArray(content, num, this.omronCip.DataFormat) == 1);
                                                num += 2;
                                                break;
                                            case ComplexDataType.Byte:
                                            case ComplexDataType.SByte:
                                                omronCIPVariable.Value = ByteArrayLib.GetByteArray(content, num, 1)[0];
                                                num++;
                                                break;
                                            case ComplexDataType.Short:
                                                omronCIPVariable.Value = ShortLib.GetShortFromByteArray(content, num, this.omronCip.DataFormat);
                                                num += 2;
                                                break;
                                            case ComplexDataType.UShort:
                                                omronCIPVariable.Value = UShortLib.GetUShortFromByteArray(content, num, this.omronCip.DataFormat);
                                                num += 2;
                                                break;
                                            case ComplexDataType.Int:
                                                omronCIPVariable.Value = IntLib.GetIntFromByteArray(content, num, this.omronCip.DataFormat);
                                                num += 4;
                                                break;
                                            case ComplexDataType.UInt:
                                                omronCIPVariable.Value = UIntLib.GetUIntFromByteArray(content, num, this.omronCip.DataFormat);
                                                num += 4;
                                                break;
                                            case ComplexDataType.Float:
                                                omronCIPVariable.Value = FloatLib.GetFloatFromByteArray(content, num, this.omronCip.DataFormat);
                                                num += 4;
                                                break;
                                            case ComplexDataType.Double:
                                                omronCIPVariable.Value = DoubleLib.GetDoubleFromByteArray(content, num, this.omronCip.DataFormat);
                                                num += 8;
                                                break;
                                            case ComplexDataType.Long:
                                                omronCIPVariable.Value = LongLib.GetLongFromByteArray(content, num, this.omronCip.DataFormat);
                                                num += 8;
                                                break;
                                            case ComplexDataType.ULong:
                                                omronCIPVariable.Value = ULongLib.GetULongFromByteArray(content, num, this.omronCip.DataFormat);
                                                num += 8;
                                                break;
                                            case ComplexDataType.String:
                                                {
                                                    ushort ushortFromByteArray = (ushort)UShortLib.GetUShortFromByteArray(new byte[]
                                                    {
                                                content[num + 1],
                                                content[num]
                                                    }, 0, DataFormat.ABCD);
                                                    omronCIPVariable.Value = ushortFromByteArray == 0 ? "" : StringLib.GetStringFromByteArray(content, num + 2, ushortFromByteArray, Encoding.ASCII);
                                                    num += ushortFromByteArray + 2;
                                                    break;
                                                }
                                        }
                                        #endregion

                                        omronCIPVariable.Value = MigrationLib.GetMigrationValue(omronCIPVariable.Value, omronCIPVariable.Scale, omronCIPVariable.Offset);
                                        UpdateCurrentValue(omronCIPVariable);
                                    }
                                    continue;
                                    #endregion
                                }
                            }
                            #endregion

                            #region 读取失败
                            else
                            {
                                ErrorTimes++;
                                if (ErrorTimes >= MaxErrorTimes)
                                {
                                    this.IsConnected = false;
                                    sw.Reset();
                                    break;
                                }
                            }
                            #endregion

                        }
                    }
                    #endregion

                    this.CommRate = this.sw.ElapsedMilliseconds;
                }
                else
                {
                    if (!this.FirstConnect)
                    {
                        Thread.Sleep(base.ReConnectTime);
                        this.omronCip.DisConnect();
                    }
                    this.omronCip = new OmronCipNet();
                    this.IsConnected = this.omronCip.Connect(this.IpAddress, 44818);
                    this.FirstConnect = false;
                }
            }
            sw.Reset();
            IsConnected = false;
            this.omronCip.DisConnect();
        }
        public CalResult Write(string keyName, string setValue)
        {
            CalResult result;
            if (!this.CurrentVarList.ContainsKey(keyName))
                result = new CalResult
                {
                    IsSuccess = false,
                    Message = "无法通过变量名称获取到变量"
                };
            else
            {
                OmronCIPVariable omronCIPVariable = this.CurrentVarList[keyName] as OmronCIPVariable;
                CalResult<string> xktResult = Common.VerifyInputValue(omronCIPVariable, omronCIPVariable.VarType, setValue);
                return xktResult.IsSuccess ? this.omronCip.Write(omronCIPVariable.VarAddress, xktResult.Content, omronCIPVariable.VarType) : xktResult;
            }
            return result;
        }
        public CalResult Write(string keyName, string setValue, VarType type)
        {
            OmronCIPVariable omronCIPVariable = this.CurrentVarList[keyName] as OmronCIPVariable;
            CalResult<string> xktResult = Common.VerifyInputValue(omronCIPVariable, omronCIPVariable.VarType, setValue);
            return xktResult.IsSuccess ? this.omronCip.Write(omronCIPVariable.VarAddress, xktResult.Content, omronCIPVariable.VarType) : xktResult;
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
        /// OmronCIPDeviceGroup组集合
        /// </summary>
        public List<OmronCIPDeviceGroup> DeviceGroupList;
        /// <summary>
        /// OmronCipNet对象
        /// </summary>
        public OmronCipNet omronCip;

    }
}
