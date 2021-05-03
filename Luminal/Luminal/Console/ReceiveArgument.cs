using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Console
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
                case ArgumentType.String:
                    Value = a;
                    break;
                case ArgumentType.Integer:
                    Value = int.Parse(a);
                    break;
                case ArgumentType.Boolean:
                    Value = (a == "true" || a == "yes" || a == "1");
                    break;
                case ArgumentType.Float:
                    Value = float.Parse(a);
                    break;
            }
        }

        public dynamic Get() => Value;
    }
}
