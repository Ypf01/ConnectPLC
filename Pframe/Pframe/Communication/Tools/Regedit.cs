using System;
using Microsoft.Win32;

namespace Pframe.Tools
{
	public class Regedit
	{
		static Regedit()
		{
			Regedit.registryKey_0 = Registry.CurrentUser;
			Regedit.registryKey_1 = Regedit.registryKey_0.OpenSubKey("SOFTWARE", true);
		}
        
		public static object GetData(string node, string name)
		{
			RegistryKey registryKey = Regedit.registryKey_1.OpenSubKey(node, true);
			return (registryKey != null) ? registryKey.GetValue(name) : null;
		}
        
		public static void AddItem(string node, string name, object value)
		{
			RegistryKey registryKey = Regedit.registryKey_1.CreateSubKey(node);
			if (registryKey != null)
			{
				registryKey.SetValue(name, value);
			}
		}
        
        
		private static readonly RegistryKey registryKey_0;
        
		private static readonly RegistryKey registryKey_1;
	}
}
