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
    internal static class CatalogGenerator
    {

        public static void CreateCatalog()
        {
            var template = new Template("template.html");
            var title = "《大唐刑律典》的理解与适用";
            var output = Path.Combine(Global.OutputRoot, "index.html");
            template.Write(output, title, GenerateIndex());
        }

        private static string GenerateIndex()
        {
            var template = new Template("index.html");
            var catalog1 = GenerateCatalog(true);
            var catalog2 = GenerateCatalog(false);
            var main = GenerateMain();
            return template.Format(catalog1, catalog2, main);
        }

        private static string GenerateCatalog(bool main)
        {
            var sb = new StringBuilder();
            DeepGenerate(0, main ? Global.InputRoot1 : Global.InputRoot2);
            return sb.ToString();

            void DeepGenerate(int pad, string directory)
            {
                sb.AppendLine(pad, "<ul>");
                foreach (var file in Files.GetEntries(directory))
                {
                    if (file.Number == 0)
                    {
                        if (pad > 0)
                        {
                            continue;
                        }
                    }

                    var name = file.GetTitle(main, pad);
                    if (Directory.Exists(file.Path))
                    {
                        var self = Directory.GetFiles(file.Path).FirstOrDefault(f => Path.GetFileName(f).StartsWith("0 "));
                        if (self != null)
                        {
                            sb.AppendLine(pad + 1, $"<li><a href=\"{Global.ContentsMap[self]}\">{name}</a></li>");
                        }
                        else
                        {
                            sb.AppendLine(pad + 1, $"<li>{name}</li>");
                        }
                        DeepGenerate(pad + 1, file.Path);
                    }
                    else
                    {
                        sb.AppendLine(pad + 1, $"<li><a href=\"{Global.ContentsMap[file.Path]}\">{name}</a></li>");
                    }
                }
                sb.AppendLine(pad, "</ul>");
            }
        }

        private static string GenerateMain()
        {
            var sb = new StringBuilder();
            sb.Append("<div class=\"main\">");
            foreach (var d1 in Files.GetEntries(Global.InputRoot1))
            {
                sb.Append("<div class=\"main-order1\">");
                if (d1.Number != 0)
                {
                    sb.Append($"<div class=\"title\">第{Strings.GetNumberString(d1.Number)}篇 {d1.Name}</div>");
                }
                else
                {
                    sb.Append($"<div class=\"title\">{d1.Name}</div>");
                }
                foreach (var d2 in Files.GetEntries(d1.Path))
                {
                    sb.Append("<div>");
                    if (d1.Number != 0 && d2.Number != 0)
                    {
                        sb.Append($"<div class=\"title\">第{Strings.GetNumberString(d2.Number)}章 {d2.Name}</div>");
                    }
                    if (d2.Number == 0)
                    {
                        if (d1.Number == 0)
                        {
                            var content = File.ReadAllText(d2.Path, Encoding.UTF8); content = $"<doc>{content}</doc>";
                            var doc = new XmlDocument();
                            doc.LoadXml(content);
                            var law = doc.SelectSingleNode("//law");

                            sb.Append("<div class=\"article-abstract\">");
                            sb.Append(law.InnerText.Trim());
                            sb.Append("</div>");
                        }
                        continue;
                    }
                    foreach (var f in Files.GetEntries(d2.Path))
                    {
                        if (f.Number != 0)
                        {
                            var content = File.ReadAllText(f.Path, Encoding.UTF8);
                            content = $"<doc>{content}</doc>";
                            var doc = new XmlDocument();
                            doc.LoadXml(content);

                            var law = doc.SelectSingleNode("//law");
                            if (law != null)
                            {
                                sb.Append("<div class=\"article\">");
                                sb.Append("<div class=\"article-number\">");
                                sb.Append($"{Strings.GetOrderString1(2, f.Number, f.SubNumber)} 【{f.Name}】");
                                sb.Append("</div>");
                                sb.Append("<div class=\"article-content\">");
                                sb.Append(FilterLaw(law.InnerText));
                                sb.Append("</div>");
                                sb.Append("</div>");
                            }
                        }
                    }
                    sb.Append("</div>");
                }
                sb.Append("</div>");
            }
            sb.Append("</div>");
            return sb.ToString();

            static string FilterLaw(string value)
            {
                var s = new StringBuilder();
                var lines = value.Trim().Split('\n');
                foreach (var line in lines)
                {
                    var l = line.Trim();
                    if (l.Length > 0)
                    {
                        s.AppendLine(l);
                    }
                }
                return s.ToString().TrimEnd();
            }
        }

    }
}
