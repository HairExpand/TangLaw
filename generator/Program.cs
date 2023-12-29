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

            Directory.Delete(Global.OutputRoot, true);
            Directory.CreateDirectory(Global.OutputRoot);

            GenerateWord();
            ContentGenerator.CreateContents();
            CatalogGenerator.CreateCatalog();
        }

        private static void GenerateWord()
        {
            foreach (var dir in Directory.GetDirectories(Global.InputWordRoot))
            {
                foreach (var word in Directory.GetFiles(dir))
                {
                    Global.WordMap[Path.GetFileNameWithoutExtension(word)] = word;
                }
            }
        }
    }
}