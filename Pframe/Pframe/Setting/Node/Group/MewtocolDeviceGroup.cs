using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Group
{
	public class MewtocolDeviceGroup : NodeClass, IXmlConvert
	{
		public MewtocolDeviceGroup()
		{
			this.varList = new List<MewtocolVariable>();
			base.Name = "组对象";
			base.Description = "作为一次批量的字节数据读取";
			this.Length = 10;
			this.StoreArea = PanasonicStoreArea.D存储区;
			this.Start = "D0";
			base.NodeType = 100;
			base.NodeHead = "DeviceGroup";
			this.Type = "Mewtocol";
			this.IsActive = true;
		}
        
		public PanasonicStoreArea StoreArea { get; set; }
        
		public string Start { get; set; }
        
		public int Length { get; set; }
        
		public string Type { get; set; }
        
		public bool IsActive { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.Type = element.Attribute("Type").Value;
			this.StoreArea = (PanasonicStoreArea)Enum.Parse(typeof(PanasonicStoreArea), element.Attribute("StoreArea").Value, true);
			this.Length = int.Parse(element.Attribute("Length").Value);
			this.Start = element.Attribute("Start").Value;
			this.IsActive = bool.Parse(element.Attribute("IsActive").Value);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("Type", this.Type);
			xelement.SetAttributeValue("StoreArea", this.StoreArea);
			xelement.SetAttributeValue("Length", this.Length);
			xelement.SetAttributeValue("Start", this.Start);
			xelement.SetAttributeValue("IsActive", this.IsActive);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("激活情况", this.IsActive ? "已激活" : "未激活"));
			nodeClassRenders.Add(new NodeClassRenderItem("存储区", this.StoreArea.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("起始地址", this.Start));
			nodeClassRenders.Add(new NodeClassRenderItem("长度", this.Length.ToString()));
			return nodeClassRenders;
		}
        
		public List<MewtocolVariable> varList;
        
	}
}
