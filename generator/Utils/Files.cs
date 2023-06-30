using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator.Utils
{

    public static class Files
    {


        public static FileEntry[] GetEntries(string path)
        {
            var files = Directory.GetFileSystemEntries(path);
            return files.Select(ToEntry).OrderBy(entry => entry.Number).ThenBy(entry => entry.SubNumber).ToArray();
        }

        private static FileEntry ToEntry(string file)
        { 
            var name = Path.GetFileNameWithoutExtension(file);
            var tokens = name.Split(' ', 2);
            Parse(tokens[0], out var number, out var sub);
            return new FileEntry()
            {
                Name = tokens[1],
                Number = number,
                SubNumber = sub,
                Path = file,
            };
        }

        private static void Parse(string value, out int number, out int sub)
        {
            if (value.Contains('.'))
            {
                var tokens = value.Split('.');
                number = int.Parse(tokens[0]);
                sub = int.Parse(tokens[1]);
            }
            else
            {
                number = int.Parse(value);
                sub = 0;
            }
        }
    }

    public struct FileEntry
    {
        public int Number;
        public int SubNumber;
        public string Name;
        public string Path;

        public string GetTitle(bool main, int pad)
        {
            if (main)
            {
                if (Number > 0)
                {
                    return $"{Strings.GetOrderString1(pad, Number, SubNumber)} {Name}";
                }
            }
            else
            {
                return $"{Name}";
            }
            return Name;
        }
    }

}
