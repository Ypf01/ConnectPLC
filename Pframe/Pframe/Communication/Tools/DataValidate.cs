using System;
using System.Text.RegularExpressions;

namespace Pframe.Tools
{
	public class DataValidate
	{
		public static bool IsInteger(string txt)
		{
			Regex regex = new Regex("^[0-9]\\d*$");
			return regex.IsMatch(txt);
		}
		public static bool IsEmail(string txt)
		{
			Regex regex = new Regex("\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*");
			return regex.IsMatch(txt);
		}
        
		public static bool IsIdentityCard(string txt)
		{
			Regex regex = new Regex("^(\\d{15}$|^\\d{18}$|^\\d{17}(\\d|X|x))$");
			return regex.IsMatch(txt);
		}
        
		public static bool smethod_0(string ip)
		{
			bool result;
			if (string.IsNullOrEmpty(ip) || ip.Length < 7 || ip.Length > 15)
			{
				result = false;
			}
			else
			{
				string pattern = "^(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])$";
				Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
				result = regex.IsMatch(ip);
			}
			return result;
		}
        
		public static bool IsIPPort(string port)
		{
			int num;
			return int.TryParse(port, out num) && num >= 0 && num <= 65535;
		}
        
	}
}
