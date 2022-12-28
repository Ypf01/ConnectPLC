using System;
using System.Collections.Generic;
using System.Xml;
using Pframe.Common;
using NodeSettings.Node.Device;
using NodeSettings.Node.Group;
using NodeSettings.Node.Variable;

namespace NodeSettings
{
	public class OmronCFG
	{
		public static List<NodeOmron> LoadXmlFile(string xml)
		{
			List<NodeOmron> list = new List<NodeOmron>();
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
							if (a == 40.ToString())
							{
								NodeOmron nodeOmron = new NodeOmron
								{
									Name = XMLCFG.XMLAttributeGetValue(xmlNode3, "Name"),
									Description = XMLCFG.XMLAttributeGetValue(xmlNode3, "Description"),
									IpAddress = XMLCFG.XMLAttributeGetValue(xmlNode3, "IpAddress"),
									Port = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "Port")),
									SA1 = byte.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "SA1")),
									DA1 = byte.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "DA1")),
									DA2 = byte.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "DA2")),
									PlcType = (OmronProtocol)Enum.Parse(typeof(OmronProtocol), XMLCFG.XMLAttributeGetValue(xmlNode3, "PlcType"), true),
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
									OmronDeviceGroup omronDeviceGroup = new OmronDeviceGroup
									{
										IsActive = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode4, "IsActive")),
										Name = XMLCFG.XMLAttributeGetValue(xmlNode4, "Name"),
										Description = XMLCFG.XMLAttributeGetValue(xmlNode4, "Description"),
										StoreArea = (OmronStoreArea)Enum.Parse(typeof(OmronStoreArea), XMLCFG.XMLAttributeGetValue(xmlNode4, "StoreArea"), true),
										Start = XMLCFG.XMLAttributeGetValue(xmlNode4, "Start"),
										Length = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode4, "Length"))
									};
									foreach (object obj4 in xmlNode4)
									{
										XmlNode rootxml = (XmlNode)obj4;
										OmronVariable omronVariable = new OmronVariable
										{
											Name = XMLCFG.XMLAttributeGetValue(rootxml, "Name"),
											Description = XMLCFG.XMLAttributeGetValue(rootxml, "Description"),
											VarAddress = XMLCFG.XMLAttributeGetValue(rootxml, "VarAddress"),
											VarType = (DataType)Enum.Parse(typeof(DataType), XMLCFG.XMLAttributeGetValue(rootxml, "VarType"), true),
											Start = XMLCFG.XMLAttributeGetValue(rootxml, "Start"),
											Scale = XMLCFG.XMLAttributeGetValue(rootxml, "Scale"),
											Offset = XMLCFG.XMLAttributeGetValue(rootxml, "Offset"),
											AccessProperty = (ReadWrite)Enum.Parse(typeof(ReadWrite), XMLCFG.XMLAttributeGetValue(rootxml, "AccessProperty"), true),
											DeviceVarName = string.Join(".", new string[]
											{
												nodeOmron.Name,
												XMLCFG.XMLAttributeGetValue(rootxml, "Name")
											}),
											GroupVarName = string.Join(".", new string[]
											{
												omronDeviceGroup.Name,
												XMLCFG.XMLAttributeGetValue(rootxml, "Name")
											}),
											DeviceGroupVarName = string.Join(".", new string[]
											{
												nodeOmron.Name,
												omronDeviceGroup.Name,
												XMLCFG.XMLAttributeGetValue(rootxml, "Name")
											}),
											KeyWay = nodeOmron.KeyWay
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
										{
											configEnitity.ArchivePeriod = int.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "ArchivePeriod"));
										}
										if (configEnitity.SetLimitEnable)
										{
											configEnitity.SetLimitMax = float.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "SetLimitMax"));
											configEnitity.SetLimitMin = float.Parse(XMLCFG.XMLAttributeGetValue(rootxml, "SetLimitMin"));
										}
										omronVariable.Config = configEnitity;
										omronDeviceGroup.varList.Add(omronVariable);
									}
									nodeOmron.DeviceGroupList.Add(omronDeviceGroup);
								}
								list.Add(nodeOmron);
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
