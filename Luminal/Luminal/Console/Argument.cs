using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Console
{
    public enum ArgumentType
    {
        STRING,
        INT,
        FLOAT,
        BOOL
    }

    public class Argument
    {
        public ArgumentType Type;
        public string Name;
        public bool Optional = false;
        public bool Overflow = false;

        public Argument(ArgumentType t, string n, bool optional = false)
        {
            Type = t;
            Name = n;
            Optional = optional;
        }
    }
}
