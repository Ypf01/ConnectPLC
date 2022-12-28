using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Pframe.Common;
using Pframe;
using Pframe.DataConvert;
using Pframe.PLC.AB;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Device
{
    public class NodeABCIP : DeviceNode, IXmlConvert
    {
        public NodeABCIP()
        {
            this.sw = new Stopwatch();
            this.DeviceGroupList = new List<ABCIPDeviceGroup>();
            this.Slot = 0;
            base.Name = "AB PLC";
            base.Description = "真空系统1#PLC";
            base.DeviceType = 170;
            this.IpAddress = "192.168.1.14";
            this.Port = 44818;
            this.FirstConnect = true;
        }

        public bool FirstConnect { get; set; }

        public long CommRate { get; set; }

        public string IpAddress { get; set; }

        public int Port { get; set; }

        public bool IsConnected { get; set; }

        public byte Slot { get; set; }

        public override void LoadByXmlElement(XElement element)
        {
            base.LoadByXmlElement(element);
            this.IpAddress = element.Attribute("IpAddress").Value;
            this.Port = int.Parse(element.Attribute("Port").Value);
            this.Slot = Convert.ToByte(element.Attribute("Slot").Value);
        }

        public override XElement ToXmlElement()
        {
            XElement xelement = base.ToXmlElement();
            xelement.SetAttributeValue("IpAddress", this.IpAddress);
            xelement.SetAttributeValue("Port", this.Port);
            xelement.SetAttributeValue("Slot", this.Slot);
            return xelement;
        }

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

        public void Start()
        {
            foreach (ABCIPDeviceGroup abcipdeviceGroup in this.DeviceGroupList)
            {
                foreach (ABCIPVariable abcipvariable in abcipdeviceGroup.varList)
                {
                    if (abcipvariable.Config.ArchiveEnable)
                        this.StoreVarList.Add(abcipvariable);
                    if (this.CurrentVarList.ContainsKey(abcipvariable.KeyName))
                        this.CurrentVarList[abcipvariable.KeyName] = abcipvariable;
                    else
                        this.CurrentVarList.Add(abcipvariable.KeyName, abcipvariable);
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
                    foreach (ABCIPDeviceGroup abcipdeviceGroup in this.DeviceGroupList)
                    {
                        if (abcipdeviceGroup.IsActive && abcipdeviceGroup.VarNameList.Count > 0)
                        {
                            CalResult<byte[]> xktResult = this.abCip.Read(abcipdeviceGroup.VarNameList.ToArray());
                            //this.CommRate = this.sw.ElapsedMilliseconds;
                            if (xktResult.IsSuccess)
                            {
                                ErrorTimes = 0;
                                byte[] content = xktResult.Content;
                                int num = 0;
                                using (List<ABCIPVariable>.Enumerator enumerator2 = abcipdeviceGroup.VariableList.GetEnumerator())
                                {
                                    while (enumerator2.MoveNext())
                                    {
                                        ABCIPVariable abcipvariable = enumerator2.Current;
                                        switch (abcipvariable.VarType)
                                        {
                                            case ComplexDataType.Bool:
                                                abcipvariable.Value = (ShortLib.GetShortFromByteArray(content, num, this.abCip.DataFormat) == 1);
                                                num += 2;
                                                break;
                                            case ComplexDataType.Byte:
                                            case ComplexDataType.SByte:
                                                abcipvariable.Value = ByteArrayLib.GetByteArray(content, num, 1)[0];
                                                num++;
                                                break;
                                            case ComplexDataType.Short:
                                                abcipvariable.Value = ShortLib.GetShortFromByteArray(content, num, this.abCip.DataFormat);
                                                num += 2;
                                                break;
                                            case ComplexDataType.UShort:
                                                abcipvariable.Value = UShortLib.GetUShortFromByteArray(content, num, this.abCip.DataFormat);
                                                num += 2;
                                                break;
                                            case ComplexDataType.Int:
                                                abcipvariable.Value = IntLib.GetIntFromByteArray(content, num, this.abCip.DataFormat);
                                                num += 4;
                                                break;
                                            case ComplexDataType.UInt:
                                                abcipvariable.Value = UIntLib.GetUIntFromByteArray(content, num, this.abCip.DataFormat);
                                                num += 4;
                                                break;
                                            case ComplexDataType.Float:
                                                abcipvariable.Value = FloatLib.GetFloatFromByteArray(content, num, this.abCip.DataFormat);
                                                num += 4;
                                                break;
                                            case ComplexDataType.Double:
                                                abcipvariable.Value = DoubleLib.GetDoubleFromByteArray(content, num, this.abCip.DataFormat);
                                                num += 8;
                                                break;
                                            case ComplexDataType.Long:
                                                abcipvariable.Value = LongLib.GetLongFromByteArray(content, num, this.abCip.DataFormat);
                                                num += 8;
                                                break;
                                            case ComplexDataType.ULong:
                                                abcipvariable.Value = ULongLib.GetULongFromByteArray(content, num, this.abCip.DataFormat);
                                                num += 8;
                                                break;
                                            case ComplexDataType.String:
                                                {
                                                    int ushortFromByteArray = (int)UShortLib.GetUShortFromByteArray(new byte[]
                                                    {
                                                content[num + 1],
                                                content[num]
                                                    }, 0, DataFormat.ABCD);
                                                    abcipvariable.Value = StringLib.GetStringFromByteArray(content, num + 2, ushortFromByteArray, Encoding.ASCII);
                                                    num += ushortFromByteArray + 2;
                                                    break;
                                                }
                                        }
                                        abcipvariable.Value = MigrationLib.GetMigrationValue(abcipvariable.Value, abcipvariable.Scale, abcipvariable.Offset);
                                        base.UpdateCurrentValue(abcipvariable);
                                    }
                                    continue;
                                }
                            }
                            else
                            {
                                ErrorTimes++;
                                if (base.ErrorTimes >= base.MaxErrorTimes)
                                {
                                    this.IsConnected = false;
                                    sw.Stop();
                                    break;
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
                        Thread.Sleep(ReConnectTime);
                        this.abCip.DisConnect();
                    }
                    this.abCip = new ABCIP();
                    this.IsConnected = this.abCip.Connect(this.IpAddress, 44818);
                    this.FirstConnect = false;
                }
            }
            sw.Reset();
            abCip?.DisConnect();
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
                ABCIPVariable abcipvariable = this.CurrentVarList[keyName] as ABCIPVariable;
                CalResult<string> xktResult = Common.VerifyInputValue(abcipvariable, abcipvariable.VarType, setValue);
                result = xktResult.IsSuccess ? this.abCip.Write(abcipvariable.VarAddress, xktResult.Content, abcipvariable.VarType) : xktResult;
            }
            return result;
        }

        public CancellationTokenSource cts;

        public Stopwatch sw;

        public List<ABCIPDeviceGroup> DeviceGroupList;

        public ABCIP abCip;

    }
}
