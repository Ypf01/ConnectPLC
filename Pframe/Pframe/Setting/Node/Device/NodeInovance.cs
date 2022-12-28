using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Pframe;
using Pframe.Common;
using Pframe.DataConvert;
using Pframe.PLC.Inovance;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Device
{
    public class NodeInovance : DeviceNode, IXmlConvert
    {
        public NodeInovance()
        {
            this.sw = new Stopwatch();
            this.DeviceGroupList = new List<InovanceDeviceGroup>();
            base.Name = "汇川系列PLC";
            base.Description = "称重系统1#PLC";
            base.DeviceType = 110;
            this.PortNum = "COM3";
            this.Paud = 19200;
            this.Parity = Parity.Even;
            this.DataBits = "8";
            this.StopBits = StopBits.One;
            this.FirstConnect = true;
            this.SleepTime = 20;
        }

        public bool FirstConnect { get; set; }

        public bool ConnectState { get; set; }

        public long CommRate { get; set; }

        public string PortNum { get; set; }

        public int Paud { get; set; }

        public Parity Parity { get; set; }

        public string DataBits { get; set; }

        public StopBits StopBits { get; set; }

        public bool IsConnected { get; set; }

        public int SleepTime { get; set; }

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
            nodeClassRenders.Add(new NodeClassRenderItem("连接情况", this.IsConnected ? "已连接" : "未连接"));
            if (this.IsConnected)
                nodeClassRenders.Add(new NodeClassRenderItem("通信周期", this.CommRate.ToString() + "ms"));
            return nodeClassRenders;
        }

        public void Start()
        {
            foreach (InovanceDeviceGroup inovanceDeviceGroup in this.DeviceGroupList)
            {
                foreach (InovanceVariable inovanceVariable in inovanceDeviceGroup.varList)
                {
                    if (inovanceVariable.Config.ArchiveEnable)
                        this.StoreVarList.Add(inovanceVariable);
                    if (this.CurrentVarList.ContainsKey(inovanceVariable.KeyName))
                        this.CurrentVarList[inovanceVariable.KeyName] = inovanceVariable;
                    else
                        this.CurrentVarList.Add(inovanceVariable.KeyName, inovanceVariable);
                }
            }
            this.cts = new CancellationTokenSource();
            Task.Run(new Action(this.GetValue), this.cts.Token);
        }

        public void Stop()
        {
            IsConnected = false;
            inovance?.DisConnect();
            this.cts?.Cancel();
        }

        private void GetValue()
        {
            while (!this.cts.IsCancellationRequested)
            {
                if (this.IsConnected)
                {
                    this.sw.Restart();
                    foreach (InovanceDeviceGroup inovanceDeviceGroup in this.DeviceGroupList)
                    {
                        if (inovanceDeviceGroup.IsActive)
                        {
                            InovanceStoreArea storeArea = inovanceDeviceGroup.StoreArea;
                            InovanceStoreArea inovanceStoreArea = storeArea;
                            if (inovanceStoreArea > InovanceStoreArea.C存储区)
                            {
                                if (inovanceStoreArea - InovanceStoreArea.D存储区 <= 2)
                                {
                                    CalResult<byte[]> xktResult = this.inovance.ReadBytes(inovanceDeviceGroup.Start, Convert.ToUInt16(inovanceDeviceGroup.Length), inovanceDeviceGroup.SlaveID);
                                    this.ConnectState = xktResult.IsSuccess;
                                    if (xktResult.IsSuccess)
                                    {
                                        base.ErrorTimes = 0;
                                        int inovanceStart = this.GetInovanceStart(inovanceDeviceGroup.Start.Substring(inovanceDeviceGroup.Start.IndexOf(inovanceDeviceGroup.Start.First((char c) => char.IsDigit(c)))), inovanceDeviceGroup.StoreArea);
                                        using (List<InovanceVariable>.Enumerator enumerator2 = inovanceDeviceGroup.varList.GetEnumerator())
                                        {
                                            while (enumerator2.MoveNext())
                                            {
                                                InovanceVariable inovanceVariable = enumerator2.Current;
                                                int num;
                                                int num2;
                                                if (this.VerifyInovanceAddress(false, inovanceDeviceGroup.StoreArea, inovanceVariable.Start, out num, out num2))
                                                {
                                                    num -= inovanceStart;
                                                    num *= 2;
                                                    switch (inovanceVariable.VarType)
                                                    {
                                                        case DataType.Bool:
                                                            inovanceVariable.Value = BitLib.GetBitFrom2ByteArray(xktResult.Content, num, num2, false);
                                                            break;
                                                        case DataType.Short:
                                                            inovanceVariable.Value = ShortLib.GetShortFromByteArray(xktResult.Content, num, this.inovance.DataFormat);
                                                            break;
                                                        case DataType.UShort:
                                                            inovanceVariable.Value = UShortLib.GetUShortFromByteArray(xktResult.Content, num, this.inovance.DataFormat);
                                                            break;
                                                        case DataType.Int:
                                                            inovanceVariable.Value = IntLib.GetIntFromByteArray(xktResult.Content, num, this.inovance.DataFormat);
                                                            break;
                                                        case DataType.UInt:
                                                            inovanceVariable.Value = UIntLib.GetUIntFromByteArray(xktResult.Content, num, this.inovance.DataFormat);
                                                            break;
                                                        case DataType.Float:
                                                            inovanceVariable.Value = FloatLib.GetFloatFromByteArray(xktResult.Content, num, this.inovance.DataFormat);
                                                            break;
                                                        case DataType.Double:
                                                            inovanceVariable.Value = DoubleLib.GetDoubleFromByteArray(xktResult.Content, num, this.inovance.DataFormat);
                                                            break;
                                                        case DataType.Long:
                                                            inovanceVariable.Value = LongLib.GetLongFromByteArray(xktResult.Content, num, this.inovance.DataFormat);
                                                            break;
                                                        case DataType.ULong:
                                                            inovanceVariable.Value = ULongLib.GetULongFromByteArray(xktResult.Content, num, this.inovance.DataFormat);
                                                            break;
                                                        case DataType.String:
                                                            inovanceVariable.Value = StringLib.GetStringFromByteArray(xktResult.Content, num, num2 * 2, Encoding.ASCII);
                                                            break;
                                                        case DataType.ByteArray:
                                                            inovanceVariable.Value = ByteArrayLib.GetByteArray(xktResult.Content, num, num2 * 2);
                                                            break;
                                                        case DataType.HexString:
                                                            inovanceVariable.Value = StringLib.GetHexStringFromByteArray(xktResult.Content, num, num2 * 2, ' ');
                                                            break;
                                                    }
                                                    inovanceVariable.Value = MigrationLib.GetMigrationValue(inovanceVariable.Value, inovanceVariable.Scale, inovanceVariable.Offset);
                                                    base.UpdateCurrentValue(inovanceVariable);
                                                }
                                            }
                                            continue;
                                        }
                                    }
                                    int errorTimes = base.ErrorTimes;
                                    base.ErrorTimes = errorTimes + 1;
                                    if (base.ErrorTimes >= base.MaxErrorTimes)
                                    {
                                        if (SerialPort.GetPortNames().Contains(this.PortNum))
                                        {
                                            Thread.Sleep(10);
                                        }
                                        else
                                        {
                                            this.IsConnected = false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                CalResult<byte[]> xktResult2 = this.inovance.ReadBytes(inovanceDeviceGroup.Start, Convert.ToUInt16(inovanceDeviceGroup.Length), inovanceDeviceGroup.SlaveID);
                                this.ConnectState = xktResult2.IsSuccess;
                                if (xktResult2.IsSuccess)
                                {
                                    base.ErrorTimes = 0;
                                    int inovanceStart2 = this.GetInovanceStart(inovanceDeviceGroup.Start.Substring(inovanceDeviceGroup.Start.IndexOf(inovanceDeviceGroup.Start.First((char c) => char.IsDigit(c)))), inovanceDeviceGroup.StoreArea);
                                    using (List<InovanceVariable>.Enumerator enumerator3 = inovanceDeviceGroup.varList.GetEnumerator())
                                    {
                                        while (enumerator3.MoveNext())
                                        {
                                            InovanceVariable inovanceVariable2 = enumerator3.Current;
                                            int num3;
                                            int num4;
                                            if (this.VerifyInovanceAddress(true, inovanceDeviceGroup.StoreArea, inovanceVariable2.Start, out num3, out num4))
                                            {
                                                if (inovanceVariable2.VarType == DataType.Bool)
                                                {
                                                    num3 -= inovanceStart2;
                                                    inovanceVariable2.Value = BitLib.GetBitArrayFromByteArray(xktResult2.Content, false)[num3];
                                                }
                                                base.UpdateCurrentValue(inovanceVariable2);
                                            }
                                        }
                                        continue;
                                    }
                                }
                                int errorTimes = base.ErrorTimes;
                                base.ErrorTimes = errorTimes + 1;
                                if (base.ErrorTimes >= base.MaxErrorTimes)
                                {
                                    if (SerialPort.GetPortNames().Contains(this.PortNum))
                                    {
                                        Thread.Sleep(10);
                                    }
                                    else
                                    {
                                        this.IsConnected = false;
                                    }
                                }
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
                        InovanceModbus inovanceModbus = this.inovance;
                        if (inovanceModbus != null)
                        {
                            inovanceModbus.DisConnect();
                        }
                    }
                    this.inovance = new InovanceModbus();
                    this.inovance.SleepTime = this.SleepTime;
                    this.IsConnected = this.inovance.Connect(this.PortNum, this.Paud, int.Parse(this.DataBits), this.Parity, this.StopBits);
                    this.FirstConnect = false;
                }
            }
        }

        public int GetInovanceStart(string start, InovanceStoreArea store)
        {
            int result;
            switch (store)
            {
                case InovanceStoreArea.M存储区:
                    result = Convert.ToInt32(start, InovanceDataType.M.FromBase);
                    break;
                case InovanceStoreArea.X存储区:
                    result = Convert.ToInt32(start, InovanceDataType.X.FromBase);
                    break;
                case InovanceStoreArea.Y存储区:
                    result = Convert.ToInt32(start, InovanceDataType.Y.FromBase);
                    break;
                case InovanceStoreArea.S存储区:
                    result = Convert.ToInt32(start, InovanceDataType.S.FromBase);
                    break;
                case InovanceStoreArea.T存储区:
                    result = Convert.ToInt32(start, InovanceDataType.T.FromBase);
                    break;
                case InovanceStoreArea.C存储区:
                    result = Convert.ToInt32(start, InovanceDataType.C.FromBase);
                    break;
                case InovanceStoreArea.D存储区:
                    result = Convert.ToInt32(start, InovanceDataType.D.FromBase);
                    break;
                case InovanceStoreArea.TR存储区:
                    result = Convert.ToInt32(start, InovanceDataType.TR.FromBase);
                    break;
                case InovanceStoreArea.CR存储区:
                    result = Convert.ToInt32(start, InovanceDataType.CR.FromBase);
                    break;
                default:
                    result = 0;
                    break;
            }
            return result;
        }

        public bool VerifyInovanceAddress(bool isBoolStore, InovanceStoreArea store, string address, out int start, out int offset)
        {
            bool result;
            if (isBoolStore)
            {
                offset = 0;
                start = 0;
                try
                {
                    start = this.GetInovanceStart(address, store);
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
                        start = this.GetInovanceStart(array[0], store);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                    try
                    {
                        offset = int.Parse(array[1]);
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
                result = int.TryParse(address, out start);
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
                InovanceVariable inovanceVariable = this.CurrentVarList[keyName] as InovanceVariable;
                CalResult<string> xktResult = Common.VerifyInputValue(inovanceVariable, inovanceVariable.VarType, setValue);
                result = xktResult.IsSuccess ? this.inovance.Write(inovanceVariable.VarAddress, xktResult.Content, inovanceVariable.VarType, 1) : xktResult;
            }
            return result;
        }

        public CancellationTokenSource cts;

        public Stopwatch sw;

        public const int Paud9600 = 9600;

        public const int Paud19200 = 19200;

        public const int Paud38400 = 38400;

        public const string DataBitsSeven = "7";

        public const string DataBitsEight = "8";

        public InovanceModbus inovance;

        public List<InovanceDeviceGroup> DeviceGroupList;
    }
}
