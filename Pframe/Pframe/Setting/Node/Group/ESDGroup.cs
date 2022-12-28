using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Group
{
    /// <summary>
    /// ESD组对象
    /// </summary>
	public class ESDGroup : NodeClass, IXmlConvert
	{
		public ESDGroup()
		{
			this.varList = new List<ESDVariable>();
			base.Name = "组对象";
			base.Description = "ESD变量组名称";
			this.UpdateRate = 250;
			base.NodeType = 100;
			base.NodeHead = "CustomGroup";
			this.Type = "ESD";
		}
        /// <summary>
        /// 更新周期
        /// </summary>
		public int UpdateRate { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
		public string Type { get; set; }
        /// <summary>
        ///  根据Xml元素读取
        /// </summary>
        /// <param name="element"></param>
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.Type = element.Attribute("Type").Value;
			this.UpdateRate = int.Parse(element.Attribute("UpdateRate").Value);
		}
        /// <summary>
        /// 转换成Xml元素
        /// </summary>
        /// <returns></returns>
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("Type", this.Type);
			xelement.SetAttributeValue("UpdateRate", this.UpdateRate);
			return xelement;
		}
        /// <summary>
        /// 获取节点信息
        /// </summary>
        /// <returns></returns>
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("Custom类型", this.Type));
			nodeClassRenders.Add(new NodeClassRenderItem("更新速率", this.UpdateRate.ToString()));
			return nodeClassRenders;
		}
        /// <summary>
        ///  变量集合
        /// </summary>
		public List<ESDVariable> varList;
	}
}
