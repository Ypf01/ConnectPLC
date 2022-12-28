using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Group
{
	public class ABCIPDeviceGroup : NodeClass, IXmlConvert
	{
		public ABCIPDeviceGroup()
		{
			this.varList = new List<ABCIPVariable>();
			this.VariableList = new List<ABCIPVariable>();
			this.VarNameList = new List<string>();
			base.Name = "组对象";
			base.Description = "作为一次批量的字节数据读取";
			base.NodeType = 100;
			base.NodeHead = "DeviceGroup";
			this.Type = "ABCIP";
			this.IsActive = true;
		}
        
		public List<string> VarNameList { get; set; }
        
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
        
		public List<ABCIPVariable> varList;
        
		public List<ABCIPVariable> VariableList;
        
	}
}
