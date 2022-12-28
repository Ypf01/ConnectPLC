using System;

namespace Pframe.Common
{
	public class CommonMethods
	{
		public static bool IsBoolean(string value)
		{
			return value == "1" || value.ToLower() == "true";
		}
	}
}
