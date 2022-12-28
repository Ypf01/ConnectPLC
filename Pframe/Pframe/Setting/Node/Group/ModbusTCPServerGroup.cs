using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Group
{
	public class ModbusTCPServerGroup : NodeClass, IXmlConvert
	{
		public ModbusTCPServerGroup()
		{
			this.varList = new List<ModbusTCPServerVariable>();
			base.Name = "组对象";
			base.Description = "作为服务器的某个存储区";
			base.NodeType = 100;
			base.NodeHead = "ServerGroup";
			this.Type = "ModbusTCPServer";
			this.Start = "0";
			this.Length = 100;
		}
        
		public string StoreArea { get; set; }
        
		public string Type { get; set; }
        
		public string Start { get; set; }
        
		public int Length { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.Type = element.Attribute("Type").Value;
			this.StoreArea = element.Attribute("StoreArea").Value;
			this.Start = element.Attribute("Start").Value;
			this.Length = int.Parse(element.Attribute("Length").Value);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("Type", this.Type);
			xelement.SetAttributeValue("StoreArea", this.StoreArea);
			xelement.SetAttributeValue("Start", this.Start);
			xelement.SetAttributeValue("Length", this.Length);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("存储区", this.StoreArea));
			nodeClassRenders.Add(new NodeClassRenderItem("起始地址", this.Start));
			nodeClassRenders.Add(new NodeClassRenderItem("长度", this.Length.ToString()));
			return nodeClassRenders;
		}
        
		public List<ModbusTCPServerVariable> varList;
	}
}
