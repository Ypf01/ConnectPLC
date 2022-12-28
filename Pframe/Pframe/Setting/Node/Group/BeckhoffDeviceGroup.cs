using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Group
{
	public class BeckhoffDeviceGroup : NodeClass, IXmlConvert
	{
		public BeckhoffDeviceGroup()
		{
			this.varList = new List<BeckhoffVariable>();
			base.Name = "组对象";
			base.Description = "倍福PLC的通信组";
			base.NodeType = 100;
			base.NodeHead = "DeviceGroup";
			this.Type = "Beckhoff";
			this.IsActive = true;
		}
        
		public string Type { get; set; }
        
		public bool IsActive { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.Type = element.Attribute("Type").Value;
			this.IsActive = bool.Parse(element.Attribute("IsActive").Value);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("Type", this.Type);
			xelement.SetAttributeValue("IsActive", this.IsActive);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("激活情况", this.IsActive ? "已激活" : "未激活"));
			return nodeClassRenders;
		}
        
		public List<BeckhoffVariable> varList;       
	}
}
