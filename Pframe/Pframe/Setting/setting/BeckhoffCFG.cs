using System;
using System.Collections.Generic;
using System.Xml;
using Pframe.Common;
using NodeSettings.Node.Device;
using NodeSettings.Node.Group;
using NodeSettings.Node.Variable;

namespace NodeSettings
{
    public class BeckhoffCFG
    {
        public static List<NodeBeckhoff> LoadXmlFile(string xml)
        {
            List<NodeBeckhoff> list = new List<NodeBeckhoff>();
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
                        if (xmlNode3.Name == "DeviceNode")
                        {
                            string a = XMLCFG.XMLAttributeGetValue(xmlNode3, "DeviceType");
                            if (a == 130.ToString())
                            {
                                NodeBeckhoff nodeBeckhoff = new NodeBeckhoff
                                {
                                    Name = XMLCFG.XMLAttributeGetValue(xmlNode3, "Name"),
                                    Description = XMLCFG.XMLAttributeGetValue(xmlNode3, "Description"),
                                    IpAddress = XMLCFG.XMLAttributeGetValue(xmlNode3, "IpAddress"),
                                    Port = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "Port")),
                                    IsActive = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "IsActive")),
                                    ConnectTimeOut = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "ConnectTimeOut")),
                                    ReConnectTime = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "ReConnectTime")),
                                    MaxErrorTimes = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "MaxErrorTimes")),
                                    KeyWay = (KeyWay)Enum.Parse(typeof(KeyWay), XMLCFG.XMLAttributeGetValue(xmlNode3, "KeyWay"), true),
                                    UseAlarmCheck = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "UseAlarmCheck"))
                                };
                                childNodes3 = xmlNode3.ChildNodes;
                                foreach (object obj3 in childNodes3)
                                {
                                    XmlNode xmlNode4 = (XmlNode)obj3;
                                    BeckhoffDeviceGroup beckhoffDeviceGroup = new BeckhoffDeviceGroup
                                    {
                                        IsActive = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode4, "IsActive")),
                                        Name = XMLCFG.XMLAttributeGetValue(xmlNode4, "Name"),
                                        Description = XMLCFG.XMLAttributeGetValue(xmlNode4, "Description")
                                    };
                                    BeckhoffCFG.GetBeckhoffGroupNode(xmlNode3.Name, xmlNode4, beckhoffDeviceGroup, nodeBeckhoff.KeyWay);
                                    nodeBeckhoff.DeviceGroupList.Add(beckhoffDeviceGroup);
                                }
                                list.Add(nodeBeckhoff);
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

        private static void GetBeckhoffGroupNode(string devName, XmlNode group, BeckhoffDeviceGroup gp, KeyWay keyWay)
        {
            foreach (object obj in group)
            {
                XmlNode xmlNode = (XmlNode)obj;
                BeckhoffVariable beckhoffVariable = new BeckhoffVariable
                {
                    Name = XMLCFG.XMLAttributeGetValue(xmlNode, "Name"),
                    Description = XMLCFG.XMLAttributeGetValue(xmlNode, "Description"),
                    VarAddress = XMLCFG.XMLAttributeGetValue(xmlNode, "VarAddress"),
                    VarType = (ComplexDataType)Enum.Parse(typeof(ComplexDataType), XMLCFG.XMLAttributeGetValue(xmlNode, "VarType"), true),
                    Scale = XMLCFG.XMLAttributeGetValue(xmlNode, "Scale"),
                    Offset = XMLCFG.XMLAttributeGetValue(xmlNode, "Offset"),
                    IndexOffset = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "IndexOffset")),
                    AccessProperty = (ReadWrite)Enum.Parse(typeof(ReadWrite), XMLCFG.XMLAttributeGetValue(xmlNode, "AccessProperty"), true),
                    DeviceVarName = string.Join(".", new string[]
                    {
                        devName,
                        XMLCFG.XMLAttributeGetValue(xmlNode, "Name")
                    }),
                    GroupVarName = string.Join(".", new string[]
                    {
                        gp.Name,
                        XMLCFG.XMLAttributeGetValue(xmlNode, "Name")
                    }),
                    DeviceGroupVarName = string.Join(".", new string[]
                    {
                        devName,
                        gp.Name,
                        XMLCFG.XMLAttributeGetValue(xmlNode, "Name")
                    }),
                    KeyWay = keyWay
                };
                ConfigEnitity configEnitity = new ConfigEnitity
                {
                    AlarmEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "AlarmEnable")),
                    ArchiveEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "ArchiveEnable")),
                    SetLimitEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "SetLimitEnable"))
                };
                if (configEnitity.AlarmEnable)
                {
                    configEnitity.IsConditionAlarmType = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "AlarmType"));
                    configEnitity.DiscreteAlarmType = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "DiscreteAlarmType"));
                    configEnitity.DiscreteAlarmPriority = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "DiscreteAlarmPriority"));
                    configEnitity.DiscreteAlarmNote = XMLCFG.XMLAttributeGetValue(xmlNode, "DiscreteAlarmNote");
                    configEnitity.LoLoAlarmEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "LoLoAlarmEnable"));
                    configEnitity.LoLoAlarmValue = float.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "LoLoAlarmValue"));
                    configEnitity.LoLoAlarmPriority = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "LoLoAlarmPriority"));
                    configEnitity.LoLoAlarmNote = XMLCFG.XMLAttributeGetValue(xmlNode, "LoLoAlarmNote");
                    configEnitity.LowAlarmEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "LowAlarmEnable"));
                    configEnitity.LowAlarmValue = float.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "LowAlarmValue"));
                    configEnitity.LowAlarmPriority = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "LowAlarmPriority"));
                    configEnitity.LowAlarmNote = XMLCFG.XMLAttributeGetValue(xmlNode, "LowAlarmNote");
                    configEnitity.HighAlarmEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "HighAlarmEnable"));
                    configEnitity.HighAlarmValue = float.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "HighAlarmValue"));
                    configEnitity.HighAlarmPriority = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "HighAlarmPriority"));
                    configEnitity.HighAlarmNote = XMLCFG.XMLAttributeGetValue(xmlNode, "HighAlarmNote");
                    configEnitity.HiHiAlarmEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "HiHiAlarmEnable"));
                    configEnitity.HiHiAlarmValue = float.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "HiHiAlarmValue"));
                    configEnitity.HiHiAlarmPriority = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "HiHiAlarmPriority"));
                    configEnitity.HiHiAlarmNote = XMLCFG.XMLAttributeGetValue(xmlNode, "HiHiAlarmNote");
                }
                if (configEnitity.ArchiveEnable)
                    configEnitity.ArchivePeriod = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "ArchivePeriod"));
                if (configEnitity.SetLimitEnable)
                {
                    configEnitity.SetLimitMax = float.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "SetLimitMax"));
                    configEnitity.SetLimitMin = float.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "SetLimitMin"));
                }
                beckhoffVariable.Config = configEnitity;
                gp.varList.Add(beckhoffVariable);
                if (xmlNode.HasChildNodes)
                    BeckhoffCFG.GetBeckhoffVariableNode(devName, xmlNode, beckhoffVariable, keyWay);
            }
        }

        private static void GetBeckhoffVariableNode(string devName, XmlNode group, BeckhoffVariable beckVariable, KeyWay keyWay)
        {
            foreach (object obj in group.ChildNodes)
            {
                XmlNode xmlNode = (XmlNode)obj;
                BeckhoffVariable beckhoffVariable = new BeckhoffVariable
                {
                    Name = XMLCFG.XMLAttributeGetValue(xmlNode, "Name"),
                    Description = XMLCFG.XMLAttributeGetValue(xmlNode, "Description"),
                    VarAddress = XMLCFG.XMLAttributeGetValue(xmlNode, "VarAddress"),
                    VarType = (ComplexDataType)Enum.Parse(typeof(ComplexDataType), XMLCFG.XMLAttributeGetValue(xmlNode, "VarType"), true),
                    Scale = XMLCFG.XMLAttributeGetValue(xmlNode, "Scale"),
                    Offset = XMLCFG.XMLAttributeGetValue(xmlNode, "Offset"),
                    IndexOffset = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "IndexOffset")),
                    AccessProperty = (ReadWrite)Enum.Parse(typeof(ReadWrite), XMLCFG.XMLAttributeGetValue(xmlNode, "AccessProperty"), true),
                    DeviceVarName = string.Join(".", new string[]
                    {
                        devName,
                        XMLCFG.XMLAttributeGetValue(xmlNode, "Name")
                    }),
                    GroupVarName = string.Join(".", new string[]
                    {
                        group.Name,
                        XMLCFG.XMLAttributeGetValue(xmlNode, "Name")
                    }),
                    DeviceGroupVarName = string.Join(".", new string[]
                    {
                        devName,
                        group.Name,
                        XMLCFG.XMLAttributeGetValue(xmlNode, "Name")
                    }),
                    KeyWay = keyWay
                };
                ConfigEnitity configEnitity = new ConfigEnitity
                {
                    AlarmEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "AlarmEnable")),
                    ArchiveEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "ArchiveEnable")),
                    SetLimitEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "SetLimitEnable"))
                };
                if (configEnitity.AlarmEnable)
                {
                    configEnitity.IsConditionAlarmType = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "AlarmType"));
                    configEnitity.DiscreteAlarmType = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "DiscreteAlarmType"));
                    configEnitity.DiscreteAlarmPriority = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "DiscreteAlarmPriority"));
                    configEnitity.DiscreteAlarmNote = XMLCFG.XMLAttributeGetValue(xmlNode, "DiscreteAlarmNote");
                    configEnitity.LoLoAlarmEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "LoLoAlarmEnable"));
                    configEnitity.LoLoAlarmValue = float.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "LoLoAlarmValue"));
                    configEnitity.LoLoAlarmPriority = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "LoLoAlarmPriority"));
                    configEnitity.LoLoAlarmNote = XMLCFG.XMLAttributeGetValue(xmlNode, "LoLoAlarmNote");
                    configEnitity.LowAlarmEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "LowAlarmEnable"));
                    configEnitity.LowAlarmValue = float.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "LowAlarmValue"));
                    configEnitity.LowAlarmPriority = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "LowAlarmPriority"));
                    configEnitity.LowAlarmNote = XMLCFG.XMLAttributeGetValue(xmlNode, "LowAlarmNote");
                    configEnitity.HighAlarmEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "HighAlarmEnable"));
                    configEnitity.HighAlarmValue = float.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "HighAlarmValue"));
                    configEnitity.HighAlarmPriority = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "HighAlarmPriority"));
                    configEnitity.HighAlarmNote = XMLCFG.XMLAttributeGetValue(xmlNode, "HighAlarmNote");
                    configEnitity.HiHiAlarmEnable = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "HiHiAlarmEnable"));
                    configEnitity.HiHiAlarmValue = float.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "HiHiAlarmValue"));
                    configEnitity.HiHiAlarmPriority = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "HiHiAlarmPriority"));
                    configEnitity.HiHiAlarmNote = XMLCFG.XMLAttributeGetValue(xmlNode, "HiHiAlarmNote");
                }
                if (configEnitity.ArchiveEnable)
                    configEnitity.ArchivePeriod = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "ArchivePeriod"));
                if (configEnitity.SetLimitEnable)
                {
                    configEnitity.SetLimitMax = float.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "SetLimitMax"));
                    configEnitity.SetLimitMin = float.Parse(XMLCFG.XMLAttributeGetValue(xmlNode, "SetLimitMin"));
                }
                beckhoffVariable.Config = configEnitity;
                beckVariable.varList.Add(beckhoffVariable);
                if (xmlNode.HasChildNodes)
                {
                    BeckhoffCFG.GetBeckhoffVariableNode(devName, xmlNode, beckhoffVariable, keyWay);
                }
            }
        }

    }
}
