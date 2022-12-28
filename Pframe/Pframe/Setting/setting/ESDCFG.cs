using System;
using System.Collections.Generic;
using System.Xml;
using Pframe.Common;
using NodeSettings.Node.Custom;
using NodeSettings.Node.Group;
using NodeSettings.Node.Variable;

namespace NodeSettings
{
    public class ESDCFG
    {
        public static List<NodeESD> LoadXmlFile(string xml)
        {
            List<NodeESD> list = new List<NodeESD>();
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
                        if (xmlNode3.Name == "CustomNode")
                        {
                            string a = XMLCFG.XMLAttributeGetValue(xmlNode3, "CustomType");
                            if (a == 100000.ToString())
                            {
                                NodeESD nodeESD = new NodeESD
                                {
                                    Name = XMLCFG.XMLAttributeGetValue(xmlNode3, "Name"),
                                    Description = XMLCFG.XMLAttributeGetValue(xmlNode3, "Description"),
                                    IpAddress = XMLCFG.XMLAttributeGetValue(xmlNode3, "IpAddress"),
                                    Port = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "Port")),
                                    ESDType = XMLCFG.XMLAttributeGetValue(xmlNode3, "ESDType"),
                                    MaxVoltage = float.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "MaxVoltage")),
                                    MaxCurrent = float.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "MaxCurrent")),
                                    IsActive = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "IsActive")),
                                    KeyWay = (KeyWay)Enum.Parse(typeof(KeyWay), XMLCFG.XMLAttributeGetValue(xmlNode3, "KeyWay"), true),
                                    UseAlarmCheck = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "UseAlarmCheck"))
                                };
                                childNodes3 = xmlNode3.ChildNodes;
                                foreach (object obj3 in childNodes3)
                                {
                                    XmlNode xmlNode4 = (XmlNode)obj3;
                                    ESDGroup esdgroup = new ESDGroup
                                    {
                                        Name = XMLCFG.XMLAttributeGetValue(xmlNode4, "Name"),
                                        Description = XMLCFG.XMLAttributeGetValue(xmlNode4, "Description"),
                                        UpdateRate = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode4, "UpdateRate"))
                                    };
                                    foreach (object obj4 in xmlNode4)
                                    {
                                        XmlNode rootxml = (XmlNode)obj4;
                                        ESDVariable esdvariable = new ESDVariable
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
                                                nodeESD.Name,
                                                XMLCFG.XMLAttributeGetValue(rootxml, "Name")
                                            }),
                                            GroupVarName = string.Join(".", new string[]
                                            {
                                                esdgroup.Name,
                                                XMLCFG.XMLAttributeGetValue(rootxml, "Name")
                                            }),
                                            DeviceGroupVarName = string.Join(".", new string[]
                                            {
                                                nodeESD.Name,
                                                esdgroup.Name,
                                                XMLCFG.XMLAttributeGetValue(rootxml, "Name")
                                            }),
                                            KeyWay = nodeESD.KeyWay
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
                                        esdvariable.Config = configEnitity;
                                        esdgroup.varList.Add(esdvariable);
                                    }
                                    nodeESD.CustomGroupList.Add(esdgroup);
                                }
                                list.Add(nodeESD);
                            }
                        }
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
