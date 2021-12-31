using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Assets.Simple
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SimpleTypeAttribute : Attribute
    {
        public string TypeName;

        public SimpleTypeAttribute(string name)
        {
            TypeName = name;
        }
    }
}
