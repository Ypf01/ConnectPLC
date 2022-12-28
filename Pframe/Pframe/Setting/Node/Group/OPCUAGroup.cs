﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Group
{
	public class OPCUAGroup : NodeClass, IXmlConvert
	{
		public OPCUAGroup()
		{
			this.varList = new List<OPCUAVariable>();
			base.Name = "组对象";
			base.Description = "OPCUA变量组名称";
			this.UpdateRate = 250;
			this.ClientHandle = 1;
			base.NodeType = 100;
			base.NodeHead = "OPCGroup";
			this.Type = "OPCUA";
		}
        
		public int UpdateRate { get; set; }
        
		public int ClientHandle { get; set; }
        
		public string Type { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.ClientHandle = int.Parse(element.Attribute("ClientHandle").Value);
			this.Type = element.Attribute("Type").Value;
			this.UpdateRate = int.Parse(element.Attribute("UpdateRate").Value);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("ClientHandle", this.ClientHandle);
			xelement.SetAttributeValue("Type", this.Type);
			xelement.SetAttributeValue("UpdateRate", this.UpdateRate);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("Client句柄", this.ClientHandle.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("OPCUA类型", this.Type));
			nodeClassRenders.Add(new NodeClassRenderItem("更新速率", this.UpdateRate.ToString()));
			return nodeClassRenders;
		}
        
		public List<OPCUAVariable> varList;
	}
}
