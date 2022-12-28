using System;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace Pframe.Tools
{
	public static class AuthorizationHelper
	{
		public static string GetComputerInfo()
		{
			string str = AuthorizationHelper.smethod_0();
			string str2 = AuthorizationHelper.smethod_1();
			return str + str2;
		}

		private static string smethod_0()
		{
			return AuthorizationHelper.GetHardWareInfo("Win32_Processor", "ProcessorId");
		}

		private static string smethod_1()
		{
			string text = AuthorizationHelper.GetHardWareInfo("Win32_BIOS", "SerialNumber");
			string result;
			if (!string.IsNullOrEmpty(text) && text != "To be filled by O.E.M" && !text.Contains("O.E.M") && !text.Contains("OEM") && !text.Contains("Default"))
			{
				result = text;
			}
			else
			{
				result = AuthorizationHelper.defaultValue;
			}
			return result;
		}

		private static string GetHardWareInfo(string string_4, string string_5)
		{
			ManagementClass managementClass = new ManagementClass(string_4);
			ManagementObjectCollection instances = managementClass.GetInstances();
			PropertyDataCollection properties = managementClass.Properties;
			foreach (PropertyData propertyData in properties)
			{
				if (propertyData.Name == string_5)
				{
					using (ManagementObjectCollection.ManagementObjectEnumerator enumerator2 = instances.GetEnumerator())
					{
						if (enumerator2.MoveNext())
						{
							ManagementObject managementObject = (ManagementObject)enumerator2.Current;
							return managementObject.Properties[propertyData.Name].Value.ToString();
						}
					}
				}
			}
			return string.Empty;
		}

		public static string GetMD5String(string str)
		{
			str = AuthorizationHelper.md5Begin + str + AuthorizationHelper.md5End;
			MD5 md = new MD5CryptoServiceProvider();
			byte[] bytes = Encoding.Unicode.GetBytes(str);
			byte[] array = md.ComputeHash(bytes);
			string text = string.Empty;
			foreach (byte b in array)
			{
				text += b.ToString("x2");
			}
			return text;
		}

		public static string Encrypt(string str)
		{
			DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider();
			byte[] bytes = Encoding.Default.GetBytes(str);
			descryptoServiceProvider.Key = Encoding.ASCII.GetBytes(AuthorizationHelper.key);
			descryptoServiceProvider.IV = Encoding.ASCII.GetBytes(AuthorizationHelper.key);
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, descryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(bytes, 0, bytes.Length);
			cryptoStream.FlushFinalBlock();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in memoryStream.ToArray())
			{
				stringBuilder.AppendFormat("{0:X2}", b);
			}
			stringBuilder.ToString();
			return stringBuilder.ToString();
		}

		public static string Encrypt()
		{
			return AuthorizationHelper.Encrypt(AuthorizationHelper.GetMD5String(AuthorizationHelper.GetComputerInfo()));
		}

		public static string Decrypt(string pToDecrypt)
		{
			DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider();
			byte[] array = new byte[pToDecrypt.Length / 2];
			for (int i = 0; i < pToDecrypt.Length / 2; i++)
			{
				int num = Convert.ToInt32(pToDecrypt.Substring(i * 2, 2), 16);
				array[i] = (byte)num;
			}
			descryptoServiceProvider.Key = Encoding.ASCII.GetBytes(AuthorizationHelper.key);
			descryptoServiceProvider.IV = Encoding.ASCII.GetBytes(AuthorizationHelper.key);
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, descryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(array, 0, array.Length);
			cryptoStream.FlushFinalBlock();
			new StringBuilder();
			return Encoding.Default.GetString(memoryStream.ToArray());
		}

		public static bool Check(string Code)
		{
			return AuthorizationHelper.Decrypt(Code) == AuthorizationHelper.GetMD5String(AuthorizationHelper.GetComputerInfo());
		}

		static AuthorizationHelper()
		{
			AuthorizationHelper.key = "thinger1";
			AuthorizationHelper.md5Begin = "Hello";
			AuthorizationHelper.md5End = "World";
			AuthorizationHelper.defaultValue = "123456789";
		}

		private static string key;

		private static string md5Begin;

		private static string md5End;

		private static string defaultValue;
	}
}
