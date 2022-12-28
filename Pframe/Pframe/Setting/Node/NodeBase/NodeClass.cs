using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace NodeSettings.Node.NodeBase
{
    /// <summary>
    /// Node基类
    /// </summary>
	public class NodeClass : IXmlConvert
	{   
		public NodeClass()
		{
			this.NodeType = 1;
			this.NodeHead = "NodeClass";
		}
        /// <summary>
        /// 主键索引,用于区分同类型PLC
        /// </summary>
        public string KeyArea { get; set; }
        /// <summary>
        /// 节点的名称，在节点上显示的
        /// </summary>
		public string Name { get; set; }
        /// <summary>
        ///  描述信息
        /// </summary>
		public string Description { get; set; }
        /// <summary>
        /// 节点的类型，标记其派生类不同的类型对象
        /// </summary>
		public int NodeType { get; protected set; }
        /// <summary>
        /// 节点的描述信息
        /// </summary>
		protected string NodeHead { get; set; }
        /// <summary>
        /// 从XML元素对象中获取对象属性
        /// </summary>
        /// <param name="element"></param>
		public virtual void LoadByXmlElement(XElement element)
		{
			this.Name = element.Attribute("Name").Value;
			this.Description = element.Attribute("Description").Value;
		}
        /// <summary>
        /// 将对象属性保存至XML元素对象
        /// </summary>
        /// <returns></returns>
		public virtual XElement ToXmlElement()
		{
			XElement xelement = new XElement(this.NodeHead);
			xelement.SetAttributeValue("Name", this.Name);
			xelement.SetAttributeValue("Description", this.Description);
			return xelement;
		}
        /// <summary>
        /// 获取用于在数据表信息中显示的键值数据对信息
        /// </summary>
        /// <returns></returns>
		public virtual List<NodeClassRenderItem> GetNodeClassRenders()
		{
			return new List<NodeClassRenderItem>
			{
				NodeClassRenderItem.CreatNodeName(this.Name),
				NodeClassRenderItem.CreateNodeDescription(this.Description)
			};
		}
        
	}
}
