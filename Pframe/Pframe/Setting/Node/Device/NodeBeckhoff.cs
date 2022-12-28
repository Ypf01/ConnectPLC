using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Pframe;
using Pframe.Common;
using Pframe.DataConvert;
using Pframe.PLC.Beckoff;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Device
{
    public class NodeBeckhoff : DeviceNode, IXmlConvert
    {
        public NodeBeckhoff()
        {
            this.sw = new Stopwatch();
            this.DeviceGroupList = new List<BeckhoffDeviceGroup>();
            base.Name = "倍福PLC";
            base.Description = "真空系统1#PLC";
            base.DeviceType = 130;
            this.IpAddress = "192.168.1.123.1.1";
            this.Port = 851;
            this.FirstConnect = true;
        }

        public bool FirstConnect { get; set; }

        public long CommRate { get; set; }

        public string IpAddress { get; set; }

        public int Port { get; set; }

        public bool IsConnected { get; set; }

        public override void LoadByXmlElement(XElement element)
        {
            base.LoadByXmlElement(element);
            this.IpAddress = element.Attribute("IpAddress").Value;
            this.Port = int.Parse(element.Attribute("Port").Value);
        }

        public override XElement ToXmlElement()
        {
            XElement xelement = base.ToXmlElement();
            xelement.SetAttributeValue("IpAddress", this.IpAddress);
            xelement.SetAttributeValue("Port", this.Port);
            return xelement;
        }

        public override List<NodeClassRenderItem> GetNodeClassRenders()
        {
            List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
            nodeClassRenders.Add(new NodeClassRenderItem("Net ID", this.IpAddress));
            nodeClassRenders.Add(new NodeClassRenderItem("Port", this.Port.ToString()));
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
            foreach (BeckhoffDeviceGroup beckhoffDeviceGroup in this.DeviceGroupList)
            {
                foreach (BeckhoffVariable beckhoffVariable in beckhoffDeviceGroup.varList)
                {
                    if (beckhoffVariable.Config.ArchiveEnable)
                    {
                        this.StoreVarList.Add(beckhoffVariable);
                    }
                    if (this.CurrentVarList.ContainsKey(beckhoffVariable.KeyName))
                    {
                        this.CurrentVarList[beckhoffVariable.KeyName] = beckhoffVariable;
                    }
                    else
                    {
                        this.CurrentVarList.Add(beckhoffVariable.KeyName, beckhoffVariable);
                    }
                    if (beckhoffVariable.varList.Count > 0)
                    {
                        foreach (BeckhoffVariable beckhoffVariable2 in beckhoffVariable.varList)
                        {
                            if (this.CurrentVarList.ContainsKey(beckhoffVariable2.KeyName))
                            {
                                this.CurrentVarList[beckhoffVariable2.KeyName] = beckhoffVariable2;
                            }
                            else
                            {
                                this.CurrentVarList.Add(beckhoffVariable2.KeyName, beckhoffVariable2);
                            }
                            if (beckhoffVariable2.varList.Count > 0)
                            {
                                foreach (BeckhoffVariable beckhoffVariable3 in beckhoffVariable2.varList)
                                {
                                    if (this.CurrentVarList.ContainsKey(beckhoffVariable3.KeyName))
                                    {
                                        this.CurrentVarList[beckhoffVariable3.KeyName] = beckhoffVariable3;
                                    }
                                    else
                                    {
                                        this.CurrentVarList.Add(beckhoffVariable3.KeyName, beckhoffVariable3);
                                    }
                                    if (beckhoffVariable3.varList.Count > 0)
                                    {
                                        foreach (BeckhoffVariable beckhoffVariable4 in beckhoffVariable3.varList)
                                        {
                                            if (this.CurrentVarList.ContainsKey(beckhoffVariable4.KeyName))
                                            {
                                                this.CurrentVarList[beckhoffVariable4.KeyName] = beckhoffVariable4;
                                            }
                                            else
                                            {
                                                this.CurrentVarList.Add(beckhoffVariable4.KeyName, beckhoffVariable4);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
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
                    foreach (BeckhoffDeviceGroup beckhoffDeviceGroup in this.DeviceGroupList)
                    {
                        if (beckhoffDeviceGroup.IsActive)
                        {
                            foreach (BeckhoffVariable beckhoffVariable in beckhoffDeviceGroup.varList)
                            {
                                switch (beckhoffVariable.VarType)
                                {
                                    case ComplexDataType.Bool:
                                    case ComplexDataType.Byte:
                                    case ComplexDataType.SByte:
                                    case ComplexDataType.Short:
                                    case ComplexDataType.UShort:
                                    case ComplexDataType.Int:
                                    case ComplexDataType.UInt:
                                    case ComplexDataType.Float:
                                    case ComplexDataType.Double:
                                    case ComplexDataType.Long:
                                    case ComplexDataType.ULong:
                                    case ComplexDataType.String:
                                        {
                                            CalResult<object> xktResult = this.beckhoff.Read(beckhoffVariable.VarAddress, beckhoffVariable.VarType);
                                            if (xktResult.IsSuccess)
                                            {
                                                beckhoffVariable.Value = xktResult.Content;
                                                beckhoffVariable.Value = MigrationLib.GetMigrationValue(beckhoffVariable.Value, beckhoffVariable.Scale, beckhoffVariable.Offset);
                                                base.UpdateCurrentValue(beckhoffVariable);
                                                continue;
                                            }
                                            continue;
                                        }
                                    case ComplexDataType.WString:
                                        continue;
                                    case ComplexDataType.Struct:
                                        {
                                            if (beckhoffVariable.varList.Count == 0)
                                            {
                                                continue;
                                            }
                                            int num = 0;
                                            foreach (BeckhoffVariable beckhoffVariable_ in beckhoffVariable.varList)
                                            {
                                                num += this.CalValue(beckhoffVariable_);
                                            }
                                            int num2;
                                            if (num % 8 == 0)
                                            {
                                                num2 = num;
                                            }
                                            else
                                            {
                                                num2 = (num / 8 + 1) * 8;
                                            }
                                            BinaryReader binaryReader = this.beckhoff.ReadStruct(beckhoffVariable.VarAddress, num2);
                                            if (binaryReader != null)
                                            {
                                                byte[] source = binaryReader.ReadBytes(num2);
                                                beckhoffVariable.Value = StringLib.GetHexStringFromByteArray(source, ' ');
                                                base.UpdateCurrentValue(beckhoffVariable);
                                                beckhoffVariable.Value = binaryReader.ToString();
                                                int i = 0;
                                                while (i < beckhoffVariable.varList.Count)
                                                {
                                                    switch (beckhoffVariable.varList[i].VarType)
                                                    {
                                                        case ComplexDataType.Bool:
                                                            beckhoffVariable.varList[i].Value = (ByteLib.GetByteFromByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset)) == 1);
                                                            base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                            break;
                                                        case ComplexDataType.Byte:
                                                            beckhoffVariable.varList[i].Value = ByteLib.GetByteFromByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset));
                                                            base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                            break;
                                                        case ComplexDataType.SByte:
                                                            beckhoffVariable.varList[i].Value = ByteLib.GetByteFromByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset));
                                                            base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                            break;
                                                        case ComplexDataType.Short:
                                                            beckhoffVariable.varList[i].Value = ShortLib.GetShortFromByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), DataFormat.DCBA);
                                                            base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                            break;
                                                        case ComplexDataType.UShort:
                                                            beckhoffVariable.varList[i].Value = UShortLib.GetUShortFromByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), DataFormat.DCBA);
                                                            base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                            break;
                                                        case ComplexDataType.Int:
                                                            beckhoffVariable.varList[i].Value = IntLib.GetIntFromByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), DataFormat.DCBA);
                                                            base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                            break;
                                                        case ComplexDataType.UInt:
                                                            beckhoffVariable.varList[i].Value = UIntLib.GetUIntFromByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), DataFormat.DCBA);
                                                            base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                            break;
                                                        case ComplexDataType.Float:
                                                            beckhoffVariable.varList[i].Value = FloatLib.GetFloatFromByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), DataFormat.DCBA);
                                                            base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                            break;
                                                        case ComplexDataType.Double:
                                                            beckhoffVariable.varList[i].Value = DoubleLib.GetDoubleFromByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), DataFormat.DCBA);
                                                            base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                            break;
                                                        case ComplexDataType.Long:
                                                            beckhoffVariable.varList[i].Value = LongLib.GetLongFromByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), DataFormat.DCBA);
                                                            base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                            break;
                                                        case ComplexDataType.ULong:
                                                            beckhoffVariable.varList[i].Value = ULongLib.GetULongFromByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), DataFormat.DCBA);
                                                            base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                            break;
                                                        case ComplexDataType.String:
                                                            beckhoffVariable.varList[i].Value = StringLib.GetStringFromByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), 81);
                                                            base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                            break;
                                                        case ComplexDataType.BoolArray:
                                                            beckhoffVariable.varList[i].Value = StringLib.GetHexStringFromByteArray(ByteArrayLib.GetByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), beckhoffVariable.varList[i].varList.Count), ' ');
                                                            base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                            using (List<BeckhoffVariable>.Enumerator enumerator4 = beckhoffVariable.varList[i].varList.GetEnumerator())
                                                            {
                                                                while (enumerator4.MoveNext())
                                                                {
                                                                    BeckhoffVariable beckhoffVariable2 = enumerator4.Current;
                                                                    beckhoffVariable2.Value = (ByteLib.GetByteFromByteArray(source, Convert.ToInt32(beckhoffVariable2.IndexOffset)) == 1);
                                                                    base.UpdateCurrentValue(beckhoffVariable2);
                                                                }
                                                                break;
                                                            }
                                                            //goto IL_70E;
                                                        case ComplexDataType.ByteArray:
                                                            goto IL_70E;
                                                        case ComplexDataType.SByteArray:
                                                            goto IL_7D7;
                                                        case ComplexDataType.ShortArray:
                                                            goto IL_8A0;
                                                        case ComplexDataType.UShortArray:
                                                            goto IL_972;
                                                        case ComplexDataType.IntArray:
                                                            goto IL_A44;
                                                        case ComplexDataType.UIntArray:
                                                            goto IL_B16;
                                                        case ComplexDataType.FloatArray:
                                                            goto IL_BE8;
                                                        case ComplexDataType.DoubleArray:
                                                            goto IL_CBA;
                                                        case ComplexDataType.LongArray:
                                                            goto IL_D8C;
                                                        case ComplexDataType.ULongArray:
                                                            goto IL_E5E;
                                                        case ComplexDataType.StringArray:
                                                            goto IL_F30;
                                                    }
                                                    IL_1933:
                                                    i++;
                                                    continue;
                                                    IL_70E:
                                                    beckhoffVariable.varList[i].Value = StringLib.GetHexStringFromByteArray(ByteArrayLib.GetByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), beckhoffVariable.varList[i].varList.Count), ' ');
                                                    base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                    using (List<BeckhoffVariable>.Enumerator enumerator5 = beckhoffVariable.varList[i].varList.GetEnumerator())
                                                    {
                                                        while (enumerator5.MoveNext())
                                                        {
                                                            BeckhoffVariable beckhoffVariable3 = enumerator5.Current;
                                                            beckhoffVariable3.Value = ByteLib.GetByteFromByteArray(source, Convert.ToInt32(beckhoffVariable3.IndexOffset));
                                                            base.UpdateCurrentValue(beckhoffVariable3);
                                                        }
                                                        goto IL_1933;
                                                    }
                                                    IL_7D7:
                                                    beckhoffVariable.varList[i].Value = StringLib.GetHexStringFromByteArray(ByteArrayLib.GetByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), beckhoffVariable.varList[i].varList.Count), ' ');
                                                    base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                    using (List<BeckhoffVariable>.Enumerator enumerator6 = beckhoffVariable.varList[i].varList.GetEnumerator())
                                                    {
                                                        while (enumerator6.MoveNext())
                                                        {
                                                            BeckhoffVariable beckhoffVariable4 = enumerator6.Current;
                                                            beckhoffVariable4.Value = ByteLib.GetByteFromByteArray(source, Convert.ToInt32(beckhoffVariable4.IndexOffset));
                                                            base.UpdateCurrentValue(beckhoffVariable4);
                                                        }
                                                        goto IL_1933;
                                                    }
                                                    IL_8A0:
                                                    beckhoffVariable.varList[i].Value = StringLib.GetStringFromValueArray<short>(ShortLib.GetShortArrayFromByteArray(ByteArrayLib.GetByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), beckhoffVariable.varList[i].varList.Count * 2), DataFormat.DCBA), ' ');
                                                    base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                    using (List<BeckhoffVariable>.Enumerator enumerator7 = beckhoffVariable.varList[i].varList.GetEnumerator())
                                                    {
                                                        while (enumerator7.MoveNext())
                                                        {
                                                            BeckhoffVariable beckhoffVariable5 = enumerator7.Current;
                                                            beckhoffVariable5.Value = ShortLib.GetShortFromByteArray(source, Convert.ToInt32(beckhoffVariable5.IndexOffset), DataFormat.DCBA);
                                                            base.UpdateCurrentValue(beckhoffVariable5);
                                                        }
                                                        goto IL_1933;
                                                    }
                                                    IL_972:
                                                    beckhoffVariable.varList[i].Value = StringLib.GetStringFromValueArray<ushort>(UShortLib.GetUShortArrayFromByteArray(ByteArrayLib.GetByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), beckhoffVariable.varList[i].varList.Count * 2), DataFormat.DCBA), ' ');
                                                    base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                    using (List<BeckhoffVariable>.Enumerator enumerator8 = beckhoffVariable.varList[i].varList.GetEnumerator())
                                                    {
                                                        while (enumerator8.MoveNext())
                                                        {
                                                            BeckhoffVariable beckhoffVariable6 = enumerator8.Current;
                                                            beckhoffVariable6.Value = UShortLib.GetUShortFromByteArray(source, Convert.ToInt32(beckhoffVariable6.IndexOffset), DataFormat.DCBA);
                                                            base.UpdateCurrentValue(beckhoffVariable6);
                                                        }
                                                        goto IL_1933;
                                                    }
                                                    IL_A44:
                                                    beckhoffVariable.varList[i].Value = StringLib.GetStringFromValueArray<int>(IntLib.GetIntArrayFromByteArray(ByteArrayLib.GetByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), beckhoffVariable.varList[i].varList.Count * 4), DataFormat.DCBA), ' ');
                                                    base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                    using (List<BeckhoffVariable>.Enumerator enumerator9 = beckhoffVariable.varList[i].varList.GetEnumerator())
                                                    {
                                                        while (enumerator9.MoveNext())
                                                        {
                                                            BeckhoffVariable beckhoffVariable7 = enumerator9.Current;
                                                            beckhoffVariable7.Value = IntLib.GetIntFromByteArray(source, Convert.ToInt32(beckhoffVariable7.IndexOffset), DataFormat.DCBA);
                                                            base.UpdateCurrentValue(beckhoffVariable7);
                                                        }
                                                        goto IL_1933;
                                                    }
                                                    IL_B16:
                                                    beckhoffVariable.varList[i].Value = StringLib.GetStringFromValueArray<uint>(UIntLib.GetUIntArrayFromByteArray(ByteArrayLib.GetByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), beckhoffVariable.varList[i].varList.Count * 4), DataFormat.DCBA), ' ');
                                                    base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                    using (List<BeckhoffVariable>.Enumerator enumerator10 = beckhoffVariable.varList[i].varList.GetEnumerator())
                                                    {
                                                        while (enumerator10.MoveNext())
                                                        {
                                                            BeckhoffVariable beckhoffVariable8 = enumerator10.Current;
                                                            beckhoffVariable8.Value = UIntLib.GetUIntFromByteArray(source, Convert.ToInt32(beckhoffVariable8.IndexOffset), DataFormat.DCBA);
                                                            base.UpdateCurrentValue(beckhoffVariable8);
                                                        }
                                                        goto IL_1933;
                                                    }
                                                    IL_BE8:
                                                    beckhoffVariable.varList[i].Value = StringLib.GetStringFromValueArray<float>(FloatLib.GetFloatArrayFromByteArray(ByteArrayLib.GetByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), beckhoffVariable.varList[i].varList.Count * 4), DataFormat.DCBA), ' ');
                                                    base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                    using (List<BeckhoffVariable>.Enumerator enumerator11 = beckhoffVariable.varList[i].varList.GetEnumerator())
                                                    {
                                                        while (enumerator11.MoveNext())
                                                        {
                                                            BeckhoffVariable beckhoffVariable9 = enumerator11.Current;
                                                            beckhoffVariable9.Value = FloatLib.GetFloatFromByteArray(source, Convert.ToInt32(beckhoffVariable9.IndexOffset), DataFormat.DCBA);
                                                            base.UpdateCurrentValue(beckhoffVariable9);
                                                        }
                                                        goto IL_1933;
                                                    }
                                                    IL_CBA:
                                                    beckhoffVariable.varList[i].Value = StringLib.GetStringFromValueArray<double>(DoubleLib.GetDoubleArrayFromByteArray(ByteArrayLib.GetByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), beckhoffVariable.varList[i].varList.Count * 8), DataFormat.DCBA), ' ');
                                                    base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                    using (List<BeckhoffVariable>.Enumerator enumerator12 = beckhoffVariable.varList[i].varList.GetEnumerator())
                                                    {
                                                        while (enumerator12.MoveNext())
                                                        {
                                                            BeckhoffVariable beckhoffVariable10 = enumerator12.Current;
                                                            beckhoffVariable10.Value = DoubleLib.GetDoubleFromByteArray(source, Convert.ToInt32(beckhoffVariable10.IndexOffset), DataFormat.DCBA);
                                                            base.UpdateCurrentValue(beckhoffVariable10);
                                                        }
                                                        goto IL_1933;
                                                    }
                                                    IL_D8C:
                                                    beckhoffVariable.varList[i].Value = StringLib.GetStringFromValueArray<long>(LongLib.GetLongArrayFromByteArray(ByteArrayLib.GetByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), beckhoffVariable.varList[i].varList.Count * 8), DataFormat.DCBA), ' ');
                                                    base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                    using (List<BeckhoffVariable>.Enumerator enumerator13 = beckhoffVariable.varList[i].varList.GetEnumerator())
                                                    {
                                                        while (enumerator13.MoveNext())
                                                        {
                                                            BeckhoffVariable beckhoffVariable11 = enumerator13.Current;
                                                            beckhoffVariable11.Value = LongLib.GetLongFromByteArray(source, Convert.ToInt32(beckhoffVariable11.IndexOffset), DataFormat.DCBA);
                                                            base.UpdateCurrentValue(beckhoffVariable11);
                                                        }
                                                        goto IL_1933;
                                                    }
                                                    IL_E5E:
                                                    beckhoffVariable.varList[i].Value = StringLib.GetStringFromValueArray<ulong>(ULongLib.GetULongArrayFromByteArray(ByteArrayLib.GetByteArray(source, Convert.ToInt32(beckhoffVariable.varList[i].IndexOffset), beckhoffVariable.varList[i].varList.Count * 8), DataFormat.DCBA), ' ');
                                                    base.UpdateCurrentValue(beckhoffVariable.varList[i]);
                                                    using (List<BeckhoffVariable>.Enumerator enumerator14 = beckhoffVariable.varList[i].varList.GetEnumerator())
                                                    {
                                                        while (enumerator14.MoveNext())
                                                        {
                                                            BeckhoffVariable beckhoffVariable12 = enumerator14.Current;
                                                            beckhoffVariable12.Value = ULongLib.GetULongFromByteArray(source, Convert.ToInt32(beckhoffVariable12.IndexOffset), DataFormat.DCBA);
                                                            base.UpdateCurrentValue(beckhoffVariable12);
                                                        }
                                                        goto IL_1933;
                                                    }
                                                    IL_F30:
                                                    using (List<BeckhoffVariable>.Enumerator enumerator15 = beckhoffVariable.varList[i].varList.GetEnumerator())
                                                    {
                                                        while (enumerator15.MoveNext())
                                                        {
                                                            BeckhoffVariable beckhoffVariable13 = enumerator15.Current;
                                                            beckhoffVariable13.Value = StringLib.GetStringFromByteArray(source, Convert.ToInt32(beckhoffVariable13.IndexOffset), 81);
                                                            base.UpdateCurrentValue(beckhoffVariable13);
                                                        }
                                                        goto IL_1933;
                                                    }
                                                    //goto IL_F95;
                                                }
                                                continue;
                                            }
                                            continue;
                                        }
                                    case ComplexDataType.BoolArray:
                                        break;
                                    case ComplexDataType.ByteArray:
                                        {
                                            byte[] array = this.beckhoff.ReadArray<byte>(beckhoffVariable.VarAddress, beckhoffVariable.varList.Count);
                                            if (array != null && array.Length == beckhoffVariable.varList.Count)
                                            {
                                                beckhoffVariable.Value = array.ToString();
                                                for (int j = 0; j < beckhoffVariable.varList.Count; j++)
                                                {
                                                    beckhoffVariable.varList[j].Value = array[j];
                                                    beckhoffVariable.varList[j].Value = MigrationLib.GetMigrationValue(beckhoffVariable.varList[j].Value, beckhoffVariable.varList[j].Scale, beckhoffVariable.varList[j].Offset);
                                                    base.UpdateCurrentValue(beckhoffVariable);
                                                }
                                                continue;
                                            }
                                            continue;
                                        }
                                    case ComplexDataType.SByteArray:
                                        {
                                            sbyte[] array2 = this.beckhoff.ReadArray<sbyte>(beckhoffVariable.VarAddress, beckhoffVariable.varList.Count);
                                            if (array2 != null && array2.Length == beckhoffVariable.varList.Count)
                                            {
                                                beckhoffVariable.Value = array2.ToString();
                                                for (int k = 0; k < beckhoffVariable.varList.Count; k++)
                                                {
                                                    beckhoffVariable.varList[k].Value = array2[k];
                                                    beckhoffVariable.varList[k].Value = MigrationLib.GetMigrationValue(beckhoffVariable.varList[k].Value, beckhoffVariable.varList[k].Scale, beckhoffVariable.varList[k].Offset);
                                                    base.UpdateCurrentValue(beckhoffVariable);
                                                }
                                                continue;
                                            }
                                            continue;
                                        }
                                    case ComplexDataType.ShortArray:
                                        {
                                            short[] array3 = this.beckhoff.ReadArray<short>(beckhoffVariable.VarAddress, beckhoffVariable.varList.Count);
                                            if (array3 != null && array3.Length == beckhoffVariable.varList.Count)
                                            {
                                                beckhoffVariable.Value = array3.ToString();
                                                for (int l = 0; l < beckhoffVariable.varList.Count; l++)
                                                {
                                                    beckhoffVariable.varList[l].Value = array3[l];
                                                    beckhoffVariable.varList[l].Value = MigrationLib.GetMigrationValue(beckhoffVariable.varList[l].Value, beckhoffVariable.varList[l].Scale, beckhoffVariable.varList[l].Offset);
                                                    base.UpdateCurrentValue(beckhoffVariable);
                                                }
                                                continue;
                                            }
                                            continue;
                                        }
                                    case ComplexDataType.UShortArray:
                                        {
                                            ushort[] array4 = this.beckhoff.ReadArray<ushort>(beckhoffVariable.VarAddress, beckhoffVariable.varList.Count);
                                            if (array4 != null && array4.Length == beckhoffVariable.varList.Count)
                                            {
                                                beckhoffVariable.Value = array4.ToString();
                                                for (int m = 0; m < beckhoffVariable.varList.Count; m++)
                                                {
                                                    beckhoffVariable.varList[m].Value = array4[m];
                                                    beckhoffVariable.varList[m].Value = MigrationLib.GetMigrationValue(beckhoffVariable.varList[m].Value, beckhoffVariable.varList[m].Scale, beckhoffVariable.varList[m].Offset);
                                                    base.UpdateCurrentValue(beckhoffVariable);
                                                }
                                                continue;
                                            }
                                            continue;
                                        }
                                    case ComplexDataType.IntArray:
                                        {
                                            int[] array5 = this.beckhoff.ReadArray<int>(beckhoffVariable.VarAddress, beckhoffVariable.varList.Count);
                                            if (array5 != null && array5.Length == beckhoffVariable.varList.Count)
                                            {
                                                beckhoffVariable.Value = array5.ToString();
                                                for (int n = 0; n < beckhoffVariable.varList.Count; n++)
                                                {
                                                    beckhoffVariable.varList[n].Value = array5[n];
                                                    beckhoffVariable.varList[n].Value = MigrationLib.GetMigrationValue(beckhoffVariable.varList[n].Value, beckhoffVariable.varList[n].Scale, beckhoffVariable.varList[n].Offset);
                                                    base.UpdateCurrentValue(beckhoffVariable);
                                                }
                                                continue;
                                            }
                                            continue;
                                        }
                                    case ComplexDataType.UIntArray:
                                        {
                                            uint[] array6 = this.beckhoff.ReadArray<uint>(beckhoffVariable.VarAddress, beckhoffVariable.varList.Count);
                                            if (array6 != null && array6.Length == beckhoffVariable.varList.Count)
                                            {
                                                for (int num3 = 0; num3 < beckhoffVariable.varList.Count; num3++)
                                                {
                                                    beckhoffVariable.varList[num3].Value = array6[num3];
                                                    beckhoffVariable.varList[num3].Value = MigrationLib.GetMigrationValue(beckhoffVariable.varList[num3].Value, beckhoffVariable.varList[num3].Scale, beckhoffVariable.varList[num3].Offset);
                                                    base.UpdateCurrentValue(beckhoffVariable);
                                                }
                                                continue;
                                            }
                                            continue;
                                        }
                                    case ComplexDataType.FloatArray:
                                        {
                                            float[] array7 = this.beckhoff.ReadArray<float>(beckhoffVariable.VarAddress, beckhoffVariable.varList.Count);
                                            if (array7 != null && array7.Length == beckhoffVariable.varList.Count)
                                            {
                                                beckhoffVariable.Value = array7.ToString();
                                                for (int num4 = 0; num4 < beckhoffVariable.varList.Count; num4++)
                                                {
                                                    beckhoffVariable.varList[num4].Value = array7[num4];
                                                    beckhoffVariable.varList[num4].Value = MigrationLib.GetMigrationValue(beckhoffVariable.varList[num4].Value, beckhoffVariable.varList[num4].Scale, beckhoffVariable.varList[num4].Offset);
                                                    base.UpdateCurrentValue(beckhoffVariable);
                                                }
                                                continue;
                                            }
                                            continue;
                                        }
                                    case ComplexDataType.DoubleArray:
                                        {
                                            double[] array8 = this.beckhoff.ReadArray<double>(beckhoffVariable.VarAddress, beckhoffVariable.varList.Count);
                                            if (array8 != null && array8.Length == beckhoffVariable.varList.Count)
                                            {
                                                beckhoffVariable.Value = array8.ToString();
                                                for (int num5 = 0; num5 < beckhoffVariable.varList.Count; num5++)
                                                {
                                                    beckhoffVariable.varList[num5].Value = array8[num5];
                                                    beckhoffVariable.varList[num5].Value = MigrationLib.GetMigrationValue(beckhoffVariable.varList[num5].Value, beckhoffVariable.varList[num5].Scale, beckhoffVariable.varList[num5].Offset);
                                                    base.UpdateCurrentValue(beckhoffVariable);
                                                }
                                                continue;
                                            }
                                            continue;
                                        }
                                    case ComplexDataType.LongArray:
                                        {
                                            long[] array9 = this.beckhoff.ReadArray<long>(beckhoffVariable.VarAddress, beckhoffVariable.varList.Count);
                                            if (array9 != null && array9.Length == beckhoffVariable.varList.Count)
                                            {
                                                beckhoffVariable.Value = array9.ToString();
                                                for (int num6 = 0; num6 < beckhoffVariable.varList.Count; num6++)
                                                {
                                                    beckhoffVariable.varList[num6].Value = array9[num6];
                                                    beckhoffVariable.varList[num6].Value = MigrationLib.GetMigrationValue(beckhoffVariable.varList[num6].Value, beckhoffVariable.varList[num6].Scale, beckhoffVariable.varList[num6].Offset);
                                                    base.UpdateCurrentValue(beckhoffVariable);
                                                }
                                                continue;
                                            }
                                            continue;
                                        }
                                    case ComplexDataType.ULongArray:
                                        {
                                            ulong[] array10 = this.beckhoff.ReadArray<ulong>(beckhoffVariable.VarAddress, beckhoffVariable.varList.Count);
                                            if (array10 != null && array10.Length == beckhoffVariable.varList.Count)
                                            {
                                                beckhoffVariable.Value = array10.ToString();
                                                for (int num7 = 0; num7 < beckhoffVariable.varList.Count; num7++)
                                                {
                                                    beckhoffVariable.varList[num7].Value = array10[num7];
                                                    beckhoffVariable.varList[num7].Value = MigrationLib.GetMigrationValue(beckhoffVariable.varList[num7].Value, beckhoffVariable.varList[num7].Scale, beckhoffVariable.varList[num7].Offset);
                                                    base.UpdateCurrentValue(beckhoffVariable);
                                                }
                                                continue;
                                            }
                                            continue;
                                        }
                                    case ComplexDataType.StringArray:
                                        {
                                            string[] array11 = this.beckhoff.ReadArray<string>(beckhoffVariable.VarAddress, beckhoffVariable.varList.Count);
                                            if (array11 != null && array11.Length == beckhoffVariable.varList.Count)
                                            {
                                                beckhoffVariable.Value = array11.ToString();
                                                for (int num8 = 0; num8 < beckhoffVariable.varList.Count; num8++)
                                                {
                                                    beckhoffVariable.varList[num8].Value = array11[num8];
                                                    base.UpdateCurrentValue(beckhoffVariable);
                                                }
                                                continue;
                                            }
                                            continue;
                                        }
                                    default:
                                        continue;
                                }
                                //IL_F95:
                                bool[] array12 = this.beckhoff.ReadArray<bool>(beckhoffVariable.VarAddress, beckhoffVariable.varList.Count);
                                if (array12 != null && array12.Length == beckhoffVariable.varList.Count)
                                {
                                    beckhoffVariable.Value = array12.ToString();
                                    for (int num9 = 0; num9 < beckhoffVariable.varList.Count; num9++)
                                    {
                                        beckhoffVariable.varList[num9].Value = array12[num9];
                                        base.UpdateCurrentValue(beckhoffVariable);
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
                        Beckhoff beckhoff = this.beckhoff;
                        if (beckhoff != null)
                        {
                            beckhoff.DisConnect();
                        }
                    }
                    this.beckhoff = new Beckhoff();
                    this.IsConnected = this.beckhoff.Connect(this.IpAddress, this.Port);
                    this.FirstConnect = false;
                }
            }
        }

        private int CalValue(BeckhoffVariable beckhoffVariable_0)
        {
            switch (beckhoffVariable_0.VarType)
            {
                case ComplexDataType.Bool:
                case ComplexDataType.Byte:
                case ComplexDataType.SByte:
                    return 1;
                case ComplexDataType.Short:
                case ComplexDataType.UShort:
                    return 2;
                case ComplexDataType.Int:
                case ComplexDataType.UInt:
                case ComplexDataType.Float:
                    return 4;
                case ComplexDataType.Double:
                case ComplexDataType.Long:
                case ComplexDataType.ULong:
                    return 8;
                case ComplexDataType.String:
                    if (beckhoffVariable_0.VarAddress.Contains("|"))
                    {
                        return Convert.ToInt32(beckhoffVariable_0.VarAddress.Split(new char[]
                        {
                        '|'
                        })[1]) + 1;
                    }
                    return 81;
                case ComplexDataType.BoolArray:
                case ComplexDataType.ByteArray:
                case ComplexDataType.SByteArray:
                    return beckhoffVariable_0.varList.Count;
                case ComplexDataType.ShortArray:
                case ComplexDataType.UShortArray:
                    return 2 * beckhoffVariable_0.varList.Count;
                case ComplexDataType.IntArray:
                case ComplexDataType.UIntArray:
                case ComplexDataType.FloatArray:
                    return 4 * beckhoffVariable_0.varList.Count;
                case ComplexDataType.DoubleArray:
                case ComplexDataType.LongArray:
                case ComplexDataType.ULongArray:
                    return 8 * beckhoffVariable_0.varList.Count;
                case ComplexDataType.StringArray:
                    if (beckhoffVariable_0.VarAddress.Contains("|"))
                    {
                        return (Convert.ToInt32(beckhoffVariable_0.VarAddress.Split(new char[]
                        {
                        '|'
                        })[1]) + 1) * beckhoffVariable_0.varList.Count;
                    }
                    return 81 * beckhoffVariable_0.varList.Count;
            }
            return 0;
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
                BeckhoffVariable beckhoffVariable = this.CurrentVarList[keyName] as BeckhoffVariable;
                CalResult<string> xktResult = Common.VerifyInputValue(beckhoffVariable, beckhoffVariable.VarType, setValue);
                if (xktResult.IsSuccess)
                {
                    result = this.beckhoff.Write(beckhoffVariable.VarAddress, xktResult.Content, beckhoffVariable.VarType);
                }
                else
                {
                    result = xktResult;
                }
            }
            return result;
        }

        public CancellationTokenSource cts;

        public Stopwatch sw;

        public List<BeckhoffDeviceGroup> DeviceGroupList;

        public Beckhoff beckhoff;
    }
}
