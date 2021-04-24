using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Console
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

    [Flags]
    public enum ConVarFlags
    {
        READONLY = 1 << 1,
        MOMENTARY = 1 << 2
    }

    public static class ConVarFlagsExtensions
    {
        public static string GetFlagString(this ConVarFlags t)
        {
            var features = (from e in Enum.GetNames(typeof(ConVarFlags))
                let val = Enum.Parse(typeof(ConVarFlags), e)
                let present = t.Has((ConVarFlags) val)
                where present select e.ToLower()).ToList();

            return string.Join(" ", features);
        }

        public static bool Has(this ConVarFlags a, ConVarFlags b)
        {
            return (a & b) > 0;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ConVarAttribute : Attribute
    {
        public string Name;
        public string Description = null;
        public ConVarFlags Flags = 0;

        public ConVarAttribute(string name, string description = null, ConVarFlags flags = 0)
        {
            Name = name;
            Description = description;
            Flags = flags;
        }

        public bool Momentary => Flags.Has(ConVarFlags.MOMENTARY);

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
