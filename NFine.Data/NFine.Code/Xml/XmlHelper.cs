using System.IO;
using System.Xml;

namespace NFine.Code.Xml
{
    public static class XmlHelper
    {
        public static XmlNodeList SelectSingleNode(string path, string node)
        {
            if (!File.Exists(path)) return null;
            var xml = new XmlDocument();
            xml.Load(path);
            var xn = xml.SelectSingleNode(node);
            return xn != null && xn.ChildNodes.Count > 0 ? xn.ChildNodes : null;
        }
    }
}
