using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Pframe.Tools
{
	public class IniConfigHelper
	{
		[DllImport("kernel32")]
		private static extern long WritePrivateProfileString(string string_1, string string_2, string string_3, string string_4);
        
		[DllImport("kernel32")]
		private static extern long GetPrivateProfileString(string string_1, string string_2, string string_3, StringBuilder stringBuilder_0, int int_0, string string_4);
        
		[DllImport("kernel32", EntryPoint = "GetPrivateProfileString")]
		private static extern uint GetPrivateProfileString_1(string string_1, string string_2, string string_3, byte[] byte_0, int int_0, string string_4);
        
		public static List<string> ReadSections(string iniFilename)
		{
			List<string> list = new List<string>();
			byte[] array = new byte[65536];
			uint privateProfileString_ = IniConfigHelper.GetPrivateProfileString_1(null, null, null, array, array.Length, iniFilename);
			int num = 0;
			int num2 = 0;
			while ((long)num2 < (long)((ulong)privateProfileString_))
			{
				if (array[num2] == 0)
				{
					list.Add(Encoding.Default.GetString(array, num, num2 - num));
					num = num2 + 1;
				}
				num2++;
			}
			return list;
		}
        
		public static List<string> ReadKeys(string SectionName, string iniFilename)
		{
			List<string> list = new List<string>();
			byte[] array = new byte[65536];
			uint privateProfileString_ = IniConfigHelper.GetPrivateProfileString_1(SectionName, null, null, array, array.Length, iniFilename);
			int num = 0;
			int num2 = 0;
			while ((long)num2 < (long)((ulong)privateProfileString_))
			{
				if (array[num2] == 0)
				{
					list.Add(Encoding.Default.GetString(array, num, num2 - num));
					num = num2 + 1;
				}
				num2++;
			}
			return list;
		}
        
		public static string ReadIniData(string Section, string Key, string NoText)
		{
			return IniConfigHelper.ReadIniData(Section, Key, NoText, IniConfigHelper.string_0);
		}
        
		public static string ReadIniData(string Section, string Key, string NoText, string iniFilePath)
		{
			string result;
			if (File.Exists(iniFilePath))
			{
				StringBuilder stringBuilder = new StringBuilder(1024);
				IniConfigHelper.GetPrivateProfileString(Section, Key, NoText, stringBuilder, 1024, iniFilePath);
				result = stringBuilder.ToString();
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}
        
		public static bool WriteIniData(string Section, string Key, string Value)
		{
			return IniConfigHelper.WriteIniData(Section, Key, Value, IniConfigHelper.string_0);
		}
        
		public static bool WriteIniData(string Section, string Key, string Value, string iniFilePath)
		{
			bool result;
			if (File.Exists(iniFilePath))
			{
				long num = IniConfigHelper.WritePrivateProfileString(Section, Key, Value, iniFilePath);
				result = (num != 0L);
			}
			else
			{
				result = false;
			}
			return result;
		}
		static IniConfigHelper()
		{
			IniConfigHelper.string_0 = "";
		}
		private static string string_0;
	}
}
