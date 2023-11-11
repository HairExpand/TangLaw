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

            ContentGenerator.CreateContents();
            CatalogGenerator.CreateCatalog();
        }

    }
}