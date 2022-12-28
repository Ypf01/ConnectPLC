using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Group
{
	public class AIBUSGroup : NodeClass, IXmlConvert
	{
		public AIBUSGroup()
		{
			this.varList = new List<AIBUSVariable>();
			base.Name = "组对象";
			base.Description = "宇电变量组名称";
			this.DelayTime = 5U;
			base.NodeType = 100;
			base.NodeHead = "CustomGroup";
			this.Type = "AIBUS";
			this.DevID = 1;
			this.IsActive = true;
		}
        
		public uint DelayTime { get; set; }
        
		public byte DevID { get; set; }
        
		public string Type { get; set; }
        
		public bool IsActive { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.Type = element.Attribute("Type").Value;
			this.DevID = byte.Parse(element.Attribute("DevID").Value);
			this.DelayTime = uint.Parse(element.Attribute("DelayTime").Value);
			this.IsActive = bool.Parse(element.Attribute("IsActive").Value);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("Type", this.Type);
			xelement.SetAttributeValue("DelayTime", this.DelayTime);
			xelement.SetAttributeValue("DevID", this.DevID);
			xelement.SetAttributeValue("IsActive", this.IsActive);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("激活情况", this.IsActive ? "已激活" : "未激活"));
			nodeClassRenders.Add(new NodeClassRenderItem("延时时间", this.DelayTime.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("设备编号", this.DevID.ToString()));
			return nodeClassRenders;
		}
        
		public List<AIBUSVariable> varList;
	}
}
