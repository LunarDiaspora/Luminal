using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Console
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class ConCommandAttribute : Attribute
    {
        public string Name;
        public string Description = null;

        public ConCommandAttribute(string name, string description = null)
        {
            Name = name;
            Description = description;
        }
    }

    public interface ArgumentAttribute
    {
        Argument ToArg();
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class RequiredArgumentAttribute : Attribute, ArgumentAttribute
    {
        readonly string Name;
        readonly ArgumentType Type;

        public RequiredArgumentAttribute(string name, ArgumentType t)
        {
            Name = name;
            Type = t;
        }

        public Argument ToArg()
        {
            var e = new Argument(Type, Name, false);
            return e;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class OptionalArgumentAttribute : Attribute, ArgumentAttribute
    {
        readonly string Name;
        readonly ArgumentType Type;

        public OptionalArgumentAttribute(string name, ArgumentType t)
        {
            Name = name;
            Type = t;
        }

        public Argument ToArg()
        {
            var e = new Argument(Type, Name, true);
            return e;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OverflowArgumentAttribute : Attribute, ArgumentAttribute
    {
        readonly string Name;

        public OverflowArgumentAttribute(string n)
        {
            Name = n;
        }

        public Argument ToArg()
        {
            var e = new Argument(ArgumentType.String, Name, true);
            e.Overflow = true;
            return e;
        }
    }

    public class ConCommandContainer
    {
        public IConCommand Command;
        public List<Argument> Arguments;
        public string Name;
        public string Description = null;
    }
}
