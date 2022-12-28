using System;
using System.Collections.Generic;
using System.Xml;
using Pframe.Common;
using NodeSettings.Node.Device;
using NodeSettings.Node.Group;
using NodeSettings.Node.Variable;

namespace NodeSettings
{
    public class OmronCIPCFG
    {
        public static List<NodeOmronCIP> LoadXmlFile(string xml)
        {
            List<NodeOmronCIP> list = new List<NodeOmronCIP>();
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
                            if (a == 160.ToString())
                            {
                                NodeOmronCIP nodeOmronCIP = new NodeOmronCIP
                                {
                                    Name = XMLCFG.XMLAttributeGetValue(xmlNode3, "Name"),
                                    Description = XMLCFG.XMLAttributeGetValue(xmlNode3, "Description"),
                                    IpAddress = XMLCFG.XMLAttributeGetValue(xmlNode3, "IpAddress"),
                                    Port = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "Port")),
                                    Slot = byte.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "Slot")),
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
                                    OmronCIPDeviceGroup gclass = new OmronCIPDeviceGroup
                                    {
                                        IsActive = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode4, "IsActive")),
                                        Name = XMLCFG.XMLAttributeGetValue(xmlNode4, "Name"),
                                        Description = XMLCFG.XMLAttributeGetValue(xmlNode4, "Description"),
                                        KeyArea = nodeOmronCIP.Name
                                    };
                                    OmronCIPCFG.GetOmronCIPGroupNode(nodeOmronCIP.Name, xmlNode4, gclass, nodeOmronCIP.KeyWay);
                                    nodeOmronCIP.DeviceGroupList.Add(gclass);
                                }
                                list.Add(nodeOmronCIP);
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

        private static void GetOmronCIPGroupNode(string devName, XmlNode group, OmronCIPDeviceGroup gp, KeyWay keyWay)
        {
            foreach (object obj in group)
            {
                XmlNode xmlNode = (XmlNode)obj;
                OmronCIPVariable omronCIPVariable = new OmronCIPVariable
                {
                    Name = XMLCFG.XMLAttributeGetValue(xmlNode, "Name"),
                    Description = XMLCFG.XMLAttributeGetValue(xmlNode, "Description"),
                    VarAddress = XMLCFG.XMLAttributeGetValue(xmlNode, "VarAddress"),
                    VarType = (ComplexDataType)Enum.Parse(typeof(ComplexDataType), XMLCFG.XMLAttributeGetValue(xmlNode, "VarType"), true),
                    Scale = XMLCFG.XMLAttributeGetValue(xmlNode, "Scale"),
                    Offset = XMLCFG.XMLAttributeGetValue(xmlNode, "Offset"),
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
                    KeyWay = keyWay,
                    KeyArea = gp.KeyArea
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
                omronCIPVariable.Config = configEnitity;
                gp.varList.Add(omronCIPVariable);
                if (xmlNode.HasChildNodes)
                    OmronCIPCFG.GetOmronCIPVariableNode(devName, xmlNode, gp, omronCIPVariable, keyWay);
                else
                {
                    gp.VarNameList.Add(omronCIPVariable.VarAddress);
                    gp.VariableList.Add(omronCIPVariable);
                }
            }
        }

        private static void GetOmronCIPVariableNode(string devName, XmlNode group, OmronCIPDeviceGroup gp, OmronCIPVariable cIPVariable, KeyWay keyWay)
        {
            foreach (object obj in group.ChildNodes)
            {
                XmlNode xmlNode = (XmlNode)obj;
                OmronCIPVariable omronCIPVariable = new OmronCIPVariable
                {
                    Name = XMLCFG.XMLAttributeGetValue(xmlNode, "Name"),
                    Description = XMLCFG.XMLAttributeGetValue(xmlNode, "Description"),
                    VarAddress = XMLCFG.XMLAttributeGetValue(xmlNode, "VarAddress"),
                    VarType = (ComplexDataType)Enum.Parse(typeof(ComplexDataType), XMLCFG.XMLAttributeGetValue(xmlNode, "VarType"), true),
                    Scale = XMLCFG.XMLAttributeGetValue(xmlNode, "Scale"),
                    Offset = XMLCFG.XMLAttributeGetValue(xmlNode, "Offset"),
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
                    KeyWay = keyWay,
                    KeyArea= gp.KeyArea
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
                omronCIPVariable.Config = configEnitity;
                gp.varList.Add(omronCIPVariable);
                cIPVariable.varList.Add(omronCIPVariable);
                if (xmlNode.HasChildNodes)
                    OmronCIPCFG.GetOmronCIPVariableNode(devName, xmlNode, gp, omronCIPVariable, keyWay);
                else
                {
                    gp.VarNameList.Add(omronCIPVariable.VarAddress);
                    gp.VariableList.Add(omronCIPVariable);
                }
            }
        }
    }
}
