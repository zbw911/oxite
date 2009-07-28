using System;
using System.Xml.Serialization ;

namespace BlogML
{
	public enum ContentTypes : short 
	{
		[System.Xml.Serialization.XmlEnumAttribute("Html")]
		Html = 1,
		[System.Xml.Serialization.XmlEnumAttribute("Xhtml")]
		Xhtml = 2,
		[System.Xml.Serialization.XmlEnumAttribute("Text")]
		Text = 3,
		[System.Xml.Serialization.XmlEnumAttribute("Base64")]
		Base64 = 4,
	}
}
