using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Pframe.Common;
using NodeSettings.Node.NodeBase;

namespace NodeSettings.Node.Variable
{
	public class ABCIPVariable : VariableNode, IXmlConvert
	{
		public ABCIPVariable()
		{
			this.varList = new List<ABCIPVariable>();
			base.VarAddress = "BoolTest";
			base.Type = "ABCIP";
			this.VarType = ComplexDataType.Bool;
		}
        
		public ComplexDataType VarType { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.VarType = (ComplexDataType)Enum.Parse(typeof(ComplexDataType), element.Attribute("VarType").Value, true);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("VarType", this.VarType);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("变量类型", this.VarType.ToString()));
			List<NodeClassRenderItem> list = nodeClassRenders;
			string valueName = "当前数值";
			object value = base.Value;
			list.Add(new NodeClassRenderItem(valueName, (value != null) ? value.ToString() : null));
			return nodeClassRenders;
		}
        
		public List<ABCIPVariable> varList;
        
	}
}
