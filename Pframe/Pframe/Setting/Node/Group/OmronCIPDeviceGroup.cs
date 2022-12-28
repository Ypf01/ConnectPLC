using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Group
{
    public class OmronCIPDeviceGroup : NodeClass, IXmlConvert
    {
        /// <summary>
        /// 构造方法
        /// </summary>
		public OmronCIPDeviceGroup()
        {
            this.varList = new List<OmronCIPVariable>();
            this.VariableList = new List<OmronCIPVariable>();
            this.VarNameList = new List<string>();
            Name = "组对象";
            Description = "作为一次批量的字节数据读取";
            NodeType = 100;
            NodeHead = "DeviceGroup";
            this.Type = "OmronCIP";
            this.IsActive = true;
        }

        /// <summary>
        /// 变量名称集合
        /// </summary>
		public List<string> VarNameList { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
		public string Type { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
		public bool IsActive { get; set; }
        /// <summary>
        /// 从XML元素对象中获取对象属性
        /// </summary>
        /// <param name="element">元素对象</param>
		public override void LoadByXmlElement(XElement element)
        {
            base.LoadByXmlElement(element);
            this.Type = element.Attribute("Type").Value;
            this.IsActive = bool.Parse(element.Attribute("IsActive").Value);
        }
        /// <summary>
        /// 将对象属性保存至XML元素对象
        /// </summary>
        /// <returns>元素对象</returns>
		public override XElement ToXmlElement()
        {
            XElement xelement = base.ToXmlElement();
            xelement.SetAttributeValue("Type", this.Type);
            xelement.SetAttributeValue("IsActive", this.IsActive);
            return xelement;
        }
        /// <summary>
        /// 获取节点信息
        /// </summary>
        /// <returns>节点信息集合</returns>
		public override List<NodeClassRenderItem> GetNodeClassRenders()
        {
            List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
            nodeClassRenders.Add(new NodeClassRenderItem("激活情况", this.IsActive ? "已激活" : "未激活"));
            return nodeClassRenders;
        }
        /// <summary>
        /// 变量集合
        /// </summary>
		public List<OmronCIPVariable> varList;
        /// <summary>
        /// 所有变量集合
        /// </summary>
		public List<OmronCIPVariable> VariableList;

    }
}
