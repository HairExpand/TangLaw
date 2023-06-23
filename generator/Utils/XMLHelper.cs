using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Generator.Utils
{
    public static class XMLHelper
    {

        public static void ReplaceSelf(this XmlNode node, XmlNode other)
        {
            node.ParentNode!.ReplaceChild(other, node);
        }

        public static void ReplaceSelfElement(this XmlNode node, XmlNode other)
        {
            other.InnerXml = node.InnerXml;
            node.ParentNode!.ReplaceChild(other, node);
        }

        public static void RemoveSelf(this XmlNode node)
        {
            node.ParentNode!.RemoveChild(node);
        }

    }
}
