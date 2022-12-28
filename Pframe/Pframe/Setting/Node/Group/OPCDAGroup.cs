using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Group
{
	public class OPCDAGroup : NodeClass, IXmlConvert
	{
		public OPCDAGroup()
		{
			this.varList = new List<OPCDAVariable>();
			base.Name = "组对象";
			base.Description = "OPCDA变量组名称";
			this.UpdateRate = 250;
			base.NodeType = 100;
			base.NodeHead = "OPCGroup";
			this.Type = "OPCDA";
		}
        
		public int UpdateRate { get; set; }
        
		public int ClientHandle { get; set; }
        
		public string Type { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.Type = element.Attribute("Type").Value;
			this.UpdateRate = int.Parse(element.Attribute("UpdateRate").Value);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("Type", this.Type);
			xelement.SetAttributeValue("UpdateRate", this.UpdateRate);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("OPCDA类型", this.Type));
			nodeClassRenders.Add(new NodeClassRenderItem("更新速率", this.UpdateRate.ToString()));
			return nodeClassRenders;
		}
        
		public List<OPCDAVariable> varList;
	}
}
