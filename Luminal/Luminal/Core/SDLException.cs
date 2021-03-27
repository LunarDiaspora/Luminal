using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Core
{
    public class SDLException : ApplicationException
    {
        new public string Message;

        public SDLException(string msg) => Message = msg;
    }
}
