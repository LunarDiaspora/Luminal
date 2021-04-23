using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Editor.Console
{
    public enum ConVarType
    {
        STRING,
        INT,
        FLOAT,
        DOUBLE,
        BOOL,
        UNKNOWN
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ConVarAttribute : Attribute
    {
        public string Name;
        public string Description = null;

        public ConVarAttribute(string name, string description = null)
        {
            Name = name;
            Description = description;
        }

        public static ConVarType ToConVarType(Type t)
        {
            if (t == typeof(string)) return ConVarType.STRING;
            else if (t == typeof(int)) return ConVarType.INT;
            else if (t == typeof(float)) return ConVarType.FLOAT;
            else if (t == typeof(double)) return ConVarType.DOUBLE;
            else if (t == typeof(bool)) return ConVarType.BOOL;
            return ConVarType.UNKNOWN;
        }

        public static dynamic Parse(string t, ConVarType cvt)
        {
            switch (cvt)
            {
                case ConVarType.STRING:
                    return t;
                case ConVarType.INT:
                    return int.Parse(t);
                case ConVarType.FLOAT:
                    return float.Parse(t);
                case ConVarType.DOUBLE:
                    return double.Parse(t);
                case ConVarType.BOOL:
                    return (t == "true" || t == "yes" || t == "1");
            }

            throw new ArgumentException("Failed to parse your input!");
        }
    }
}
