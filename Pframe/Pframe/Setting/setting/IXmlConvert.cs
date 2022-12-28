using System;
using System.Xml.Linq;

internal interface IXmlConvert
{
	XElement ToXmlElement();

	void LoadByXmlElement(XElement element);
}
