using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Group
{
	public class ModbusRTUOverUDPGroup : NodeClass, IXmlConvert
	{
		public ModbusRTUOverUDPGroup()
		{
			this.varList = new List<ModbusRTUOverUDPVariable>();
			base.Name = "组对象";
			base.Description = "作为一次批量的线圈或寄存器数据读取";
			this.StoreArea = ModbusStoreArea.保持寄存器;
			this.Start = 0;
			this.Length = 10;
			this.SlaveID = 1;
			base.NodeType = 100;
			base.NodeHead = "ModbusGroup";
			this.Type = "ModbusRTUOverUDP";
			this.IsActive = true;
		}
        
		public ModbusStoreArea StoreArea { get; set; }
        
		public ushort Start { get; set; }
        
		public ushort Length { get; set; }
        
		public byte SlaveID { get; set; }
        
		public string Type { get; set; }
        
		public bool IsActive { get; set; }
        
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.Type = element.Attribute("Type").Value;
			this.StoreArea = (ModbusStoreArea)Enum.Parse(typeof(ModbusStoreArea), element.Attribute("StoreArea").Value, true);
			this.Length = ushort.Parse(element.Attribute("Length").Value);
			this.Start = ushort.Parse(element.Attribute("Start").Value);
			this.SlaveID = byte.Parse(element.Attribute("SlaveID").Value);
			this.IsActive = bool.Parse(element.Attribute("IsActive").Value);
		}
        
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("Type", this.Type);
			xelement.SetAttributeValue("StoreArea", this.StoreArea);
			xelement.SetAttributeValue("Length", this.Length);
			xelement.SetAttributeValue("Start", this.Start);
			xelement.SetAttributeValue("SlaveID", this.SlaveID);
			xelement.SetAttributeValue("IsActive", this.IsActive);
			return xelement;
		}
        
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("激活情况", this.IsActive ? "已激活" : "未激活"));
			nodeClassRenders.Add(new NodeClassRenderItem("存储区", this.StoreArea.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("起始地址", this.Start.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("长度", this.Length.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("从站地址", this.SlaveID.ToString()));
			return nodeClassRenders;
		}
        
		public List<ModbusRTUOverUDPVariable> varList;
	}
}
