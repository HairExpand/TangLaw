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
            DeepCreateContent(null, 0, Global.InputRoot);
        }

        private static void DeepCreateContent(FileEntry? parent, int pad, string directory)
        {
            var rootTempalte = new Template("template.html");
            var contentTemplate = new Template("content.html");
            foreach (var file in Files.GetEntries(directory))
            {
                if (File.Exists(file.Path))
                {
                    var name = $"{file.Name}.html";
                    var output = Path.Combine(Global.OutputRoot, name);
                    var content = GenerateContent(pad, file);

                    var title = file.Name;
                    if (file.Number > 0)
                    {
                        title = $"{Strings.GetOrderString(pad, file.Number, file.SubNumber)} {file.Name}";
                    }
                    rootTempalte.Write(output, title, contentTemplate.Format(title, content));
                    Global.ContentsMap.Add(file.Path, name);
                }
                else
                {
                    DeepCreateContent(parent, pad+1, file.Path);
                }
            }
        }

        private static string GenerateContent(int pad, FileEntry file)
        {
            var content = File.ReadAllText(file.Path, Encoding.UTF8);
            if (content.Length == 0)
            {
                return "";
            }
            var doc = new XmlDocument();
            doc.LoadXml(content);

            var root = doc.DocumentElement!;
            foreach (XmlNode node in root.SelectNodes("law")!)
            {
                node.InnerXml = node.InnerXml.Trim();
                var div = doc.CreateElement("div");
                div.SetAttribute("class", "law");
                node.ReplaceSelf(div);
            }
            foreach (XmlNode node in root.SelectNodes("text")!)
            {
                var tokens = node.InnerText.Split("\r\n\r\n");
                node.InnerText = "";
                foreach (var token in tokens)
                {
                    var p = doc.CreateElement("p");
                    p.InnerText = token.Trim();
                    node.AppendChild(p);
                }

                var div = doc.CreateElement("div");
                div.SetAttribute("class", "text");
                node.ReplaceSelf(div);
            }
            return root.InnerXml;
        }
    }
}
