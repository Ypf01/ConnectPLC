using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using NodeSettings.Node.NodeBase;

namespace NodeSettings.Node.Server
{
	public class ServerNode : NodeClass
	{
		public ServerNode()
		{
			base.NodeType = 2;
			base.NodeHead = "ServerNode";
			this.CreateTime = DateTime.Now;
			this.ConnectTimeOut = 2000;
			this.InstallationDate = DateTime.Now;
			this.IsActive = true;
		}
        
		public int ServerType { get; set; }
        
		public DateTime InstallationDate { get; set; }
        
		public int ConnectTimeOut { get; set; }
        
		public DateTime CreateTime { get; set; }
        
		public bool IsActive { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.ServerType = int.Parse(element.Attribute("ServerType").Value);
			this.ConnectTimeOut = int.Parse(element.Attribute("ConnectTimeOut").Value);
			this.CreateTime = DateTime.Parse(element.Attribute("CreateTime").Value);
			this.InstallationDate = DateTime.Parse(element.Attribute("InstallationDate").Value);
			this.IsActive = bool.Parse(element.Attribute("IsActive").Value);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("ServerType", this.ServerType);
			xelement.SetAttributeValue("ConnectTimeOut", this.ConnectTimeOut);
			xelement.SetAttributeValue("CreateTime", this.CreateTime.ToString());
			xelement.SetAttributeValue("InstallationDate", this.InstallationDate.ToString());
			xelement.SetAttributeValue("IsActive", this.IsActive.ToString());
			return xelement;
		}
        
		public const int ModbusTCP = 10000;
        
		public const int OPCUA = 20000;
        
		public const int MQTTClient = 30000;
	}
}
