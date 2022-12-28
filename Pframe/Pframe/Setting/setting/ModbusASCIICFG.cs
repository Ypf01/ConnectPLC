using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Xml;
using Pframe.Common;
using NodeSettings.Node.Group;
using NodeSettings.Node.Modbus;
using NodeSettings.Node.Variable;

namespace NodeSettings
{
	public class ModbusASCIICFG
	{
		public static List<NodeModbusASCII> LoadXmlFile(string xml)
		{
			List<NodeModbusASCII> list = new List<NodeModbusASCII>();
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
						if (xmlNode3.Name == "ModbusNode")
						{
							string a = XMLCFG.XMLAttributeGetValue(xmlNode3, "ModbusType");
							if (a == 3000.ToString())
							{
								NodeModbusASCII nodeModbusASCII = new NodeModbusASCII
								{
									Name = XMLCFG.XMLAttributeGetValue(xmlNode3, "Name"),
									Description = XMLCFG.XMLAttributeGetValue(xmlNode3, "Description"),
									PortNum = XMLCFG.XMLAttributeGetValue(xmlNode3, "PortNum"),
									Paud = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "Paud")),
									Parity = (Parity)Enum.Parse(typeof(Parity), XMLCFG.XMLAttributeGetValue(xmlNode3, "Parity"), true),
									DataBits = XMLCFG.XMLAttributeGetValue(xmlNode3, "DataBits"),
									StopBits = (StopBits)Enum.Parse(typeof(StopBits), XMLCFG.XMLAttributeGetValue(xmlNode3, "StopBits"), true),
									IsActive = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "IsActive")),
									SleepTime = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "SleepTime")),
									DataFormat = (DataFormat)Enum.Parse(typeof(DataFormat), XMLCFG.XMLAttributeGetValue(xmlNode3, "DataFormat"), true),
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
									ModbusASCIIGroup modbusASCIIGroup = new ModbusASCIIGroup
									{
										IsActive = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode4, "IsActive")),
										Name = XMLCFG.XMLAttributeGetValue(xmlNode4, "Name"),
										Description = XMLCFG.XMLAttributeGetValue(xmlNode4, "Description"),
										SlaveID = byte.Parse(XMLCFG.XMLAttributeGetValue(xmlNode4, "SlaveID")),
										StoreArea = (ModbusStoreArea)Enum.Parse(typeof(ModbusStoreArea), XMLCFG.XMLAttributeGetValue(xmlNode4, "StoreArea"), true),
										Start = ushort.Parse(XMLCFG.XMLAttributeGetValue(xmlNode4, "Start")),
										Length = ushort.Parse(XMLCFG.XMLAttributeGetValue(xmlNode4, "Length"))
									};
									foreach (object obj4 in xmlNode4)
									{
										XmlNode rootxml = (XmlNode)obj4;
										ModbusASCIIVariable gclass = new ModbusASCIIVariable
										{
											Name = XMLCFG.XMLAttributeGetValue(rootxml, "Name"),
											Description = XMLCFG.XMLAttributeGetValue(rootxml, "Description"),
											VarType = (DataType)Enum.Parse(typeof(DataType), XMLCFG.XMLAttributeGetValue(rootxml, "VarType"), true),
											VarAddress = XMLCFG.XMLAttributeGetValue(rootxml, "VarAddress"),
											Scale = XMLCFG.XMLAttributeGetValue(rootxml, "Scale"),
											Offset = XMLCFG.XMLAttributeGetValue(rootxml, "Offset"),
											AccessProperty = (ReadWrite)Enum.Parse(typeof(ReadWrite), XMLCFG.XMLAttributeGetValue(rootxml, "AccessProperty"), true),
											GroupID = modbusASCIIGroup.SlaveID,
											DeviceVarName = string.Join(".", new string[]
											{
												nodeModbusASCII.Name,
												XMLCFG.XMLAttributeGetValue(rootxml, "Name")
											}),
											GroupVarName = string.Join(".", new string[]
											{
												modbusASCIIGroup.Name,
												XMLCFG.XMLAttributeGetValue(rootxml, "Name")
											}),
											DeviceGroupVarName = string.Join(".", new string[]
											{
												nodeModbusASCII.Name,
												modbusASCIIGroup.Name,
												XMLCFG.XMLAttributeGetValue(rootxml, "Name")
											}),
											KeyWay = nodeModbusASCII.KeyWay
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
										gclass.Config = configEnitity;
										modbusASCIIGroup.varList.Add(gclass);
									}
									nodeModbusASCII.ModbusASCIIGroupList.Add(modbusASCIIGroup);
								}
								list.Add(nodeModbusASCII);
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
