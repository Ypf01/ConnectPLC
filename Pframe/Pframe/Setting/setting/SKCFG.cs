using System;
using System.Collections.Generic;
using System.Xml;
using Pframe.Common;
using NodeSettings.Node.Custom;
using NodeSettings.Node.Group;
using NodeSettings.Node.Variable;

namespace NodeSettings
{
    /// <summary>
    /// xml文本操作
    /// </summary>
    public class SKCFG
    {
        /// <summary>
        /// 读取自定义协议Socket节点
        /// </summary>
        /// <param name="xml">文件地址</param>
        /// <returns></returns>
        public static List<NodeSk> LoadXmlFile(string xml)
        {
            List<NodeSk> list = new List<NodeSk>();
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xml);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("Settings");
                XmlNodeList childNodes = xmlNode.ChildNodes;
                foreach (object obj in childNodes)
                {
                    XmlNode xmlNode2 = (XmlNode)obj;
                    XmlElement xmlElement = xmlNode2 as XmlElement;
                    XmlNodeList childNodes2 = xmlElement.ChildNodes;
                    XmlNodeList childNodes3 = xmlElement.ChildNodes;
                    foreach (object obj2 in childNodes2)
                    {
                        XmlNode xmlNode3 = (XmlNode)obj2;

                        #region 当前项为自定义节点时
                        if (xmlNode3.Name == "CustomNode")
                        {
                            #region 当前节点类型为SK时
                            if (XMLCFG.XMLAttributeGetValue(xmlNode3, "CustomType") == 500000.ToString())
                            {
                                NodeSk nodeSK = new NodeSk
                                {
                                    Name = XMLCFG.XMLAttributeGetValue(xmlNode3, "Name"),
                                    Description = XMLCFG.XMLAttributeGetValue(xmlNode3, "Description"),
                                    IpAddress = XMLCFG.XMLAttributeGetValue(xmlNode3, "IpAddress"),
                                    Port = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "Port")),
                                    IsActive = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "IsActive")),
                                    KeyWay = (KeyWay)Enum.Parse(typeof(KeyWay), XMLCFG.XMLAttributeGetValue(xmlNode3, "KeyWay"), true),
                                    UseAlarmCheck = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "UseAlarmCheck")),
                                    ReConnectTime = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "ReConnectTime")),
                                    ConnectTimeOut = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "ConnectTimeOut")),
                                    ProtocolClass = (SkProtocol)Enum.Parse(typeof(SkProtocol), XMLCFG.XMLAttributeGetValue(xmlNode3, "SkProtocol")),
                                    UpdateRate = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "UpdateRate")),
                                    MaxErrorTimes = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "MaxErrorTimes"))
                                };
                                childNodes3 = xmlNode3.ChildNodes;
                                foreach (object obj3 in childNodes3)
                                {
                                    XmlNode xmlNode4 = (XmlNode)obj3;
                                    SkGroup skgroup = new SkGroup
                                    {
                                        Name = XMLCFG.XMLAttributeGetValue(xmlNode4, "Name"),
                                        Description = XMLCFG.XMLAttributeGetValue(xmlNode4, "Description"),
                                        UpdateRate = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode4, "UpdateRate")),
                                        IsActive = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode4, "IsActive")),
                                        Code = XMLCFG.XMLAttributeGetValue(xmlNode4, "Code"),
                                        KeyArea = nodeSK.Name
                                    };
                                    foreach (object obj4 in xmlNode4)
                                    {
                                        XmlNode rootxml = (XmlNode)obj4;
                                        SKVariable skvariable = new SKVariable
                                        {
                                            Name = XMLCFG.XMLAttributeGetValue(rootxml, "Name"),
                                            Description = XMLCFG.XMLAttributeGetValue(rootxml, "Description"),
                                            VarAddress = XMLCFG.XMLAttributeGetValue(rootxml, "VarAddress"),
                                            VarType = (DataType)Enum.Parse(typeof(DataType), XMLCFG.XMLAttributeGetValue(rootxml, "VarType"), true),
                                            Scale = XMLCFG.XMLAttributeGetValue(rootxml, "Scale"),
                                            Offset = XMLCFG.XMLAttributeGetValue(rootxml, "Offset"),
                                            AccessProperty = (ReadWrite)Enum.Parse(typeof(ReadWrite), XMLCFG.XMLAttributeGetValue(rootxml, "AccessProperty"), true),
                                            DeviceVarName = string.Join(".", new string[]
                                            {
                                                nodeSK.Name,
                                                XMLCFG.XMLAttributeGetValue(rootxml, "Name")
                                            }),
                                            GroupVarName = string.Join(".", new string[]
                                            {
                                               skgroup.Name,
                                                XMLCFG.XMLAttributeGetValue(rootxml, "Name")
                                            }),
                                            DeviceGroupVarName = string.Join(".", new string[]
                                            {
                                                nodeSK.Name,
                                                skgroup.Name,
                                                XMLCFG.XMLAttributeGetValue(rootxml, "Name")
                                            }),
                                            KeyWay = nodeSK.KeyWay,
                                            KeyArea = skgroup.KeyArea
                                        };
                                        ConfigEnitity configEnitity = new ConfigEnitity
                                        {
                                            AlarmEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "AlarmEnable")),
                                            ArchiveEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "ArchiveEnable")),
                                            SetLimitEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "SetLimitEnable"))
                                        };
                                        if (configEnitity.AlarmEnable)
                                        {
                                            configEnitity.IsConditionAlarmType = bool.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "AlarmType"));
                                            configEnitity.DiscreteAlarmType = bool.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "DiscreteAlarmType"));
                                            configEnitity.DiscreteAlarmPriority = int.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "DiscreteAlarmPriority"));
                                            configEnitity.DiscreteAlarmNote = XMLCFG.XMLAttributeGetValue(rootxml, "DiscreteAlarmNote");
                                            configEnitity.LoLoAlarmEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "LoLoAlarmEnable"));
                                            configEnitity.LoLoAlarmValue = float.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "LoLoAlarmValue"));
                                            configEnitity.LoLoAlarmPriority = int.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "LoLoAlarmPriority"));
                                            configEnitity.LoLoAlarmNote = XMLCFG.XMLAttributeGetValue(rootxml, "LoLoAlarmNote");
                                            configEnitity.LowAlarmEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "LowAlarmEnable"));
                                            configEnitity.LowAlarmValue = float.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "LowAlarmValue"));
                                            configEnitity.LowAlarmPriority = int.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "LowAlarmPriority"));
                                            configEnitity.LowAlarmNote = XMLCFG.XMLAttributeGetValue(rootxml, "LowAlarmNote");
                                            configEnitity.HighAlarmEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "HighAlarmEnable"));
                                            configEnitity.HighAlarmValue = float.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "HighAlarmValue"));
                                            configEnitity.HighAlarmPriority = int.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "HighAlarmPriority"));
                                            configEnitity.HighAlarmNote = XMLCFG.XMLAttributeGetValue(rootxml, "HighAlarmNote");
                                            configEnitity.HiHiAlarmEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "HiHiAlarmEnable"));
                                            configEnitity.HiHiAlarmValue = float.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "HiHiAlarmValue"));
                                            configEnitity.HiHiAlarmPriority = int.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "HiHiAlarmPriority"));
                                            configEnitity.HiHiAlarmNote = XMLCFG.XMLAttributeGetValue(rootxml, "HiHiAlarmNote");
                                        }
                                        if (configEnitity.ArchiveEnable)
                                            configEnitity.ArchivePeriod = int.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "ArchivePeriod"));
                                        if (configEnitity.SetLimitEnable)
                                        {
                                            configEnitity.SetLimitMax = float.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "SetLimitMax"));
                                            configEnitity.SetLimitMin = float.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "SetLimitMin"));
                                        }
                                        skvariable.Config = configEnitity;
                                        skgroup.varList.Add(skvariable);
                                    }
                                    nodeSK.CustomGroupList.Add(skgroup);
                                }
                                list.Add(nodeSK);
                            }
                            #endregion
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
    }
}
