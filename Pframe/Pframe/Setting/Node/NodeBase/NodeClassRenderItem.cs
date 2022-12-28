using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NodeSettings.Node.NodeBase
{
    /// <summary>
    /// NodeClass类目信息
    /// </summary>
	public class NodeClassRenderItem
	{
        /// <summary>
        ///  实例化一个默认的对象
        /// </summary>
		public NodeClassRenderItem()
		{
			this.ValueName = "Name";
			this.Value = "Value";
		}
        /// <summary>
        /// 实例化一个对象，需要指定当前的键值信息
        /// </summary>
        /// <param name="valueName">名称</param>
        /// <param name="value">值</param>
		public NodeClassRenderItem(string valueName, string value)
		{
			this.ValueName = valueName;
			this.Value = value;
		}
        /// <summary>
        /// 数据名称
        /// </summary>
		public string ValueName { get; set; }
        /// <summary>
        /// 数据值
        /// </summary>
		public string Value { get; set; }
        /// <summary>
        /// 创建一个显示的节点对象
        /// </summary>
        /// <param name="value">节点值</param>
        /// <returns>键值对象</returns>
		public static NodeClassRenderItem CreatNodeName(string value)
		{
			return new NodeClassRenderItem("节点名称", value);
		}
        /// <summary>
        /// 创建一个显示描述的键值对象
        /// </summary>
        /// <param name="description">描述信息</param>
        /// <returns>键值对象</returns>
		public static NodeClassRenderItem CreateNodeDescription(string description)
		{
			return new NodeClassRenderItem("节点描述", description);
		}
        
	}
}
