using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Editor.Console
{
    public class ReceiveArgument
    {
        public ArgumentType Type;
        public string Name;
        private dynamic Value = null;

        public ReceiveArgument(ArgumentType t, string n)
        {
            Type = t;
            Name = n;
        }

        public void Parse(string a)
        {
            switch (Type)
            {
                case ArgumentType.STRING:
                    Value = a;
                    break;
                case ArgumentType.INT:
                    Value = int.Parse(a);
                    break;
                case ArgumentType.BOOL:
                    Value = (a == "true" || a == "yes");
                    break;
                case ArgumentType.FLOAT:
                    Value = float.Parse(a);
                    break;
            }
        }

        public dynamic Get() => Value;
    }
}
