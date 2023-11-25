using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    public class GenerateException : Exception
    {

        public GenerateException(string message) : base(message) { }

    }
}
