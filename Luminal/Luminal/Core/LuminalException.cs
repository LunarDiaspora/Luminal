using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Core
{
    public class LuminalException : ApplicationException
    {
        new public string Message;

        public LuminalException(string msg) => Message = msg;
    }
}
