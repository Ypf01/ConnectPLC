using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Pframe.Common;
using NodeSettings.Node.NodeBase;

namespace NodeSettings.Node.Variable
{
	public class OmronCIPVariable : VariableNode, IXmlConvert
	{
        /// <summary>
        /// 构造方法
        /// </summary>
		public OmronCIPVariable()
		{
			this.varList = new List<OmronCIPVariable>();
			base.VarAddress = "BoolTest";
			base.Type = "OmronCIP";
			this.VarType = ComplexDataType.Bool;
		}

        /// <summary>
        /// 变量类型
        /// </summary>
		public ComplexDataType VarType { get; set; }
        /// <summary>
        /// 根据Xml元素读取
        /// </summary>
        /// <param name="element"></param>
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.VarType = (ComplexDataType)Enum.Parse(typeof(ComplexDataType), element.Attribute("VarType").Value, true);
		}
        /// <summary>
        /// 转换成Xml元素
        /// </summary>
        /// <returns></returns>
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("VarType", this.VarType);
			return xelement;
		}
        /// <summary>
        /// 获取节点信息
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 变量集合
        /// </summary>
		public List<OmronCIPVariable> varList;
	}
}
