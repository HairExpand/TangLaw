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
            BuildMap(0, Global.InputRoot1, true);
            BuildMap(0, Global.InputRoot2, false);
            DeepCreateContent(0, Global.InputRoot1, true);
            DeepCreateContent(0, Global.InputRoot2, false);
        }

        private static void BuildMap(int pad, string directory, bool main)
        {
            foreach (var file in Files.GetEntries(directory))
            {
                if (File.Exists(file.Path))
                {
                    var title = file.GetTitle(main, pad);
                    var name = $"{title}.html";
                    Global.ContentsMap.Add(file.Path, name);
                    if (main && file.Number != 0)
                    {
                        if (file.SubNumber > 0)
                        {
                            Global.LinkMap.Add($"{file.Number}.{file.SubNumber}", name);
                        }
                        else
                        {
                            Global.LinkMap.Add($"{file.Number}", name);
                        }
                    }
                }
                else
                {
                    BuildMap(pad + 1, file.Path, main);
                }
            }
        }

        private static void DeepCreateContent(int pad, string directory, bool main)
        {
            var rootTempalte = new Template("template.html");
            var contentTemplate = new Template("content.html");
            foreach (var file in Files.GetEntries(directory))
            {
                try
                {
                    if (File.Exists(file.Path))
                    {
                        var title = file.GetTitle(main, pad);
                        var name = $"{title}.html";
                        var output = Path.Combine(Global.OutputRoot, name);
                        var content = GenerateContent(pad, file);

                        rootTempalte.Write(output, title, contentTemplate.Format(title, content));
                    }
                    else
                    {
                        DeepCreateContent(pad + 1, file.Path, main);
                    }
                }
                catch (GenerateException e)
                {
                    Console.Error.WriteLine($"{file.Path} {e.Message}");
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
                var filename = Path.Combine(Global.InputWordRoot, $"{name}.txt");
                if (!File.Exists(filename))
                {
                    throw new GenerateException($"释义不存在：{name}");
                }
                var value = File.ReadAllText(filename);
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
            }
            #endregion

            #region link
            foreach (XmlElement link in root.SelectNodes("//link")!)
            {
                var a = doc.CreateElement("a");

                var value = link.InnerXml;
                if (value.Contains("."))
                {
                    var tokens = value.Split('.');
                    var x1 = Strings.GetNumberString(int.Parse(tokens[0]));
                    var x2 = Strings.GetNumberString(int.Parse(tokens[1]));
                    a.InnerXml = $"第{x1}条之{x2}";
                }
                else
                {
                    var x = Strings.GetNumberString(int.Parse(value));
                    a.InnerXml = $"第{x}条";
                }
                a.SetAttribute("href", Global.LinkMap[value]);
                link.ReplaceSelf(a);
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
            const int None = 0;
            const int Text = 1;
            const int Inline = 2;
            const int Block = 3;

            var div = doc.CreateElement("div");
            XmlElement prev = null;
            int state = None;

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Text)
                {
                    var tokens = child.InnerText.Split("\r\n\r\n");
                    foreach (var token in tokens)
                    {
                        if (token.Trim().Length == 0)
                        {
                            continue;
                        }

                        XmlElement p;
                        if (state == Inline)
                        {
                            p = prev;
                        }
                        else
                        {
                            p = doc.CreateElement("p");
                            div.AppendChild(p);
                        }
                        p.InnerXml += token.Trim();
                        prev = p;
                        state = Text;
                    }

                }
                else if (child is XmlElement element && element.Name == "a")
                {
                    if (prev == null)
                    {
                        prev = doc.CreateElement("p");
                        div.AppendChild(prev);
                    }
                    prev.AppendChild(child.Clone());
                    state = Inline;
                }
                else
                {
                    var block = div.AppendChild(child.Clone());
                    if (block.Name == "div")
                    {
                        Perfect(doc, block);
                    }
                    prev = null;
                    state = Block;
                }
            }
            div.SetAttribute("class", "text");
            node.InnerXml = div.OuterXml;
        }
    }
}
