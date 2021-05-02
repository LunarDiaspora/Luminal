using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Reflection
{
    public static class TypeExtensions
    {
        public static FieldInfo[] GetFilteredFields(this Type t)
        {
            return (from f in t.GetFields()
                    where !Attribute.IsDefined(f, typeof(SkipAttribute))
                    select f).ToArray();
        }
    }
}