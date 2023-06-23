﻿using Generator.templates;
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

                    var title = file.GetTitle(pad);
                    if (parent != null && parent.Value.Name == "附录")
                    {
                        title = file.Name;
                    }
                    rootTempalte.Write(output, title, contentTemplate.Format(title, content));
                    Global.ContentsMap.Add(file.Path, name);
                }
                else
                {
                    DeepCreateContent(file, pad+1, file.Path);
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

            #region law
            foreach (XmlNode node in root.SelectNodes("//law")!)
            {
                node.InnerXml = node.InnerXml.Trim();
                var div = doc.CreateElement("div");
                div.SetAttribute("class", "law");
                node.ReplaceSelfElement(div);
            }
            #endregion

            #region text
            foreach (XmlNode node in root.SelectNodes("//text")!)
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
                node.ReplaceSelf(div);
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

            return root.InnerXml;
        }
    }
}
