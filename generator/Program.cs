using Generator.templates;
using Generator.Utils;
using System.Text;

namespace Generator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Global.InputRoot = args[0];
            Global.OutputRoot = args[1];

            var template = new Template("template.html");
            var title = "《大唐刑律典》的理解与适用";
            var output = Path.Combine(Global.OutputRoot, "index.html");
            template.Write(output, title, GenerateIndex());
        }

        private static string GenerateIndex()
        {
            var template = new Template("index.html");
            var catalog = GenerateCatalog();
            return template.Format(null, catalog);
        }

        private static string GenerateCatalog()
        {
            var sb = new StringBuilder();
            DeepGenerate(0, Global.InputRoot);
            return sb.ToString();

            void DeepGenerate(int pad, string directory)
            {
                sb.AppendLine(pad, "<ul>");
                foreach (var file in Files.GetEntries(directory))
                {
                    if (file.Number == 0)
                    {
                        continue;
                    }
                    sb.AppendLine(pad+1, $"<li>{Strings.GetOrderString(pad, file.Number, file.SubNumber)} {file.Name}</li>");
                    if (Directory.Exists(file.Path))
                    {
                        DeepGenerate(pad + 1, file.Path);
                    }
                }
                sb.AppendLine(pad, "</ul>");
            }
        }

    }
}