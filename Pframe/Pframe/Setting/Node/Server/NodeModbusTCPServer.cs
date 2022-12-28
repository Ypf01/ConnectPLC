using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Pframe;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;

namespace NodeSettings.Node.Server
{
	public class NodeModbusTCPServer : ServerNode, IXmlConvert
	{
		public NodeModbusTCPServer()
		{
			this.ServerGroupList = new List<ModbusTCPServerGroup>();
			base.Name = "ModbusTCP Server";
			base.Description = "ModbusTCP服务器";
			base.ServerType = 10000;
			this.ServerURL = "127.0.0.1";
			this.Port = 502;
		}
        
		public string ServerURL { get; set; }
        
		public string DataFormat { get; set; }
        
		public int Port { get; set; }
        
		public int OnlineCount { get; set; }
        
		public bool IsConnected { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.ServerURL = element.Attribute("ServerURL").Value;
			this.Port = int.Parse(element.Attribute("Port").Value);
			this.DataFormat = element.Attribute("DataFormat").Value;
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("ServerURL", this.ServerURL);
			xelement.SetAttributeValue("Port", this.Port);
			xelement.SetAttributeValue("DataFormat", this.DataFormat);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("IP地址", this.ServerURL));
			nodeClassRenders.Add(new NodeClassRenderItem("端口号", this.Port.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("数据格式", this.DataFormat));
			nodeClassRenders.Add(new NodeClassRenderItem("激活情况", base.IsActive ? "已激活" : "未激活"));
			nodeClassRenders.Add(new NodeClassRenderItem("连接情况", this.IsConnected ? "已连接" : "未连接"));
			nodeClassRenders.Add(new NodeClassRenderItem("在线客户端", this.OnlineCount.ToString()));
			return nodeClassRenders;
		}
        
		public ModbusTcpServer modtcpserver;
        
		public List<ModbusTCPServerGroup> ServerGroupList;
	}
}
