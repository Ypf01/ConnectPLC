using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NodeSettings
{
    /// <summary>
    /// 报警时间参数
    /// </summary>
	public class AlarmEventArgs : EventArgs
	{
        /// <summary>
        /// 报警信息
        /// </summary>
		public string alarmInfo { get; set; }
        /// <summary>
        /// 报警值
        /// </summary>
		public string CurrentValue { get; set; }

        /// <summary>
        /// 报警设定值
        /// </summary>
		public string SetValue { get; set; }
        /// <summary>
        /// 触发或消除
        /// </summary>
		public bool IsACK { get; set; }

	}
}
