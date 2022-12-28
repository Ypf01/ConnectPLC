using System;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Pframe.Tools
{
	public class XmlHelper
	{
		public static XmlDocument CreateXmlDocument(string name, string type)
		{
			XmlDocument xmlDocument = null;
			try
			{
				xmlDocument = new XmlDocument();
				xmlDocument.LoadXml("<" + name + "/>");
				XmlElement documentElement = xmlDocument.DocumentElement;
				documentElement.SetAttribute("type", type);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return xmlDocument;
		}
        
		public static string Read(string path, string node, string attribute)
		{
			string result = "";
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(path);
				XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
				result = (attribute.Equals("") ? xmlNode.InnerText : xmlNode.Attributes[attribute].Value);
			}
			catch
			{
			}
			return result;
		}
        
		public static void Insert(string path, string node, string element, string attribute, string value)
		{
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(path);
				XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
				if (element.Equals(""))
				{
					if (!attribute.Equals(""))
					{
						XmlElement xmlElement = (XmlElement)xmlNode;
						xmlElement.SetAttribute(attribute, value);
					}
				}
				else
				{
					XmlElement xmlElement2 = xmlDocument.CreateElement(element);
					if (attribute.Equals(""))
					{
						xmlElement2.InnerText = value;
					}
					else
					{
						xmlElement2.SetAttribute(attribute, value);
					}
					xmlNode.AppendChild(xmlElement2);
				}
				xmlDocument.Save(path);
			}
			catch
			{
			}
		}
        
		public static void Update(string path, string node, string attribute, string value)
		{
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(path);
				XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
				XmlElement xmlElement = (XmlElement)xmlNode;
				if (attribute.Equals(""))
				{
					xmlElement.InnerText = value;
				}
				else
				{
					xmlElement.SetAttribute(attribute, value);
				}
				xmlDocument.Save(path);
			}
			catch
			{
			}
		}
        
		public static void Delete(string path, string node, string attribute)
		{
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(path);
				XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
				XmlElement xmlElement = (XmlElement)xmlNode;
				if (attribute.Equals(""))
				{
					xmlNode.ParentNode.RemoveChild(xmlNode);
				}
				else
				{
					xmlElement.RemoveAttribute(attribute);
				}
				xmlDocument.Save(path);
			}
			catch
			{
			}
		}
        
		public static DataSet GetDataSet(string source, XmlHelper.XmlType xmlType)
		{
			DataSet dataSet = new DataSet();
			if (xmlType == XmlHelper.XmlType.File)
			{
				dataSet.ReadXml(source);
			}
			else
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(source);
				XmlNodeReader reader = new XmlNodeReader(xmlDocument);
				dataSet.ReadXml(reader);
			}
			return dataSet;
		}
        
		public static string GetNodeInfoByNodeName(string path, string nodeName)
		{
			string result = "";
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(path);
			XmlElement documentElement = xmlDocument.DocumentElement;
			XmlNode xmlNode = documentElement.SelectSingleNode("//" + nodeName);
			if (xmlNode != null)
			{
				result = xmlNode.InnerText;
			}
			return result;
		}
        
		public static void get_XmlValue_ds(string xml_string, ref DataSet ds)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml_string);
			XmlNodeReader xmlNodeReader = new XmlNodeReader(xmlDocument);
			ds.ReadXml(xmlNodeReader);
			xmlNodeReader.Close();
			int count = ds.Tables.Count;
		}
        
		public static DataTable GetTable(string source, XmlHelper.XmlType xmlType, string tableName)
		{
			DataSet dataSet = new DataSet();
			if (xmlType == XmlHelper.XmlType.File)
			{
				dataSet.ReadXml(source);
			}
			else
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(source);
				XmlNodeReader reader = new XmlNodeReader(xmlDocument);
				dataSet.ReadXml(reader);
			}
			return dataSet.Tables[tableName];
		}
        
		public static object GetTableCell(string source, XmlHelper.XmlType xmlType, string tableName, int rowIndex, string colName)
		{
			DataSet dataSet = new DataSet();
			if (xmlType == XmlHelper.XmlType.File)
			{
				dataSet.ReadXml(source);
			}
			else
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(source);
				XmlNodeReader reader = new XmlNodeReader(xmlDocument);
				dataSet.ReadXml(reader);
			}
			return dataSet.Tables[tableName].Rows[rowIndex][colName];
		}
        
		public static object GetTableCell(string source, XmlHelper.XmlType xmlType, string tableName, int rowIndex, int colIndex)
		{
			DataSet dataSet = new DataSet();
			if (xmlType == XmlHelper.XmlType.File)
			{
				dataSet.ReadXml(source);
			}
			else
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(source);
				XmlNodeReader reader = new XmlNodeReader(xmlDocument);
				dataSet.ReadXml(reader);
			}
			return dataSet.Tables[tableName].Rows[rowIndex][colIndex];
		}
        
		public static void SaveTableToFile(DataTable dt, string filePath)
		{
			new DataSet("Config")
			{
				Tables = 
				{
					dt.Copy()
				}
			}.WriteXml(filePath);
		}
        
		public static void SaveTableToFile(DataTable dt, string rootName, string filePath)
		{
			new DataSet(rootName)
			{
				Tables = 
				{
					dt.Copy()
				}
			}.WriteXml(filePath);
		}
        
		public static bool UpdateTableCell(string filePath, string tableName, int rowIndex, string colName, string content)
		{
			DataSet dataSet = new DataSet();
			dataSet.ReadXml(filePath);
			DataTable dataTable = dataSet.Tables[tableName];
			bool result;
			if (dataTable.Rows[rowIndex][colName] != null)
			{
				dataTable.Rows[rowIndex][colName] = content;
				dataSet.WriteXml(filePath);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
        
		public static bool UpdateTableCell(string filePath, string tableName, int rowIndex, int colIndex, string content)
		{
			DataSet dataSet = new DataSet();
			dataSet.ReadXml(filePath);
			DataTable dataTable = dataSet.Tables[tableName];
			bool result;
			if (dataTable.Rows[rowIndex][colIndex] != null)
			{
				dataTable.Rows[rowIndex][colIndex] = content;
				dataSet.WriteXml(filePath);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
        
		public static object GetNodeValue(string source, XmlHelper.XmlType xmlType, string nodeName)
		{
			XmlDocument xmlDocument = new XmlDocument();
			if (xmlType == XmlHelper.XmlType.File)
			{
				xmlDocument.Load(source);
			}
			else
			{
				xmlDocument.LoadXml(source);
			}
			XmlElement documentElement = xmlDocument.DocumentElement;
			XmlNode xmlNode = documentElement.SelectSingleNode("//" + nodeName);
			object result;
			if (xmlNode != null)
			{
				result = xmlNode.InnerText;
			}
			else
			{
				result = null;
			}
			return result;
		}
        
		public static object GetNodeValue(string source, string nodeName)
		{
			object result;
			if (source == null || nodeName == null || source == "" || nodeName == "" || source.Length < nodeName.Length * 2)
			{
				result = null;
			}
			else
			{
				int num = source.IndexOf("<" + nodeName + ">") + nodeName.Length + 2;
				int num2 = source.IndexOf("</" + nodeName + ">");
				if (num == -1 || num2 == -1)
				{
					result = null;
				}
				else if (num >= num2)
				{
					result = null;
				}
				else
				{
					result = source.Substring(num, num2 - num);
				}
			}
			return result;
		}
        
		public static bool UpdateNode(string filePath, string nodeName, string nodeValue)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(filePath);
			XmlElement documentElement = xmlDocument.DocumentElement;
			XmlNode xmlNode = documentElement.SelectSingleNode("//" + nodeName);
			bool result;
			if (xmlNode != null)
			{
				xmlNode.InnerText = nodeValue;
				result = true;
				xmlDocument.Save(filePath);
			}
			else
			{
				result = false;
			}
			return result;
		}
        
		public static T ReadXML<T>(string path)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			StreamReader textReader = new StreamReader(path);
			return (T)((object)xmlSerializer.Deserialize(textReader));
		}
        
		public static string WriteXML<T>(T item, string path, string jjdbh, string ends)
		{
			if (string.IsNullOrEmpty(ends))
			{
				ends = "send";
			}
			int num = 0;
			XmlSerializer xmlSerializer = new XmlSerializer(item.GetType());
			object[] args = new object[]
			{
				path,
				"\\",
				jjdbh,
				ends,
				".xml"
			};
			string path2 = string.Concat(args);
			for (;;)
			{
				try
				{
					FileStream fileStream = File.Create(path2);
					fileStream.Close();
					TextWriter textWriter = new StreamWriter(path2, false, Encoding.UTF8);
					XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
					xmlSerializerNamespaces.Add(string.Empty, string.Empty);
					xmlSerializer.Serialize(textWriter, item, xmlSerializerNamespaces);
					textWriter.Flush();
					textWriter.Close();
					break;
				}
				catch (Exception)
				{
					if (num >= 5)
					{
						break;
					}
					num++;
				}
			}
			return XmlHelper.SerializeToXmlStr<T>(item, true);
		}
        
		public static string SerializeToXmlStr<T>(T obj, bool omitXmlDeclaration)
		{
			return XmlHelper.XmlSerialize<T>(obj, omitXmlDeclaration);
		}
        
		public static string XmlSerialize<T>(T obj, bool omitXmlDeclaration)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.OmitXmlDeclaration = omitXmlDeclaration;
			xmlWriterSettings.Encoding = new UTF8Encoding(false);
			MemoryStream memoryStream = new MemoryStream();
			XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
			XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
			xmlSerializerNamespaces.Add(string.Empty, string.Empty);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			xmlSerializer.Serialize(xmlWriter, obj, xmlSerializerNamespaces);
			return Encoding.UTF8.GetString(memoryStream.ToArray());
		}
        
		public static void XmlSerialize<T>(string path, T obj, bool omitXmlDeclaration, bool removeDefaultNamespace)
		{
			using (XmlWriter xmlWriter = XmlWriter.Create(path, new XmlWriterSettings
			{
				OmitXmlDeclaration = omitXmlDeclaration
			}))
			{
				XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
				if (removeDefaultNamespace)
				{
					xmlSerializerNamespaces.Add(string.Empty, string.Empty);
				}
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
				xmlSerializer.Serialize(xmlWriter, obj, xmlSerializerNamespaces);
			}
		}
        
		private static byte[] smethod_0(string string_0)
		{
			byte[] array;
			using (FileStream fileStream = new FileStream(string_0, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				array = new byte[fileStream.Length];
				int i = (int)fileStream.Length;
				int num = 0;
				while (i > 0)
				{
					int num2 = fileStream.Read(array, num, i);
					if (num2 == 0)
					{
						break;
					}
					num += num2;
					i -= num2;
				}
			}
			return array;
		}
        
		public static T XmlFileDeserialize<T>(string path)
		{
			byte[] array = XmlHelper.smethod_0(path);
			if (array.Length < 1)
			{
				for (int i = 0; i < 5; i++)
				{
					array = XmlHelper.smethod_0(path);
					if (array.Length != 0)
					{
						break;
					}
					Thread.Sleep(50);
				}
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(new MemoryStream(array));
			T result;
			if (xmlDocument.DocumentElement != null)
			{
				result = (T)((object)new XmlSerializer(typeof(T)).Deserialize(new XmlNodeReader(xmlDocument.DocumentElement)));
			}
			else
			{
				result = default(T);
			}
			return result;
		}
        
		public static T XmlDeserialize<T>(string xmlOfObject) where T : class
		{
			XmlReader xmlReader = XmlReader.Create(new StringReader(xmlOfObject), new XmlReaderSettings());
			return (T)((object)new XmlSerializer(typeof(T)).Deserialize(xmlReader));
		}
               
		public enum XmlType
		{
			File,
			String
		}
	}
}
