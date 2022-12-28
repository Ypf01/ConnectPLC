using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Group
{
	public class OPCUAServerGroup : NodeClass, IXmlConvert
	{
		public OPCUAServerGroup()
		{
			this.varList = new List<OPCUAServerVariable>();
			base.Name = "组对象";
			base.Description = "作为服务器的某个存储区";
			base.NodeType = 100;
			base.NodeHead = "ServerGroup";
			this.Type = "OPCUAServer";
		}
        
		public string Type { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.Type = element.Attribute("Type").Value;
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("Type", this.Type);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			return base.GetNodeClassRenders();
		}
        
		public List<OPCUAServerVariable> varList;
	}
}
