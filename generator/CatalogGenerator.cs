using Generator.templates;
using Generator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return template.Format(catalog1, catalog2);
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


    }
}
