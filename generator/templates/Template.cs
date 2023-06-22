using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator.templates
{
    public class Template
    {

        private readonly string content;

        public Template(string name)
        {
            var path = $"Generator.templates.{name}";
            using var stream = typeof(Template).Assembly.GetManifestResourceStream(path);
            if (stream == null)
            {
                throw new FileNotFoundException(path);
            }
            using var reader = new StreamReader(stream, Encoding.UTF8);
            content = reader.ReadToEnd();
        }

        public string Format(params string[] args)
        {
            return string.Format(content, args);
        }

        public void Write(string path, params string[] args)
        {
            var content = Format(args);
            File.WriteAllText(path, content, Encoding.UTF8);
        }
    }
}
