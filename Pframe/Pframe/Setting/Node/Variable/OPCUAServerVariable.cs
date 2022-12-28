using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using NodeSettings.Node.NodeBase;

namespace NodeSettings.Node.Variable
{
	public class OPCUAServerVariable : VariableNode, IXmlConvert
	{
		public OPCUAServerVariable()
		{
			base.Name = "变量";
			base.Description = "具体的通讯变量地址及类型";
			base.NodeType = 100;
			base.NodeHead = "Variable";
			base.Type = "OPCUAServer";
			base.Scale = "1";
		}
        
		public string VarType { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			base.Type = element.Attribute("Type").Value;
			base.Scale = element.Attribute("Scale").Value;
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("Type", base.Type);
			xelement.SetAttributeValue("VarType", this.VarType);
			xelement.SetAttributeValue("Scale", base.Scale);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("变量名称", base.Name));
			nodeClassRenders.Add(new NodeClassRenderItem("比例系数", base.Scale.ToString()));
			List<NodeClassRenderItem> list = nodeClassRenders;
			string valueName = "当前数值";
			object value = base.Value;
			list.Add(new NodeClassRenderItem(valueName, (value != null) ? value.ToString() : null));
			return nodeClassRenders;
		}
        
	}
}
