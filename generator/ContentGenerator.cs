using Generator.templates;
using Generator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Generator
{
    internal static class ContentGenerator
    {

        public static void CreateContents()
        {
            DeepCreateContent(0, Global.InputRoot1, true);
            DeepCreateContent(0, Global.InputRoot2, false);
        }

        private static void DeepCreateContent(int pad, string directory, bool main)
        {
            var rootTempalte = new Template("template.html");
            var contentTemplate = new Template("content.html");
            foreach (var file in Files.GetEntries(directory))
            {
                if (File.Exists(file.Path))
                {
                    var title = file.GetTitle(main, pad);
                    var name = $"{title}.html";
                    var output = Path.Combine(Global.OutputRoot, name);
                    var content = GenerateContent(pad, file);

                    rootTempalte.Write(output, title, contentTemplate.Format(title, content));
                    Global.ContentsMap.Add(file.Path, name);
                }
                else
                {
                    DeepCreateContent(pad+1, file.Path, main);
                }
            }
        }

        private static string GenerateContent(int pad, FileEntry file)
        {
            var content = File.ReadAllText(file.Path, Encoding.UTF8);
            content = $"<doc>{content}</doc>";
            var doc = new XmlDocument();
            doc.LoadXml(content);

            var root = doc.DocumentElement!;

            #region law
            foreach (XmlNode node in root.SelectNodes("//law")!)
            {
                node.InnerXml = node.InnerXml.Trim();
                var div = doc.CreateElement("div");
                div.SetAttribute("class", "law");
                node.ReplaceSelfElement(div);
            }
            #endregion

            #region table
            foreach (XmlElement node in root.SelectNodes("//table")!)
            {
                var clazz = node.GetAttribute("class");
                clazz += " table table-bordered";
                node.SetAttribute("class", clazz);
            }
            #endregion

            #region word
            foreach (XmlElement node in root.SelectNodes("//word")!)
            {
                var name = node.GetAttribute("value");
                var value = File.ReadAllText(Path.Combine(Global.InputWordRoot, $"{name}.txt"));
                var div = doc.CreateElement("div");
                div.SetAttribute("class", "word");
                var divName = doc.CreateElement("div");
                divName.SetAttribute("class", "word-name");
                var divValue = doc.CreateElement("div");
                divValue.SetAttribute("class", "word-value");
                node.ReplaceSelfElement(div);
                divName.InnerXml = name;
                divValue.InnerXml = value;
                div.AppendChild(divName);
                div.AppendChild(divValue);

                Perfect(doc, divValue);
            }
            #endregion

            #region text
            foreach (XmlNode node in root.SelectNodes("/doc")!)
            {
                Perfect(doc, node);
            }
            #endregion

            return root.InnerXml;
        }

        private static void Perfect(XmlDocument doc, XmlNode node)
        {
            var div = doc.CreateElement("div");
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Text)
                {
                    var tokens = child.InnerText.Split("\r\n\r\n");
                    foreach (var token in tokens)
                    {
                        if (token.Length == 0)
                        {
                            continue;
                        }
                        var p = doc.CreateElement("p");
                        p.InnerText = token.Trim();
                        div.AppendChild(p);
                    }

                }
                else
                {
                    div.AppendChild(child.Clone());
                }
            }
            div.SetAttribute("class", "text");
            node.InnerXml = div.OuterXml;
        }
    }
}
