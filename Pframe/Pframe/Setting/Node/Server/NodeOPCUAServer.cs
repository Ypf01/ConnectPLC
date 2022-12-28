using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;

namespace NodeSettings.Node.Server
{
	public class NodeOPCUAServer : ServerNode, IXmlConvert
	{
		public NodeOPCUAServer()
		{
			this.ServerGroupList = new List<OPCUAServerGroup>();
			base.Name = "OPCUA Server";
			base.Description = "OPCUA服务器";
			base.ServerType = 20000;
			this.ServerURL = "127.0.0.1";
		}
        
		public string ServerURL { get; set; }
        
		public bool IsConnected { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.ServerURL = element.Attribute("ServerURL").Value;
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("ServerURL", this.ServerURL);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("ServerURL", this.ServerURL));
			nodeClassRenders.Add(new NodeClassRenderItem("激活情况", base.IsActive ? "已激活" : "未激活"));
			return nodeClassRenders;
		}
        
		public List<OPCUAServerGroup> ServerGroupList;
	}
}
