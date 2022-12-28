using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using NodeSettings.Node.NodeBase;

namespace NodeSettings.Node.Variable
{
	public class ModbusTCPServerVariable : NodeClass, IXmlConvert
	{
		public ModbusTCPServerVariable()
		{
			base.Name = "变量";
			base.Description = "具体的通讯变量地址及类型";
			this.VarAddress = "1";
			base.NodeType = 100;
			base.NodeHead = "Variable";
			this.Type = "ModbusTCPServer";
			this.Scale = "1";
		}
        
		public string VarAddress { get; set; }
        
		public string VarType { get; set; }
        
		public string Type { get; set; }
        
		public string Scale { get; set; }
        
		public object Value { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.Type = element.Attribute("Type").Value;
			this.VarAddress = element.Attribute("VarAddress").Value;
			this.VarType = element.Attribute("VarType").Value;
			this.Scale = element.Attribute("Scale").Value;
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("Type", this.Type);
			xelement.SetAttributeValue("VarAddress", this.VarAddress);
			xelement.SetAttributeValue("VarType", this.VarType);
			xelement.SetAttributeValue("Scale", this.Scale);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("变量地址", this.VarAddress));
			nodeClassRenders.Add(new NodeClassRenderItem("变量类型", this.VarType));
			nodeClassRenders.Add(new NodeClassRenderItem("比例系数", this.Scale.ToString()));
			List<NodeClassRenderItem> list = nodeClassRenders;
			string valueName = "当前数值";
			object value = this.Value;
			list.Add(new NodeClassRenderItem(valueName, (value != null) ? value.ToString() : null));
			return nodeClassRenders;
		}
        
	}
}
