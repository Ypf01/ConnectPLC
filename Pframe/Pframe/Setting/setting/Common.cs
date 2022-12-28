using System;
using Pframe;
using Pframe.Common;
using Pframe.DataConvert;
using NodeSettings.Node.Variable;

namespace NodeSettings
{
	public class Common
	{
        /// <summary>
        /// 比较方法
        /// </summary>
        /// <param name="current">当前值</param>
        /// <param name="set">设定值</param>
        /// <param name="last">缓存值</param>
        /// <param name="isPositive">正向或方向检测</param>
        /// <returns>1表示触发，-1表示消除，0表示无变化</returns>
		public static int Compare(float current, float set, float last, bool isPositive)
		{
			if (isPositive)
			{
				if (current >= set && last < set)
					return 1;
				if (current < set && last >= set)
					return -1;
			}
			else
			{
				if (current <= set && last > set)
					return 1;
				if (current > set && last <= set)
					return -1;
			}
			return 0;
		}
        /// <summary>
        /// 验证输入数据是否符合要求
        /// </summary>
        /// <param name="variable">变量对象</param>
        /// <param name="type">变量类型</param>
        /// <param name="setValue">设定值</param>
        /// <returns></returns>
		public static CalResult<string> VerifyInputValue(VariableNode variable, DataType type, string setValue)
		{
			CalResult<string> xktResult = new CalResult<string>();
			CalResult<string> result;
			if (setValue.Length == 0)
			{
				xktResult.IsSuccess = false;
				xktResult.Message = "参数错误：不能为空";
				result = xktResult;
			}
			else
			{
				if (variable.Config.SetLimitEnable)
				{
					float num = 0f;
					if (!float.TryParse(setValue, out num))
					{
						xktResult.IsSuccess = false;
						xktResult.Message = "参数错误：格式不正确";
						return xktResult;
					}
					if (num > variable.Config.SetLimitMax || num < variable.Config.SetLimitMin)
					{
						xktResult.IsSuccess = false;
						xktResult.Message = "参数错误：不在设置范围内";
						return xktResult;
					}
				}
				result = MigrationLib.SetMigrationValue(setValue, type, variable.Scale, variable.Offset);
			}
			return result;
		}
        /// <summary>
        ///  验证输入数据是否符合要求
        /// </summary>
        /// <param name="variable">变量对象</param>
        /// <param name="type">变量类型</param>
        /// <param name="setValue">设定值</param>
        /// <returns>带有错误信息的返回值</returns>
		public static CalResult<string> VerifyInputValue(VariableNode variable, ComplexDataType type, string setValue)
		{
			CalResult<string> xktResult = new CalResult<string>();
			CalResult<string> result;
			if (setValue.Length == 0)
			{
				xktResult.IsSuccess = false;
				xktResult.Message = "参数错误：不能为空";
				result = xktResult;
			}
			else
			{
				if (variable.Config.SetLimitEnable)
				{
					float num = 0f;
					if (!float.TryParse(setValue, out num))
					{
						xktResult.IsSuccess = false;
						xktResult.Message = "参数错误：格式不正确";
						return xktResult;
					}
					if (num > variable.Config.SetLimitMax || num < variable.Config.SetLimitMin)
					{
						xktResult.IsSuccess = false;
						xktResult.Message = "参数错误：不在设置范围内";
						return xktResult;
					}
				}
				result = MigrationLib.SetMigrationValue(setValue, type, variable.Scale, variable.Offset);
			}
			return result;
		}

	}
}
