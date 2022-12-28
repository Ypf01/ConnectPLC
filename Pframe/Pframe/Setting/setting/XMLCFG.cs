using System;
using System.Xml;

namespace NodeSettings
{
	public class XMLCFG
	{
		public static string XMLAttributeGetValue(XmlNode rootxml, string name)
		{
			string result = string.Empty;
			if (rootxml != null && rootxml.Attributes != null && rootxml.Attributes[name] != null)
			{
				result = rootxml.Attributes[name].Value;
			}
			return result;
		}

	}
}
