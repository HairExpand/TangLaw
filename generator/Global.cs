using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    public static class Global
    {

        public static string InputRoot;
        public static string InputRoot1 => Path.Combine(InputRoot, "正文");
        public static string InputRoot2 => Path.Combine(InputRoot, "附录");
        public static string InputWordRoot => Path.Combine(InputRoot, "释义");
        public static string OutputRoot;

        public static readonly Dictionary<string, string> ContentsMap = new Dictionary<string, string>();

    }
}
