using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using NodeSettings.Node.NodeBase;

namespace NodeSettings.Node.Variable
{
    /// <summary>
    /// 变量节点
    /// </summary>
	public class VariableNode : NodeClass
	{
		public VariableNode()
		{
			this.KeyWay = KeyWay.VarName;
			//this.Config = new ConfigEnitity();
			base.NodeType = 100;
			base.NodeHead = "Variable";
			base.Name = "变量";
			base.Description = "具体的通讯变量地址及类型";
			this.Start = "0";
			this.Scale = "1";
			this.Offset = "0";
			this.AccessProperty = ReadWrite.只读;
		}
		public string GroupVarName { get; set; }
        
		public string DeviceVarName { get; set; }
        
		public string DeviceGroupVarName { get; set; }
        /// <summary>
        /// 键名
        /// </summary>
		public string KeyName
		{
			get
			{
				string result;
				switch (this.KeyWay)
				{
				case KeyWay.VarName:
					result = base.Name;
					break;
				case KeyWay.VarAddress:
					result = this.VarAddress;
					break;
				case KeyWay.VarDescription:
					result = base.Description;
					break;
				case KeyWay.GroupVarName:
					result = this.GroupVarName;
					break;
				case KeyWay.DeviceVarName:
					result = this.DeviceVarName;
					break;
				case KeyWay.DeviceGroupVarName:
					result = this.DeviceGroupVarName;
					break;
				default:
					result = base.Name;
					break;
				}
				return result;
			}
		}
        /// <summary>
        /// 使用键的形式
        /// </summary>
		public KeyWay KeyWay { get; set; }
        /// <summary>
        /// 变量地址
        /// </summary>
		public string VarAddress { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
		public string Type { get; set; }
        /// <summary>
        /// 开始地址
        /// </summary>
		public string Start { get; set; }
        /// <summary>
        /// 比例
        /// </summary>
		public string Scale { get; set; }
        /// <summary>
        /// 偏移
        /// </summary>
		public string Offset { get; set; }
        /// <summary>
        /// 变量值
        /// </summary>
		public object Value { get; set; }
        /// <summary>
        /// 读写
        /// </summary>
		public ReadWrite AccessProperty { get; set; }
        /// <summary>
        /// 从XML元素对象中获取对象属性
        /// </summary>
        /// <param name="element"></param>
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			base.Name = element.Attribute("Name").Value;
			base.Description = element.Attribute("Description").Value;
			this.Type = element.Attribute("Type").Value;
			this.VarAddress = element.Attribute("VarAddress").Value;
			this.Scale = element.Attribute("Scale").Value;
			this.Offset = element.Attribute("Offset").Value;
			this.Start = element.Attribute("Start").Value;
			this.AccessProperty = (ReadWrite)Enum.Parse(typeof(ReadWrite), element.Attribute("AccessProperty").Value, true);
			this.Config.AlarmEnable = bool.Parse(element.Attribute("AlarmEnable").Value);
			this.Config.ArchiveEnable = bool.Parse(element.Attribute("ArchiveEnable").Value);
			this.Config.SetLimitEnable = bool.Parse(element.Attribute("SetLimitEnable").Value);
			if (this.Config.AlarmEnable)
			{
				this.Config.IsConditionAlarmType = bool.Parse(element.Attribute("AlarmType").Value);
				this.Config.DiscreteAlarmType = bool.Parse(element.Attribute("DiscreteAlarmType").Value);
				this.Config.DiscreteAlarmPriority = int.Parse(element.Attribute("DiscreteAlarmPriority").Value);
				this.Config.DiscreteAlarmNote = element.Attribute("DiscreteAlarmNote").Value;
				this.Config.LoLoAlarmEnable = bool.Parse(element.Attribute("LoLoAlarmEnable").Value);
				this.Config.LoLoAlarmValue = float.Parse(element.Attribute("LoLoAlarmValue").Value);
				this.Config.LoLoAlarmPriority = int.Parse(element.Attribute("LoLoAlarmPriority").Value);
				this.Config.LoLoAlarmNote = element.Attribute("LoLoAlarmNote").Value;
				this.Config.LowAlarmEnable = bool.Parse(element.Attribute("LowAlarmEnable").Value);
				this.Config.LowAlarmValue = float.Parse(element.Attribute("LowAlarmValue").Value);
				this.Config.LowAlarmPriority = int.Parse(element.Attribute("LowAlarmPriority").Value);
				this.Config.LowAlarmNote = element.Attribute("LowAlarmNote").Value;
				this.Config.HighAlarmEnable = bool.Parse(element.Attribute("HighAlarmEnable").Value);
				this.Config.HighAlarmValue = float.Parse(element.Attribute("HighAlarmValue").Value);
				this.Config.HighAlarmPriority = int.Parse(element.Attribute("HighAlarmPriority").Value);
				this.Config.HighAlarmNote = element.Attribute("HighAlarmNote").Value;
				this.Config.HiHiAlarmEnable = bool.Parse(element.Attribute("HiHiAlarmEnable").Value);
				this.Config.HiHiAlarmValue = float.Parse(element.Attribute("HiHiAlarmValue").Value);
				this.Config.HiHiAlarmPriority = int.Parse(element.Attribute("HiHiAlarmPriority").Value);
				this.Config.HiHiAlarmNote = element.Attribute("HiHiAlarmNote").Value;
			}
			if (this.Config.ArchiveEnable)
				this.Config.ArchivePeriod = int.Parse(element.Attribute("ArchivePeriod").Value);
			if (this.Config.SetLimitEnable)
			{
				this.Config.SetLimitMax = float.Parse(element.Attribute("SetLimitMax").Value);
				this.Config.SetLimitMin = float.Parse(element.Attribute("SetLimitMin").Value);
			}
		}
        /// <summary>
        /// 将对象属性保存至XML元素对象
        /// </summary>
        /// <returns></returns>
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("Type", this.Type);
			xelement.SetAttributeValue("VarAddress", this.VarAddress);
			xelement.SetAttributeValue("Scale", this.Scale);
			xelement.SetAttributeValue("Offset", this.Offset);
			xelement.SetAttributeValue("Start", this.Start);
			xelement.SetAttributeValue("AccessProperty", this.AccessProperty);
			xelement.SetAttributeValue("AlarmEnable", this.Config.AlarmEnable.ToString());
			xelement.SetAttributeValue("ArchiveEnable", this.Config.ArchiveEnable.ToString());
			xelement.SetAttributeValue("SetLimitEnable", this.Config.SetLimitEnable.ToString());
			if (this.Config.AlarmEnable)
			{
				xelement.SetAttributeValue("AlarmType", this.Config.IsConditionAlarmType.ToString());
				xelement.SetAttributeValue("DiscreteAlarmType", this.Config.DiscreteAlarmType.ToString());
				xelement.SetAttributeValue("DiscreteAlarmPriority", this.Config.DiscreteAlarmPriority.ToString());
				xelement.SetAttributeValue("DiscreteAlarmNote", this.Config.DiscreteAlarmNote.ToString());
				xelement.SetAttributeValue("LoLoAlarmEnable", this.Config.LoLoAlarmEnable.ToString());
				xelement.SetAttributeValue("LoLoAlarmValue", this.Config.LoLoAlarmValue.ToString());
				xelement.SetAttributeValue("LoLoAlarmPriority", this.Config.LoLoAlarmPriority.ToString());
				xelement.SetAttributeValue("LoLoAlarmNote", this.Config.LoLoAlarmNote.ToString());
				xelement.SetAttributeValue("LowAlarmEnable", this.Config.LowAlarmEnable.ToString());
				xelement.SetAttributeValue("LowAlarmValue", this.Config.LowAlarmValue.ToString());
				xelement.SetAttributeValue("LowAlarmPriority", this.Config.LowAlarmPriority.ToString());
				xelement.SetAttributeValue("LowAlarmNote", this.Config.LowAlarmNote.ToString());
				xelement.SetAttributeValue("HighAlarmEnable", this.Config.HighAlarmEnable.ToString());
				xelement.SetAttributeValue("HighAlarmValue", this.Config.HighAlarmValue.ToString());
				xelement.SetAttributeValue("HighAlarmPriority", this.Config.HighAlarmPriority.ToString());
				xelement.SetAttributeValue("HighAlarmNote", this.Config.HighAlarmNote.ToString());
				xelement.SetAttributeValue("HiHiAlarmEnable", this.Config.HiHiAlarmEnable.ToString());
				xelement.SetAttributeValue("HiHiAlarmValue", this.Config.HiHiAlarmValue.ToString());
				xelement.SetAttributeValue("HiHiAlarmPriority", this.Config.HiHiAlarmPriority.ToString());
				xelement.SetAttributeValue("HiHiAlarmNote", this.Config.HiHiAlarmNote.ToString());
			}
			if (this.Config.ArchiveEnable)
			{
				xelement.SetAttributeValue("ArchivePeriod", this.Config.ArchivePeriod.ToString());
			}
			if (this.Config.SetLimitEnable)
			{
				xelement.SetAttributeValue("SetLimitEnable", this.Config.SetLimitEnable.ToString());
				xelement.SetAttributeValue("SetLimitMax", this.Config.SetLimitMax.ToString());
				xelement.SetAttributeValue("SetLimitMin", this.Config.SetLimitMin.ToString());
			}
			return xelement;
		}
        /// <summary>
        /// 获取用于在数据表信息中显示的键值数据对信息
        /// </summary>
        /// <returns></returns>
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("变量地址", this.VarAddress));
			nodeClassRenders.Add(new NodeClassRenderItem("比例系数", this.Scale.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("偏移量", this.Offset.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("开始字节", this.Start));
			nodeClassRenders.Add(new NodeClassRenderItem("读写属性", this.AccessProperty.ToString()));
			if (this.Config.AlarmEnable)
			{
				nodeClassRenders.Add(new NodeClassRenderItem("报警启用", this.Config.AlarmEnable ? "启用" : "禁用"));
				nodeClassRenders.Add(new NodeClassRenderItem("报警类型", this.Config.IsConditionAlarmType ? "条件报警" : "离散报警"));
				if (!this.Config.IsConditionAlarmType)
				{
					nodeClassRenders.Add(new NodeClassRenderItem("离散报警类型", this.Config.DiscreteAlarmType ? "上升沿" : "下降沿"));
					nodeClassRenders.Add(new NodeClassRenderItem("离散报警优先级", this.Config.DiscreteAlarmPriority.ToString()));
					nodeClassRenders.Add(new NodeClassRenderItem("离散报警说明", this.Config.DiscreteAlarmNote));
				}
				else
				{
					if (this.Config.LoLoAlarmEnable)
					{
						nodeClassRenders.Add(new NodeClassRenderItem("LoLo启用", this.Config.LoLoAlarmEnable ? "启用" : "禁用"));
						nodeClassRenders.Add(new NodeClassRenderItem("LoLo报警值", this.Config.LoLoAlarmValue.ToString()));
						nodeClassRenders.Add(new NodeClassRenderItem("LoLo优先级", this.Config.LoLoAlarmPriority.ToString()));
						nodeClassRenders.Add(new NodeClassRenderItem("LoLo报警说明", this.Config.LoLoAlarmNote));
					}
					if (this.Config.LowAlarmEnable)
					{
						nodeClassRenders.Add(new NodeClassRenderItem("Low启用", this.Config.LowAlarmEnable ? "启用" : "禁用"));
						nodeClassRenders.Add(new NodeClassRenderItem("Low报警值", this.Config.LowAlarmValue.ToString()));
						nodeClassRenders.Add(new NodeClassRenderItem("Low优先级", this.Config.LowAlarmPriority.ToString()));
						nodeClassRenders.Add(new NodeClassRenderItem("Low报警说明", this.Config.LowAlarmNote));
					}
					if (this.Config.HighAlarmEnable)
					{
						nodeClassRenders.Add(new NodeClassRenderItem("High启用", this.Config.HighAlarmEnable ? "启用" : "禁用"));
						nodeClassRenders.Add(new NodeClassRenderItem("High报警值", this.Config.HighAlarmValue.ToString()));
						nodeClassRenders.Add(new NodeClassRenderItem("High优先级", this.Config.HighAlarmPriority.ToString()));
						nodeClassRenders.Add(new NodeClassRenderItem("High报警说明", this.Config.HighAlarmNote));
					}
					if (this.Config.HiHiAlarmEnable)
					{
						nodeClassRenders.Add(new NodeClassRenderItem("HiHi启用", this.Config.HiHiAlarmEnable ? "启用" : "禁用"));
						nodeClassRenders.Add(new NodeClassRenderItem("HiHi报警值", this.Config.HiHiAlarmValue.ToString()));
						nodeClassRenders.Add(new NodeClassRenderItem("HiHi优先级", this.Config.HiHiAlarmPriority.ToString()));
						nodeClassRenders.Add(new NodeClassRenderItem("HiHi报警说明", this.Config.HiHiAlarmNote));
					}
				}
			}
			if (this.Config.ArchiveEnable)
			{
				nodeClassRenders.Add(new NodeClassRenderItem("归档启用", this.Config.ArchiveEnable ? "启用" : "禁用"));
				nodeClassRenders.Add(new NodeClassRenderItem("归档周期", string.Concat(new string[]
				{
					(this.Config.ArchivePeriod / 3600).ToString(),
					"小时",
					(this.Config.ArchivePeriod % 3600 / 60).ToString(),
					"分钟",
					(this.Config.ArchivePeriod % 60).ToString(),
					"秒"
				})));
			}
			if (this.Config.SetLimitEnable)
			{
				nodeClassRenders.Add(new NodeClassRenderItem("设定限制启用", this.Config.SetLimitEnable ? "启用" : "禁用"));
				nodeClassRenders.Add(new NodeClassRenderItem("设定高限", this.Config.SetLimitMax.ToString()));
				nodeClassRenders.Add(new NodeClassRenderItem("设定低限", this.Config.SetLimitMin.ToString()));
			}
			return nodeClassRenders;
		}
        /// <summary>
        /// 配置信息
        /// </summary>
		public ConfigEnitity Config;
	}
}
