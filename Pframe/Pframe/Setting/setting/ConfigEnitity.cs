using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NodeSettings
{
    /// <summary>
    /// 变量报警、归档、设定等信息的实体类
    /// </summary>
	public class ConfigEnitity
	{
        /// <summary>
        /// 是否启动报警
        /// </summary>
        public bool AlarmEnable { get; set; }
        /// <summary>
        /// 归档是否启用
        /// </summary>
        public bool ArchiveEnable { get; set; }
        /// <summary>
        /// 设定限值是否启用
        /// </summary>
        public bool SetLimitEnable { get; set; }
        /// <summary>
        /// 设定值最大值
        /// </summary>
        public float SetLimitMax { get; set; }
        /// <summary>
        /// 设定值最小值
        /// </summary>
        public float SetLimitMin { get; set; }
        /// <summary>
        /// 报警类型，True为条件报警，False为离散报警
        /// </summary>
		public bool IsConditionAlarmType { get; set; }
        /// <summary>
        /// 离散报警类型，True为上升沿报警，False为下降沿报警
        /// </summary>
		public bool DiscreteAlarmType { get; set; }
        /// <summary>
        /// 离散报警上升缓存值
        /// </summary>
		public bool DiscreteRiseCacheValue { get; set; }
        /// <summary>
        /// 离散报警下降缓存值
        /// </summary>
        public bool DiscreteFallCacheValue { get; set; }
        /// <summary>
        /// 离散报警优先级
        /// </summary>
        public int DiscreteAlarmPriority { get; set; }
        /// <summary>
        /// 离散报警说明
        /// </summary>
        public string DiscreteAlarmNote { get; set; }
        /// <summary>
        /// 低低报警是否启用
        /// </summary>
        public bool LoLoAlarmEnable { get; set; }
        /// <summary>
        /// 低低报警设定值
        /// </summary>
        public float LoLoAlarmValue { get; set; }
        /// <summary>
        /// 低低报警缓存值
        /// </summary>
        public float LoLoCacheValue { get; set; }
        /// <summary>
        /// 低低报警优先级
        /// </summary>
        public int LoLoAlarmPriority { get; set; }
        /// <summary>
        /// 低低报警说明
        /// </summary>
        public string LoLoAlarmNote { get; set; }
        /// <summary>
        /// 低报警是否启用
        /// </summary>
        public bool LowAlarmEnable { get; set; }
        /// <summary>
        /// 低报警设定值
        /// </summary>
        public float LowAlarmValue { get; set; }
        /// <summary>
        /// 低报警缓存值
        /// </summary>
        public float LowCacheValue { get; set; }
        /// <summary>
        /// 低报警优先级
        /// </summary>
        public int LowAlarmPriority { get; set; }
        /// <summary>
        /// 低报警说明
        /// </summary>
        public string LowAlarmNote { get; set; }
        /// <summary>
        /// 高报警是否启用
        /// </summary>
        public bool HighAlarmEnable { get; set; }
        /// <summary>
        /// 高报警设定值
        /// </summary>
        public float HighAlarmValue { get; set; }
        /// <summary>
        /// 高报警缓存值
        /// </summary>
        public float HighCacheValue { get; set; }
        /// <summary>
        /// 高报警优先级
        /// </summary>
        public int HighAlarmPriority { get; set; }
        /// <summary>
        /// 高报警说明
        /// </summary>
        public string HighAlarmNote { get; set; }
        /// <summary>
        /// 高高报警是否启用
        /// </summary>
        public bool HiHiAlarmEnable { get; set; }
        /// <summary>
        /// 高高报警设定值
        /// </summary>
        public float HiHiAlarmValue { get; set; }
        /// <summary>
        /// 高高报警缓存值
        /// </summary>
        public float HiHiCacheValue { get; set; }
        /// <summary>
        /// 高高报警优先级
        /// </summary>
        public int HiHiAlarmPriority { get; set; }
        /// <summary>
        /// 高高报警说明
        /// </summary>
        public string HiHiAlarmNote { get; set; }
        /// <summary>
        /// 归档时间
        /// </summary>
        public int ArchivePeriod { get; set; }
		public ConfigEnitity()
		{
			this.AlarmEnable = false;
			this.ArchiveEnable = false;
			this.SetLimitEnable = false;
			this.SetLimitMax = 100f;
			this.SetLimitMin = 0f;
			this.IsConditionAlarmType = false;
			this.DiscreteAlarmType = false;
			this.DiscreteRiseCacheValue = false;
			this.DiscreteFallCacheValue = true;
			this.DiscreteAlarmPriority = 0;
			this.DiscreteAlarmNote = string.Empty;
			this.LoLoAlarmEnable = false;
			this.LoLoAlarmValue = 0f;
			this.LoLoCacheValue = float.MaxValue;
			this.LoLoAlarmPriority = 0;
			this.LoLoAlarmNote = string.Empty;
			this.LowAlarmEnable = false;
			this.LowAlarmValue = 0f;
			this.LowCacheValue = float.MaxValue;
			this.LowAlarmPriority = 0;
			this.LowAlarmNote = string.Empty;
			this.HighAlarmEnable = false;
			this.HighAlarmValue = 0f;
			this.HighCacheValue = float.MinValue;
			this.HighAlarmPriority = 0;
			this.HighAlarmNote = string.Empty;
			this.HiHiAlarmEnable = false;
			this.HiHiAlarmValue = 0f;
			this.HiHiCacheValue = float.MinValue;
			this.HiHiAlarmPriority = 0;
			this.HiHiAlarmNote = string.Empty;
		}
	}
}
