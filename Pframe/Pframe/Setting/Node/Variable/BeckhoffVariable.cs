using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Pframe.Common;
using NodeSettings.Node.NodeBase;

namespace NodeSettings.Node.Variable
{
	public class BeckhoffVariable : VariableNode, IXmlConvert
	{
		public BeckhoffVariable()
		{
			this.varList = new List<BeckhoffVariable>();
			base.VarAddress = "BoolTest";
			base.Type = "Beckhoff";
			this.VarType = ComplexDataType.Bool;
			this.IndexOffset = 0;
		}
        
		public ComplexDataType VarType { get; set; }
        
		public int IndexOffset { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.VarType = (ComplexDataType)Enum.Parse(typeof(ComplexDataType), element.Attribute("VarType").Value, true);
			this.IndexOffset = int.Parse(element.Attribute("IndexOffset").Value);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("VarType", this.VarType);
			xelement.SetAttributeValue("IndexOffset", this.IndexOffset);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("变量类型", this.VarType.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("索引偏移", this.IndexOffset.ToString()));
			List<NodeClassRenderItem> list = nodeClassRenders;
			string valueName = "当前数值";
			object value = base.Value;
			list.Add(new NodeClassRenderItem(valueName, (value != null) ? value.ToString() : null));
			return nodeClassRenders;
		}
        
		public List<BeckhoffVariable> varList;
        
	}
}
