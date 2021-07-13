using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using System.Xml;

namespace C_program
{
    class Program
    {
        static void Main(string[] args)
        {
            //UseXPathWithXPathDocument();
 
            UseXPathWithXmlDocument();
 
            Console.Read();
        }
 
        static void UseXPathWithXmlDocument()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("./src/testxml.xml");
            //使用xPath选择需要的节点
            XmlNodeList nodes = doc.SelectNodes("/rss/channel/item[position()<=10]");
            foreach (XmlNode item in nodes)
            {
                string title = item.SelectSingleNode("title").InnerText;
                string url = item.SelectSingleNode("link").InnerText;
                Console.WriteLine("{0} = {1}", title, url);
            }
        }
    }
}
