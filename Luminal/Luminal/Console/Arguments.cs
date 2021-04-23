using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Console
{
    public class Arguments
    {
        public Dictionary<string, ReceiveArgument> values = new();

        public dynamic Get(string n, dynamic d = null)
        {
            if (!values.ContainsKey(n))
                return d;
            var a = values[n];
            var r = a.Get();
            return r ?? d;
        }

        public bool Has(string n)
        {
            return values.ContainsKey(n);
        }
    }
}
