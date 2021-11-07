using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Assets.Complex
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ComplexTypeAttribute : Attribute
    {
        public string TypeName;

        public ComplexTypeAttribute(string name)
        {
            TypeName = name;
        }
    }
}
