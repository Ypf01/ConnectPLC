using System;
using System.Collections.Generic;
using System.Xml;
using NodeSettings.Node.Group;
using NodeSettings.Node.Server;
using NodeSettings.Node.Variable;

namespace NodeSettings
{
	public class ModbusTCPServerCFG
	{
		public static List<NodeModbusTCPServer> LoadXmlFile(string xml)
		{
			List<NodeModbusTCPServer> list = new List<NodeModbusTCPServer>();
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
						if (xmlNode3.Name == "ServerNode")
						{
							string a = XMLCFG.XMLAttributeGetValue(xmlNode3, "ServerType");
							if (a == 10000.ToString())
							{
								NodeModbusTCPServer gclass = new NodeModbusTCPServer
								{
									Name = XMLCFG.XMLAttributeGetValue(xmlNode3, "Name"),
									Description = XMLCFG.XMLAttributeGetValue(xmlNode3, "Description"),
                                    ServerURL = XMLCFG.XMLAttributeGetValue(xmlNode3, "ServerURL"),
									Port = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "Port")),
									IsActive = bool.Parse(XMLCFG.XMLAttributeGetValue(xmlNode3, "IsActive"))
								};
								childNodes3 = xmlNode3.ChildNodes;
								foreach (object obj3 in childNodes3)
								{
									XmlNode xmlNode4 = (XmlNode)obj3;
									ModbusTCPServerGroup gclass2 = new ModbusTCPServerGroup
									{
										Name = XMLCFG.XMLAttributeGetValue(xmlNode4, "Name"),
										StoreArea = XMLCFG.XMLAttributeGetValue(xmlNode4, "StoreArea"),
										Start = XMLCFG.XMLAttributeGetValue(xmlNode4, "Start"),
										Length = int.Parse(XMLCFG.XMLAttributeGetValue(xmlNode4, "Length"))
									};
									foreach (object obj4 in xmlNode4)
									{
										XmlNode rootxml = (XmlNode)obj4;
										ModbusTCPServerVariable item = new ModbusTCPServerVariable
										{
											Name = XMLCFG.XMLAttributeGetValue(rootxml, "Name"),
											VarType = XMLCFG.XMLAttributeGetValue(rootxml, "VarType"),
											VarAddress = XMLCFG.XMLAttributeGetValue(rootxml, "VarAddress"),
											Scale = XMLCFG.XMLAttributeGetValue(rootxml, "Scale")
										};
										gclass2.varList.Add(item);
									}
									gclass.ServerGroupList.Add(gclass2);
								}
								list.Add(gclass);
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
